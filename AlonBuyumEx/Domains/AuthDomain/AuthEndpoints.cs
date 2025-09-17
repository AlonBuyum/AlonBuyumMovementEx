using AlonBuyumEx.Models;
using AlonBuyumEx.Services;

using Microsoft.AspNetCore.Mvc;

namespace AlonBuyumEx.Domains.AuthDomain
{
    public  static class AuthEndpoints
    {
        private const string baseUrl = "/api/Auth";
        public static void MapAuthEndPoints(WebApplication app)
        {
            app.MapPost($"{baseUrl}/Login", ([FromServices] IAuthService service, [FromBody] string user) =>
            {
                var result =  service.CreateToken(user);
                return result is not null ? Results.Created($"{baseUrl}/Login", result) : Results.Problem("The token was not created.");
            }).WithTags("Auth");
        }
    }
}
