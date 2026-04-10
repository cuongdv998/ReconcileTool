using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Oracle.ManagedDataAccess.Client;
using ReconcileTool.UI.Config;

namespace ReconcileTool.UI.Forms;

public partial class GiamDinhControl : UserControl
{
    private const string SpCoreProc = "DT_BSH.PBH_DSOAT_CORE_HSGD";
    private const string SpBshProc  = "CUONGDV.PBH_DSOAT_MYBSH_HSGD";
    private const string SyncApiUrl = "https://openapi.bshc.vn/api/Core/Claim/HSGDPushToCore";
    // ─────────────────────────────────────────────────────────────────

    public static ApiCredentialConfig ApiCredential { get; set; } = new();

    private static readonly HttpClient _httpClient = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }) { Timeout = TimeSpan.FromSeconds(30) };

    private DataTable _fullData = new();
    private int _currentPage = 1;
    private int _pageSize = 20;
    private int _totalPages
        => GetDisplayData() is var d && d.Rows.Count == 0
            ? 1
            : (int)Math.Ceiling(d.Rows.Count / (double)_pageSize);

    private DataTable GetDisplayData()
    {
        string filter = cboLocTrangThai?.SelectedItem?.ToString() ?? "Toàn bộ";
        if (filter == "Toàn bộ") return _fullData;
        var result = _fullData.Clone();
        foreach (DataRow row in _fullData.Rows)
            if (row["TTRANG_BT"]?.ToString() != "Đã đồng bộ")
                result.ImportRow(row);
        return result;
    }

    private static readonly Dictionary<string, (System.Drawing.Color Back, System.Drawing.Color Fore)> _statusColors = new()
    {
        ["Đã đồng bộ"]      = (System.Drawing.Color.FromArgb(210, 245, 215), System.Drawing.Color.FromArgb(25, 110, 45)),
        ["Lệch trạng thái"] = (System.Drawing.Color.FromArgb(255, 244, 180), System.Drawing.Color.FromArgb(160, 95, 0)),
        ["Chưa đồng bộ"]    = (System.Drawing.Color.FromArgb(255, 218, 218), System.Drawing.Color.FromArgb(180, 35, 35)),
        ["???"]             = (System.Drawing.Color.FromArgb(225, 225, 235), System.Drawing.Color.FromArgb(80, 80, 100)),
    };

    public GiamDinhControl()
    {
        InitializeComponent();
        dgvResult.CellFormatting   += dgvResult_CellFormatting;
        dgvResult.CellPainting     += dgvResult_CellPainting;
        dgvResult.CellContentClick += dgvResult_CellContentClick;
        dgvResult.CellClick        += dgvResult_CellClick;
        txtSoHs.TextChanged        += txtSoHs_TextChanged;
        UpdatePagingControls();
    }

    private bool _updatingTxt = false;
    private void txtSoHs_TextChanged(object? sender, EventArgs e)
    {
        if (_updatingTxt) return;
        string text = txtSoHs.Text;
        if (!text.Contains('\n') && !text.Contains('\r')) return;
        _updatingTxt = true;
        int sel = txtSoHs.SelectionStart;
        string newText = text.Replace("\r\n", ",").Replace("\r", ",").Replace("\n", ",");
        txtSoHs.Text = newText;
        txtSoHs.SelectionStart = Math.Min(sel, newText.Length);
        _updatingTxt = false;
    }

    private void btnKiemTra_MouseEnter(object? sender, EventArgs e)
        => btnKiemTra.BackColor = System.Drawing.Color.FromArgb(175, 65, 10);
    private void btnKiemTra_MouseLeave(object? sender, EventArgs e)
        => btnKiemTra.BackColor = System.Drawing.Color.FromArgb(220, 95, 20);

    private void cboLocTrangThai_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_fullData.Rows.Count == 0) return;
        _currentPage = 1;
        LoadPage();
    }

    // ─────────────────────────────────────────────────────────────────
    // Kiểm tra
    // ─────────────────────────────────────────────────────────────────
    private async void btnKiemTra_Click(object sender, EventArgs e)
    {
        var cfg = AppConfig.OracleConfig;
        if (string.IsNullOrWhiteSpace(cfg.MyBshHost) || string.IsNullOrWhiteSpace(cfg.MyBshUser))
        {
            MessageBox.Show("Vui lòng cấu hình và lưu thông tin kết nối DB MYBSH.",
                "Chưa cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
        }
        if (string.IsNullOrWhiteSpace(cfg.BshHost) || string.IsNullOrWhiteSpace(cfg.BshUser))
        {
            MessageBox.Show("Vui lòng cấu hình và lưu thông tin kết nối DB BSH.",
                "Chưa cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
        }

        int ngayDau  = int.Parse(dtpNgayDau.Value.ToString("yyyyMMdd"));
        int ngayCuoi = int.Parse(dtpNgayCuoi.Value.ToString("yyyyMMdd"));
        if (ngayDau > ngayCuoi)
        {
            MessageBox.Show("Ngày đầu không được lớn hơn ngày cuối.",
                "Lỗi ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
        }

        string connMyBsh = cfg.BuildConnectionString(isBsh: false);
        string connBsh   = cfg.BuildConnectionString(isBsh: true);
        string dsHs      = BuildDsHsParam(txtSoHs.Text);
        const string bTtrang = "D";

        SetLoadingState(true);
        try
        {
            var taskCore = Task.Run(() => LoadCoreData(connMyBsh, ngayDau, ngayCuoi, bTtrang, dsHs));
            var taskBsh  = Task.Run(() => LoadBshSideData(connBsh, ngayDau, ngayCuoi, bTtrang, dsHs));
            await Task.WhenAll(taskCore, taskBsh);

            DataTable coreData = taskCore.Result;
            DataTable bshData  = taskBsh.Result;

            _fullData = PerformReconciliation(coreData, bshData);
            _currentPage = 1;
            _pageSize = int.TryParse(cboPageSize.SelectedItem?.ToString(), out var ps) ? ps : 20;

            ApplyColumns(_fullData);
            LoadPage();

            var counts = _fullData.AsEnumerable()
                .GroupBy(r => r["TTRANG_BT"]?.ToString() ?? "")
                .ToDictionary(g => g.Key, g => g.Count());

            lblStatus.ForeColor = System.Drawing.Color.FromArgb(34, 139, 34);
            lblStatus.Text = $"✔  Tải xong lúc {DateTime.Now:HH:mm:ss}  |  HS Giám định  |  " +
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

    private static DataTable LoadCoreData(string connStr, int ngayDau, int ngayCuoi,
                                          string bTtrang, string dsHs)
    {
        using var conn = new OracleConnection(connStr);
        conn.Open();
        try
        {
            using var cmd = new OracleCommand(
                $"BEGIN {SpCoreProc}(:b_nsdId,:b_pas,:cs_ct,:b_ngay_dau,:b_ngay_cuoi,:b_ttrang,:b_ds_hs); END;",
                conn) { CommandType = CommandType.Text, CommandTimeout = 120, BindByName = true };
            cmd.Parameters.Add("b_nsdId",    OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("b_pas",      OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("cs_ct",      OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("b_ngay_dau", OracleDbType.Decimal).Value  = ngayDau;
            cmd.Parameters.Add("b_ngay_cuoi",OracleDbType.Decimal).Value  = ngayCuoi;
            cmd.Parameters.Add("b_ttrang",   OracleDbType.Varchar2).Value = bTtrang;
            cmd.Parameters.Add("b_ds_hs",    OracleDbType.Varchar2).Value = dsHs;
            return ExecuteRefCursor(cmd, "cs_ct");
        }
        catch (Exception ex)
        {
            using var c = new OracleCommand("SELECT USER FROM DUAL", conn);
            throw new Exception($"[DB MYBSH - User:{c.ExecuteScalar()}] {ex.Message}", ex);
        }
    }

    private static DataTable LoadBshSideData(string connStr, int ngayDau, int ngayCuoi,
                                             string bTtrang, string dsHs)
    {
        using var conn = new OracleConnection(connStr);
        conn.Open();
        try
        {
            using var cmd = new OracleCommand(
                $"BEGIN {SpBshProc}(:b_nsdId,:b_pas,:cs_ct,:b_ngay_dau,:b_ngay_cuoi,:b_ttrang,:b_ds_hs); END;",
                conn) { CommandType = CommandType.Text, CommandTimeout = 120, BindByName = true };
            cmd.Parameters.Add("b_nsdId",    OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("b_pas",      OracleDbType.Varchar2).Value = "";
            cmd.Parameters.Add("cs_ct",      OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("b_ngay_dau", OracleDbType.Decimal).Value  = ngayDau;
            cmd.Parameters.Add("b_ngay_cuoi",OracleDbType.Decimal).Value  = ngayCuoi;
            cmd.Parameters.Add("b_ttrang",   OracleDbType.Varchar2).Value = bTtrang;
            cmd.Parameters.Add("b_ds_hs",    OracleDbType.Varchar2).Value = dsHs;
            return ExecuteRefCursor(cmd, "cs_ct");
        }
        catch (Exception ex)
        {
            using var c = new OracleCommand("SELECT USER FROM DUAL", conn);
            throw new Exception($"[DB BSH - User:{c.ExecuteScalar()}] {ex.Message}", ex);
        }
    }

    private static DataTable ExecuteRefCursor(OracleCommand cmd, string cursorParam)
    {
        cmd.ExecuteNonQuery();
        var cursor = (Oracle.ManagedDataAccess.Types.OracleRefCursor)cmd.Parameters[cursorParam].Value;
        var dt = new DataTable();
        using var reader = cursor.GetDataReader();
        dt.Load(reader);
        return dt;
    }

    private static DataTable PerformReconciliation(DataTable coreData, DataTable bshData)
    {
        // BSH lookup: SO_ID → (SO_ID_CORE, TTRANG, NGAY_NHC)
        var lookup = new Dictionary<string, (string SoIdCore, string Ttrang, string NgayNhc)>(
            StringComparer.OrdinalIgnoreCase);
        foreach (DataRow row in bshData.Rows)
        {
            string soId = row["SO_ID"]?.ToString() ?? "";
            if (!string.IsNullOrEmpty(soId))
                lookup[soId] = (row["SO_ID_CORE"]?.ToString() ?? "",
                                row["TTRANG"]?.ToString() ?? "",
                                row["NGAY_NHC"]?.ToString() ?? "");
        }

        // Kết quả: các cột từ MyBSH + đối soát từ BSH
        var result = new DataTable();
        result.Columns.Add("SO_ID_MYBSH",  typeof(string));
        result.Columns.Add("MA_DVI",       typeof(string));
        result.Columns.Add("SO_HS",        typeof(string));
        result.Columns.Add("K_MA",         typeof(string));
        result.Columns.Add("MA",           typeof(string));
        result.Columns.Add("CB_DU",        typeof(string));
        result.Columns.Add("TIEN_TT",      typeof(string));
        result.Columns.Add("TEN_TTRANG",   typeof(string));
        result.Columns.Add("NGAY_DU",      typeof(string));
        result.Columns.Add("SO_ID_CORE",   typeof(string));
        result.Columns.Add("TTRANG_CORE",  typeof(string));
        result.Columns.Add("NGAY_NHC",     typeof(string));
        result.Columns.Add("TTRANG_BT",    typeof(string));

        foreach (DataRow src in coreData.Rows)
        {
            string soId      = src["SO_ID"]?.ToString() ?? "";
            string ttrangDtBsh = src["TTRANG"]?.ToString() ?? "";
            string soIdCore, ttrangCore, ngayNhc, ttrangBt;

            if (!lookup.TryGetValue(soId, out var bsh))
            { soIdCore = ""; ttrangCore = ""; ngayNhc = ""; ttrangBt = "Chưa đồng bộ"; }
            else
            {
                soIdCore   = bsh.SoIdCore;
                ttrangCore = bsh.Ttrang;
                ngayNhc    = bsh.NgayNhc;
                // "DGD" (DT_BSH) tương ứng với "D" (CUONGDV)
                string dtBshMapped = ttrangDtBsh.Equals("DGD", StringComparison.OrdinalIgnoreCase)
                    ? "D" : ttrangDtBsh;
                ttrangBt = string.Equals(dtBshMapped, ttrangCore, StringComparison.OrdinalIgnoreCase)
                    ? "Đã đồng bộ"
                    : "Lệch trạng thái";
            }

            result.Rows.Add(
                soId,
                src["MA_DVI"]?.ToString()   ?? "",
                src["SO_HS"]?.ToString()    ?? "",
                src["K_MA"]?.ToString()     ?? "",
                src["MA"]?.ToString()       ?? "",
                src["CB_DU"]?.ToString()    ?? "",
                src["TIEN_TT"]?.ToString()  ?? "",
                ttrangDtBsh,
                src["NGAY_DU"]?.ToString()  ?? "",
                soIdCore, ttrangCore, ngayNhc, ttrangBt
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

    private static readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SO_ID_MYBSH"] = "Số ID MyBSH",
        ["MA_DVI"]      = "Mã đơn vị",
        ["SO_HS"]       = "Số HS",
        ["K_MA"]        = "Loại",
        ["MA"]          = "Mã",
        ["CB_DU"]       = "Cán bộ duyệt",
        ["TIEN_TT"]     = "Tiền TT",
        ["TEN_TTRANG"]  = "Tình trạng MyBSH",
        ["NGAY_DU"]     = "Ngày duyệt",
        ["SO_ID_CORE"]  = "Số ID Core",
        ["TTRANG_CORE"] = "Tình trạng CORE",
        ["NGAY_NHC"]    = "Ngày ĐB gần nhất",
        ["TTRANG_BT"]   = "TRẠNG THÁI ĐỒNG BỘ",
    };

    private void ApplyColumns(DataTable dt)
    {
        dgvResult.DataSource = null;
        dgvResult.Columns.Clear();
        foreach (DataColumn dc in dt.Columns)
        {
            int w = dc.ColumnName switch
            {
                "SO_ID_MYBSH"  => 130,
                "MA_DVI"       => 75,
                "SO_HS"        => 120,
                "K_MA"         => 40,
                "MA"           => 80,
                "CB_DU"        => 90,
                "TIEN_TT"      => 60,
                "TEN_TTRANG"   => 120,
                "NGAY_DU"      => 100,
                "SO_ID_CORE"   => 110,
                "TTRANG_CORE"  => 50,
                "NGAY_NHC"     => 120,
                "TTRANG_BT"    => 100,
                _ => 100,
            };
            var col = new DataGridViewTextBoxColumn
            {
                Name = dc.ColumnName, DataPropertyName = dc.ColumnName,
                HeaderText = _headers.TryGetValue(dc.ColumnName, out var h) ? h : dc.ColumnName,
                Width = w, MinimumWidth = 50, AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            };
            if (dc.ColumnName == "TTRANG_BT")
            {
                col.DefaultCellStyle.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            string u = dc.ColumnName.ToUpper();
            if (u.StartsWith("NGAY") || u.Contains("SO_ID") || u.StartsWith("TTRANG")
             || u == "TEN_TTRANG" || u == "MA_DVI" || u == "K_MA" || u == "MA")
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (u == "TIEN_TT")
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvResult.Columns.Add(col);
        }
        var actionCol = new DataGridViewButtonColumn
        {
            Name = "COL_ACTION", HeaderText = "Chức năng", Text = "Đồng bộ lại",
            UseColumnTextForButtonValue = true, Width = 115, MinimumWidth = 90,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None, FlatStyle = FlatStyle.Flat,
        };
        actionCol.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(220, 95, 20);
        actionCol.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
        actionCol.DefaultCellStyle.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
        actionCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dgvResult.Columns.Add(actionCol);
    }

    private void dgvResult_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || !dgvResult.Columns.Contains("TTRANG_BT")) return;
        string status = dgvResult.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
        if (!_statusColors.TryGetValue(status, out var colors)) return;
        e.CellStyle.BackColor = colors.Back;
        e.CellStyle.ForeColor = colors.Fore;
        e.CellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(
            Math.Max(0, colors.Back.R - 35), Math.Max(0, colors.Back.G - 35), Math.Max(0, colors.Back.B - 35));
        e.CellStyle.SelectionForeColor = colors.Fore;
    }

    private void dgvResult_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (!dgvResult.Columns.Contains("COL_ACTION")) return;
        if (e.ColumnIndex != dgvResult.Columns["COL_ACTION"]!.Index) return;
        string status = dgvResult.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
        if (status == "Chưa đồng bộ" || status == "Lệch trạng thái") return;
        var bc = _statusColors.TryGetValue(status, out var c) ? c.Back : System.Drawing.Color.White;
        using var brush = new System.Drawing.SolidBrush(bc);
        e.Graphics!.FillRectangle(brush, e.CellBounds);
        using var pen = new System.Drawing.Pen(dgvResult.GridColor);
        e.Graphics!.DrawRectangle(pen, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);
        e.Handled = true;
    }

    private async void dgvResult_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || !dgvResult.Columns.Contains("COL_ACTION")) return;
        if (e.ColumnIndex != dgvResult.Columns["COL_ACTION"]!.Index) return;
        string status = dgvResult.Rows[e.RowIndex].Cells["TTRANG_BT"].Value?.ToString() ?? "";
        if (status != "Chưa đồng bộ" && status != "Lệch trạng thái") return;
        string soId = dgvResult.Rows[e.RowIndex].Cells["SO_ID_MYBSH"].Value?.ToString() ?? "";
        string soHs = dgvResult.Rows[e.RowIndex].Cells["SO_HS"].Value?.ToString() ?? "";
        if (string.IsNullOrEmpty(soId)) { ShowApiResult("⚠ Không tìm thấy SO_ID.", isError: true); return; }
        var confirm = MessageBox.Show($"Đồng bộ lại hồ sơ?\n\nSố ID MyBSH : {soId}\nSố HS        : {soHs}",
            "Xác nhận đồng bộ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;
        await CallSyncApiAsync(soId);
    }

    private void dgvResult_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || !dgvResult.Columns.Contains("SO_ID_MYBSH")) return;
        string soId = dgvResult.Rows[e.RowIndex].Cells["SO_ID_MYBSH"].Value?.ToString() ?? "";
        if (string.IsNullOrEmpty(soId)) return;
        Clipboard.SetText(soId);
        rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(25, 110, 45);
        rtxtApiResponse.Text = $"📋 Đã copy SO_ID MyBSH:\n{soId}";
    }

    private async void btnSyncAll_Click(object sender, EventArgs e)
    {
        var pending = _fullData.AsEnumerable()
            .Where(r => r["TTRANG_BT"]?.ToString() == "Chưa đồng bộ"
                     || r["TTRANG_BT"]?.ToString() == "Lệch trạng thái")
            .Select(r => r["SO_ID_MYBSH"]?.ToString() ?? "")
            .Where(s => !string.IsNullOrEmpty(s)).ToList();
        if (pending.Count == 0) { MessageBox.Show("Không có hồ sơ \"Chưa đồng bộ\".", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (string.IsNullOrWhiteSpace(ApiCredential.ApiUser)) { ShowApiResult("⚠ Chưa cấu hình API.", isError: true); return; }
        var confirm = MessageBox.Show($"Sẽ đồng bộ lại {pending.Count:N0} hồ sơ.\n\nTiếp tục?",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        btnSyncAll.Enabled = false; btnKiemTra.Enabled = false;
        int ok = 0, fail = 0;
        for (int i = 0; i < pending.Count; i++)
        {
            string soId = pending[i];
            rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            rtxtApiResponse.Text = $"⏳ Đang đồng bộ {i + 1}/{pending.Count}...\nSO_ID: {soId}";
            Application.DoEvents();
            if (await SyncSingleAsync(soId)) ok++; else fail++;
        }
        btnSyncAll.Enabled = true; btnKiemTra.Enabled = true;
        ShowApiResult($"✅ Hoàn tất\n──────────────────────────────\nTổng: {pending.Count:N0}  ✓ {ok:N0}  ✗ {fail:N0}", isError: fail > 0);
    }

    private async Task<bool> SyncSingleAsync(string soId)
    {
        if (!long.TryParse(soId, out long n)) return false;
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, SyncApiUrl);
            req.Headers.Add("username", ApiCredential.ApiUser);
            req.Headers.Add("password", ApiCredential.ApiPassword);
            req.Content = new StringContent(JsonSerializer.Serialize(new { so_id = n }), Encoding.UTF8, "application/json");
            using var resp = await _httpClient.SendAsync(req);
            string body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("code", out var code) && code.GetString() == "000")
            { UpdateRowStatus(soId); return true; }
            return false;
        }
        catch { return false; }
    }

    private async Task CallSyncApiAsync(string soId)
    {
        if (string.IsNullOrWhiteSpace(ApiCredential.ApiUser)) { ShowApiResult("⚠ Chưa cấu hình API.", isError: true); return; }
        if (!long.TryParse(soId, out long n)) { ShowApiResult($"⚠ SO_ID không hợp lệ: {soId}", isError: true); return; }
        rtxtApiResponse.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
        rtxtApiResponse.Text = $"⏳ Đang gọi API...\nSO_ID: {soId}";
        var sw = Stopwatch.StartNew();
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, SyncApiUrl);
            req.Headers.Add("username", ApiCredential.ApiUser);
            req.Headers.Add("password", ApiCredential.ApiPassword);
            req.Content = new StringContent(JsonSerializer.Serialize(new { so_id = n }), Encoding.UTF8, "application/json");
            using var resp = await _httpClient.SendAsync(req);
            sw.Stop();
            string body = await resp.Content.ReadAsStringAsync();
            bool success = false;
            try { using var doc = JsonDocument.Parse(body); success = doc.RootElement.TryGetProperty("code", out var c) && c.GetString() == "000"; } catch { }
            ShowApiResult($"▶ SO_ID : {soId}\n⏱ {sw.ElapsedMilliseconds} ms  |  HTTP {(int)resp.StatusCode}\n──────────────────────────────\n{TryPrettyJson(body)}", isError: !success);
            if (success) UpdateRowStatus(soId);
        }
        catch (Exception ex) { sw.Stop(); ShowApiResult($"✘ Lỗi ({sw.ElapsedMilliseconds} ms):\n{ex.Message}", isError: true); }
    }

    private void UpdateRowStatus(string soId)
    {
        foreach (DataRow row in _fullData.Rows)
            if (row["SO_ID_MYBSH"]?.ToString() == soId) row["TTRANG_BT"] = "Đã đồng bộ";
        var counts = _fullData.AsEnumerable().GroupBy(r => r["TTRANG_BT"]?.ToString() ?? "").ToDictionary(g => g.Key, g => g.Count());
        lblRecordCount.Text = $"Tổng: {_fullData.Rows.Count:N0}   ✅ Đã ĐB: {counts.GetValueOrDefault("Đã đồng bộ", 0):N0}   ⚠ Lệch TT: {counts.GetValueOrDefault("Lệch trạng thái", 0):N0}   ❌ Chưa ĐB: {counts.GetValueOrDefault("Chưa đồng bộ", 0):N0}   ❓ ???: {counts.GetValueOrDefault("???", 0):N0}";
        LoadPage();
    }

    private void ShowApiResult(string text, bool isError)
    {
        rtxtApiResponse.ForeColor = isError ? System.Drawing.Color.FromArgb(180, 35, 35) : System.Drawing.Color.FromArgb(25, 110, 45);
        rtxtApiResponse.Text = text;
    }

    private static string TryPrettyJson(string json)
    {
        try { return JsonSerializer.Serialize(JsonDocument.Parse(json), new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }); }
        catch { return json; }
    }

    public void ClearData()
    {
        _fullData            = new DataTable();
        _currentPage         = 1;
        dgvResult.DataSource = null;
        dgvResult.Columns.Clear();
        lblStatus.ForeColor  = System.Drawing.Color.FromArgb(100, 110, 125);
        lblStatus.Text       = "Chọn điều kiện và nhấn \"Kiểm tra\" để tải dữ liệu.";
        lblRecordCount.Text  = "";
        rtxtApiResponse.Text = "";
        UpdatePagingControls();
    }

    private void LoadPage()
    {
        var data = GetDisplayData();
        if (data.Rows.Count == 0) { dgvResult.DataSource = null; UpdatePagingControls(); return; }
        int skip = (_currentPage - 1) * _pageSize;
        var page = data.Clone();
        foreach (var row in data.AsEnumerable().Skip(skip).Take(_pageSize)) page.ImportRow(row);
        dgvResult.DataSource = page;
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
        btnFirst.Enabled = _currentPage > 1; btnPrev.Enabled = _currentPage > 1;
        btnNext.Enabled = _currentPage < _totalPages; btnLast.Enabled = _currentPage < _totalPages;
    }

    private void btnFirst_Click(object sender, EventArgs e) { _currentPage = 1; LoadPage(); }
    private void btnPrev_Click(object sender, EventArgs e)  { if (_currentPage > 1) { _currentPage--; LoadPage(); } }
    private void btnNext_Click(object sender, EventArgs e)  { if (_currentPage < _totalPages) { _currentPage++; LoadPage(); } }
    private void btnLast_Click(object sender, EventArgs e)  { _currentPage = _totalPages; LoadPage(); }
    private void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cboPageSize.SelectedItem is null) return;
        _pageSize = int.Parse(cboPageSize.SelectedItem.ToString()!);
        _currentPage = 1; LoadPage();
    }
}
