using System.Net.Http;
using System.Text.Json;
using Oracle.ManagedDataAccess.Client;
using ReconcileTool.UI.Config;

namespace ReconcileTool.UI.Forms;

public partial class MainForm : Form
{
    private readonly BoiThuongControl   _boiThuongControl   = new();
    private readonly BoiThuongV2Control _boiThuongV2Control = new();
    private readonly GiamDinhControl    _giamDinhControl    = new();
    private readonly ThanhToanControl   _thanhToanControl   = new();
    private readonly List<string>     _userRoles;

    public MainForm(List<string>? roles = null, string? userName = null)
    {
        _userRoles = roles ?? new List<string>();

        InitializeComponent();
        if (DesignMode) return;

        if (IconHelper.AppIcon != null) this.Icon = IconHelper.AppIcon;

        lblVersion.Text = $"v{AppVersion}";
        if (!string.IsNullOrEmpty(userName))
            this.Text = $"Công Cụ Đối Soát  —  {userName}";

        LoadSavedConfig();
        btnSaveConfig.MouseEnter += btnSaveConfig_MouseEnter;
        btnSaveConfig.MouseLeave += btnSaveConfig_MouseLeave;

        // Khởi tạo content controls
        _boiThuongControl.Dock = DockStyle.Fill;
        _boiThuongControl.Visible = false;
        _boiThuongV2Control.Dock = DockStyle.Fill;
        _boiThuongV2Control.Visible = false;
        _giamDinhControl.Dock  = DockStyle.Fill;
        _giamDinhControl.Visible  = false;
        _thanhToanControl.Dock = DockStyle.Fill;
        _thanhToanControl.Visible = false;
        pnlContent.Controls.Add(_boiThuongControl);
        pnlContent.Controls.Add(_boiThuongV2Control);
        pnlContent.Controls.Add(_giamDinhControl);
        pnlContent.Controls.Add(_thanhToanControl);

        // Ẩn tab theo phân quyền
        ApplyRoleVisibility();

        // Mặc định: vào tab đầu tiên được phép, hoặc Cấu hình nếu chưa có config
        if (IsConfigComplete())
            SetActiveMenu(GetDefaultMenuButton(), GetDefaultControl());
        else
            SetActiveMenu(btnMenuCauHinh, pnlDbConfig);
    }

    private bool HasRole(string role)
        => _userRoles.Any(r => string.Equals(r.Replace(" ", ""), role.Replace(" ", ""),
                                StringComparison.OrdinalIgnoreCase));

    private void ApplyRoleVisibility()
    {
        btnMenuBoiThuong.Visible   = HasRole("BOI_THUONG");
        btnMenuBoiThuongV2.Visible = HasRole("BOI_THUONG_V2");
        btnMenuGiamDinh.Visible    = HasRole("GIAM_DINH");
        btnMenuThanhToan.Visible   = HasRole("THANH_TOAN");
    }

    private Button GetDefaultMenuButton()
    {
        if (HasRole("BOI_THUONG"))    return btnMenuBoiThuong;
        if (HasRole("BOI_THUONG_V2")) return btnMenuBoiThuongV2;
        if (HasRole("GIAM_DINH"))     return btnMenuGiamDinh;
        if (HasRole("THANH_TOAN"))    return btnMenuThanhToan;
        return btnMenuCauHinh;
    }

    private Control GetDefaultControl()
    {
        if (HasRole("BOI_THUONG"))    return _boiThuongControl;
        if (HasRole("BOI_THUONG_V2")) return _boiThuongV2Control;
        if (HasRole("GIAM_DINH"))     return _giamDinhControl;
        if (HasRole("THANH_TOAN"))    return _thanhToanControl;
        return pnlDbConfig;
    }

    private bool IsConfigComplete()
    {
        var cfg = AppConfig.OracleConfig;
        return cfg != null
            && !string.IsNullOrWhiteSpace(cfg.BshHost)
            && !string.IsNullOrWhiteSpace(cfg.BshUser)
            && !string.IsNullOrWhiteSpace(cfg.MyBshHost)
            && !string.IsNullOrWhiteSpace(cfg.MyBshUser);
    }

    private void SetActiveMenu(Button activeBtn, Control activeControl)
    {
        // Reset tất cả tab
        foreach (Button btn in new[] { btnMenuCauHinh, btnMenuBoiThuong, btnMenuBoiThuongV2, btnMenuGiamDinh, btnMenuThanhToan })
        {
            if (!btn.Visible) continue;
            btn.Font      = new System.Drawing.Font("Segoe UI", 10F);
            btn.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            btn.BackColor = System.Drawing.Color.White;
        }

        // Active tab
        activeBtn.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        activeBtn.ForeColor = System.Drawing.Color.FromArgb(220, 95, 20);
        activeBtn.BackColor = System.Drawing.Color.FromArgb(255, 237, 213);

        // Ẩn + clear data tất cả content
        pnlDbConfig.Visible       = false;
        lblPlaceholder.Visible    = false;
        if (_boiThuongControl.Visible)   { _boiThuongControl.ClearData();   _boiThuongControl.Visible   = false; }
        if (_boiThuongV2Control.Visible) { _boiThuongV2Control.ClearData(); _boiThuongV2Control.Visible = false; }
        if (_giamDinhControl.Visible)    { _giamDinhControl.ClearData();    _giamDinhControl.Visible    = false; }
        if (_thanhToanControl.Visible)   { _thanhToanControl.ClearData();   _thanhToanControl.Visible   = false; }

        // Hiện content tương ứng
        activeControl.Visible = true;
    }

    private void btnMenuCauHinh_Click(object sender, EventArgs e)
        => SetActiveMenu(btnMenuCauHinh, pnlDbConfig);

    private void btnMenuBoiThuong_Click(object sender, EventArgs e)
    {
        if (!IsConfigComplete())
        {
            MessageBox.Show("Vui lòng cài đặt cấu hình trước khi sử dụng.",
                "Chưa có cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetActiveMenu(btnMenuCauHinh, pnlDbConfig);
            return;
        }
        SetActiveMenu(btnMenuBoiThuong, _boiThuongControl);
    }

    private void btnMenuBoiThuongV2_Click(object sender, EventArgs e)
    {
        if (!IsConfigComplete())
        {
            MessageBox.Show("Vui lòng cài đặt cấu hình trước khi sử dụng.",
                "Chưa có cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetActiveMenu(btnMenuCauHinh, pnlDbConfig);
            return;
        }
        SetActiveMenu(btnMenuBoiThuongV2, _boiThuongV2Control);
    }

    private void btnMenuGiamDinh_Click(object sender, EventArgs e)
    {
        if (!IsConfigComplete())
        {
            MessageBox.Show("Vui lòng cài đặt cấu hình trước khi sử dụng.",
                "Chưa có cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetActiveMenu(btnMenuCauHinh, pnlDbConfig);
            return;
        }
        SetActiveMenu(btnMenuGiamDinh, _giamDinhControl);
    }

    private void btnMenuThanhToan_Click(object sender, EventArgs e)
    {
        if (!IsConfigComplete())
        {
            MessageBox.Show("Vui lòng cài đặt cấu hình trước khi sử dụng.",
                "Chưa có cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetActiveMenu(btnMenuCauHinh, pnlDbConfig);
            return;
        }
        SetActiveMenu(btnMenuThanhToan, _thanhToanControl);
    }

    private void btnSaveConfig_MouseEnter(object? sender, EventArgs e)
        => btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(175, 65, 10);

    private void btnSaveConfig_MouseLeave(object? sender, EventArgs e)
        => btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(220, 95, 20);

    private void LoadSavedConfig()
    {
        // Oracle config
        var cfg = ConfigStorage.Load();
        AppConfig.OracleConfig = cfg;

        txtBshHost.Text    = cfg.BshHost;
        txtBshPort.Text    = cfg.BshPort;
        txtBshService.Text = cfg.BshService;
        txtBshUser.Text    = cfg.BshUser;
        txtBshPassword.Text = cfg.BshPassword;

        txtMyBshHost.Text    = cfg.MyBshHost;
        txtMyBshPort.Text    = cfg.MyBshPort;
        txtMyBshService.Text = cfg.MyBshService;
        txtMyBshUser.Text    = cfg.MyBshUser;
        txtMyBshPassword.Text = cfg.MyBshPassword;

        // API credential
        var apiCred = ConfigStorage.LoadApiCredential();
        BoiThuongControl.ApiCredential    = apiCred;
        // BoiThuongV2: no API credential needed
        GiamDinhControl.ApiCredential     = apiCred;
        ThanhToanControl.ApiCredential    = apiCred;
        txtApiUser.Text     = apiCred.ApiUser;
        txtApiPassword.Text = apiCred.ApiPassword;
    }

    // ── Lưu tất cả: Oracle DB + API credential ───────────────────────
    private void btnSaveConfig_Click(object sender, EventArgs e)
    {
        SaveAllConfig();
        MessageBox.Show("Đã lưu tất cả cấu hình thành công.",
            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SaveAllConfig()
    {
        // Oracle
        var cfg = new OracleConnectionConfig
        {
            BshHost    = txtBshHost.Text.Trim(),
            BshPort    = txtBshPort.Text.Trim(),
            BshService = txtBshService.Text.Trim(),
            BshUser    = txtBshUser.Text.Trim(),
            BshPassword = txtBshPassword.Text,

            MyBshHost    = txtMyBshHost.Text.Trim(),
            MyBshPort    = txtMyBshPort.Text.Trim(),
            MyBshService = txtMyBshService.Text.Trim(),
            MyBshUser    = txtMyBshUser.Text.Trim(),
            MyBshPassword = txtMyBshPassword.Text
        };
        ConfigStorage.Save(cfg);
        AppConfig.OracleConfig = cfg;

        // API credential
        var apiCred = new ApiCredentialConfig
        {
            ApiUser     = txtApiUser.Text.Trim(),
            ApiPassword = txtApiPassword.Text
        };
        ConfigStorage.SaveApiCredential(apiCred);
        BoiThuongControl.ApiCredential    = apiCred;
        // BoiThuongV2: no API credential needed
        GiamDinhControl.ApiCredential     = apiCred;
        ThanhToanControl.ApiCredential    = apiCred;
    }

    private async void btnTestBSH_Click(object sender, EventArgs e)
    {
        await TestConnectionAsync(
            host:        txtBshHost.Text.Trim(),
            port:        txtBshPort.Text.Trim(),
            service:     txtBshService.Text.Trim(),
            user:        txtBshUser.Text.Trim(),
            password:    txtBshPassword.Text,
            statusLabel: lblBshStatus,
            testButton:  btnTestBSH
        );
    }

    private async void btnTestMyBSH_Click(object sender, EventArgs e)
    {
        await TestConnectionAsync(
            host:        txtMyBshHost.Text.Trim(),
            port:        txtMyBshPort.Text.Trim(),
            service:     txtMyBshService.Text.Trim(),
            user:        txtMyBshUser.Text.Trim(),
            password:    txtMyBshPassword.Text,
            statusLabel: lblMyBshStatus,
            testButton:  btnTestMyBSH
        );
    }

    private async Task TestConnectionAsync(
        string host, string port, string service,
        string user, string password,
        Label statusLabel, Button testButton)
    {
        testButton.Enabled = false;
        statusLabel.ForeColor = Color.Gray;
        statusLabel.Text = "Đang kết nối...";

        var connStr = $"User Id={user};Password={password};Data Source={host}:{port}/{service};Connection Timeout=5;";

        try
        {
            await Task.Run(() =>
            {
                using var conn = new OracleConnection(connStr);
                conn.Open();
            });

            statusLabel.ForeColor = Color.FromArgb(34, 139, 34);
            statusLabel.Text = "Kết nối thành công!";

            // Tự động lưu tất cả sau khi kết nối thành công
            SaveAllConfig();
        }
        catch (Exception ex)
        {
            statusLabel.ForeColor = Color.FromArgb(200, 50, 50);
            statusLabel.Text = "Lỗi: " + ex.Message.Split('\n')[0];
        }
        finally
        {
            testButton.Enabled = true;
        }
    }

    private void lblBshHost_Click(object sender, EventArgs e) { }

    // ── Cập nhật phiên bản ────────────────────────────────────────────
    private const string AppVersion = "1.0.0";
    private const string GithubRepo = "cuongdv998/ReconcileTool";

    private static readonly HttpClient _updateClient = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }) { Timeout = TimeSpan.FromSeconds(60) };

    private async void btnUpdate_Click(object sender, EventArgs e)
    {
        btnUpdate.Enabled = false;
        btnUpdate.Text = "Đang kiểm tra...";
        try
        {
            // 1. Gọi GitHub API lấy release mới nhất
            _updateClient.DefaultRequestHeaders.UserAgent.ParseAdd("ReconcileTool/1.0");
            string apiUrl = $"https://api.github.com/repos/{GithubRepo}/releases/latest";
            string json = await _updateClient.GetStringAsync(apiUrl);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            string latestTag = root.GetProperty("tag_name").GetString()?.TrimStart('v') ?? "";
            string releaseName = root.GetProperty("name").GetString() ?? latestTag;

            if (latestTag == AppVersion)
            {
                MessageBox.Show($"Bạn đang dùng phiên bản mới nhất (v{AppVersion}).",
                    "Cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. Tìm asset .exe trong release
            string? downloadUrl = null;
            foreach (var asset in root.GetProperty("assets").EnumerateArray())
            {
                string name = asset.GetProperty("name").GetString() ?? "";
                if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    downloadUrl = asset.GetProperty("browser_download_url").GetString();
                    break;
                }
            }

            if (downloadUrl == null)
            {
                MessageBox.Show("Không tìm thấy file .exe trong release mới nhất.\nVui lòng liên hệ admin.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Có phiên bản mới: {releaseName}\nHiện tại: v{AppVersion}\n\nTải về và cập nhật ngay?",
                "Cập nhật phiên bản", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            // 3. Download file .exe mới
            btnUpdate.Text = "Đang tải...";
            byte[] bytes = await _updateClient.GetByteArrayAsync(downloadUrl);
            string tempExe = Path.Combine(Path.GetTempPath(), "ReconcileTool_update.exe");
            File.WriteAllBytes(tempExe, bytes);

            // 4. Batch script: chờ app thoát → replace → restart
            string currentExe = Environment.ProcessPath ?? Application.ExecutablePath;
            string backupPath = currentExe + ".bak";
            string scriptPath = Path.Combine(Path.GetTempPath(), "reconcile_update.bat");

            File.WriteAllText(scriptPath,
                $"""
                @echo off
                timeout /t 2 /nobreak >nul
                if exist "{backupPath}" del /f /q "{backupPath}"
                move /y "{currentExe}" "{backupPath}"
                copy /y "{tempExe}" "{currentExe}"
                del /f /q "{tempExe}"
                start "" "{currentExe}"
                """);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = scriptPath,
                WindowStyle     = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow  = true,
                UseShellExecute = true,
            });

            Application.Exit();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Cập nhật thất bại:\n" + ex.Message,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnUpdate.Enabled = true;
            btnUpdate.Text = "Cập nhật phiên bản";
        }
    }
}
