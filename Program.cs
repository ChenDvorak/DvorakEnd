using DvorakEnd.EntityFramework;
using DvorakEnd.Utilities;
using Microsoft.EntityFrameworkCore;
using DvorakEnd.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddDbContext<BlogsContext>(
    options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("BlogsDatabase"));
    }
);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
       options.Cookie.SameSite = SameSiteMode.None;
       options.ExpireTimeSpan = TimeSpan.FromDays(1);
       options.SlidingExpiration = true;
   });

builder.AddAuthorOnlyAuthorization();
builder.AuthorizationFailureToUnauthorized();

const string ALLOW_FRONT_CROS_POLICY = "allow_front_cros_policy";
builder.Services.AddCors(options =>
    options.AddPolicy(name: ALLOW_FRONT_CROS_POLICY, b =>
    {
        var allowOrigins = builder.Configuration.GetSection("Cors:AllowOrigins").Value;
        b.WithOrigins(allowOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    })
);

builder.EnsureConfiguration();

var app = builder.Build()
                 .MigrateDbContext<BlogsContext>();

app.UseHttpsRedirection();

app.UseCors(ALLOW_FRONT_CROS_POLICY);
app.UseAuthentication();
app.UseAuthorization();

app.UseLoginController();

app.MapGet("/", (IConfiguration configuration) =>
{
    return Results.Redirect(configuration.GetSection("IndexAddress").Value);
});

app.UsePostController();

app.Run();
