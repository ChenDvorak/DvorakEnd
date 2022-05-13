namespace DvorakEnd.Utilities;

public static class EnsureConfigurationExtensions
{
    public static WebApplication EnsureConfiguration(this WebApplication web)
    {
        var account = web.Configuration.GetSection("Account").Value;
        var password = web.Configuration.GetSection("Password").Value;
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password)) {
            throw new Exception("configuration feild 'Account' or 'Password' can't be empty");
        }
        return web;
    }
}