using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Oracle.ManagedDataAccess.Client;
using ReconcileTool.UI.Config;

namespace ReconcileTool.UI.Forms;

public partial class ThanhToanControl : UserControl
{
    private const string SpDtBsh        = "DT_BSH.PBH_DSOAT_CORE_TT";
    private const string SpCuongdv      = "CUONGDV.PBH_DSOAT_MYBSH_TT";
    private const string SyncApiUrl     = "https://openapi.bshc.com.vn/api/Core/Claim/KT_BTDU";
    private const string SyncApiUser    = "KETNOIBSH_OPES";
    private const string SyncApiPass    = "12912b163a24531bc32de5527abaf372";

    public static ApiCredentialConfig ApiCredential { get; set; } = new();

    private static readonly HttpClient _httpClient = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }) { Timeout = TimeSpan.FromSeconds(30) };

    // Display → SP value mapping
    private static readonly Dictionary<string, string> _loaiValues = new()
    {
        ["Duyệt bồi thường"]      = "BT_DU",
        ["Duyệt giám định"]       = "GD_DU",
        ["Thanh toán bồi thường"] = "BT_TT",
        ["Thanh toán giám định"]  = "GD_TT",
    };

    // Reconciled result (MA_DVI, SO_ID, NGAY_HT, SO_CT, NDUNG, TIEN, NSDID, NGAY_NH,
    //                    SO_ID_CORE, TTRANG_BT)
    private DataTable _fullData = new();
    // DT_BSH cs_ct — kept for "Xem chi tiết" popup, filtered by SO_ID_KT = SO_ID
    private DataTable _csCtData = new();

    private int _currentPage = 1;
    private int _pageSize    = 20;
    private int _totalPages
        => GetDisplayData().Rows.Count == 0
            ? 1
            : (int)Math.Ceiling(GetDisplayData().Rows.Count / (double)_pageSize);

    private DataTable GetDisplayData()
    {
        string filter = cboTrangThai?.SelectedItem?.ToString() ?? "Tất cả";
        if (filter == "Tất cả") return _fullData;
        var result = _fullData.Clone();
        foreach (DataRow row in _fullData.Rows)
            if (row["TTRANG_BT"]?.ToString() == "Chưa đồng bộ")
                result.ImportRow(row);
        return result;
    }

    private static readonly Dictionary<string, (System.Drawing.Color Back, System.Drawing.Color Fore)> _statusColors = new()
    {
        ["Đã đồng bộ"]   = (System.Drawing.Color.FromArgb(210, 245, 215), System.Drawing.Color.FromArgb(25, 110, 45)),
        ["Chưa đồng bộ"] = (System.Drawing.Color.FromArgb(255, 218, 218), System.Drawing.Color.FromArgb(180, 35, 35)),
    };

    public ThanhToanControl()
    {
        InitializeComponent();
        cboPageSize.SelectedIndex = 0;
        dgvMain.CellFormatting    += dgvMain_CellFormatting;
        dgvMain.CellPainting      += dgvMain_CellPainting;
        dgvMain.CellContentClick  += dgvMain_CellContentClick;
        txtSoHs.TextChanged       += txtSoHs_TextChanged;
        UpdatePagingControls();
    }

    // ─────────────────────────────────────────────────────────────────
    // txtSoHs — newline → comma conversion
    // ─────────────────────────────────────────────────────────────────
    private bool _updatingTxt = false;
    private void txtSoHs_TextChanged(object? sender, EventArgs e)
    {
        if (_updatingTxt) return;
        string text = txtSoHs.Text;
        if (!text.Contains('\n') && !text.Contains('\r')) return;
        _updatingTxt = true;
        int sel     = txtSoHs.SelectionStart;
        string newText = text.Replace("\r\n", ",").Replace("\r", ",").Replace("\n", ",");
        txtSoHs.Text = newText;
        txtSoHs.SelectionStart = Math.Min(sel, newText.Length);
        _updatingTxt = false;
    }

    private void btnKiemTra_MouseEnter(object? sender, EventArgs e)
        => btnKiemTra.BackColor = System.Drawing.Color.FromArgb(175, 65, 10);
    private void btnKiemTra_MouseLeave(object? sender, EventArgs e)
        => btnKiemTra.BackColor = System.Drawing.Color.FromArgb(220, 95, 20);

    // ─────────────────────────────────────────────────────────────────
    // Kiểm tra — load both DBs in parallel, reconcile, display
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

        string displayLoai = cboLoai.SelectedItem?.ToString() ?? "";
        if (displayLoai == "Thanh toán bồi thường" || displayLoai == "Thanh toán giám định")
        {
            MessageBox.Show("Chức năng đang phát triển.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        int ngayDau  = int.Parse(dtpNgayDau.Value.ToString("yyyyMMdd"));
        int ngayCuoi = int.Parse(dtpNgayCuoi.Value.ToString("yyyyMMdd"));
        if (ngayDau > ngayCuoi)
        {
            MessageBox.Show("Ngày đầu không được lớn hơn ngày cuối.",
                "Lỗi ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (dtpNgayCuoi.Value > dtpNgayDau.Value.AddMonths(2))
        {
            MessageBox.Show("Khoảng thời gian tìm kiếm không được vượt quá 2 tháng.",
                "Khoảng thời gian quá lớn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        displayLoai = cboLoai.SelectedItem?.ToString() ?? "Duyệt bồi thường";
        string loaiCt      = _loaiValues.TryGetValue(displayLoai, out var v) ? v : "BT_DU";
        string connMyBsh   = cfg.BuildConnectionString(isBsh: false);
        string connBsh     = cfg.BuildConnectionString(isBsh: true);
        string dsHs        = BuildDsHsParam(txtSoHs.Text);

        SetLoadingState(true);
        try
        {
            var taskDtBsh   = Task.Run(() => LoadDtBshData(connMyBsh, ngayDau, ngayCuoi, loaiCt));
            var taskCuongdv = Task.Run(() => LoadCuongdvData(connBsh, ngayDau, ngayCuoi, loaiCt, dsHs));
            await Task.WhenAll(taskDtBsh, taskCuongdv);

            (DataTable csData, DataTable csCtData) = taskDtBsh.Result;
            DataTable cuongdvData = taskCuongdv.Result;

            _csCtData = csCtData;
            _fullData = PerformReconciliation(csData, cuongdvData);

            _currentPage = 1;
            _pageSize    = int.TryParse(cboPageSize.SelectedItem?.ToString(), out var ps) ? ps : 20;

            ApplyColumns();
            LoadPage();

            var counts = _fullData.AsEnumerable()
                .GroupBy(r => r["TTRANG_BT"]?.ToString() ?? "")
                .ToDictionary(g => g.Key, g => g.Count());

            lblStatus.ForeColor = System.Drawing.Color.FromArgb(34, 139, 34);
            lblStatus.Text =
                $"✔  Tải xong lúc {DateTime.Now:HH:mm:ss}  |  {displayLoai}  |  " +
                $"{dtpNgayDau.Value:dd/MM/yyyy} → {dtpNgayCuoi.Value:dd/MM/yyyy}  |  " +
                $"DT_BSH: {csData.Rows.Count:N0}   CUONGDV: {cuongdvData.Rows.Count:N0}";

            lblRecordCount.Text =
                $"Tổng: {_fullData.Rows.Count:N0}   " +
                $"✅ Đã ĐB: {counts.GetValueOrDefault("Đã đồng bộ", 0):N0}   " +
                $"❌ Chưa ĐB: {counts.GetValueOrDefault("Chưa đồng bộ", 0):N0}";
        }
        catch (Exception ex)
        {
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(200, 50, 50);
            lblStatus.Text = "✘  Lỗi: " + ex.Message.Split('\n')[0];
            lblRecordCount.Text = "";
            _fullData.Clear();
            _csCtData.Clear();
            dgvMain.DataSource = null;
            UpdatePagingControls();
        }
        finally { SetLoadingState(false); }
    }

    private void SetLoadingState(bool loading)
    {
        btnKiemTra.Enabled = !loading;
        btnSyncAll.Enabled = !loading;
        btnKiemTra.Text    = loading ? "Đang tải..." : "Kiểm tra";
        if (loading)
        {
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            lblStatus.Text      = "⏳  Đang tải dữ liệu...";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // DB load — DT_BSH (cs + cs_ct cursors)
    // ─────────────────────────────────────────────────────────────────
    private static (DataTable cs, DataTable csCt) LoadDtBshData(
        string connStr, int ngayDau, int ngayCuoi, string loaiCt)
    {
        using var conn = new OracleConnection(connStr);
        conn.Open();
        try
        {
            using var cmd = new OracleCommand(
                $"BEGIN {SpDtBsh}(:b_nsdId,:b_pas,:cs,:cs_ct,:b_ngay_dau,:b_ngay_cuoi,:b_loai_ct); END;",
                conn)
            {
                CommandType    = CommandType.Text,
                CommandTimeout = 120,
                BindByName     = true,
            };
            cmd.Parameters.Add("b_nsdId",    OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("b_pas",      OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("cs",         OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("cs_ct",      OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("b_ngay_dau", OracleDbType.Decimal).Value  = ngayDau;
            cmd.Parameters.Add("b_ngay_cuoi",OracleDbType.Decimal).Value  = ngayCuoi;
            cmd.Parameters.Add("b_loai_ct",  OracleDbType.Varchar2).Value = loaiCt;

            cmd.ExecuteNonQuery();
            DataTable csTable   = ReadCursor(cmd, "cs");
            DataTable csCtTable = ReadCursor(cmd, "cs_ct");
            return (csTable, csCtTable);
        }
        catch (Exception ex)
        {
            using var c = new OracleCommand("SELECT USER FROM DUAL", conn);
            throw new Exception($"[DB MYBSH - User:{c.ExecuteScalar()}] {ex.Message}", ex);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // DB load — CUONGDV (cs_ct cursor with SO_ID_CT_MYBSH, SO_ID_CORE, TTRANG, NGAY_NHC)
    // ─────────────────────────────────────────────────────────────────
    private static DataTable LoadCuongdvData( 
        string connStr, int ngayDau, int ngayCuoi, string loaiCt, string dsHs)
    {
        using var conn = new OracleConnection(connStr);
        conn.Open();
        try
        {
            using var cmd = new OracleCommand(
                $"BEGIN {SpCuongdv}(:b_nsdId,:b_pas,:cs_ct,:b_ngay_dau,:b_ngay_cuoi,:b_loai_ct,:b_ds_hs); END;",
                conn)
            {
                CommandType    = CommandType.Text,
                CommandTimeout = 120,
                BindByName     = true,
            };
            cmd.Parameters.Add("b_nsdId",    OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("b_pas",      OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("cs_ct",      OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("b_ngay_dau", OracleDbType.Decimal).Value  = ngayDau;
            cmd.Parameters.Add("b_ngay_cuoi",OracleDbType.Decimal).Value  = ngayCuoi;
            cmd.Parameters.Add("b_loai_ct",  OracleDbType.Varchar2).Value = loaiCt;
            cmd.Parameters.Add("b_ds_hs",    OracleDbType.Varchar2).Value = dsHs;

            cmd.ExecuteNonQuery();
            return ReadCursor(cmd, "cs_ct");
        }
        catch (Exception ex)
        {
            using var c = new OracleCommand("SELECT USER FROM DUAL", conn);
            throw new Exception($"[DB BSH - User:{c.ExecuteScalar()}] {ex.Message}", ex);
        }
    }

    private static DataTable ReadCursor(OracleCommand cmd, string paramName)
    {
        var cursor = (Oracle.ManagedDataAccess.Types.OracleRefCursor)cmd.Parameters[paramName].Value;
        var dt = new DataTable();
        using var reader = cursor.GetDataReader();
        dt.Load(reader);
        return dt;
    }

    // ─────────────────────────────────────────────────────────────────
    // Reconciliation — join DT_BSH cs.so_ct = CUONGDV cs_ct.so_ct
    // ─────────────────────────────────────────────────────────────────
    private static DataTable PerformReconciliation(DataTable csData, DataTable cuongdvData)
    {
        // Build lookup: tập hợp SO_CT có trong CUONGDV
        var matched = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (DataRow row in cuongdvData.Rows)
        {
            string key = row["SO_CT"]?.ToString() ?? "";
            if (!string.IsNullOrEmpty(key)) matched.Add(key);
        }

        var result = new DataTable();
        result.Columns.Add("MA_DVI",     typeof(string));
        result.Columns.Add("SO_ID",      typeof(string));
        result.Columns.Add("NGAY_HT",    typeof(string));
        result.Columns.Add("SO_CT",      typeof(string));
        result.Columns.Add("NDUNG",      typeof(string));
        result.Columns.Add("TIEN",       typeof(string));
        result.Columns.Add("NSDID",      typeof(string));
        result.Columns.Add("NGAY_NH",    typeof(string));
        result.Columns.Add("TTRANG_BT",  typeof(string));

        foreach (DataRow src in csData.Rows)
        {
            string soId     = src["SO_ID"]?.ToString() ?? "";
            string soCt     = src["SO_CT"]?.ToString() ?? "";
            string ttrangBt = matched.Contains(soCt) ? "Đã đồng bộ" : "Chưa đồng bộ";

            result.Rows.Add(
                src["MA_DVI"]?.ToString()  ?? "",
                soId,
                src["NGAY_HT"]?.ToString() ?? "",
                src["SO_CT"]?.ToString()   ?? "",
                src["NDUNG"]?.ToString()   ?? "",
                src["TIEN"]?.ToString()    ?? "",
                src["NSDID"]?.ToString()   ?? "",
                src["NGAY_NH"]?.ToString() ?? "",
                ttrangBt
            );
        }
        return result;
    }

    private static string BuildDsHsParam(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        return string.Join(",", input
            .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));
    }

    // ─────────────────────────────────────────────────────────────────
    // Column definitions
    // ─────────────────────────────────────────────────────────────────
    private static readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["MA_DVI"]    = "Mã đơn vị",
        ["SO_ID"]     = "Số ID",
        ["NGAY_HT"]   = "Ngày hạch toán",
        ["SO_CT"]     = "Số chứng từ",
        ["NDUNG"]     = "Số HSBT",
        ["TIEN"]      = "Tiền",
        ["NSDID"]     = "Người tạo",
        ["NGAY_NH"]   = "Ngày nhập",
        ["TTRANG_BT"] = "TRẠNG THÁI ĐỒNG BỘ",
    };

    private void ApplyColumns()
    {
        dgvMain.DataSource = null;
        dgvMain.Columns.Clear();

        var columnOrder = new[]
        {
            "MA_DVI", "SO_ID", "NGAY_HT", "SO_CT", "NDUNG", "TIEN",
            "NSDID",  "NGAY_NH", "TTRANG_BT"
        };

        foreach (string colName in columnOrder)
        {
            if (!_fullData.Columns.Contains(colName)) continue;
            int w = colName switch
            {
                "MA_DVI"      => 60,
                "SO_ID"       => 120,
                "NGAY_HT"     => 105,
                "SO_CT"       => 160,
                "NDUNG"       => 240,
                "TIEN"        => 90,
                "NSDID"       => 90,
                "NGAY_NH"     => 105,
                "TTRANG_BT"   => 140,
                _             => 100,
            };
            var col = new DataGridViewTextBoxColumn
            {
                Name             = colName,
                DataPropertyName = colName,
                HeaderText       = _headers.TryGetValue(colName, out var h) ? h : colName,
                Width            = w,
                MinimumWidth     = 50,
                AutoSizeMode     = DataGridViewAutoSizeColumnMode.None,
            };

            if (colName == "TTRANG_BT")
            {
                col.DefaultCellStyle.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            string u = colName.ToUpper();
            if (u.StartsWith("NGAY") || u.Contains("SO_ID") || u.StartsWith("TTRANG")
             || u == "MA_DVI" || u == "SO_CT" || u == "NSDID")
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (u == "TIEN")
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvMain.Columns.Add(col);
        }

        // "Xem chi tiết" button column — hiển thị ở tất cả các dòng
        var btnChiTiet = new DataGridViewButtonColumn
        {
            Name                        = "COL_CHITIET",
            HeaderText                  = "Xem chi tiết",
            Text                        = "Xem chi tiết",
            UseColumnTextForButtonValue = true,
            Width                       = 105,
            MinimumWidth                = 90,
            AutoSizeMode                = DataGridViewAutoSizeColumnMode.None,
            FlatStyle                   = FlatStyle.Standard,
        };
        btnChiTiet.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(220, 95, 20);
        btnChiTiet.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
        btnChiTiet.DefaultCellStyle.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
        btnChiTiet.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        btnChiTiet.DefaultCellStyle.Padding   = new Padding(2);
        dgvMain.Columns.Add(btnChiTiet);

        // "Đồng bộ lại" button column — chỉ hiển thị cho dòng "Chưa đồng bộ"
        var btnSync = new DataGridViewButtonColumn
        {
            Name                        = "COL_SYNC",
            HeaderText                  = "Đồng bộ lại",
            Text                        = "Đồng bộ lại",
            UseColumnTextForButtonValue = true,
            Width                       = 110,
            MinimumWidth                = 90,
            AutoSizeMode                = DataGridViewAutoSizeColumnMode.None,
            FlatStyle                   = FlatStyle.Standard,
        };
        btnSync.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(180, 35, 35);
        btnSync.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
        btnSync.DefaultCellStyle.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
        btnSync.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        btnSync.DefaultCellStyle.Padding   = new Padding(2);
        dgvMain.Columns.Add(btnSync);
    }

    // ─────────────────────────────────────────────────────────────────
    // Cell formatting — colour TTRANG_BT column
    // ─────────────────────────────────────────────────────────────────
    private void dgvMain_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || !dgvMain.Columns.Contains("TTRANG_BT")) return;
        string status = dgvMain.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
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
    // Cell painting — ẩn nút "Đồng bộ lại" cho dòng "Đã đồng bộ"
    // ─────────────────────────────────────────────────────────────────
    private void dgvMain_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (!dgvMain.Columns.Contains("COL_SYNC")) return;
        if (e.ColumnIndex != dgvMain.Columns["COL_SYNC"]!.Index) return;
        string status = dgvMain.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
        if (status != "Đã đồng bộ") return;
        // Vẽ ô trống thay cho nút khi đã đồng bộ
        var bc = _statusColors.TryGetValue(status, out var c) ? c.Back : System.Drawing.Color.White;
        using var brush = new System.Drawing.SolidBrush(bc);
        e.Graphics!.FillRectangle(brush, e.CellBounds);
        using var pen = new System.Drawing.Pen(dgvMain.GridColor);
        e.Graphics!.DrawRectangle(pen,
            e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);
        e.Handled = true;
    }

    // ─────────────────────────────────────────────────────────────────
    // Cell click — "Xem chi tiết" (all rows) & "Đồng bộ lại" (Chưa ĐB)
    // ─────────────────────────────────────────────────────────────────
    private async void dgvMain_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        string soId = dgvMain.Rows[e.RowIndex].Cells["SO_ID"].Value?.ToString() ?? "";

        // Xem chi tiết
        if (dgvMain.Columns.Contains("COL_CHITIET") &&
            e.ColumnIndex == dgvMain.Columns["COL_CHITIET"]!.Index)
        {
            if (!string.IsNullOrEmpty(soId)) ShowDetail(soId);
            return;
        }

        // Đồng bộ lại
        if (dgvMain.Columns.Contains("COL_SYNC") &&
            e.ColumnIndex == dgvMain.Columns["COL_SYNC"]!.Index)
        {
            string status = dgvMain.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
            if (status != "Chưa đồng bộ") return;
            if (string.IsNullOrEmpty(soId)) return;

            var confirm = MessageBox.Show(
                $"Đồng bộ lại hồ sơ?\n\nSO_ID: {soId}",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            await CallSyncApiAsync(soId);
        }
    }

    private void ShowDetail(string soId)
    {
        DataTable filtered;
        if (_csCtData.Rows.Count == 0 || !_csCtData.Columns.Contains("SO_ID"))
        {
            filtered = new DataTable();
        }
        else
        {
            filtered = _csCtData.Clone();
            foreach (DataRow row in _csCtData.Rows)
            {
                if (string.Equals(row["SO_ID"]?.ToString(), soId, StringComparison.OrdinalIgnoreCase))
                    filtered.ImportRow(row);
            }
        }

        var popup = new Form
        {
            Text          = $"Chi tiết — Số ID: {soId}  ({filtered.Rows.Count:N0} dòng)",
            Size          = new Size(1152, 260),
            MinimumSize   = new Size(600, 160),
            StartPosition = FormStartPosition.CenterScreen,
            BackColor     = System.Drawing.Color.White,
            Font          = new System.Drawing.Font("Segoe UI", 9.5F),
        };
        if (IconHelper.AppIcon != null) popup.Icon = IconHelper.AppIcon;

        var dgv = new DataGridView
        {
            Dock                          = DockStyle.Fill,
            AllowUserToAddRows            = false,
            AllowUserToDeleteRows         = false,
            AutoGenerateColumns           = false,
            AutoSizeColumnsMode           = DataGridViewAutoSizeColumnsMode.None,
            BackgroundColor               = System.Drawing.Color.White,
            BorderStyle                   = BorderStyle.None,
            EnableHeadersVisualStyles     = false,
            Font                          = new System.Drawing.Font("Segoe UI", 9.5F),
            GridColor                     = System.Drawing.Color.FromArgb(220, 225, 235),
            ReadOnly                      = true,
            RowHeadersVisible             = false,
            SelectionMode                 = DataGridViewSelectionMode.FullRowSelect,
        };

        dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            Alignment          = DataGridViewContentAlignment.MiddleCenter,
            BackColor          = System.Drawing.Color.FromArgb(220, 95, 20),
            ForeColor          = System.Drawing.Color.White,
            Font               = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold),
            SelectionBackColor = System.Drawing.Color.FromArgb(220, 95, 20),
            SelectionForeColor = System.Drawing.Color.White,
        };
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            { BackColor = System.Drawing.Color.FromArgb(255, 248, 235) };
        dgv.RowsDefaultCellStyle = new DataGridViewCellStyle
            { BackColor = System.Drawing.Color.White };

        var detailCols = new (string Field, string Header, int Width, DataGridViewContentAlignment Align)[]
        {
            ("SO_ID",  "Số ID",      130, DataGridViewContentAlignment.MiddleCenter),
            ("MA_DVI", "Mã đơn vị",   80, DataGridViewContentAlignment.MiddleCenter),
            ("BT",     "BT",          50, DataGridViewContentAlignment.MiddleCenter),
            ("MA",     "Số HSBT",    260, DataGridViewContentAlignment.MiddleLeft),
            ("TTOAN",  "TTOAN",       73, DataGridViewContentAlignment.MiddleRight),
            ("PS",     "PS",          35, DataGridViewContentAlignment.MiddleRight),
            ("MA_TK",  "Mã TK",       65, DataGridViewContentAlignment.MiddleCenter),
            ("MA_TKE", "Mã TKE",      65, DataGridViewContentAlignment.MiddleCenter),
            ("NOTE",   "ND",         400, DataGridViewContentAlignment.MiddleLeft),
        };

        foreach (var (field, header, width, align) in detailCols)
        {
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name             = field,
                DataPropertyName = field,
                HeaderText       = header,
                Width            = width,
                MinimumWidth     = 40,
                AutoSizeMode     = DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = { Alignment = align },
            });
        }

        dgv.DataSource = filtered;
        popup.Controls.Add(dgv);
        popup.Show(FindForm());
    }

    // ─────────────────────────────────────────────────────────────────
    // btnSyncAll — sync all "Chưa đồng bộ" rows
    // ─────────────────────────────────────────────────────────────────
    private async void btnSyncAll_Click(object sender, EventArgs e)
    {
        var pending = _fullData.AsEnumerable()
            .Where(r => r["TTRANG_BT"]?.ToString() == "Chưa đồng bộ")
            .Select(r => r["SO_ID"]?.ToString() ?? "")
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        if (pending.Count == 0)
        {
            MessageBox.Show("Không có hồ sơ \"Chưa đồng bộ\".",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var confirm = MessageBox.Show(
            $"Sẽ đồng bộ lại {pending.Count:N0} hồ sơ.\n\nTiếp tục?",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        btnSyncAll.Enabled = false;
        btnKiemTra.Enabled = false;
        int ok = 0, fail = 0;
        for (int i = 0; i < pending.Count; i++)
        {
            string soId = pending[i];
            rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            rtxtApiResponse.Text = $"⏳ Đang đồng bộ {i + 1}/{pending.Count}...\nSO_ID: {soId}";
            Application.DoEvents();
            if (await SyncSingleAsync(soId)) ok++; else fail++;
        }
        btnSyncAll.Enabled = true;
        btnKiemTra.Enabled = true;
        ShowApiResult(
            $"✅ Hoàn tất\n──────────────────────────────\n" +
            $"Tổng: {pending.Count:N0}  ✓ {ok:N0}  ✗ {fail:N0}",
            isError: fail > 0);
    }

    private async Task<bool> SyncSingleAsync(string soId)
    {
        if (!long.TryParse(soId, out long n)) return false;
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, SyncApiUrl);
            req.Headers.Add("username", SyncApiUser);
            req.Headers.Add("password", SyncApiPass);
            req.Content = new StringContent(
                JsonSerializer.Serialize(new { so_id = n }),
                Encoding.UTF8, "application/json");
            using var resp = await _httpClient.SendAsync(req);
            string body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("code", out var code) && code.GetString() == "000")
            {
                UpdateRowStatus(soId);
                return true;
            }
            return false;
        }
        catch { return false; }
    }

    private async Task CallSyncApiAsync(string soId)
    {
        if (!long.TryParse(soId, out long n))
        { ShowApiResult($"⚠ SO_ID không hợp lệ: {soId}", isError: true); return; }

        rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
        rtxtApiResponse.Text = $"⏳ Đang gọi API...\nSO_ID: {soId}";
        var sw = Stopwatch.StartNew();
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, SyncApiUrl);
            req.Headers.Add("username", SyncApiUser);
            req.Headers.Add("password", SyncApiPass);
            req.Content = new StringContent(
                JsonSerializer.Serialize(new { so_id = n }),
                Encoding.UTF8, "application/json");
            using var resp = await _httpClient.SendAsync(req);
            sw.Stop();
            string body = await resp.Content.ReadAsStringAsync();
            bool success = false;
            try
            {
                using var doc = JsonDocument.Parse(body);
                success = doc.RootElement.TryGetProperty("code", out var c) && c.GetString() == "000";
            }
            catch { }
            ShowApiResult(
                $"▶ SO_ID : {soId}\n⏱ {sw.ElapsedMilliseconds} ms  |  HTTP {(int)resp.StatusCode}\n" +
                $"──────────────────────────────\n{TryPrettyJson(body)}",
                isError: !success);
            if (success) UpdateRowStatus(soId);
        }
        catch (Exception ex)
        {
            sw.Stop();
            ShowApiResult($"✘ Lỗi ({sw.ElapsedMilliseconds} ms):\n{ex.Message}", isError: true);
        }
    }

    private void UpdateRowStatus(string soId)
    {
        foreach (DataRow row in _fullData.Rows)
            if (row["SO_ID"]?.ToString() == soId)
                row["TTRANG_BT"] = "Đã đồng bộ";

        var counts = _fullData.AsEnumerable()
            .GroupBy(r => r["TTRANG_BT"]?.ToString() ?? "")
            .ToDictionary(g => g.Key, g => g.Count());
        lblRecordCount.Text =
            $"Tổng: {_fullData.Rows.Count:N0}   " +
            $"✅ Đã ĐB: {counts.GetValueOrDefault("Đã đồng bộ", 0):N0}   " +
            $"❌ Chưa ĐB: {counts.GetValueOrDefault("Chưa đồng bộ", 0):N0}";
        LoadPage();
    }

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
            return JsonSerializer.Serialize(
                JsonDocument.Parse(json),
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }
        catch { return json; }
    }

    // ─────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────
    // Clear data khi chuyển tab
    // ─────────────────────────────────────────────────────────────────
    public void ClearData()
    {
        _fullData            = new DataTable();
        _csCtData            = new DataTable();
        _currentPage         = 1;
        dgvMain.DataSource   = null;
        dgvMain.Columns.Clear();
        lblStatus.ForeColor  = System.Drawing.Color.FromArgb(100, 110, 125);
        lblStatus.Text       = "Chọn điều kiện và nhấn \"Kiểm tra\" để tải dữ liệu.";
        lblRecordCount.Text  = "";
        rtxtApiResponse.Text = "";
        UpdatePagingControls();
    }

    // ─────────────────────────────────────────────────────────────────
    // Paging
    // ─────────────────────────────────────────────────────────────────
    private void LoadPage()
    {
        var data = GetDisplayData();
        if (data.Rows.Count == 0)
        {
            dgvMain.DataSource = null;
            UpdatePagingControls();
            return;
        }
        int skip = (_currentPage - 1) * _pageSize;
        var page = data.Clone();
        foreach (var row in data.AsEnumerable().Skip(skip).Take(_pageSize))
            page.ImportRow(row);
        dgvMain.DataSource = page;
        UpdatePagingControls();
    }

    private void UpdatePagingControls()
    {
        var data = GetDisplayData();
        bool has = data.Rows.Count > 0;
        int from = has ? (_currentPage - 1) * _pageSize + 1 : 0;
        int to   = has ? Math.Min(_currentPage * _pageSize, data.Rows.Count) : 0;
        lblPageInfo.Text     = has ? $"Trang  {_currentPage} / {_totalPages}" : "—";
        lblTotalRecords.Text = has ? $"(Hiển thị {from:N0} – {to:N0} / {data.Rows.Count:N0})" : "";
        btnFirst.Enabled = _currentPage > 1;
        btnPrev.Enabled  = _currentPage > 1;
        btnNext.Enabled  = _currentPage < _totalPages;
        btnLast.Enabled  = _currentPage < _totalPages;
    }

    private void btnFirst_Click(object sender, EventArgs e) { _currentPage = 1; LoadPage(); }
    private void btnPrev_Click(object sender, EventArgs e)  { if (_currentPage > 1) { _currentPage--; LoadPage(); } }
    private void btnNext_Click(object sender, EventArgs e)  { if (_currentPage < _totalPages) { _currentPage++; LoadPage(); } }
    private void btnLast_Click(object sender, EventArgs e)  { _currentPage = _totalPages; LoadPage(); }

    private void cboTrangThai_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_fullData.Rows.Count == 0) return;
        _currentPage = 1;
        LoadPage();
    }

    private void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cboPageSize.SelectedItem is null) return;
        _pageSize    = int.Parse(cboPageSize.SelectedItem.ToString()!);
        _currentPage = 1;
        LoadPage();
    }
}
