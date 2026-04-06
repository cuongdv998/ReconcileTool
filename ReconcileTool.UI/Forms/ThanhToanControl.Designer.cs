namespace ReconcileTool.UI.Forms;

partial class ThanhToanControl
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label lblTitle;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(24, 90, 157);
        lblTitle.Location = new Point(0, 0);
        lblTitle.Name = "lblTitle";
        lblTitle.Padding = new Padding(8, 0, 0, 0);
        lblTitle.Size = new Size(1881, 40);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Đối soát Thanh toán";
        lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // ThanhToanControl
        // 
        BackColor = Color.FromArgb(245, 247, 250);
        Controls.Add(lblTitle);
        Name = "ThanhToanControl";
        Size = new Size(1881, 724);
        ResumeLayout(false);
    }
}
