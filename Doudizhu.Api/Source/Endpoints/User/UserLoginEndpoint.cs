using System.Security.Claims;
using System.Text.Json.Serialization;
using Doudizhu.Api.Service.GameService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.User;

public class UserLoginEndpoint(GameContainer gameContainer) : 
    Endpoint<UserLoginRequest, Results<Ok<Models.User>,NotFound>>
{
    public override void Configure()
    {
        Post("/user/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Models.User>,NotFound>> ExecuteAsync(UserLoginRequest req, CancellationToken ct)
    {
        var user = gameContainer.Users.FirstOrDefault(t => t.Qq == req.Qq);

        if (user is null)
        {
            return TypedResults.NotFound();
        }
        
        await CookieAuth.SignInAsync(
            option =>
            {
                option.Claims.Add((ClaimTypes.NameIdentifier, user.Id.ToString()));
                option.Claims.Add((ClaimTypes.Name, user.Name));
            });

        return TypedResults.Ok(user);
    }
}

public class UserLoginRequest
{
    [JsonPropertyName("qq")]
    public string Qq { get; set; }
}