using System.Data;
using Oracle.ManagedDataAccess.Client;
using ReconcileTool.UI.Config;

namespace ReconcileTool.UI.Forms;

public partial class BoiThuongV2Control : UserControl
{
    // ── Dữ liệu ──────────────────────────────────────────────────────
    private DataTable _fullData  = new();
    private int       _currentPage = 1;
    private int       _pageSize    = 20;

    private int _totalPages
    {
        get
        {
            var d = GetDisplayData();
            return d.Rows.Count == 0 ? 1 : (int)Math.Ceiling(d.Rows.Count / (double)_pageSize);
        }
    }

    // Lọc local theo TTRANG (dropdown cboLocTrangThai)
    private DataTable GetDisplayData()
    {
        string filter = cboLocTrangThai?.SelectedItem?.ToString() ?? "Toàn bộ";
        if (filter == "Toàn bộ") return _fullData;

        var result = _fullData.Clone();
        foreach (DataRow row in _fullData.Rows)
            if (string.Equals(row["TTRANG"]?.ToString(), filter, StringComparison.OrdinalIgnoreCase))
                result.ImportRow(row);
        return result;
    }

    // ── Màu hàng theo TTRANG ─────────────────────────────────────────
    private static readonly Dictionary<string, (Color Back, Color Fore)> _ttrangColors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["D"]   = (Color.FromArgb(210, 245, 215), Color.FromArgb(25, 110, 45)),
            ["DGD"] = (Color.FromArgb(210, 245, 215), Color.FromArgb(25, 110, 45)),
            ["CD"]  = (Color.FromArgb(255, 248, 235), Color.FromArgb(160, 95, 0)),
            ["KD"]  = (Color.FromArgb(255, 218, 218), Color.FromArgb(180, 35, 35)),
        };

    public BoiThuongV2Control()
    {
        InitializeComponent();
        dgvResult.CellFormatting += dgvResult_CellFormatting;
        txtSoHs.TextChanged      += txtSoHs_TextChanged;
        UpdatePagingControls();
    }

    // ─────────────────────────────────────────────────────────────────
    // Button hover
    // ─────────────────────────────────────────────────────────────────
    private void btnKiemTra_MouseEnter(object? sender, EventArgs e)
        => btnKiemTra.BackColor = Color.FromArgb(175, 65, 10);
    private void btnKiemTra_MouseLeave(object? sender, EventArgs e)
        => btnKiemTra.BackColor = Color.FromArgb(220, 95, 20);

    private void cboLocTrangThai_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_fullData.Rows.Count == 0) return;
        _currentPage = 1;
        LoadPage();
    }

    // ─────────────────────────────────────────────────────────────────
    // Kiểm tra — gọi SP DT_BSH.PBH_DSOAT_CORE_HSBT_V2
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

        int ngayDau  = int.Parse(dtpNgayDau.Value.ToString("yyyyMMdd"));
        int ngayCuoi = int.Parse(dtpNgayCuoi.Value.ToString("yyyyMMdd"));

        if (ngayDau > ngayCuoi)
        {
            MessageBox.Show("Ngày đầu không được lớn hơn ngày cuối.",
                "Lỗi ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string connMyBsh = cfg.BuildConnectionString(isBsh: false);
        string dsHs      = BuildDsHsParam(txtSoHs.Text);

        SetLoadingState(true);
        try
        {
            _fullData = await Task.Run(() => LoadData(connMyBsh, ngayDau, ngayCuoi, dsHs));

            _currentPage = 1;
            _pageSize    = int.TryParse(cboPageSize.SelectedItem?.ToString(), out var ps) ? ps : 20;

            ApplyColumns();
            LoadPage();

            // Thống kê theo TTRANG
            var counts = _fullData.AsEnumerable()
                .GroupBy(r => r["TTRANG"]?.ToString() ?? "")
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());

            string countText = string.Join("   ",
                counts.Select(kv => $"{kv.Key}: {kv.Value:N0}"));

            lblStatus.ForeColor = Color.FromArgb(34, 139, 34);
            lblStatus.Text =
                $"✔  Tải xong lúc {DateTime.Now:HH:mm:ss}  |  " +
                $"{dtpNgayDau.Value:dd/MM/yyyy} → {dtpNgayCuoi.Value:dd/MM/yyyy}";

            lblRecordCount.Text =
                $"Tổng: {_fullData.Rows.Count:N0}   {countText}";
        }
        catch (Exception ex)
        {
            lblStatus.ForeColor = Color.FromArgb(200, 50, 50);
            lblStatus.Text      = "✘  Lỗi: " + ex.Message.Split('\n')[0];
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
    // Gọi SP DT_BSH.PBH_DSOAT_CORE_HSBT_V2
    // Trả về: SO_ID, SO_HS, TTRANG, TEN_TTRANG, NG_DUYET, UOC, THUE, TTOAN
    // ─────────────────────────────────────────────────────────────────
    private static DataTable LoadData(string connStr, int ngayDau, int ngayCuoi, string dsHs)
    {
        using var conn = new OracleConnection(connStr);
        conn.Open();
        try
        {
            using var cmd = new OracleCommand(
                "BEGIN DT_BSH.PBH_DSOAT_CORE_HSBT_V2(:b_nsdId,:b_pas,:cs_ct,:b_ngay_dau,:b_ngay_cuoi,:b_ttrang,:b_ds_hs); END;",
                conn)
            {
                CommandType    = CommandType.Text,
                CommandTimeout = 120,
                BindByName     = true
            };

            cmd.Parameters.Add("b_nsdId",    OracleDbType.Decimal).Value   = 0;
            cmd.Parameters.Add("b_pas",       OracleDbType.Varchar2).Value  = "";
            cmd.Parameters.Add("cs_ct",       OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("b_ngay_dau",  OracleDbType.Decimal).Value  = ngayDau;
            cmd.Parameters.Add("b_ngay_cuoi", OracleDbType.Decimal).Value  = ngayCuoi;
            cmd.Parameters.Add("b_ttrang",    OracleDbType.Varchar2).Value  = "";
            cmd.Parameters.Add("b_ds_hs",     OracleDbType.Varchar2).Value  = dsHs;

            cmd.ExecuteNonQuery();

            var cursor = (Oracle.ManagedDataAccess.Types.OracleRefCursor)
                         cmd.Parameters["cs_ct"].Value;
            var dt = new DataTable();
            using var reader = cursor.GetDataReader();
            dt.Load(reader);
            return dt;
        }
        catch (Exception ex)
        {
            using var cmdUser = new OracleCommand("SELECT USER FROM DUAL", conn);
            string u = cmdUser.ExecuteScalar()?.ToString() ?? "?";
            throw new Exception($"[DB MYBSH V2 - User:{u}] {ex.Message}", ex);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Chuẩn hoá input textbox → chuỗi "HS001,HS002,..."
    // ─────────────────────────────────────────────────────────────────
    private static string BuildDsHsParam(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        return string.Join(",",
            input.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(s => s.Trim())
                 .Where(s => !string.IsNullOrEmpty(s)));
    }

    // Auto-convert newlines → commas khi paste
    private bool _updatingTxtSoHs = false;
    private void txtSoHs_TextChanged(object? sender, EventArgs e)
    {
        if (_updatingTxtSoHs) return;
        string text = txtSoHs.Text;
        if (!text.Contains('\n') && !text.Contains('\r')) return;
        _updatingTxtSoHs = true;
        int sel     = txtSoHs.SelectionStart;
        string newText = text.Replace("\r\n", ",").Replace("\r", ",").Replace("\n", ",");
        txtSoHs.Text = newText;
        txtSoHs.SelectionStart = Math.Min(sel, newText.Length);
        _updatingTxtSoHs = false;
    }

    // ─────────────────────────────────────────────────────────────────
    // Định nghĩa cột — AutoGenerateColumns = false
    // ─────────────────────────────────────────────────────────────────
    private void ApplyColumns()
    {
        dgvResult.AutoGenerateColumns = false;
        dgvResult.DataSource          = null;
        dgvResult.Columns.Clear();

        var cols = new (string Name, string Header, int Width, DataGridViewContentAlignment Align, string Format)[]
        {
            ("SO_ID",      "Số ID",               120, DataGridViewContentAlignment.MiddleCenter, ""),
            ("SO_HS",      "Số hồ sơ",            175, DataGridViewContentAlignment.MiddleLeft,   ""),
            ("TTRANG",     "Tình trạng",            85, DataGridViewContentAlignment.MiddleCenter, ""),
            ("TEN_TTRANG", "Tên tình trạng",       170, DataGridViewContentAlignment.MiddleLeft,   ""),
            ("NG_DUYET",   "Ngày duyệt",           105, DataGridViewContentAlignment.MiddleCenter, ""),
            ("UOC",        "Ước (VNĐ)",            120, DataGridViewContentAlignment.MiddleRight,  "N0"),
            ("THUE",       "Thuế (VNĐ)",           110, DataGridViewContentAlignment.MiddleRight,  "N0"),
            ("TTOAN",      "Thanh toán (VNĐ)",     140, DataGridViewContentAlignment.MiddleRight,  "N0"),
        };

        foreach (var (name, header, width, align, fmt) in cols)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name             = name,
                DataPropertyName = name,
                HeaderText       = header,
                Width            = width,
                MinimumWidth     = 60,
                AutoSizeMode     = DataGridViewAutoSizeColumnMode.None,
            };
            col.DefaultCellStyle.Alignment = align;
            if (!string.IsNullOrEmpty(fmt))
                col.DefaultCellStyle.Format = fmt;
            if (name == "TTRANG")
                col.DefaultCellStyle.Font =
                    new Font("Segoe UI", 9.5F, FontStyle.Bold);

            dgvResult.Columns.Add(col);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Tô màu hàng theo TTRANG
    // ─────────────────────────────────────────────────────────────────
    private void dgvResult_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (!dgvResult.Columns.Contains("TTRANG")) return;

        string ttrang = dgvResult.Rows[e.RowIndex]
                                  .Cells["TTRANG"].Value?.ToString() ?? "";

        if (!_ttrangColors.TryGetValue(ttrang, out var colors)) return;

        e.CellStyle.BackColor = colors.Back;
        e.CellStyle.ForeColor = colors.Fore;
        e.CellStyle.SelectionBackColor = Color.FromArgb(
            Math.Max(0, colors.Back.R - 35),
            Math.Max(0, colors.Back.G - 35),
            Math.Max(0, colors.Back.B - 35));
        e.CellStyle.SelectionForeColor = colors.Fore;
    }

    // ─────────────────────────────────────────────────────────────────
    // Clear data khi chuyển tab
    // ─────────────────────────────────────────────────────────────────
    public void ClearData()
    {
        _fullData            = new DataTable();
        _currentPage         = 1;
        dgvResult.DataSource = null;
        dgvResult.Columns.Clear();
        lblStatus.ForeColor  = Color.FromArgb(100, 110, 125);
        lblStatus.Text       = "Chọn điều kiện và nhấn \"Kiểm tra\" để tải dữ liệu.";
        lblRecordCount.Text  = "";
        UpdatePagingControls();
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

    private void cboPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cboPageSize.SelectedItem is null) return;
        _pageSize    = int.Parse(cboPageSize.SelectedItem.ToString()!);
        _currentPage = 1;
        LoadPage();
    }

    // ─────────────────────────────────────────────────────────────────
    // Trạng thái loading
    // ─────────────────────────────────────────────────────────────────
    private void SetLoadingState(bool loading)
    {
        btnKiemTra.Enabled  = !loading;
        btnKiemTra.Text     = loading ? "Đang tải..." : "Kiểm tra";
        cboPageSize.Enabled = !loading;
        btnFirst.Enabled    = !loading && _currentPage > 1;
        btnPrev.Enabled     = !loading && _currentPage > 1;
        btnNext.Enabled     = !loading && _currentPage < _totalPages;
        btnLast.Enabled     = !loading && _currentPage < _totalPages;

        if (loading)
        {
            lblStatus.ForeColor = Color.FromArgb(100, 110, 125);
            lblStatus.Text      = "⏳  Đang tải dữ liệu từ DB MYBSH...";
            lblRecordCount.Text = "";
        }
    }
}
