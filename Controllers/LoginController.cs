using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DvorakEnd.Controllers;

public static class LoginControllerExtensions
{
    public static WebApplication UseLoginController(this WebApplication web)
    {
        /// <summary>
        /// Author Login here
        /// </summary>
        /// <value></value>
        web.MapPost("/login", async (HttpContext context, IConfiguration configuration, [FromBody] LoginRequest input) =>
        {
            var account = configuration.GetSection("Account").Value;
            var password = configuration.GetSection("Password").Value;

            var error = (account == input.Account && BCrypt.Net.BCrypt.Verify(input.Password, password)) ? "" : "incorrect account or password";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, BCrypt.Net.BCrypt.HashString(account))
            };
            var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties());

            return Results.Ok(error);
        }).AllowAnonymous();

        web.MapPost("/logout", [Authorize("AuthorOnly")] async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        });

        web.MapPost("/ChenkLogged", [Authorize("AuthorOnly")] () =>
        {
            // Results.Challenge
            return Results.Ok();
        });

        return web;
    }
}

public class LoginRequest
{
    public string Account { get; set; } = "";

    public string Password { get; set; } = "";
}