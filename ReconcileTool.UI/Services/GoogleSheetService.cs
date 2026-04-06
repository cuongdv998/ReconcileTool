using System.Net.Http;

namespace ReconcileTool.UI.Services;

public class UserAccount
{
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string Platform { get; set; } = "";
}

public static class GoogleSheetService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    /// <summary>
    /// Đọc danh sách tài khoản từ Google Sheet, lọc theo PLATFORM
    /// </summary>
    public static async Task<List<UserAccount>> GetAccountsByPlatformAsync(string sheetUrl, string platform)
    {
        // Chuyển link Google Sheet sang dạng export CSV
        string csvUrl = ConvertToCsvUrl(sheetUrl);

        string csvContent = await _httpClient.GetStringAsync(csvUrl);

        return ParseCsv(csvContent, platform);
    }

    /// <summary>
    /// Chuyển link Google Sheet thường sang URL export CSV
    /// VD: https://docs.google.com/spreadsheets/d/{ID}/edit → export?format=csv
    /// </summary>
    private static string ConvertToCsvUrl(string sheetUrl)
    {
        // Nếu đã là CSV URL thì dùng luôn
        if (sheetUrl.Contains("export?format=csv"))
            return sheetUrl;

        // Trích xuất Spreadsheet ID
        // https://docs.google.com/spreadsheets/d/{ID}/edit#gid=0
        var uri = new Uri(sheetUrl);
        var segments = uri.AbsolutePath.Split('/');
        int dIndex = Array.IndexOf(segments, "d");
        if (dIndex < 0 || dIndex + 1 >= segments.Length)
            throw new ArgumentException("Link Google Sheet không hợp lệ.");

        string spreadsheetId = segments[dIndex + 1];

        // Lấy gid nếu có (sheet cụ thể), mặc định gid=0
        string gid = "0";
        if (sheetUrl.Contains("gid="))
        {
            var fragment = sheetUrl.Split("gid=");
            if (fragment.Length > 1)
                gid = fragment[1].Split('&')[0].Split('#')[0];
        }

        return $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/export?format=csv&gid={gid}";
    }

    /// <summary>
    /// Parse nội dung CSV, tìm cột USER / PASSWORD / PLATFORM
    /// </summary>
    private static List<UserAccount> ParseCsv(string csvContent, string platform)
    {
        var accounts = new List<UserAccount>();
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2) return accounts;

        // Dòng đầu là header
        var headers = SplitCsvLine(lines[0]);
        int idxUser     = FindColumnIndex(headers, "USER");
        int idxPassword = FindColumnIndex(headers, "PASSWORD");
        int idxPlatform = FindColumnIndex(headers, "PLATFORM");

        if (idxUser < 0 || idxPassword < 0 || idxPlatform < 0)
            throw new Exception("Google Sheet thiếu cột USER, PASSWORD hoặc PLATFORM.");

        for (int i = 1; i < lines.Length; i++)
        {
            var cols = SplitCsvLine(lines[i]);
            if (cols.Length <= Math.Max(idxUser, Math.Max(idxPassword, idxPlatform)))
                continue;

            string platformValue = cols[idxPlatform].Trim();
            if (!platformValue.Equals(platform, StringComparison.OrdinalIgnoreCase))
                continue;

            accounts.Add(new UserAccount
            {
                User     = cols[idxUser].Trim(),
                Password = cols[idxPassword].Trim(),
                Platform = platformValue
            });
        }

        return accounts;
    }

    private static int FindColumnIndex(string[] headers, string name)
    {
        for (int i = 0; i < headers.Length; i++)
            if (headers[i].Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    /// <summary>
    /// Tách dòng CSV có hỗ trợ giá trị trong dấu ngoặc kép
    /// </summary>
    private static string[] SplitCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        result.Add(current.ToString());
        return result.ToArray();
    }
}
