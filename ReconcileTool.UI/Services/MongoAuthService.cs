using MongoDB.Bson;
using MongoDB.Driver;

namespace ReconcileTool.UI.Services;

public static class MongoAuthService
{
    private const string ConnStr = "mongodb://admin:Daocuong152@103.20.97.125:27017/?authSource=admin";
    private const string DbName  = "UserManager";

    public enum LoginResult { Success, UserNotFound, WrongPassword }

    public static async Task<(LoginResult Result, string UserId, string UserName)> LoginAsync(
        string username, string password)
    {
        var client = new MongoClient(ConnStr);
        var col    = client.GetDatabase(DbName).GetCollection<BsonDocument>("User");

        // Kiểm tra tài khoản tồn tại
        var userFilter = Builders<BsonDocument>.Filter.Eq("user", username);
        var user = await col.Find(userFilter).FirstOrDefaultAsync();
        if (user == null) return (LoginResult.UserNotFound, "", "");

        // Kiểm tra mật khẩu
        string storedPass = user.GetValue("pass", BsonNull.Value).AsString ?? "";
        if (storedPass != password) return (LoginResult.WrongPassword, "", "");

        string userId   = user.GetValue("userid",    BsonNull.Value).AsString ?? "";
        string userName = user.GetValue("user_Name", BsonNull.Value).AsString ?? username;
        return (LoginResult.Success, userId, userName);
    }

    public static async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var client = new MongoClient(ConnStr);
        var col    = client.GetDatabase(DbName).GetCollection<BsonDocument>("RoleOfUser");

        var filter = Builders<BsonDocument>.Filter.Eq("userid", userId);
        var docs   = await col.Find(filter).ToListAsync();
        return docs
            .Select(d => d.GetValue("role", BsonNull.Value).AsString?.Trim() ?? "")
            .Where(r => !string.IsNullOrEmpty(r))
            .ToList();
    }
}
