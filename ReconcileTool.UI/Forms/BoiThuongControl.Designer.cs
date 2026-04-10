namespace ReconcileTool.UI.Forms;

partial class BoiThuongControl
{
    private System.ComponentModel.IContainer components = null;

    // Filter panel (row 1: filter controls, row 2: buttons)
    private System.Windows.Forms.Panel pnlFilter;
    private System.Windows.Forms.FlowLayoutPanel flpControls;
    private System.Windows.Forms.FlowLayoutPanel flpButtons;

    // Filter group containers (label + control pairs)
    private System.Windows.Forms.Panel pnlF_LocDB;
    private System.Windows.Forms.Panel pnlF_NgayDau;
    private System.Windows.Forms.Panel pnlF_NgayCuoi;

    // Filter controls
    private System.Windows.Forms.Label lblLocTrangThai;
    private System.Windows.Forms.ComboBox cboLocTrangThai;
    private System.Windows.Forms.Label lblNgayDau;
    private System.Windows.Forms.DateTimePicker dtpNgayDau;
    private System.Windows.Forms.Label lblNgayCuoi;
    private System.Windows.Forms.DateTimePicker dtpNgayCuoi;
    private System.Windows.Forms.Label lblSoHs;
    private System.Windows.Forms.TextBox txtSoHs;
    private System.Windows.Forms.Button btnKiemTra;
    private System.Windows.Forms.Button btnSyncAll;

    // Status bar
    private System.Windows.Forms.Panel pnlStatus;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Label lblRecordCount;

    // Paging panel
    private System.Windows.Forms.Panel pnlPaging;
    private System.Windows.Forms.Button btnFirst;
    private System.Windows.Forms.Button btnPrev;
    private System.Windows.Forms.Label lblPageInfo;
    private System.Windows.Forms.Button btnNext;
    private System.Windows.Forms.Button btnLast;
    private System.Windows.Forms.Label lblPageSize;
    private System.Windows.Forms.ComboBox cboPageSize;
    private System.Windows.Forms.Label lblTotalRecords;

    // Grid panel
    private System.Windows.Forms.Panel pnlGrid;
    private System.Windows.Forms.DataGridView dgvResult;

    // API Response panel
    private System.Windows.Forms.Panel pnlApiResponse;
    private System.Windows.Forms.Panel pnlSoHsInput;
    private System.Windows.Forms.Panel pnlDivider;
    private System.Windows.Forms.Label lblApiResponseHeader;
    private System.Windows.Forms.RichTextBox rtxtApiResponse;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
        pnlFilter = new Panel();
        flpButtons = new FlowLayoutPanel();
        btnKiemTra = new Button();
        btnSyncAll = new Button();
        flpControls = new FlowLayoutPanel();
        pnlF_LocDB = new Panel();
        lblLocTrangThai = new Label();
        cboLocTrangThai = new ComboBox();
        pnlF_NgayDau = new Panel();
        lblNgayDau = new Label();
        dtpNgayDau = new DateTimePicker();
        pnlF_NgayCuoi = new Panel();
        lblNgayCuoi = new Label();
        dtpNgayCuoi = new DateTimePicker();
        lblSoHs = new Label();
        txtSoHs = new TextBox();
        pnlStatus = new Panel();
        lblStatus = new Label();
        lblRecordCount = new Label();
        pnlPaging = new Panel();
        btnFirst = new Button();
        btnPrev = new Button();
        lblPageInfo = new Label();
        btnNext = new Button();
        btnLast = new Button();
        lblPageSize = new Label();
        cboPageSize = new ComboBox();
        lblTotalRecords = new Label();
        pnlGrid = new Panel();
        dgvResult = new DataGridView();
        pnlApiResponse    = new Panel();
        pnlSoHsInput      = new Panel();
        pnlDivider        = new Panel();
        rtxtApiResponse   = new RichTextBox();
        lblApiResponseHeader = new Label();
        pnlFilter.SuspendLayout();
        flpButtons.SuspendLayout();
        flpControls.SuspendLayout();
        pnlF_LocDB.SuspendLayout();
        pnlF_NgayDau.SuspendLayout();
        pnlF_NgayCuoi.SuspendLayout();
        pnlStatus.SuspendLayout();
        pnlPaging.SuspendLayout();
        pnlGrid.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvResult).BeginInit();
        pnlSoHsInput.SuspendLayout();
        pnlApiResponse.SuspendLayout();
        SuspendLayout();
        // 
        // pnlFilter
        // 
        pnlFilter.AutoSize = true;
        pnlFilter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        pnlFilter.BackColor = Color.White;
        pnlFilter.Controls.Add(flpButtons);
        pnlFilter.Controls.Add(flpControls);
        pnlFilter.Dock = DockStyle.Top;
        pnlFilter.Location = new Point(0, 0);
        pnlFilter.MinimumSize = new Size(0, 88);
        pnlFilter.Name = "pnlFilter";
        pnlFilter.Padding = new Padding(6, 4, 6, 0);
        pnlFilter.Size = new Size(1766, 89);
        pnlFilter.TabIndex = 3;
        // 
        // flpButtons
        // 
        flpButtons.BackColor    = Color.White;
        flpButtons.Controls.Add(btnKiemTra);
        flpButtons.Controls.Add(btnSyncAll);
        flpButtons.Dock         = DockStyle.Top;
        flpButtons.Name         = "flpButtons";
        flpButtons.Padding      = new Padding(10, 0, 10, 6);
        flpButtons.Size         = new Size(800, 38);
        flpButtons.TabIndex     = 0;
        flpButtons.WrapContents = false;
        // 
        // btnKiemTra
        // 
        btnKiemTra.BackColor = Color.FromArgb(220, 95, 20);
        btnKiemTra.Cursor = Cursors.Hand;
        btnKiemTra.FlatAppearance.BorderSize = 0;
        btnKiemTra.FlatStyle = FlatStyle.Flat;
        btnKiemTra.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnKiemTra.ForeColor = Color.White;
        btnKiemTra.Location = new Point(10, 0);
        btnKiemTra.Margin   = new Padding(0, 0, 10, 0);
        btnKiemTra.Name = "btnKiemTra";
        btnKiemTra.Size = new Size(110, 30);
        btnKiemTra.TabIndex = 0;
        btnKiemTra.Text = "Kiểm tra";
        btnKiemTra.UseVisualStyleBackColor = false;
        btnKiemTra.Click += btnKiemTra_Click;
        btnKiemTra.MouseEnter += btnKiemTra_MouseEnter;
        btnKiemTra.MouseLeave += btnKiemTra_MouseLeave;
        // 
        // btnSyncAll
        // 
        btnSyncAll.BackColor = Color.FromArgb(230, 245, 235);
        btnSyncAll.Cursor = Cursors.Hand;
        btnSyncAll.FlatAppearance.BorderColor = Color.FromArgb(34, 139, 34);
        btnSyncAll.FlatStyle = FlatStyle.Flat;
        btnSyncAll.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSyncAll.ForeColor = Color.FromArgb(25, 110, 45);
        btnSyncAll.Location = new Point(130, 0);
        btnSyncAll.Margin   = new Padding(0);
        btnSyncAll.Name = "btnSyncAll";
        btnSyncAll.Size = new Size(175, 30);
        btnSyncAll.TabIndex = 1;
        btnSyncAll.Text = "Đồng bộ lại toàn bộ";
        btnSyncAll.UseVisualStyleBackColor = false;
        btnSyncAll.Click += btnSyncAll_Click;
        //
        // flpControls
        //
        flpControls.AutoSize     = true;
        flpControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flpControls.BackColor    = Color.White;
        flpControls.Controls.Add(pnlF_LocDB);
        flpControls.Controls.Add(pnlF_NgayDau);
        flpControls.Controls.Add(pnlF_NgayCuoi);
        // pnlF_SoHs moved to pnlApiResponse
        flpControls.Dock         = DockStyle.Top;
        flpControls.Name         = "flpControls";
        flpControls.Padding      = new Padding(10, 6, 10, 2);
        flpControls.WrapContents = true;
        //
        // pnlF_LocDB
        //
        pnlF_LocDB.BackColor = Color.Transparent;
        pnlF_LocDB.Controls.Add(lblLocTrangThai);
        pnlF_LocDB.Controls.Add(cboLocTrangThai);
        pnlF_LocDB.Margin  = new Padding(0, 0, 16, 0);
        pnlF_LocDB.Name    = "pnlF_LocDB";
        pnlF_LocDB.Size    = new Size(150, 52);
        pnlF_LocDB.TabIndex = 2;
        //
        // lblLocTrangThai
        //
        lblLocTrangThai.AutoSize  = true;
        lblLocTrangThai.Font      = new Font("Segoe UI", 8.5F);
        lblLocTrangThai.ForeColor = Color.FromArgb(130, 140, 155);
        lblLocTrangThai.Location  = new Point(0, 2);
        lblLocTrangThai.Name      = "lblLocTrangThai";
        lblLocTrangThai.TabIndex  = 0;
        lblLocTrangThai.Text      = "Đồng bộ";
        //
        // cboLocTrangThai
        //
        cboLocTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLocTrangThai.Font          = new Font("Segoe UI", 9.5F);
        cboLocTrangThai.Items.AddRange(new object[] { "Toàn bộ", "Chưa đồng bộ" });
        cboLocTrangThai.Location      = new Point(0, 20);
        cboLocTrangThai.Name          = "cboLocTrangThai";
        cboLocTrangThai.SelectedIndex = 0;
        cboLocTrangThai.Size          = new Size(150, 25);
        cboLocTrangThai.TabIndex      = 5;
        cboLocTrangThai.SelectedIndexChanged += cboLocTrangThai_SelectedIndexChanged;
        // 
        // pnlF_NgayDau
        // 
        pnlF_NgayDau.BackColor = Color.Transparent;
        pnlF_NgayDau.Controls.Add(lblNgayDau);
        pnlF_NgayDau.Controls.Add(dtpNgayDau);
        pnlF_NgayDau.Margin   = new Padding(0, 0, 16, 0);
        pnlF_NgayDau.Name     = "pnlF_NgayDau";
        pnlF_NgayDau.Size     = new Size(140, 52);
        pnlF_NgayDau.TabIndex = 3;
        //
        // lblNgayDau
        //
        lblNgayDau.AutoSize  = true;
        lblNgayDau.Font      = new Font("Segoe UI", 8.5F);
        lblNgayDau.ForeColor = Color.FromArgb(130, 140, 155);
        lblNgayDau.Location  = new Point(0, 2);
        lblNgayDau.Name      = "lblNgayDau";
        lblNgayDau.TabIndex  = 0;
        lblNgayDau.Text      = "Ngày đầu";
        //
        // dtpNgayDau
        //
        dtpNgayDau.Font     = new Font("Segoe UI", 9.5F);
        dtpNgayDau.Format   = DateTimePickerFormat.Short;
        dtpNgayDau.Location = new Point(0, 20);
        dtpNgayDau.Name     = "dtpNgayDau";
        dtpNgayDau.Size     = new Size(140, 24);
        dtpNgayDau.TabIndex = 7;
        dtpNgayDau.Value    = DateTime.Today;
        //
        // pnlF_NgayCuoi
        //
        pnlF_NgayCuoi.BackColor = Color.Transparent;
        pnlF_NgayCuoi.Controls.Add(lblNgayCuoi);
        pnlF_NgayCuoi.Controls.Add(dtpNgayCuoi);
        pnlF_NgayCuoi.Margin   = new Padding(0, 0, 16, 0);
        pnlF_NgayCuoi.Name     = "pnlF_NgayCuoi";
        pnlF_NgayCuoi.Size     = new Size(140, 52);
        pnlF_NgayCuoi.TabIndex = 4;
        //
        // lblNgayCuoi
        //
        lblNgayCuoi.AutoSize  = true;
        lblNgayCuoi.Font      = new Font("Segoe UI", 8.5F);
        lblNgayCuoi.ForeColor = Color.FromArgb(130, 140, 155);
        lblNgayCuoi.Location  = new Point(0, 2);
        lblNgayCuoi.Name      = "lblNgayCuoi";
        lblNgayCuoi.TabIndex  = 0;
        lblNgayCuoi.Text      = "Ngày cuối";
        //
        // dtpNgayCuoi
        //
        dtpNgayCuoi.Font     = new Font("Segoe UI", 9.5F);
        dtpNgayCuoi.Format   = DateTimePickerFormat.Short;
        dtpNgayCuoi.Location = new Point(0, 20);
        dtpNgayCuoi.Name     = "dtpNgayCuoi";
        dtpNgayCuoi.Size     = new Size(140, 24);
        dtpNgayCuoi.TabIndex = 9;
        dtpNgayCuoi.Value    = DateTime.Today;
        //
        // lblSoHs
        //
        lblSoHs.AutoSize  = false;
        lblSoHs.Dock      = DockStyle.None;
        lblSoHs.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSoHs.ForeColor = Color.FromArgb(220, 95, 20);
        lblSoHs.Location  = new Point(10, 8);
        lblSoHs.Name      = "lblSoHs";
        lblSoHs.Size      = new Size(340, 20);
        lblSoHs.TabIndex  = 0;
        lblSoHs.Text      = "📋  Nhập số hồ sơ";
        //
        // txtSoHs
        //
        txtSoHs.Dock            = DockStyle.None;
        txtSoHs.Font            = new Font("Segoe UI", 9.5F);
        txtSoHs.Location        = new Point(10, 32);
        txtSoHs.Multiline       = true;
        txtSoHs.Name            = "txtSoHs";
        txtSoHs.PlaceholderText = "Mỗi dòng một số HS:\nHS001\nHS002\nHS003";
        txtSoHs.ScrollBars      = ScrollBars.Vertical;
        txtSoHs.Size            = new Size(358, 118);
        txtSoHs.TabIndex        = 1;
        // 
        // pnlStatus
        // 
        pnlStatus.BackColor = Color.FromArgb(255, 242, 220);
        pnlStatus.Controls.Add(lblStatus);
        pnlStatus.Controls.Add(lblRecordCount);
        pnlStatus.Dock = DockStyle.Top;
        pnlStatus.Location = new Point(0, 89);
        pnlStatus.Name = "pnlStatus";
        pnlStatus.Size = new Size(1766, 30);
        pnlStatus.TabIndex = 2;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblStatus.ForeColor = Color.FromArgb(100, 110, 125);
        lblStatus.Location = new Point(12, 7);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(263, 15);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "Chọn điều kiện và nhấn \"Kiểm tra\" để tải dữ liệu.";
        // 
        // lblRecordCount
        // 
        lblRecordCount.AutoSize = true;
        lblRecordCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblRecordCount.ForeColor = Color.FromArgb(220, 95, 20);
        lblRecordCount.Location = new Point(650, 7);
        lblRecordCount.Name = "lblRecordCount";
        lblRecordCount.Size = new Size(0, 15);
        lblRecordCount.TabIndex = 1;
        // 
        // pnlPaging
        // 
        pnlPaging.BackColor = Color.White;
        pnlPaging.Controls.Add(btnFirst);
        pnlPaging.Controls.Add(btnPrev);
        pnlPaging.Controls.Add(lblPageInfo);
        pnlPaging.Controls.Add(btnNext);
        pnlPaging.Controls.Add(btnLast);
        pnlPaging.Controls.Add(lblPageSize);
        pnlPaging.Controls.Add(cboPageSize);
        pnlPaging.Controls.Add(lblTotalRecords);
        pnlPaging.Dock = DockStyle.Bottom;
        pnlPaging.Location = new Point(0, 658);
        pnlPaging.Name = "pnlPaging";
        pnlPaging.Size = new Size(1766, 42);
        pnlPaging.TabIndex = 1;
        // 
        // btnFirst
        // 
        btnFirst.Cursor = Cursors.Hand;
        btnFirst.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 225);
        btnFirst.FlatStyle = FlatStyle.Flat;
        btnFirst.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnFirst.ForeColor = Color.FromArgb(220, 95, 20);
        btnFirst.Location = new Point(12, 8);
        btnFirst.Name = "btnFirst";
        btnFirst.Size = new Size(36, 26);
        btnFirst.TabIndex = 0;
        btnFirst.Text = "«";
        btnFirst.Click += btnFirst_Click;
        // 
        // btnPrev
        // 
        btnPrev.Cursor = Cursors.Hand;
        btnPrev.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 225);
        btnPrev.FlatStyle = FlatStyle.Flat;
        btnPrev.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnPrev.ForeColor = Color.FromArgb(220, 95, 20);
        btnPrev.Location = new Point(52, 8);
        btnPrev.Name = "btnPrev";
        btnPrev.Size = new Size(36, 26);
        btnPrev.TabIndex = 1;
        btnPrev.Text = "‹";
        btnPrev.Click += btnPrev_Click;
        // 
        // lblPageInfo
        // 
        lblPageInfo.Font = new Font("Segoe UI", 9.5F);
        lblPageInfo.ForeColor = Color.FromArgb(50, 60, 75);
        lblPageInfo.Location = new Point(94, 12);
        lblPageInfo.Name = "lblPageInfo";
        lblPageInfo.Size = new Size(140, 18);
        lblPageInfo.TabIndex = 2;
        lblPageInfo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnNext
        // 
        btnNext.Cursor = Cursors.Hand;
        btnNext.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 225);
        btnNext.FlatStyle = FlatStyle.Flat;
        btnNext.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnNext.ForeColor = Color.FromArgb(220, 95, 20);
        btnNext.Location = new Point(240, 8);
        btnNext.Name = "btnNext";
        btnNext.Size = new Size(36, 26);
        btnNext.TabIndex = 3;
        btnNext.Text = "›";
        btnNext.Click += btnNext_Click;
        // 
        // btnLast
        // 
        btnLast.Cursor = Cursors.Hand;
        btnLast.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 225);
        btnLast.FlatStyle = FlatStyle.Flat;
        btnLast.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLast.ForeColor = Color.FromArgb(220, 95, 20);
        btnLast.Location = new Point(280, 8);
        btnLast.Name = "btnLast";
        btnLast.Size = new Size(36, 26);
        btnLast.TabIndex = 4;
        btnLast.Text = "»";
        btnLast.Click += btnLast_Click;
        // 
        // lblPageSize
        // 
        lblPageSize.AutoSize = true;
        lblPageSize.Font = new Font("Segoe UI", 9F);
        lblPageSize.ForeColor = Color.FromArgb(100, 110, 125);
        lblPageSize.Location = new Point(330, 13);
        lblPageSize.Name = "lblPageSize";
        lblPageSize.Size = new Size(52, 15);
        lblPageSize.TabIndex = 5;
        lblPageSize.Text = "Hiển thị:";
        // 
        // cboPageSize
        // 
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPageSize.Font = new Font("Segoe UI", 9F);
        cboPageSize.Items.AddRange(new object[] { "20", "50", "100", "200", "500" });
        cboPageSize.Location = new Point(393, 9);
        cboPageSize.Name = "cboPageSize";
        cboPageSize.Size = new Size(65, 23);
        cboPageSize.TabIndex = 6;
        cboPageSize.SelectedIndexChanged += cboPageSize_SelectedIndexChanged;
        // 
        // lblTotalRecords
        // 
        lblTotalRecords.AutoSize = true;
        lblTotalRecords.Font = new Font("Segoe UI", 9F);
        lblTotalRecords.ForeColor = Color.FromArgb(100, 110, 125);
        lblTotalRecords.Location = new Point(470, 13);
        lblTotalRecords.Name = "lblTotalRecords";
        lblTotalRecords.Size = new Size(0, 15);
        lblTotalRecords.TabIndex = 7;
        // 
        // pnlGrid
        // 
        pnlGrid.BackColor = Color.White;
        pnlGrid.Controls.Add(dgvResult);
        pnlGrid.Controls.Add(pnlApiResponse);
        pnlGrid.Dock = DockStyle.Fill;
        pnlGrid.Location = new Point(0, 119);
        pnlGrid.Name = "pnlGrid";
        pnlGrid.Size = new Size(1766, 539);
        pnlGrid.TabIndex = 0;
        // 
        // dgvResult
        // 
        dgvResult.AllowUserToAddRows = false;
        dgvResult.AllowUserToDeleteRows = false;
        dataGridViewCellStyle1.BackColor = Color.FromArgb(255, 248, 235);
        dgvResult.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
        dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dgvResult.BackgroundColor = Color.White;
        dgvResult.BorderStyle = BorderStyle.None;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle2.BackColor = Color.FromArgb(220, 95, 20);
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        dataGridViewCellStyle2.ForeColor = Color.White;
        dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(220, 95, 20);
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
        dgvResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        dgvResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvResult.Dock = DockStyle.Fill;
        dgvResult.EnableHeadersVisualStyles = false;
        dgvResult.Font = new Font("Segoe UI", 9.5F);
        dgvResult.GridColor = Color.FromArgb(220, 225, 235);
        dgvResult.Location = new Point(0, 0);
        dgvResult.Name = "dgvResult";
        dgvResult.ReadOnly = true;
        dgvResult.RowHeadersWidth = 40;
        dataGridViewCellStyle3.BackColor = Color.White;
        dgvResult.RowsDefaultCellStyle = dataGridViewCellStyle3;
        dgvResult.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvResult.Size = new Size(1396, 539);
        dgvResult.TabIndex = 0;
        // 
        // pnlApiResponse
        // 
        pnlApiResponse.BackColor = Color.FromArgb(248, 250, 253);
        pnlApiResponse.Controls.Add(rtxtApiResponse);
        pnlApiResponse.Controls.Add(lblApiResponseHeader);
        pnlApiResponse.Controls.Add(pnlDivider);
        pnlApiResponse.Controls.Add(pnlSoHsInput);
        pnlApiResponse.Dock     = DockStyle.Right;
        pnlApiResponse.Name     = "pnlApiResponse";
        pnlApiResponse.Padding  = new Padding(0);
        pnlApiResponse.Width    = 380;
        pnlApiResponse.TabIndex = 1;
        //
        // pnlSoHsInput — phần trên: nhập số hồ sơ
        //
        pnlSoHsInput.BackColor = Color.FromArgb(240, 244, 252);
        pnlSoHsInput.Controls.Add(lblSoHs);
        pnlSoHsInput.Controls.Add(txtSoHs);
        pnlSoHsInput.Dock    = DockStyle.Top;
        pnlSoHsInput.Name    = "pnlSoHsInput";
        pnlSoHsInput.Padding = new Padding(0);
        pnlSoHsInput.Size    = new Size(380, 165);
        pnlSoHsInput.TabIndex = 3;
        //
        // pnlDivider — đường kẻ ngăn cách
        //
        pnlDivider.BackColor = Color.FromArgb(200, 210, 225);
        pnlDivider.Dock      = DockStyle.Top;
        pnlDivider.Height    = 1;
        pnlDivider.Name      = "pnlDivider";
        pnlDivider.TabIndex  = 2;
        // 
        // rtxtApiResponse
        // 
        rtxtApiResponse.BackColor = Color.FromArgb(248, 250, 253);
        rtxtApiResponse.BorderStyle = BorderStyle.None;
        rtxtApiResponse.Dock = DockStyle.Fill;
        rtxtApiResponse.Font = new Font("Consolas", 9F);
        rtxtApiResponse.ForeColor = Color.FromArgb(100, 110, 125);
        rtxtApiResponse.Location = new Point(8, 40);
        rtxtApiResponse.Name = "rtxtApiResponse";
        rtxtApiResponse.ReadOnly = true;
        rtxtApiResponse.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtxtApiResponse.Size = new Size(354, 491);
        rtxtApiResponse.TabIndex = 0;
        rtxtApiResponse.Text = "Nhấn \"Đồng bộ lại\" để xem kết quả tại đây.";
        // 
        // lblApiResponseHeader
        // 
        lblApiResponseHeader.Dock = DockStyle.Top;
        lblApiResponseHeader.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblApiResponseHeader.ForeColor = Color.FromArgb(220, 95, 20);
        lblApiResponseHeader.Location = new Point(8, 8);
        lblApiResponseHeader.Name = "lblApiResponseHeader";
        lblApiResponseHeader.Padding = new Padding(2, 0, 0, 0);
        lblApiResponseHeader.Size = new Size(354, 32);
        lblApiResponseHeader.TabIndex = 1;
        lblApiResponseHeader.Text = "📋  Kết quả gọi API";
        lblApiResponseHeader.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // BoiThuongControl
        // 
        BackColor = Color.FromArgb(250, 246, 242);
        Controls.Add(pnlGrid);
        Controls.Add(pnlPaging);
        Controls.Add(pnlStatus);
        Controls.Add(pnlFilter);
        Name = "BoiThuongControl";
        Size = new Size(1766, 700);
        pnlFilter.ResumeLayout(false);
        pnlFilter.PerformLayout();
        flpButtons.ResumeLayout(false);
        flpControls.ResumeLayout(false);
        pnlF_LocDB.ResumeLayout(false);
        pnlF_LocDB.PerformLayout();
        pnlF_NgayDau.ResumeLayout(false);
        pnlF_NgayDau.PerformLayout();
        pnlF_NgayCuoi.ResumeLayout(false);
        pnlF_NgayCuoi.PerformLayout();
        pnlStatus.ResumeLayout(false);
        pnlStatus.PerformLayout();
        pnlPaging.ResumeLayout(false);
        pnlPaging.PerformLayout();
        pnlGrid.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvResult).EndInit();
        pnlApiResponse.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
}
