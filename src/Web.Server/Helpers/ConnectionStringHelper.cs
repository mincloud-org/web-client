namespace Web.Server.Helpers;

public class ConnectionStringHelper
{
    public static string ConvertRedisConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return connectionString;

        if (!connectionString.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
            return connectionString;

        try
        {
            var uri = new Uri(connectionString);
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 6379;

            var result = $"{host}:{port}";

            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                var userInfo = uri.UserInfo.Split(':');
                if (userInfo.Length > 1)
                {
                    var password = userInfo[1];
                    result += $",password={password}";
                }
            }

            result += ",defaultDatabase=0,ssl=false";

            return result;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid Redis connection string format: {connectionString}", ex);
        }
    }

}
