using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Oracle.ManagedDataAccess.Client;
using ReconcileTool.UI.Config;

namespace ReconcileTool.UI.Forms;

public partial class BoiThuongControl : UserControl
{
    // Thông tin API hiện tại
    public static ApiCredentialConfig ApiCredential { get; set; } = new();

    // HttpClient dùng chung — bỏ qua lỗi SSL certificate của server nội bộ
    private static readonly HttpClient _httpClient = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    })
    {
        Timeout = TimeSpan.FromSeconds(30)
    };
    // ── Dữ liệu toàn bộ (sau đối soát) ───────────────────────────────
    private DataTable _fullData = new();
    private int _currentPage = 1;
    private int _pageSize = 20;
    private int _totalPages
    {
        get { var d = GetDisplayData(); return d.Rows.Count == 0 ? 1 : (int)Math.Ceiling(d.Rows.Count / (double)_pageSize); }
    }

    // Trả về dữ liệu đã lọc theo cboLocTrangThai
    private DataTable GetDisplayData()
    {
        string filter = cboLocTrangThai?.SelectedItem?.ToString() ?? "Toàn bộ";
        if (filter == "Toàn bộ") return _fullData;

        // "Chưa đồng bộ" = tất cả trạng thái khác "Đã đồng bộ"
        var result = _fullData.Clone();
        foreach (DataRow row in _fullData.Rows)
            if (row["TTRANG_BT"]?.ToString() != "Đã đồng bộ")
                result.ImportRow(row);
        return result;
    }

    // ── Màu theo TTRANG_BT ────────────────────────────────────────────
    private static readonly Dictionary<string, (System.Drawing.Color Back, System.Drawing.Color Fore)> _statusColors = new()
    {
        ["Đã đồng bộ"] = (System.Drawing.Color.FromArgb(210, 245, 215), System.Drawing.Color.FromArgb(25, 110, 45)),
        ["Lệch trạng thái"] = (System.Drawing.Color.FromArgb(255, 244, 180), System.Drawing.Color.FromArgb(160, 95, 0)),
        ["Chưa đồng bộ"] = (System.Drawing.Color.FromArgb(255, 218, 218), System.Drawing.Color.FromArgb(180, 35, 35)),
        ["???"] = (System.Drawing.Color.FromArgb(225, 225, 235), System.Drawing.Color.FromArgb(80, 80, 100)),
    };

    public BoiThuongControl()
    {
        InitializeComponent();
        dgvResult.CellFormatting    += dgvResult_CellFormatting;
        dgvResult.CellPainting      += dgvResult_CellPainting;
        dgvResult.CellContentClick  += dgvResult_CellContentClick;
        dgvResult.CellClick         += dgvResult_CellClick;
        txtSoHs.TextChanged         += txtSoHs_TextChanged;
        UpdatePagingControls();
    }

    // Tự động chuyển ký tự xuống dòng thành dấu phẩy khi paste
    private bool _updatingTxtSoHs = false;
    private void txtSoHs_TextChanged(object? sender, EventArgs e)
    {
        if (_updatingTxtSoHs) return;
        string text = txtSoHs.Text;
        if (!text.Contains('\n') && !text.Contains('\r')) return;

        _updatingTxtSoHs = true;
        int sel = txtSoHs.SelectionStart;
        string newText = text.Replace("\r\n", ",").Replace("\r", ",").Replace("\n", ",");
        txtSoHs.Text = newText;
        txtSoHs.SelectionStart = Math.Min(sel, newText.Length);
        _updatingTxtSoHs = false;
    }

    // ─────────────────────────────────────────────────────────────────
    // Button hover
    // ─────────────────────────────────────────────────────────────────
    private void btnKiemTra_MouseEnter(object? sender, EventArgs e)
        => btnKiemTra.BackColor = System.Drawing.Color.FromArgb(18, 70, 130);
    private void btnKiemTra_MouseLeave(object? sender, EventArgs e)
        => btnKiemTra.BackColor = System.Drawing.Color.FromArgb(24, 90, 157);

    private void cboLocTrangThai_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_fullData.Rows.Count == 0) return;
        _currentPage = 1;
        LoadPage();
    }

    // ─────────────────────────────────────────────────────────────────
    // Kiểm tra — tải 2 DB song song rồi đối soát
    // ─────────────────────────────────────────────────────────────────
    private async void btnKiemTra_Click(object sender, EventArgs e)
    {
        var cfg = AppConfig.OracleConfig;

        if (string.IsNullOrWhiteSpace(cfg.MyBshHost) || string.IsNullOrWhiteSpace(cfg.MyBshUser))
        {
            MessageBox.Show("Vui lòng cấu hình và lưu thông tin kết nối DB MYBSH.",
                "Chưa cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(cfg.BshHost) || string.IsNullOrWhiteSpace(cfg.BshUser))
        {
            MessageBox.Show("Vui lòng cấu hình và lưu thông tin kết nối DB BSH.",
                "Chưa cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int ngayDau = int.Parse(dtpNgayDau.Value.ToString("yyyyMMdd"));
        int ngayCuoi = int.Parse(dtpNgayCuoi.Value.ToString("yyyyMMdd"));
        if (ngayDau > ngayCuoi)
        {
            MessageBox.Show("Ngày đầu không được lớn hơn ngày cuối.",
                "Lỗi ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboLoai.SelectedItem?.ToString() == "HS giám định"
         || cboTrangThai.SelectedItem?.ToString() == "Tất cả")
        {
            MessageBox.Show("Chức năng đang phát triển.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string bTtrang = "D";

        // DT_BSH.PBH_DSOAT_CORE_HSBT    → DB MYBSH (10.86.0.87)      isBsh: false
        // CUONGDV.PBH_DSOAT_MYBSH_HSBT  → DB BSH   (115.146.126.94)  isBsh: true
        string connMyBsh = cfg.BuildConnectionString(isBsh: false);
        string connBsh = cfg.BuildConnectionString(isBsh: true);

        SetLoadingState(true);
        try
        {
            // ── Tải 2 DB song song ──────────────────────────────────
            // coreData   = toàn bộ dữ liệu từ MYBSH (DT_BSH schema)
            // bshLookup  = dữ liệu từ BSH (CUONGDV schema) dùng để đối soát
            // Chuẩn hoá danh sách số HS thành chuỗi "HS001,HS002,..." để truyền vào SP
            string dsHs = BuildDsHsParam(txtSoHs.Text);

            var taskCore = Task.Run(() =>
                LoadCoreData(connMyBsh, ngayDau, ngayCuoi, bTtrang, dsHs));
            var taskBsh = Task.Run(() =>
                LoadBshSideData(connBsh, ngayDau, ngayCuoi, bTtrang, dsHs));

            await Task.WhenAll(taskCore, taskBsh);

            DataTable coreData = taskCore.Result;   // từ DB MYBSH
            DataTable bshData = taskBsh.Result;    // từ DB BSH

            // ── Đối soát: MYBSH là chính, BSH là lookup ─────────────
            _fullData = PerformReconciliation(coreData, bshData);

            _currentPage = 1;
            _pageSize = int.TryParse(cboPageSize.SelectedItem?.ToString(), out var ps) ? ps : 20;

            ApplyColumns(_fullData);
            LoadPage();

            // ── Thống kê ─────────────────────────────────────────────
            var counts = _fullData.AsEnumerable()
                                      .GroupBy(r => r["TTRANG_BT"]?.ToString() ?? "")
                                      .ToDictionary(g => g.Key, g => g.Count());

            lblStatus.ForeColor = System.Drawing.Color.FromArgb(34, 139, 34);
            lblStatus.Text = $"✔  Tải xong lúc {DateTime.Now:HH:mm:ss}  |  " +
                             $"{cboLoai.SelectedItem}  |  {cboTrangThai.SelectedItem}  |  " +
                             $"{dtpNgayDau.Value:dd/MM/yyyy} → {dtpNgayCuoi.Value:dd/MM/yyyy}  |  " +
                             $"MYBSH: {coreData.Rows.Count:N0}   BSH: {bshData.Rows.Count:N0}";

            lblRecordCount.Text =
                $"Tổng: {_fullData.Rows.Count:N0}   " +
                $"✅ Đã ĐB: {counts.GetValueOrDefault("Đã đồng bộ", 0):N0}   " +
                $"⚠ Lệch TT: {counts.GetValueOrDefault("Lệch trạng thái", 0):N0}   " +
                $"❌ Chưa ĐB: {counts.GetValueOrDefault("Chưa đồng bộ", 0):N0}   " +
                $"❓ ???: {counts.GetValueOrDefault("???", 0):N0}";
        }
        catch (Exception ex)
        {
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(200, 50, 50);
            lblStatus.Text = "✘  Lỗi: " + ex.Message.Split('\n')[0];
            lblRecordCount.Text = "";
            _fullData.Clear();
            dgvResult.DataSource = null;
            UpdatePagingControls();
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // DB MYBSH — DT_BSH.PBH_DSOAT_CORE_HSBT (dữ liệu chính)
    // Trả về: MA_DVI, MA_CN, SO_ID, NGAY_HT, SO_HS, SO_ID_DT,
    //         NGAY_XR, NGAY_TB, TEN_TTRANG, TTRANG, NG_DUYET
    // ─────────────────────────────────────────────────────────────────
    private static DataTable LoadCoreData(string connStr, int ngayDau, int ngayCuoi,
                                          string bTtrang, string dsHs = "")
    {
        using var conn = new OracleConnection(connStr);
        conn.Open();
        try
        {
            using var cmd = new OracleCommand(
                "BEGIN DT_BSH.PBH_DSOAT_CORE_HSBT(:b_nsdId,:b_pas,:cs_ct,:b_ngay_dau,:b_ngay_cuoi,:b_ttrang,:b_ds_hs); END;",
                conn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 120,
                BindByName = true
            };
            cmd.Parameters.Add("b_nsdId", OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("b_pas", OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("cs_ct", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("b_ngay_dau", OracleDbType.Decimal).Value = ngayDau;
            cmd.Parameters.Add("b_ngay_cuoi", OracleDbType.Decimal).Value = ngayCuoi;
            cmd.Parameters.Add("b_ttrang", OracleDbType.Varchar2).Value = bTtrang;
            cmd.Parameters.Add("b_ds_hs", OracleDbType.Varchar2).Value = dsHs;
            return ExecuteRefCursor(cmd, "cs_ct");
        }
        catch (Exception ex)
        {
            using var cmdUser = new OracleCommand("SELECT USER FROM DUAL", conn);
            string u = cmdUser.ExecuteScalar()?.ToString() ?? "?";
            throw new Exception($"[DB MYBSH - User:{u}] {ex.Message}", ex);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // DB BSH — CUONGDV.PBH_DSOAT_MYBSH_HSBT (dùng để đối soát)
    // Trả về: SO_ID (=SO_ID_HS), SO_ID_CORE (=SO_ID_BT), NGAY_NHC, TTRANG
    // ─────────────────────────────────────────────────────────────────
    private static DataTable LoadBshSideData(string connStr, int ngayDau, int ngayCuoi,
                                             string bTtrang, string dsHs = "")
    {
        using var conn = new OracleConnection(connStr);
        conn.Open();
        try
        {
            using var cmd = new OracleCommand(
                "BEGIN CUONGDV.PBH_DSOAT_MYBSH_HSBT(:b_nsdId,:b_pas,:cs_ct,:b_ngay_dau,:b_ngay_cuoi,:b_ttrang,:b_ds_hs); END;",
                conn)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 120,
                BindByName = true
            };
            cmd.Parameters.Add("b_nsdId", OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("b_pas", OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("cs_ct", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("b_ngay_dau", OracleDbType.Decimal).Value = ngayDau;
            cmd.Parameters.Add("b_ngay_cuoi", OracleDbType.Decimal).Value = ngayCuoi;
            cmd.Parameters.Add("b_ttrang", OracleDbType.Varchar2).Value = bTtrang;
            cmd.Parameters.Add("b_ds_hs", OracleDbType.Varchar2).Value = dsHs;
            return ExecuteRefCursor(cmd, "cs_ct");
        }
        catch (Exception ex)
        {
            using var cmdUser = new OracleCommand("SELECT USER FROM DUAL", conn);
            string u = cmdUser.ExecuteScalar()?.ToString() ?? "?";
            throw new Exception($"[DB BSH - User:{u}] {ex.Message}", ex);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Helper: thực thi và đọc RefCursor → DataTable
    // ─────────────────────────────────────────────────────────────────
    private static DataTable ExecuteRefCursor(OracleCommand cmd, string cursorParam)
    {
        cmd.ExecuteNonQuery();
        var cursor = (Oracle.ManagedDataAccess.Types.OracleRefCursor)
                     cmd.Parameters[cursorParam].Value;
        var dt = new DataTable();
        using var reader = cursor.GetDataReader();
        dt.Load(reader);
        return dt;
    }

    // ─────────────────────────────────────────────────────────────────
    // Đối soát:
    //   coreData = dữ liệu MYBSH (DT_BSH)  — hiển thị chính
    //   bshData  = dữ liệu BSH  (CUONGDV)  — tra cứu đối soát
    //
    //   Kết quả gồm đúng 12 cột theo thứ tự:
    //   SO_ID_MYBSH | MA_DVI | MA_CN | SO_HS | NGAY_XR | NGAY_TB |
    //   NG_DUYET | TTRANG_MYBSH | SO_ID_BT | TTRANG_CORE | NGAY_NHC | TTRANG_BT
    // ─────────────────────────────────────────────────────────────────
    private static DataTable PerformReconciliation(DataTable coreData, DataTable bshData)
    {
        // Build BSH lookup: SO_ID → (SO_ID_CORE, TTRANG, NGAY_NHC)
        var lookup = new Dictionary<string, (string SoIdBt, string Ttrang, string NgayNhc)>(
                         StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in bshData.Rows)
        {
            string soId = row["SO_ID"]?.ToString() ?? "";
            if (string.IsNullOrEmpty(soId)) continue;
            lookup[soId] = (
                row["SO_ID_CORE"]?.ToString() ?? "",
                row["TTRANG"]?.ToString() ?? "",
                row["NGAY_NHC"]?.ToString() ?? ""
            );
        }

        // Tạo DataTable mới với đúng 12 cột theo thứ tự yêu cầu
        var result = new DataTable();
        result.Columns.Add("SO_ID_MYBSH", typeof(string));
        result.Columns.Add("MA_DVI", typeof(string));
        result.Columns.Add("MA_CN", typeof(string));
        result.Columns.Add("SO_HS", typeof(string));
        result.Columns.Add("NGAY_XR", typeof(string));
        result.Columns.Add("NGAY_TB", typeof(string));
        result.Columns.Add("NG_DUYET", typeof(string));
        result.Columns.Add("TTRANG_MYBSH", typeof(string));
        result.Columns.Add("SO_ID_BT", typeof(string));
        result.Columns.Add("TTRANG_CORE", typeof(string));
        result.Columns.Add("NGAY_NHC", typeof(string));
        result.Columns.Add("TTRANG_BT", typeof(string));

        foreach (DataRow src in coreData.Rows)
        {
            string soId = src["SO_ID"]?.ToString() ?? "";
            string ttrangMybsh = src["TTRANG"]?.ToString() ?? "";

            string soIdBt, ttrangCore, ngayNhc, ttrangBt;

            if (!lookup.TryGetValue(soId, out var bsh))
            {
                soIdBt = "";
                ttrangCore = "";
                ngayNhc = "";
                ttrangBt = "Chưa đồng bộ";
            }
            else
            {
                soIdBt = bsh.SoIdBt;
                ttrangCore = bsh.Ttrang;
                ngayNhc = bsh.NgayNhc;

                bool coreValid = !string.IsNullOrEmpty(soIdBt) && soIdBt != "0";
                bool ttrangMatch = string.Equals(ttrangMybsh, ttrangCore,
                                                 StringComparison.OrdinalIgnoreCase);

                ttrangBt = (coreValid && ttrangMatch) ? "Đã đồng bộ"
                         : (!coreValid || !ttrangMatch) ? "Lệch trạng thái"
                                                        : "???";
            }

            result.Rows.Add(
                soId,
                src["MA_DVI"]?.ToString() ?? "",
                src["MA_CN"]?.ToString() ?? "",
                src["SO_HS"]?.ToString() ?? "",
                src["NGAY_XR"]?.ToString() ?? "",
                src["NGAY_TB"]?.ToString() ?? "",
                src["NG_DUYET"]?.ToString() ?? "",
                ttrangMybsh,
                soIdBt,
                ttrangCore,
                ngayNhc,
                ttrangBt
            );
        }

        return result;
    }

    // ─────────────────────────────────────────────────────────────────
    // Chuẩn hoá input textbox → chuỗi "HS001,HS002,..." truyền vào SP
    // Trả về chuỗi rỗng nếu không nhập gì (SP sẽ bỏ qua điều kiện IN)
    // ─────────────────────────────────────────────────────────────────
    private static string BuildDsHsParam(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        var items = input
            .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s));
        return string.Join(",", items);
    }

    // ─────────────────────────────────────────────────────────────────
    // Header tiếng Việt — đúng theo yêu cầu
    // ─────────────────────────────────────────────────────────────────
    private static readonly Dictionary<string, string> _headers =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["SO_ID_MYBSH"] = "Số ID MyBSH",
            ["MA_DVI"] = "Mã đơn vị",
            ["MA_CN"] = "Mã CN",
            ["SO_HS"] = "Số HS",
            ["NGAY_XR"] = "Ngày xảy ra",
            ["NGAY_TB"] = "Ngày thông báo",
            ["NG_DUYET"] = "Ngày duyệt",
            ["TTRANG_MYBSH"] = "Tình trạng MYBSH",
            ["SO_ID_BT"] = "Số ID Core",
            ["TTRANG_CORE"] = "Tình trạng CORE",
            ["NGAY_NHC"] = "Ngày ĐB gần nhất",
            ["TTRANG_BT"] = "TRẠNG THÁI ĐỒNG BỘ",
        };

    private void ApplyColumns(DataTable dt)
    {
        dgvResult.DataSource = null;
        dgvResult.Columns.Clear();

        foreach (DataColumn dc in dt.Columns)
        {
            // Độ rộng từng cột
            int colWidth = dc.ColumnName switch
            {
                "MA_DVI" => 75,
                "MA_CN" => 65,
                "TTRANG_MYBSH" => 90,
                "TTRANG_CORE" => 90,
                "TTRANG_BT" => 180,
                "SO_ID_MYBSH" => 130,
                "SO_ID_BT" => 110,
                "SO_HS" => 110,
                "NGAY_XR" => 100,
                "NGAY_TB" => 110,
                "NG_DUYET" => 100,
                "NGAY_NHC" => 120,
                _ => 100,
            };

            var col = new DataGridViewTextBoxColumn
            {
                Name = dc.ColumnName,
                DataPropertyName = dc.ColumnName,
                HeaderText = _headers.TryGetValue(dc.ColumnName, out var h) ? h : dc.ColumnName,
                Width = colWidth,
                MinimumWidth = 50,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            };

            // TTRANG_BT: in đậm, căn giữa
            if (dc.ColumnName == "TTRANG_BT")
            {
                col.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Căn giữa các cột ngày, ID, trạng thái
            string upper = dc.ColumnName.ToUpper();
            if (upper.StartsWith("NGAY") || upper.StartsWith("NG_")
             || upper.Contains("SO_ID") || upper.StartsWith("TTRANG")
             || upper == "MA_DVI" || upper == "MA_CN")
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvResult.Columns.Add(col);
        }

        // ── Cột "Chức năng": nút Đồng bộ lại (chỉ hiện khi Chưa đồng bộ) ──
        var actionCol = new DataGridViewButtonColumn
        {
            Name       = "COL_ACTION",
            HeaderText = "Chức năng",
            Text       = "Đồng bộ lại",
            UseColumnTextForButtonValue = true,
            Width      = 115,
            MinimumWidth = 90,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            FlatStyle  = FlatStyle.Flat,
        };
        actionCol.DefaultCellStyle.BackColor  = System.Drawing.Color.FromArgb(24, 90, 157);
        actionCol.DefaultCellStyle.ForeColor  = System.Drawing.Color.White;
        actionCol.DefaultCellStyle.Font       = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
        actionCol.DefaultCellStyle.Alignment  = DataGridViewContentAlignment.MiddleCenter;
        dgvResult.Columns.Add(actionCol);
    }

    // ─────────────────────────────────────────────────────────────────
    // Tô màu hàng theo TTRANG_BT
    // ─────────────────────────────────────────────────────────────────
    private void dgvResult_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || !dgvResult.Columns.Contains("TTRANG_BT")) return;

        string status = dgvResult.Rows[e.RowIndex]
                                  .Cells["TTRANG_BT"]
                                  .Value?.ToString() ?? "";

        if (!_statusColors.TryGetValue(status, out var colors)) return;

        e.CellStyle.BackColor = colors.Back;
        e.CellStyle.ForeColor = colors.Fore;
        e.CellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(
            Math.Max(0, colors.Back.R - 35),
            Math.Max(0, colors.Back.G - 35),
            Math.Max(0, colors.Back.B - 35));
        e.CellStyle.SelectionForeColor = colors.Fore;
    }

    // ─────────────────────────────────────────────────────────────────
    // Vẽ cột Chức năng:
    //   - Chưa đồng bộ  → vẽ nút bình thường (để default paint)
    //   - Còn lại        → vẽ ô trống màu theo trạng thái (ẩn nút)
    // ─────────────────────────────────────────────────────────────────
    private void dgvResult_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (!dgvResult.Columns.Contains("COL_ACTION")) return;
        if (e.ColumnIndex != dgvResult.Columns["COL_ACTION"]!.Index) return;

        string status = dgvResult.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
        if (status == "Chưa đồng bộ") return; // để default paint vẽ nút

        // Tô màu nền theo trạng thái hàng (hoặc trắng), không vẽ nút
        var backColor = _statusColors.TryGetValue(status, out var c) ? c.Back : System.Drawing.Color.White;
        using var brush = new System.Drawing.SolidBrush(backColor);
        e.Graphics!.FillRectangle(brush, e.CellBounds);

        // Vẽ border grid
        using var pen = new System.Drawing.Pen(dgvResult.GridColor);
        e.Graphics!.DrawRectangle(pen,
            e.CellBounds.X,
            e.CellBounds.Y,
            e.CellBounds.Width - 1,
            e.CellBounds.Height - 1);

        e.Handled = true;
    }

    // ─────────────────────────────────────────────────────────────────
    // Click nút Đồng bộ lại → gọi API
    // ─────────────────────────────────────────────────────────────────
    private async void dgvResult_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (!dgvResult.Columns.Contains("COL_ACTION")) return;
        if (e.ColumnIndex != dgvResult.Columns["COL_ACTION"]!.Index) return;

        string status = dgvResult.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
        if (status != "Chưa đồng bộ") return;

        string soId = dgvResult.Rows[e.RowIndex].Cells["SO_ID_MYBSH"].Value?.ToString() ?? "";
        string soHs = dgvResult.Rows[e.RowIndex].Cells["SO_HS"].Value?.ToString() ?? "";

        if (string.IsNullOrEmpty(soId))
        {
            ShowApiResult("⚠ Không tìm thấy SO_ID để đồng bộ.", isError: true);
            return;
        }

        var confirm = MessageBox.Show(
            $"Đồng bộ lại hồ sơ?\n\nSố ID MyBSH : {soId}\nSố HS        : {soHs}",
            "Xác nhận đồng bộ",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        await CallSyncApiAsync(soId);
    }

    // ─────────────────────────────────────────────────────────────────
    // Click vào dòng → copy SO_ID_MYBSH vào clipboard
    // ─────────────────────────────────────────────────────────────────
    private void dgvResult_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (!dgvResult.Columns.Contains("SO_ID_MYBSH")) return;

        string soId = dgvResult.Rows[e.RowIndex].Cells["SO_ID_MYBSH"].Value?.ToString() ?? "";
        if (string.IsNullOrEmpty(soId)) return;

        Clipboard.SetText(soId);

        // Hiển thị thông báo nhỏ trên panel response
        rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(25, 110, 45);
        rtxtApiResponse.Text = $"📋 Đã copy SO_ID MyBSH:\n{soId}";
    }

    // ─────────────────────────────────────────────────────────────────
    // Đồng bộ lại toàn bộ — gọi API cho tất cả hàng "Chưa đồng bộ"
    // ─────────────────────────────────────────────────────────────────
    private async void btnSyncAll_Click(object sender, EventArgs e)
    {
        var pending = _fullData.AsEnumerable()
            .Where(r => r["TTRANG_BT"]?.ToString() == "Chưa đồng bộ")
            .Select(r => r["SO_ID_MYBSH"]?.ToString() ?? "")
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        if (pending.Count == 0)
        {
            MessageBox.Show("Không có hồ sơ nào ở trạng thái \"Chưa đồng bộ\".",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (string.IsNullOrWhiteSpace(ApiCredential.ApiUser))
        {
            ShowApiResult("⚠ Chưa cấu hình API User/Password.", isError: true);
            return;
        }

        var confirm = MessageBox.Show(
            $"Sẽ đồng bộ lại {pending.Count:N0} hồ sơ đang \"Chưa đồng bộ\".\n\nTiếp tục?",
            "Xác nhận đồng bộ toàn bộ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        btnSyncAll.Enabled = false;
        btnKiemTra.Enabled = false;

        int successCount = 0;
        int failCount    = 0;

        for (int i = 0; i < pending.Count; i++)
        {
            string soId = pending[i];
            rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            rtxtApiResponse.Text = $"⏳ Đang đồng bộ {i + 1}/{pending.Count}...\nSO_ID: {soId}";
            Application.DoEvents();

            bool ok = await SyncSingleAsync(soId);
            if (ok) successCount++;
            else    failCount++;
        }

        btnSyncAll.Enabled = true;
        btnKiemTra.Enabled = true;

        ShowApiResult(
            $"✅ Hoàn tất đồng bộ toàn bộ\n" +
            $"──────────────────────────────\n" +
            $"Tổng xử lý : {pending.Count:N0}\n" +
            $"Thành công : {successCount:N0}\n" +
            $"Thất bại   : {failCount:N0}",
            isError: failCount > 0);
    }

    // ─────────────────────────────────────────────────────────────────
    // Gọi API đồng bộ 1 hồ sơ — trả về true nếu thành công
    // (dùng chung cho đồng bộ đơn lẻ và đồng bộ toàn bộ)
    // ─────────────────────────────────────────────────────────────────
    private async Task<bool> SyncSingleAsync(string soId)
    {
        if (!long.TryParse(soId, out long soIdNum)) return false;

        const string url = "https://openapi.bshc.vn/api/ConnectChannel/Public/Ora/Car/PushCore";
        var body = JsonSerializer.Serialize(new { so_id = soIdNum });

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("username", ApiCredential.ApiUser);
            request.Headers.Add("password", ApiCredential.ApiPassword);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("code", out var codeEl)
                && codeEl.GetString() == "000")
            {
                UpdateRowStatus(soId);
                return true;
            }
            return false;
        }
        catch { return false; }
    }

    // ─────────────────────────────────────────────────────────────────
    // Gọi API đồng bộ
    // POST https://openapi.bshc.vn/api/ConnectChannel/Public/Ora/Car/PushCore
    // Header: username / password  —  Body: { "so_id": <number> }
    // ─────────────────────────────────────────────────────────────────
    private async Task CallSyncApiAsync(string soId)
    {
        if (string.IsNullOrWhiteSpace(ApiCredential.ApiUser))
        {
            ShowApiResult("⚠ Chưa cấu hình API User/Password.\nVui lòng điền thông tin trong \"Cấu hình API\" rồi nhấn \"Lưu tất cả cấu hình\".", isError: true);
            return;
        }

        if (!long.TryParse(soId, out long soIdNum))
        {
            ShowApiResult($"⚠ SO_ID không hợp lệ: {soId}", isError: true);
            return;
        }

        // Hiển thị đang gọi
        rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
        rtxtApiResponse.Text = $"⏳ Đang gọi API...\nSO_ID: {soId}";

        const string url = "https://openapi.bshc.vn/api/ConnectChannel/Public/Ora/Car/PushCore";
        var body = JsonSerializer.Serialize(new { so_id = soIdNum });

        var sw = Stopwatch.StartNew();
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("username", ApiCredential.ApiUser);
            request.Headers.Add("password", ApiCredential.ApiPassword);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            sw.Stop();

            string responseBody = await response.Content.ReadAsStringAsync();
            string pretty = TryPrettyJson(responseBody);

            // Kiểm tra code == "000" → đồng bộ thành công
            bool isSyncSuccess = false;
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("code", out var codeEl)
                    && codeEl.GetString() == "000")
                    isSyncSuccess = true;
            }
            catch { /* giữ isSyncSuccess = false */ }

            string header =
                $"▶ SO_ID : {soId}\n" +
                $"⏱ {sw.ElapsedMilliseconds} ms  |  HTTP {(int)response.StatusCode} {response.StatusCode}\n" +
                $"──────────────────────────────\n";

            ShowApiResult(header + pretty, isError: !isSyncSuccess);

            if (isSyncSuccess)
                UpdateRowStatus(soId);
        }
        catch (Exception ex)
        {
            sw.Stop();
            ShowApiResult($"✘ Lỗi kết nối ({sw.ElapsedMilliseconds} ms):\n{ex.Message}", isError: true);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Cập nhật trạng thái hàng sau khi sync thành công (code == "000")
    // ─────────────────────────────────────────────────────────────────
    private void UpdateRowStatus(string soId)
    {
        // Cập nhật trong _fullData (data gốc)
        foreach (DataRow row in _fullData.Rows)
        {
            if (row["SO_ID_MYBSH"]?.ToString() == soId)
                row["TTRANG_BT"] = "Đã đồng bộ";
        }

        // Cập nhật label thống kê
        var counts = _fullData.AsEnumerable()
            .GroupBy(r => r["TTRANG_BT"]?.ToString() ?? "")
            .ToDictionary(g => g.Key, g => g.Count());

        lblRecordCount.Text =
            $"Tổng: {_fullData.Rows.Count:N0}   " +
            $"✅ Đã ĐB: {counts.GetValueOrDefault("Đã đồng bộ", 0):N0}   " +
            $"⚠ Lệch TT: {counts.GetValueOrDefault("Lệch trạng thái", 0):N0}   " +
            $"❌ Chưa ĐB: {counts.GetValueOrDefault("Chưa đồng bộ", 0):N0}   " +
            $"❓ ???: {counts.GetValueOrDefault("???", 0):N0}";

        // Tải lại trang hiện tại để render màu mới
        LoadPage();
    }

    // ─────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────
    private void ShowApiResult(string text, bool isError)
    {
        rtxtApiResponse.ForeColor = isError
            ? System.Drawing.Color.FromArgb(180, 35, 35)
            : System.Drawing.Color.FromArgb(25, 110, 45);
        rtxtApiResponse.Text = text;
    }

    private static string TryPrettyJson(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
        catch
        {
            return json;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Phân trang
    // ─────────────────────────────────────────────────────────────────
    private void LoadPage()
    {
        var data = GetDisplayData();
        if (data.Rows.Count == 0)
        {
            dgvResult.DataSource = null;
            UpdatePagingControls();
            return;
        }

        int skip = (_currentPage - 1) * _pageSize;
        var page = data.Clone();
        foreach (var row in data.AsEnumerable().Skip(skip).Take(_pageSize))
            page.ImportRow(row);

        dgvResult.DataSource = page;
        UpdatePagingControls();
    }

    private void UpdatePagingControls()
    {
        var data = GetDisplayData();
        bool hasData = data.Rows.Count > 0;
        int from = hasData ? (_currentPage - 1) * _pageSize + 1 : 0;
        int to   = hasData ? Math.Min(_currentPage * _pageSize, data.Rows.Count) : 0;

        lblPageInfo.Text     = hasData ? $"Trang  {_currentPage} / {_totalPages}" : "—";
        lblTotalRecords.Text = hasData ? $"(Hiển thị {from:N0} – {to:N0} / {data.Rows.Count:N0})" : "";

        btnFirst.Enabled = _currentPage > 1;
        btnPrev.Enabled  = _currentPage > 1;
        btnNext.Enabled  = _currentPage < _totalPages;
        btnLast.Enabled  = _currentPage < _totalPages;
    }

    private void btnFirst_Click(object sender, EventArgs e) { _currentPage = 1; LoadPage(); }
    private void btnPrev_Click(object sender, EventArgs e) { if (_currentPage > 1) { _currentPage--; LoadPage(); } }
    private void btnNext_Click(object sender, EventArgs e) { if (_currentPage < _totalPages) { _currentPage++; LoadPage(); } }
    private void btnLast_Click(object sender, EventArgs e) { _currentPage = _totalPages; LoadPage(); }

    private void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cboPageSize.SelectedItem is null) return;
        _pageSize = int.Parse(cboPageSize.SelectedItem.ToString()!);
        _currentPage = 1;
        LoadPage();
    }

    // ─────────────────────────────────────────────────────────────────
    // Trạng thái loading
    // ─────────────────────────────────────────────────────────────────
    private void SetLoadingState(bool loading)
    {
        btnKiemTra.Enabled = !loading;
        btnKiemTra.Text = loading ? "Đang tải..." : "Kiểm tra";
        cboPageSize.Enabled = !loading;
        btnFirst.Enabled = !loading && _currentPage > 1;
        btnPrev.Enabled = !loading && _currentPage > 1;
        btnNext.Enabled = !loading && _currentPage < _totalPages;
        btnLast.Enabled = !loading && _currentPage < _totalPages;

        if (loading)
        {
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            lblStatus.Text = "⏳  Đang tải dữ liệu từ DB MYBSH và DB BSH song song...";
            lblRecordCount.Text = "";
        }
    }

}
