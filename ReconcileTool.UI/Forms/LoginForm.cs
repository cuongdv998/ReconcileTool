using ReconcileTool.UI.Config;
using ReconcileTool.UI.Services;
using static ReconcileTool.UI.Services.MongoAuthService;

namespace ReconcileTool.UI.Forms;

public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        if (IconHelper.AppIcon != null) this.Icon = IconHelper.AppIcon;

        var imgPath = Path.Combine(Application.StartupPath, "Image", "images.jpg");
        if (File.Exists(imgPath))
            picLogo.Image = Image.FromFile(imgPath);
    }

    private void btnLogin_MouseEnter(object? sender, EventArgs e)
        => btnLogin.BackColor = System.Drawing.Color.FromArgb(18, 70, 130);

    private void btnLogin_MouseLeave(object? sender, EventArgs e)
        => btnLogin.BackColor = System.Drawing.Color.FromArgb(24, 90, 157);

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Vui lòng nhập tên đăng nhập và mật khẩu.");
            return;
        }

        SetLoading(true);

        try
        {
            var (result, userId, userName) = await MongoAuthService.LoginAsync(username, password);

            if (result == MongoAuthService.LoginResult.Success)
            {
                var roles = await MongoAuthService.GetUserRolesAsync(userId);
                this.Hide();
                var mainForm = new MainForm(roles, userName);
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else if (result == MongoAuthService.LoginResult.UserNotFound)
            {
                ShowError("Tài khoản không tồn tại.");
                txtUsername.Focus();
            }
            else
            {
                ShowError("Mật khẩu không đúng.");
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
        catch (Exception ex)
        {
            ShowError($"Lỗi kết nối: {ex.Message}");
        }
        finally
        {
            SetLoading(false);
        }
    }

    private void SetLoading(bool isLoading)
    {
        btnLogin.Enabled = !isLoading;
        btnLogin.Text = isLoading ? "Đang kiểm tra..." : "ĐĂNG NHẬP";
        txtUsername.Enabled = !isLoading;
        txtPassword.Enabled = !isLoading;
        lblError.Visible = false;
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visible = true;
    }

    private void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
            btnLogin_Click(sender, e);
    }

    private void txtUsername_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
            txtPassword.Focus();
    }

    private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
    {
        txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
    }

    private void lblAppSubtitle_Click(object sender, EventArgs e)
    {

    }
}
