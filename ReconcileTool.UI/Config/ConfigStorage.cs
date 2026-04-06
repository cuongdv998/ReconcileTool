using System.Text.Json;

namespace ReconcileTool.UI.Config;

public static class ConfigStorage
{
    private static readonly string _configDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ReconcileTool");

    private static readonly string _oracleFile =
        Path.Combine(_configDir, "oracle_config.json");

    private static readonly string _apiCredFile =
        Path.Combine(_configDir, "api_credential.json");

    // ── Oracle config ────────────────────────────────────────────────
    public static void Save(OracleConnectionConfig config)
    {
        Directory.CreateDirectory(_configDir);
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_oracleFile, json);
    }

    public static OracleConnectionConfig Load()
    {
        if (!File.Exists(_oracleFile)) return new OracleConnectionConfig();
        try
        {
            var json = File.ReadAllText(_oracleFile);
            return JsonSerializer.Deserialize<OracleConnectionConfig>(json) ?? new OracleConnectionConfig();
        }
        catch { return new OracleConnectionConfig(); }
    }

    // ── API Credential ───────────────────────────────────────────────
    public static void SaveApiCredential(ApiCredentialConfig config)
    {
        Directory.CreateDirectory(_configDir);
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_apiCredFile, json);
    }

    public static ApiCredentialConfig LoadApiCredential()
    {
        if (!File.Exists(_apiCredFile)) return new ApiCredentialConfig();
        try
        {
            var json = File.ReadAllText(_apiCredFile);
            return JsonSerializer.Deserialize<ApiCredentialConfig>(json) ?? new ApiCredentialConfig();
        }
        catch { return new ApiCredentialConfig(); }
    }
}

public class ApiCredentialConfig
{
    public string ApiUser     { get; set; } = "";
    public string ApiPassword { get; set; } = "";
}
