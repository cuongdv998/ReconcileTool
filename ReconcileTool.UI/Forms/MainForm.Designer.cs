namespace ReconcileTool.UI.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    // Header
    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblTitle;

    // DB Config Panel
    private System.Windows.Forms.Panel pnlDbConfig;
    private System.Windows.Forms.TableLayoutPanel tlpGroups;
    private System.Windows.Forms.Button btnSaveConfig;
    private System.Windows.Forms.Button btnUpdate;
    private System.Windows.Forms.Label lblVersion;

    // DB BSH
    private System.Windows.Forms.GroupBox grpBSH;
    private System.Windows.Forms.Label lblBshHost;
    private System.Windows.Forms.TextBox txtBshHost;
    private System.Windows.Forms.Label lblBshPort;
    private System.Windows.Forms.TextBox txtBshPort;
    private System.Windows.Forms.Label lblBshService;
    private System.Windows.Forms.TextBox txtBshService;
    private System.Windows.Forms.Label lblBshUser;
    private System.Windows.Forms.TextBox txtBshUser;
    private System.Windows.Forms.Label lblBshPassword;
    private System.Windows.Forms.TextBox txtBshPassword;
    private System.Windows.Forms.Button btnTestBSH;
    private System.Windows.Forms.Label lblBshStatus;

    // DB MYBSH
    private System.Windows.Forms.GroupBox grpMyBSH;
    private System.Windows.Forms.Label lblVpnNote;
    private System.Windows.Forms.Label lblMyBshHost;
    private System.Windows.Forms.TextBox txtMyBshHost;
    private System.Windows.Forms.Label lblMyBshPort;
    private System.Windows.Forms.TextBox txtMyBshPort;
    private System.Windows.Forms.Label lblMyBshService;
    private System.Windows.Forms.TextBox txtMyBshService;
    private System.Windows.Forms.Label lblMyBshUser;
    private System.Windows.Forms.TextBox txtMyBshUser;
    private System.Windows.Forms.Label lblMyBshPassword;
    private System.Windows.Forms.TextBox txtMyBshPassword;
    private System.Windows.Forms.Button btnTestMyBSH;
    private System.Windows.Forms.Label lblMyBshStatus;

    // API Credential
    private System.Windows.Forms.GroupBox grpApiConfig;
    private System.Windows.Forms.Label lblApiUser;
    private System.Windows.Forms.Label lblApiPassword;
    private System.Windows.Forms.TextBox txtApiUser;
    private System.Windows.Forms.TextBox txtApiPassword;

    // Menu bar
    private System.Windows.Forms.Panel pnlMenu;
    private System.Windows.Forms.Button btnMenuCauHinh;
    private System.Windows.Forms.Button btnMenuBoiThuong;
    private System.Windows.Forms.Button btnMenuThanhToan;

    // Content panel
    private System.Windows.Forms.Panel pnlContent;
    private System.Windows.Forms.Label lblPlaceholder;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        pnlDbConfig = new Panel();
        grpBSH = new GroupBox();
        lblBshHost = new Label();
        txtBshHost = new TextBox();
        lblBshPort = new Label();
        txtBshPort = new TextBox();
        lblBshService = new Label();
        txtBshService = new TextBox();
        lblBshUser = new Label();
        txtBshUser = new TextBox();
        lblBshPassword = new Label();
        txtBshPassword = new TextBox();
        btnTestBSH = new Button();
        lblBshStatus = new Label();
        grpMyBSH = new GroupBox();
        lblMyBshHost = new Label();
        txtMyBshHost = new TextBox();
        lblVpnNote = new Label();
        lblMyBshPort = new Label();
        txtMyBshPort = new TextBox();
        lblMyBshService = new Label();
        txtMyBshService = new TextBox();
        lblMyBshUser = new Label();
        txtMyBshUser = new TextBox();
        lblMyBshPassword = new Label();
        txtMyBshPassword = new TextBox();
        btnTestMyBSH = new Button();
        lblMyBshStatus = new Label();
        tlpGroups = new TableLayoutPanel();
        grpApiConfig = new GroupBox();
        lblApiUser = new Label();
        txtApiUser = new TextBox();
        lblApiPassword = new Label();
        txtApiPassword = new TextBox();
        btnSaveConfig = new Button();
        btnUpdate = new Button();
        lblVersion = new Label();
        pnlMenu = new Panel();
        btnMenuCauHinh   = new Button();
        btnMenuBoiThuong = new Button();
        btnMenuThanhToan = new Button();
        pnlContent = new Panel();
        lblPlaceholder = new Label();
        label1 = new Label();
        pnlHeader.SuspendLayout();
        tlpGroups.SuspendLayout();
        pnlDbConfig.SuspendLayout();
        grpBSH.SuspendLayout();
        grpMyBSH.SuspendLayout();
        grpApiConfig.SuspendLayout();
        pnlMenu.SuspendLayout();
        pnlContent.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(24, 90, 157);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1514, 56);
        pnlHeader.TabIndex = 2;
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Fill;
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Padding = new Padding(20, 0, 0, 0);
        lblTitle.Size = new Size(1514, 56);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "BSH RECONCILE TOOL  |  Đối soát đồng bộ BSH - MyBSH";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlDbConfig
        // 
        // tlpGroups — 3 equal-width columns for GroupBoxes
        tlpGroups.ColumnCount = 3;
        tlpGroups.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpGroups.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpGroups.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpGroups.Controls.Add(grpBSH,     0, 0);
        tlpGroups.Controls.Add(grpMyBSH,   1, 0);
        tlpGroups.Controls.Add(grpApiConfig, 2, 0);
        tlpGroups.Dock        = DockStyle.Top;
        tlpGroups.Height      = 138;
        tlpGroups.Name        = "tlpGroups";
        tlpGroups.Padding     = new Padding(10, 8, 10, 4);
        tlpGroups.RowCount    = 1;
        tlpGroups.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpGroups.BackColor   = Color.White;

        pnlDbConfig.BackColor = Color.White;
        pnlDbConfig.Controls.Add(tlpGroups);
        pnlDbConfig.Controls.Add(btnSaveConfig);
        pnlDbConfig.Controls.Add(btnUpdate);
        pnlDbConfig.Controls.Add(lblVersion);
        pnlDbConfig.Dock    = DockStyle.Fill;
        pnlDbConfig.Name    = "pnlDbConfig";
        pnlDbConfig.Padding = new Padding(0, 8, 0, 0);
        pnlDbConfig.Visible = false;
        pnlDbConfig.TabIndex = 1;
        // 
        // grpBSH
        // 
        grpBSH.BackColor = Color.White;
        grpBSH.Controls.Add(lblBshHost);
        grpBSH.Controls.Add(txtBshHost);
        grpBSH.Controls.Add(lblBshPort);
        grpBSH.Controls.Add(txtBshPort);
        grpBSH.Controls.Add(lblBshService);
        grpBSH.Controls.Add(txtBshService);
        grpBSH.Controls.Add(lblBshUser);
        grpBSH.Controls.Add(txtBshUser);
        grpBSH.Controls.Add(lblBshPassword);
        grpBSH.Controls.Add(txtBshPassword);
        grpBSH.Controls.Add(btnTestBSH);
        grpBSH.Controls.Add(lblBshStatus);
        grpBSH.Dock     = DockStyle.Fill;
        grpBSH.Font     = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpBSH.ForeColor = Color.FromArgb(24, 90, 157);
        grpBSH.Margin   = new Padding(0, 0, 6, 0);
        grpBSH.Name     = "grpBSH";
        grpBSH.TabIndex = 0;
        grpBSH.TabStop = false;
        grpBSH.Text = "Cầu hình DB BSH";
        // 
        // lblBshHost
        // 
        lblBshHost.AutoSize = true;
        lblBshHost.Font = new Font("Segoe UI", 8.5F);
        lblBshHost.ForeColor = Color.FromArgb(100, 110, 125);
        lblBshHost.Location = new Point(12, 28);
        lblBshHost.Name = "lblBshHost";
        lblBshHost.Size = new Size(139, 15);
        lblBshHost.TabIndex = 0;
        lblBshHost.Text = "Host /Port /ServiceName";
        lblBshHost.Click += lblBshHost_Click;
        // 
        // txtBshHost
        // 
        txtBshHost.BorderStyle = BorderStyle.FixedSingle;
        txtBshHost.Font = new Font("Segoe UI", 9.5F);
        txtBshHost.Location = new Point(156, 18);
        txtBshHost.Name = "txtBshHost";
        txtBshHost.PlaceholderText = "192.168.1.1";
        txtBshHost.Size = new Size(124, 24);
        txtBshHost.TabIndex = 1;
        // 
        // lblBshPort
        // 
        lblBshPort.AutoSize = true;
        lblBshPort.Font = new Font("Segoe UI", 8.5F);
        lblBshPort.ForeColor = Color.FromArgb(100, 110, 125);
        lblBshPort.Location = new Point(182, 28);
        lblBshPort.Name = "lblBshPort";
        lblBshPort.Size = new Size(0, 15);
        lblBshPort.TabIndex = 2;
        // 
        // txtBshPort
        // 
        txtBshPort.BorderStyle = BorderStyle.FixedSingle;
        txtBshPort.Font = new Font("Segoe UI", 9.5F);
        txtBshPort.Location = new Point(286, 18);
        txtBshPort.Name = "txtBshPort";
        txtBshPort.PlaceholderText = "1521";
        txtBshPort.Size = new Size(39, 24);
        txtBshPort.TabIndex = 3;
        // 
        // lblBshService
        // 
        lblBshService.AutoSize = true;
        lblBshService.Font = new Font("Segoe UI", 8.5F);
        lblBshService.ForeColor = Color.FromArgb(100, 110, 125);
        lblBshService.Location = new Point(252, 28);
        lblBshService.Name = "lblBshService";
        lblBshService.Size = new Size(0, 15);
        lblBshService.TabIndex = 4;
        // 
        // txtBshService
        // 
        txtBshService.BorderStyle = BorderStyle.FixedSingle;
        txtBshService.Font = new Font("Segoe UI", 9.5F);
        txtBshService.Location = new Point(331, 18);
        txtBshService.Name = "txtBshService";
        txtBshService.PlaceholderText = "ORCL";
        txtBshService.Size = new Size(98, 24);
        txtBshService.TabIndex = 5;
        // 
        // lblBshUser
        // 
        lblBshUser.AutoSize = true;
        lblBshUser.Font = new Font("Segoe UI", 8.5F);
        lblBshUser.ForeColor = Color.FromArgb(100, 110, 125);
        lblBshUser.Location = new Point(12, 59);
        lblBshUser.Name = "lblBshUser";
        lblBshUser.Size = new Size(118, 15);
        lblBshUser.TabIndex = 6;
        lblBshUser.Text = "Username/ Password";
        // 
        // txtBshUser
        // 
        txtBshUser.BorderStyle = BorderStyle.FixedSingle;
        txtBshUser.Font = new Font("Segoe UI", 9.5F);
        txtBshUser.Location = new Point(157, 50);
        txtBshUser.Name = "txtBshUser";
        txtBshUser.PlaceholderText = "username";
        txtBshUser.Size = new Size(123, 24);
        txtBshUser.TabIndex = 7;
        // 
        // lblBshPassword
        // 
        lblBshPassword.AutoSize = true;
        lblBshPassword.Font = new Font("Segoe UI", 8.5F);
        lblBshPassword.ForeColor = Color.FromArgb(100, 110, 125);
        lblBshPassword.Location = new Point(182, 88);
        lblBshPassword.Name = "lblBshPassword";
        lblBshPassword.Size = new Size(0, 15);
        lblBshPassword.TabIndex = 8;
        // 
        // txtBshPassword
        // 
        txtBshPassword.BorderStyle = BorderStyle.FixedSingle;
        txtBshPassword.Font = new Font("Segoe UI", 9.5F);
        txtBshPassword.Location = new Point(286, 50);
        txtBshPassword.Name = "txtBshPassword";
        txtBshPassword.PlaceholderText = "password";
        txtBshPassword.Size = new Size(143, 24);
        txtBshPassword.TabIndex = 9;
        txtBshPassword.UseSystemPasswordChar = true;
        // 
        // btnTestBSH
        // 
        btnTestBSH.BackColor = Color.FromArgb(235, 245, 255);
        btnTestBSH.Cursor = Cursors.Hand;
        btnTestBSH.FlatAppearance.BorderColor = Color.FromArgb(24, 90, 157);
        btnTestBSH.FlatStyle = FlatStyle.Flat;
        btnTestBSH.Font = new Font("Segoe UI", 9F);
        btnTestBSH.ForeColor = Color.FromArgb(24, 90, 157);
        btnTestBSH.Location = new Point(12, 80);
        btnTestBSH.Name = "btnTestBSH";
        btnTestBSH.Size = new Size(80, 28);
        btnTestBSH.TabIndex = 10;
        btnTestBSH.Text = "Kiểm tra";
        btnTestBSH.UseVisualStyleBackColor = false;
        btnTestBSH.Click += btnTestBSH_Click;
        // 
        // lblBshStatus
        // 
        lblBshStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblBshStatus.ForeColor = Color.Gray;
        lblBshStatus.Location = new Point(98, 88);
        lblBshStatus.Name = "lblBshStatus";
        lblBshStatus.Size = new Size(331, 20);
        lblBshStatus.TabIndex = 11;
        // 
        // grpMyBSH
        // 
        grpMyBSH.BackColor = Color.White;
        grpMyBSH.Controls.Add(lblMyBshHost);
        grpMyBSH.Controls.Add(txtMyBshHost);
        grpMyBSH.Controls.Add(lblVpnNote);
        grpMyBSH.Controls.Add(lblMyBshPort);
        grpMyBSH.Controls.Add(txtMyBshPort);
        grpMyBSH.Controls.Add(lblMyBshService);
        grpMyBSH.Controls.Add(txtMyBshService);
        grpMyBSH.Controls.Add(lblMyBshUser);
        grpMyBSH.Controls.Add(txtMyBshUser);
        grpMyBSH.Controls.Add(lblMyBshPassword);
        grpMyBSH.Controls.Add(txtMyBshPassword);
        grpMyBSH.Controls.Add(btnTestMyBSH);
        grpMyBSH.Controls.Add(lblMyBshStatus);
        grpMyBSH.Dock     = DockStyle.Fill;
        grpMyBSH.Font     = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpMyBSH.ForeColor = Color.FromArgb(24, 90, 157);
        grpMyBSH.Margin   = new Padding(6, 0, 6, 0);
        grpMyBSH.Name     = "grpMyBSH";
        grpMyBSH.TabIndex = 1;
        grpMyBSH.TabStop = false;
        grpMyBSH.Text = "Cấu hình DB MYBSH";
        // 
        // lblMyBshHost
        // 
        lblMyBshHost.AutoSize = true;
        lblMyBshHost.Font = new Font("Segoe UI", 8.5F);
        lblMyBshHost.ForeColor = Color.FromArgb(100, 110, 125);
        lblMyBshHost.Location = new Point(12, 28);
        lblMyBshHost.Name = "lblMyBshHost";
        lblMyBshHost.Size = new Size(139, 15);
        lblMyBshHost.TabIndex = 0;
        lblMyBshHost.Text = "Host /Port /ServiceName";
        // 
        // txtMyBshHost
        // 
        txtMyBshHost.BorderStyle = BorderStyle.FixedSingle;
        txtMyBshHost.Font = new Font("Segoe UI", 9.5F);
        txtMyBshHost.Location = new Point(157, 18);
        txtMyBshHost.Name = "txtMyBshHost";
        txtMyBshHost.PlaceholderText = "192.168.1.2";
        txtMyBshHost.Size = new Size(126, 24);
        txtMyBshHost.TabIndex = 1;
        // 
        // lblVpnNote
        // 
        lblVpnNote.AutoSize = true;
        lblVpnNote.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
        lblVpnNote.ForeColor = Color.Red;
        lblVpnNote.Location = new Point(252, 80);
        lblVpnNote.Name = "lblVpnNote";
        lblVpnNote.Size = new Size(181, 17);
        lblVpnNote.TabIndex = 2;
        lblVpnNote.Text = "Kết nối VPN để vào DB MYBSH";
        // 
        // lblMyBshPort
        // 
        lblMyBshPort.AutoSize = true;
        lblMyBshPort.Font = new Font("Segoe UI", 8.5F);
        lblMyBshPort.ForeColor = Color.FromArgb(100, 110, 125);
        lblMyBshPort.Location = new Point(182, 28);
        lblMyBshPort.Name = "lblMyBshPort";
        lblMyBshPort.Size = new Size(0, 15);
        lblMyBshPort.TabIndex = 3;
        // 
        // txtMyBshPort
        // 
        txtMyBshPort.BorderStyle = BorderStyle.FixedSingle;
        txtMyBshPort.Font = new Font("Segoe UI", 9.5F);
        txtMyBshPort.Location = new Point(289, 18);
        txtMyBshPort.Name = "txtMyBshPort";
        txtMyBshPort.PlaceholderText = "1521";
        txtMyBshPort.Size = new Size(38, 24);
        txtMyBshPort.TabIndex = 4;
        // 
        // lblMyBshService
        // 
        lblMyBshService.AutoSize = true;
        lblMyBshService.Font = new Font("Segoe UI", 8.5F);
        lblMyBshService.ForeColor = Color.FromArgb(100, 110, 125);
        lblMyBshService.Location = new Point(252, 28);
        lblMyBshService.Name = "lblMyBshService";
        lblMyBshService.Size = new Size(0, 15);
        lblMyBshService.TabIndex = 5;
        // 
        // txtMyBshService
        // 
        txtMyBshService.BorderStyle = BorderStyle.FixedSingle;
        txtMyBshService.Font = new Font("Segoe UI", 9.5F);
        txtMyBshService.Location = new Point(333, 18);
        txtMyBshService.Name = "txtMyBshService";
        txtMyBshService.PlaceholderText = "ORCL";
        txtMyBshService.Size = new Size(95, 24);
        txtMyBshService.TabIndex = 6;
        // 
        // lblMyBshUser
        // 
        lblMyBshUser.AutoSize = true;
        lblMyBshUser.Font = new Font("Segoe UI", 8.5F);
        lblMyBshUser.ForeColor = Color.FromArgb(100, 110, 125);
        lblMyBshUser.Location = new Point(12, 59);
        lblMyBshUser.Name = "lblMyBshUser";
        lblMyBshUser.Size = new Size(118, 15);
        lblMyBshUser.TabIndex = 7;
        lblMyBshUser.Text = "Username/ Password";
        // 
        // txtMyBshUser
        // 
        txtMyBshUser.BorderStyle = BorderStyle.FixedSingle;
        txtMyBshUser.Font = new Font("Segoe UI", 9.5F);
        txtMyBshUser.Location = new Point(157, 48);
        txtMyBshUser.Name = "txtMyBshUser";
        txtMyBshUser.PlaceholderText = "username";
        txtMyBshUser.Size = new Size(126, 24);
        txtMyBshUser.TabIndex = 8;
        // 
        // lblMyBshPassword
        // 
        lblMyBshPassword.AutoSize = true;
        lblMyBshPassword.Font = new Font("Segoe UI", 8.5F);
        lblMyBshPassword.ForeColor = Color.FromArgb(100, 110, 125);
        lblMyBshPassword.Location = new Point(182, 78);
        lblMyBshPassword.Name = "lblMyBshPassword";
        lblMyBshPassword.Size = new Size(0, 15);
        lblMyBshPassword.TabIndex = 9;
        // 
        // txtMyBshPassword
        // 
        txtMyBshPassword.BorderStyle = BorderStyle.FixedSingle;
        txtMyBshPassword.Font = new Font("Segoe UI", 9.5F);
        txtMyBshPassword.Location = new Point(289, 48);
        txtMyBshPassword.Name = "txtMyBshPassword";
        txtMyBshPassword.PlaceholderText = "password";
        txtMyBshPassword.Size = new Size(139, 24);
        txtMyBshPassword.TabIndex = 10;
        txtMyBshPassword.UseSystemPasswordChar = true;
        // 
        // btnTestMyBSH
        // 
        btnTestMyBSH.BackColor = Color.FromArgb(235, 245, 255);
        btnTestMyBSH.Cursor = Cursors.Hand;
        btnTestMyBSH.FlatAppearance.BorderColor = Color.FromArgb(24, 90, 157);
        btnTestMyBSH.FlatStyle = FlatStyle.Flat;
        btnTestMyBSH.Font = new Font("Segoe UI", 9F);
        btnTestMyBSH.ForeColor = Color.FromArgb(24, 90, 157);
        btnTestMyBSH.Location = new Point(12, 80);
        btnTestMyBSH.Name = "btnTestMyBSH";
        btnTestMyBSH.Size = new Size(80, 28);
        btnTestMyBSH.TabIndex = 11;
        btnTestMyBSH.Text = "Kiểm tra";
        btnTestMyBSH.UseVisualStyleBackColor = false;
        btnTestMyBSH.Click += btnTestMyBSH_Click;
        // 
        // lblMyBshStatus
        // 
        lblMyBshStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblMyBshStatus.ForeColor = Color.Gray;
        lblMyBshStatus.Location = new Point(100, 85);
        lblMyBshStatus.Name = "lblMyBshStatus";
        lblMyBshStatus.Size = new Size(328, 20);
        lblMyBshStatus.TabIndex = 12;
        // 
        // grpApiConfig
        // 
        grpApiConfig.BackColor = Color.White;
        grpApiConfig.Controls.Add(label1);
        grpApiConfig.Controls.Add(lblApiUser);
        grpApiConfig.Controls.Add(txtApiUser);
        grpApiConfig.Controls.Add(lblApiPassword);
        grpApiConfig.Controls.Add(txtApiPassword);
        grpApiConfig.Dock     = DockStyle.Fill;
        grpApiConfig.Font     = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpApiConfig.ForeColor = Color.FromArgb(24, 90, 157);
        grpApiConfig.Margin   = new Padding(6, 0, 0, 0);
        grpApiConfig.Name     = "grpApiConfig";
        grpApiConfig.TabIndex = 2;
        grpApiConfig.TabStop = false;
        grpApiConfig.Text = "Cấu hình API";
        // 
        // lblApiUser
        // 
        lblApiUser.AutoSize = true;
        lblApiUser.Font = new Font("Segoe UI", 8.5F);
        lblApiUser.ForeColor = Color.FromArgb(100, 110, 125);
        lblApiUser.Location = new Point(12, 29);
        lblApiUser.Name = "lblApiUser";
        lblApiUser.Size = new Size(30, 15);
        lblApiUser.TabIndex = 0;
        lblApiUser.Text = "User";
        // 
        // txtApiUser
        // 
        txtApiUser.BorderStyle = BorderStyle.FixedSingle;
        txtApiUser.Font = new Font("Segoe UI", 9.5F);
        txtApiUser.Location = new Point(120, 15);
        txtApiUser.Name = "txtApiUser";
        txtApiUser.PlaceholderText = "API User...";
        txtApiUser.Size = new Size(200, 24);
        txtApiUser.TabIndex = 1;
        // 
        // lblApiPassword
        // 
        lblApiPassword.AutoSize = true;
        lblApiPassword.Font = new Font("Segoe UI", 8.5F);
        lblApiPassword.ForeColor = Color.FromArgb(100, 110, 125);
        lblApiPassword.Location = new Point(182, 29);
        lblApiPassword.Name = "lblApiPassword";
        lblApiPassword.Size = new Size(0, 15);
        lblApiPassword.TabIndex = 2;
        // 
        // txtApiPassword
        // 
        txtApiPassword.BorderStyle = BorderStyle.FixedSingle;
        txtApiPassword.Font = new Font("Segoe UI", 9.5F);
        txtApiPassword.Location = new Point(120, 47);
        txtApiPassword.Name = "txtApiPassword";
        txtApiPassword.PlaceholderText = "API Password...";
        txtApiPassword.Size = new Size(200, 24);
        txtApiPassword.TabIndex = 3;
        txtApiPassword.UseSystemPasswordChar = true;
        // 
        // btnSaveConfig
        // 
        btnSaveConfig.BackColor = Color.FromArgb(24, 90, 157);
        btnSaveConfig.Cursor = Cursors.Hand;
        btnSaveConfig.FlatAppearance.BorderSize = 0;
        btnSaveConfig.FlatStyle = FlatStyle.Flat;
        btnSaveConfig.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSaveConfig.ForeColor = Color.White;
        btnSaveConfig.Location = new Point(16, 144);
        btnSaveConfig.Name = "btnSaveConfig";
        btnSaveConfig.Size = new Size(180, 32);
        btnSaveConfig.TabIndex = 3;
        btnSaveConfig.Text = "Lưu tất cả cấu hình";
        btnSaveConfig.UseVisualStyleBackColor = false;
        btnSaveConfig.Click += btnSaveConfig_Click;
        //
        // btnUpdate
        //
        btnUpdate.BackColor = Color.FromArgb(230, 245, 235);
        btnUpdate.Cursor = Cursors.Hand;
        btnUpdate.FlatAppearance.BorderColor = Color.FromArgb(34, 139, 34);
        btnUpdate.FlatStyle = FlatStyle.Flat;
        btnUpdate.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnUpdate.ForeColor = Color.FromArgb(25, 110, 45);
        btnUpdate.Location = new Point(210, 144);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new Size(170, 32);
        btnUpdate.TabIndex = 4;
        btnUpdate.Text = "Cập nhật phiên bản";
        btnUpdate.UseVisualStyleBackColor = false;
        btnUpdate.Click += btnUpdate_Click;
        //
        // lblVersion
        //
        lblVersion.AutoSize = true;
        lblVersion.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblVersion.ForeColor = Color.FromArgb(150, 160, 175);
        lblVersion.Location = new Point(392, 152);
        lblVersion.Name = "lblVersion";
        lblVersion.TabIndex = 5;
        lblVersion.Text = "v1.0.0";
        //
        // pnlMenu
        // 
        pnlMenu.BackColor = Color.White;
        pnlMenu.Controls.Add(btnMenuCauHinh);
        pnlMenu.Controls.Add(btnMenuBoiThuong);
        pnlMenu.Controls.Add(btnMenuThanhToan);
        pnlMenu.Dock     = DockStyle.Top;
        pnlMenu.Name     = "pnlMenu";
        pnlMenu.Size     = new Size(1514, 44);
        pnlMenu.TabIndex = 4;
        //
        // btnMenuCauHinh
        //
        btnMenuCauHinh.BackColor = Color.White;
        btnMenuCauHinh.Cursor    = Cursors.Hand;
        btnMenuCauHinh.FlatAppearance.BorderColor = Color.FromArgb(24, 90, 157);
        btnMenuCauHinh.FlatAppearance.BorderSize  = 0;
        btnMenuCauHinh.FlatStyle  = FlatStyle.Flat;
        btnMenuCauHinh.Font       = new Font("Segoe UI", 10F);
        btnMenuCauHinh.ForeColor  = Color.FromArgb(100, 110, 125);
        btnMenuCauHinh.Location   = new Point(0, 0);
        btnMenuCauHinh.Name       = "btnMenuCauHinh";
        btnMenuCauHinh.Size       = new Size(160, 44);
        btnMenuCauHinh.TabIndex   = 0;
        btnMenuCauHinh.Text       = "⚙  Cấu hình";
        btnMenuCauHinh.UseVisualStyleBackColor = false;
        btnMenuCauHinh.Click += btnMenuCauHinh_Click;
        //
        // btnMenuBoiThuong
        //
        btnMenuBoiThuong.BackColor = Color.White;
        btnMenuBoiThuong.Cursor    = Cursors.Hand;
        btnMenuBoiThuong.FlatAppearance.BorderColor = Color.FromArgb(24, 90, 157);
        btnMenuBoiThuong.FlatAppearance.BorderSize  = 0;
        btnMenuBoiThuong.FlatStyle  = FlatStyle.Flat;
        btnMenuBoiThuong.Font       = new Font("Segoe UI", 10F);
        btnMenuBoiThuong.ForeColor  = Color.FromArgb(100, 110, 125);
        btnMenuBoiThuong.Location   = new Point(160, 0);
        btnMenuBoiThuong.Name       = "btnMenuBoiThuong";
        btnMenuBoiThuong.Size       = new Size(200, 44);
        btnMenuBoiThuong.TabIndex   = 1;
        btnMenuBoiThuong.Text       = "Đối soát Bồi thường";
        btnMenuBoiThuong.UseVisualStyleBackColor = false;
        btnMenuBoiThuong.Click += btnMenuBoiThuong_Click;
        //
        // btnMenuThanhToan
        //
        btnMenuThanhToan.BackColor = Color.White;
        btnMenuThanhToan.Cursor    = Cursors.Hand;
        btnMenuThanhToan.FlatAppearance.BorderSize = 0;
        btnMenuThanhToan.FlatStyle  = FlatStyle.Flat;
        btnMenuThanhToan.Font       = new Font("Segoe UI", 10F);
        btnMenuThanhToan.ForeColor  = Color.FromArgb(100, 110, 125);
        btnMenuThanhToan.Location   = new Point(360, 0);
        btnMenuThanhToan.Name       = "btnMenuThanhToan";
        btnMenuThanhToan.Size       = new Size(200, 44);
        btnMenuThanhToan.TabIndex   = 2;
        btnMenuThanhToan.Text       = "Đối soát Thanh toán";
        btnMenuThanhToan.UseVisualStyleBackColor = false;
        btnMenuThanhToan.Click += btnMenuThanhToan_Click;
        //
        // pnlContent
        //
        pnlContent.BackColor = Color.FromArgb(245, 247, 250);
        pnlContent.Controls.Add(pnlDbConfig);
        pnlContent.Controls.Add(lblPlaceholder);
        pnlContent.Dock     = DockStyle.Fill;
        pnlContent.Name     = "pnlContent";
        pnlContent.Padding  = new Padding(0);
        pnlContent.Size = new Size(1514, 585);
        pnlContent.TabIndex = 0;
        // 
        // lblPlaceholder
        // 
        lblPlaceholder.Font = new Font("Segoe UI", 12F);
        lblPlaceholder.ForeColor = Color.FromArgb(170, 180, 195);
        lblPlaceholder.Location = new Point(200, 120);
        lblPlaceholder.Name = "lblPlaceholder";
        lblPlaceholder.Size = new Size(700, 40);
        lblPlaceholder.TabIndex = 0;
        lblPlaceholder.Text = "Vui lòng nhập thông tin kết nối Oracle ở trên rồi nhấn \"Lưu cấu hình\"";
        lblPlaceholder.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Font = new Font("Segoe UI", 8.5F);
        label1.ForeColor = Color.FromArgb(100, 110, 125);
        label1.Location = new Point(12, 54);
        label1.Name = "label1";
        label1.Size = new Size(57, 15);
        label1.TabIndex = 4;
        label1.Text = "Password";
        // 
        // MainForm
        // 
        BackColor = Color.FromArgb(245, 247, 250);
        ClientSize = new Size(1514, 861);
        Controls.Add(pnlContent);
        Controls.Add(pnlMenu);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9.5F);
        MinimumSize = new Size(800, 700);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Công Cụ Đối Soát - Oracle Database";
        pnlHeader.ResumeLayout(false);
        tlpGroups.ResumeLayout(false);
        pnlDbConfig.ResumeLayout(false);
        grpBSH.ResumeLayout(false);
        grpBSH.PerformLayout();
        grpMyBSH.ResumeLayout(false);
        grpMyBSH.PerformLayout();
        grpApiConfig.ResumeLayout(false);
        grpApiConfig.PerformLayout();
        pnlMenu.ResumeLayout(false);
        pnlContent.ResumeLayout(false);
        ResumeLayout(false);
    }
    private Label label1;
}
