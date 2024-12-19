using System.Security.Claims;
using System.Text.Json.Serialization;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.User;

public class UserLoginEndpoint(ApplicationDbContext dbContext) : 
    Endpoint<UserLoginRequest, Results<Ok<Models.User>,NotFound>>
{
    public override void Configure()
    {
        Post("/user/login");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<Models.User>,NotFound>> ExecuteAsync(UserLoginRequest req, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(t => t.Name == req.Name, cancellationToken: ct);

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
    [JsonPropertyName("name")]
    public string Name { get; set; }
}