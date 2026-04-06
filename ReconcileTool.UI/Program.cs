namespace ReconcileTool.UI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Load icon toàn app một lần
        string svgPath = Path.Combine(Application.StartupPath, "Image", "logo_bsh_sync.svg");
        ReconcileTool.UI.Forms.IconHelper.Initialize(svgPath);

        Application.Run(new ReconcileTool.UI.Forms.LoginForm());
    }    
}