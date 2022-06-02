namespace DvorakEnd.Utilities;

public static class EnsureConfigurationExtensions
{
    public static WebApplicationBuilder EnsureConfiguration(this WebApplicationBuilder builder)
    {
        var account = builder.Configuration.GetSection("Account").Value;
        var password = builder.Configuration.GetSection("Password").Value;
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password)) {
            throw new Exception("configuration feild 'Account' or 'Password' can't be empty");
        }
        return builder;
    }
}