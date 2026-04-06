namespace ReconcileTool.UI.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Panel pnlLeft;
    private System.Windows.Forms.Panel pnlRight;
    private System.Windows.Forms.Label lblAppTitle;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Label lblWelcome;
    private System.Windows.Forms.Label lblSubtitle;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.Label lblPasswordLabel;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.CheckBox chkShowPassword;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.Label lblError;
    private System.Windows.Forms.PictureBox picLogo;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlLeft = new Panel();
        lblAppTitle = new Label();
        picLogo = new PictureBox();
        lblVersion = new Label();
        pnlRight = new Panel();
        lblWelcome = new Label();
        lblSubtitle = new Label();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPasswordLabel = new Label();
        txtPassword = new TextBox();
        chkShowPassword = new CheckBox();
        lblError = new Label();
        btnLogin = new Button();
        pnlLeft.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
        pnlRight.SuspendLayout();
        SuspendLayout();
        // 
        // pnlLeft
        // 
        pnlLeft.BackColor = Color.FromArgb(24, 90, 157);
        pnlLeft.Controls.Add(lblAppTitle);
        pnlLeft.Controls.Add(picLogo);
        pnlLeft.Controls.Add(lblVersion);
        pnlLeft.Dock = DockStyle.Left;
        pnlLeft.Location = new Point(0, 0);
        pnlLeft.Name = "pnlLeft";
        pnlLeft.Size = new Size(340, 478);
        pnlLeft.TabIndex = 1;
        // 
        // lblAppTitle
        // 
        lblAppTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        lblAppTitle.ForeColor = Color.White;
        lblAppTitle.Location = new Point(20, 9);
        lblAppTitle.Name = "lblAppTitle";
        lblAppTitle.Size = new Size(300, 80);
        lblAppTitle.TabIndex = 0;
        lblAppTitle.Text = "RECONCILE\nTOOL";
        lblAppTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // picLogo
        // 
        picLogo.BackColor = Color.Transparent;
        picLogo.Location = new Point(12, 100);
        picLogo.Name = "picLogo";
        picLogo.Size = new Size(322, 344);
        picLogo.SizeMode = PictureBoxSizeMode.Zoom;
        picLogo.TabIndex = 1;
        picLogo.TabStop = false;
        // 
        // lblVersion
        // 
        lblVersion.Font = new Font("Segoe UI", 9F);
        lblVersion.ForeColor = Color.FromArgb(120, 170, 220);
        lblVersion.Location = new Point(20, 447);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(300, 25);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "v1.0.0 by cuongdv";
        lblVersion.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlRight
        // 
        pnlRight.BackColor = Color.White;
        pnlRight.Controls.Add(lblWelcome);
        pnlRight.Controls.Add(lblSubtitle);
        pnlRight.Controls.Add(lblUsername);
        pnlRight.Controls.Add(txtUsername);
        pnlRight.Controls.Add(lblPasswordLabel);
        pnlRight.Controls.Add(txtPassword);
        pnlRight.Controls.Add(chkShowPassword);
        pnlRight.Controls.Add(lblError);
        pnlRight.Controls.Add(btnLogin);
        pnlRight.Dock = DockStyle.Fill;
        pnlRight.Location = new Point(340, 0);
        pnlRight.Name = "pnlRight";
        pnlRight.Padding = new Padding(40, 30, 40, 30);
        pnlRight.Size = new Size(493, 478);
        pnlRight.TabIndex = 0;
        // 
        // lblWelcome
        // 
        lblWelcome.AutoSize = true;
        lblWelcome.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        lblWelcome.ForeColor = Color.FromArgb(24, 90, 157);
        lblWelcome.Location = new Point(50, 60);
        lblWelcome.Name = "lblWelcome";
        lblWelcome.Size = new Size(136, 37);
        lblWelcome.TabIndex = 0;
        lblWelcome.Text = "Xin chào!";
        // 
        // lblSubtitle
        // 
        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 10F);
        lblSubtitle.ForeColor = Color.Gray;
        lblSubtitle.Location = new Point(50, 100);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(199, 19);
        lblSubtitle.TabIndex = 1;
        lblSubtitle.Text = "Vui lòng đăng nhập để tiếp tục";
        // 
        // lblUsername
        // 
        lblUsername.AutoSize = true;
        lblUsername.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblUsername.ForeColor = Color.FromArgb(60, 60, 60);
        lblUsername.Location = new Point(50, 155);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(100, 17);
        lblUsername.TabIndex = 2;
        lblUsername.Text = "Tên đăng nhập";
        // 
        // txtUsername
        // 
        txtUsername.BorderStyle = BorderStyle.FixedSingle;
        txtUsername.Font = new Font("Segoe UI", 11F);
        txtUsername.Location = new Point(50, 178);
        txtUsername.Name = "txtUsername";
        txtUsername.PlaceholderText = "Nhập tên đăng nhập...";
        txtUsername.Size = new Size(320, 27);
        txtUsername.TabIndex = 3;
        txtUsername.KeyDown += txtUsername_KeyDown;
        // 
        // lblPasswordLabel
        // 
        lblPasswordLabel.AutoSize = true;
        lblPasswordLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblPasswordLabel.ForeColor = Color.FromArgb(60, 60, 60);
        lblPasswordLabel.Location = new Point(50, 230);
        lblPasswordLabel.Name = "lblPasswordLabel";
        lblPasswordLabel.Size = new Size(66, 17);
        lblPasswordLabel.TabIndex = 4;
        lblPasswordLabel.Text = "Mật khẩu";
        // 
        // txtPassword
        // 
        txtPassword.BorderStyle = BorderStyle.FixedSingle;
        txtPassword.Font = new Font("Segoe UI", 11F);
        txtPassword.Location = new Point(50, 253);
        txtPassword.Name = "txtPassword";
        txtPassword.PlaceholderText = "Nhập mật khẩu...";
        txtPassword.Size = new Size(320, 27);
        txtPassword.TabIndex = 5;
        txtPassword.UseSystemPasswordChar = true;
        txtPassword.KeyDown += txtPassword_KeyDown;
        // 
        // chkShowPassword
        // 
        chkShowPassword.AutoSize = true;
        chkShowPassword.Cursor = Cursors.Hand;
        chkShowPassword.Font = new Font("Segoe UI", 9F);
        chkShowPassword.ForeColor = Color.Gray;
        chkShowPassword.Location = new Point(50, 298);
        chkShowPassword.Name = "chkShowPassword";
        chkShowPassword.Size = new Size(121, 19);
        chkShowPassword.TabIndex = 6;
        chkShowPassword.Text = "Hiển thị mật khẩu";
        chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
        // 
        // lblError
        // 
        lblError.Font = new Font("Segoe UI", 9F);
        lblError.ForeColor = Color.FromArgb(200, 50, 50);
        lblError.Location = new Point(50, 325);
        lblError.Name = "lblError";
        lblError.Size = new Size(320, 22);
        lblError.TabIndex = 7;
        lblError.Visible = false;
        // 
        // btnLogin
        // 
        btnLogin.BackColor = Color.FromArgb(24, 90, 157);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(50, 355);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(320, 44);
        btnLogin.TabIndex = 8;
        btnLogin.Text = "ĐĂNG NHẬP";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Click += btnLogin_Click;
        btnLogin.MouseEnter += btnLogin_MouseEnter;
        btnLogin.MouseLeave += btnLogin_MouseLeave;
        // 
        // LoginForm
        // 
        BackColor = Color.White;
        ClientSize = new Size(833, 478);
        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Đăng Nhập - Công Cụ Đối Soát";
        pnlLeft.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
        pnlRight.ResumeLayout(false);
        pnlRight.PerformLayout();
        ResumeLayout(false);
    }
}
