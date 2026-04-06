using Oracle.ManagedDataAccess.Client;
using ReconcileTool.UI.Config;

namespace ReconcileTool.UI.Forms;

public partial class MainForm : Form
{
    private readonly BoiThuongControl _boiThuongControl = new();
    private readonly ThanhToanControl _thanhToanControl = new();

    public MainForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        if (IconHelper.AppIcon != null) this.Icon = IconHelper.AppIcon;

        lblVersion.Text = $"v{AppVersion}";
        LoadSavedConfig();
        btnSaveConfig.MouseEnter += btnSaveConfig_MouseEnter;
        btnSaveConfig.MouseLeave += btnSaveConfig_MouseLeave;

        // Khởi tạo content controls
        _boiThuongControl.Dock = DockStyle.Fill;
        _boiThuongControl.Visible = false;
        _thanhToanControl.Dock = DockStyle.Fill;
        _thanhToanControl.Visible = false;
        pnlContent.Controls.Add(_boiThuongControl);
        pnlContent.Controls.Add(_thanhToanControl);

        // Mặc định: vào Cấu hình nếu chưa có config, ngược lại vào Bồi thường
        if (IsConfigComplete())
            SetActiveMenu(btnMenuBoiThuong, _boiThuongControl);
        else
            SetActiveMenu(btnMenuCauHinh, pnlDbConfig);
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
        foreach (Button btn in new[] { btnMenuCauHinh, btnMenuBoiThuong, btnMenuThanhToan })
        {
            btn.Font      = new System.Drawing.Font("Segoe UI", 10F);
            btn.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            btn.BackColor = System.Drawing.Color.White;
        }

        // Active tab
        activeBtn.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        activeBtn.ForeColor = System.Drawing.Color.FromArgb(24, 90, 157);
        activeBtn.BackColor = System.Drawing.Color.FromArgb(235, 245, 255);

        // Ẩn tất cả content
        pnlDbConfig.Visible           = false;
        _boiThuongControl.Visible     = false;
        _thanhToanControl.Visible     = false;
        lblPlaceholder.Visible        = false;

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
        => btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(18, 70, 130);

    private void btnSaveConfig_MouseLeave(object? sender, EventArgs e)
        => btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(24, 90, 157);

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
        BoiThuongControl.ApiCredential = apiCred;
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
        BoiThuongControl.ApiCredential = apiCred;
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

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title  = "Chọn file phiên bản mới",
            Filter = "Executable|*.exe",
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        string newExePath  = dlg.FileName;
        string currentExe  = Environment.ProcessPath ?? Application.ExecutablePath;
        string backupPath  = currentExe + ".bak";
        string updaterScript = Path.Combine(Path.GetTempPath(), "reconcile_update.bat");

        var confirm = MessageBox.Show(
            $"Cập nhật phiên bản từ file:\n{newExePath}\n\nỨng dụng sẽ tự động khởi động lại. Tiếp tục?",
            "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            // Tạo batch script: chờ app thoát → backup → copy mới → khởi động lại
            File.WriteAllText(updaterScript,
                $"""
                @echo off
                timeout /t 2 /nobreak >nul
                if exist "{backupPath}" del /f /q "{backupPath}"
                move /y "{currentExe}" "{backupPath}"
                copy /y "{newExePath}" "{currentExe}"
                start "" "{currentExe}"
                """);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName        = updaterScript,
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
    }
}
