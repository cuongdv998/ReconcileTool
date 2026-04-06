namespace ReconcileTool.UI.Config;

public static class AppConfig
{
    public static string GoogleSheetUrl { get; set; } =
        "https://docs.google.com/spreadsheets/d/1A9yXKJmTXcHJTMayRKcCY3AEaNnEaQyRm1nkk-HHjbk/export?format=csv&gid=0";

    public const string Platform = "BSH_TOOL";

    public static OracleConnectionConfig OracleConfig { get; set; } = new();
}

public class OracleConnectionConfig
{
    // DB BSH
    public string BshHost     { get; set; } = "";
    public string BshPort     { get; set; } = "1521";
    public string BshService  { get; set; } = "";
    public string BshUser     { get; set; } = "";
    public string BshPassword { get; set; } = "";

    // DB MYBSH
    public string MyBshHost     { get; set; } = "";
    public string MyBshPort     { get; set; } = "1521";
    public string MyBshService  { get; set; } = "";
    public string MyBshUser     { get; set; } = "";
    public string MyBshPassword { get; set; } = "";

    public string BuildConnectionString(bool isBsh) => isBsh
        ? $"User Id={BshUser};Password={BshPassword};Data Source={BshHost}:{BshPort}/{BshService};"
        : $"User Id={MyBshUser};Password={MyBshPassword};Data Source={MyBshHost}:{MyBshPort}/{MyBshService};";
}
