namespace Common;

public static class ConnectionStringExtensions
{
    public static string GetDeviceId(this string connectionString)
        => connectionString.Split(";")[1].Split("=")[1];
}
