using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Doudizhu.Api.Endpoints.User;

public class UserRegisterEndpoint(ApplicationDbContext dbContext) : Endpoint<UserRegisterRequest, Ok<Models.User>>
{
    public override void Configure()
    {
        Post("/user/register");
        AllowAnonymous();
    }

    public override async Task<Ok<Models.User>> ExecuteAsync(UserRegisterRequest req, CancellationToken ct)
    {
        var user = new Models.User
        {
            Name = req.UserName,
            Coin = 100
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        return TypedResults.Ok(user);
    }
}

public class UserRegisterRequest
{
    public required string UserName { get; set; }
}