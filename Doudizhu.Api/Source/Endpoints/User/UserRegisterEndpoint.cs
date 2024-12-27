using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using NJsonSchema.Annotations;

namespace Doudizhu.Api.Endpoints.User;

public class UserRegisterEndpoint(ApplicationDbContext dbContext) : Endpoint<UserRegisterRequest, Created<Models.User>>
{
    public override void Configure()
    {
        Post("/user/register");
        AllowAnonymous();
    }

    public override async Task<Created<Models.User>> ExecuteAsync(UserRegisterRequest req, CancellationToken ct)
    {
        var user = new Models.User
        {
            Name = req.UserName,
            Coin = 100,
            Qq = req.Qq
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        return TypedResults.Created("/user/" + user.Id, user);
    }
}

public class UserRegisterRequest
{
    public required string UserName { get; set; }
    
    public required string Qq { get; set; }
}