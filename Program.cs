using System.Security.Claims;
using DvorakEnd.EntityFramework;
using DvorakEnd.Utilities;
using Microsoft.EntityFrameworkCore;
using DvorakEnd.Controllers;
using DvorakEnd.Controllers.Post;
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
       options.ExpireTimeSpan = TimeSpan.FromDays(1);
       options.SlidingExpiration = true;
   });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthorOnly", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var account = builder.Configuration.GetSection("Account").Value;
            var nameClaim = context.User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Name);
            if (nameClaim is null)
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(account, nameClaim.Value);
        });
    });
});

var app = builder.Build()
                 .MigrateDbContext<BlogsContext>();

app.EnsureConfiguration();

app.UseAuthentication();
app.UseAuthorization();

app.UseLoginController();

app.MapGet("/", (IConfiguration configuration) =>
{
    return Results.Redirect(configuration.GetSection("IndexAddress").Value);
});

app.UsePostController();

app.Run();
