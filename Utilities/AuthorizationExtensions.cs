using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace DvorakEnd.Utilities;

public static class AuthorizationExtensions
{
    public static WebApplicationBuilder AddAuthorOnlyAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AuthorOnly", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        var account = builder.Configuration.GetSection("Account").Value;
                        var nameClaim = context.User.Claims.FirstOrDefault(t => t.Type
                                                                                == ClaimTypes.Name);
                        if (nameClaim is null)
                        {
                            return false;
                        }
                        return BCrypt.Net.BCrypt.Verify(account, nameClaim.Value);
                    });
                });
            });
        return builder;
    }

    public static WebApplicationBuilder AuthorizationFailureToUnauthorized(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<
            IAuthorizationMiddlewareResultHandler, ToChallengeAuthorizationMiddlewareResultHandler>();
        return builder;
    }
}

public class ToChallengeAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // Fall back to the default implementation.
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
