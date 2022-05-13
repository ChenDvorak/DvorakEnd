using Microsoft.EntityFrameworkCore;
using Polly;

namespace DvorakEnd.Utilities;

public static class MigrateDbContextExtensions
{
    public static WebApplication MigrateDbContext<TContext>(
    this WebApplication webApplication)
    where TContext : DbContext
    {
        using var scope = webApplication.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
        try
        {
            logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");
            var retry = Policy.Handle<Exception>().WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
            });

            retry.Execute(() =>
            {
                context.Database.Migrate();
            });
            logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
        }

        return webApplication;
    }
}