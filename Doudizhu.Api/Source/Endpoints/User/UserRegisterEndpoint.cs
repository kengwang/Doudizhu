
using Doudizhu.Api.Service.GameService;
using Microsoft.AspNetCore.Http.HttpResults;
using NJsonSchema.Annotations;

namespace Doudizhu.Api.Endpoints.User;

public class UserRegisterEndpoint(GameContainer gameContainer) : Endpoint<UserRegisterRequest, Results<Created<Models.User>, BadRequest>>
{
    public override void Configure()
    {
        Post("/user/register");
        AllowAnonymous();
    }

    public override async Task<Results<Created<Models.User>, BadRequest>> ExecuteAsync(UserRegisterRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.UserName) || string.IsNullOrEmpty(req.Qq))
        {
            return TypedResults.BadRequest();
        }
        var user = new Models.User
        {
            Id = Guid.CreateVersion7(),
            Name = req.UserName,
            Coin = 100,
            Qq = req.Qq
        };
        gameContainer.Users.Add(user);

        return TypedResults.Created("/user/" + user.Id, user);
    }
}

public class UserRegisterRequest
{
    public required string UserName { get; set; }
    
    public required string Qq { get; set; }
}