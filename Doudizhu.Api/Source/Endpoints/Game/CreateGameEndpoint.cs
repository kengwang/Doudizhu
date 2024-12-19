using System.Security.Claims;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game;

public class CreateGameEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<Results<CreatedAtRoute<CreateGameResponse>, BadRequest<string>, ForbidHttpResult>>
{
    public override void Configure()
    {
        Post("/games");
    }

    public override async Task<Results<CreatedAtRoute<CreateGameResponse>, BadRequest<string>, ForbidHttpResult>> ExecuteAsync(
        CancellationToken ct)
    {
        var userIdStr = User.Claims.FirstOrDefault(t => t.Type == "id")?.Value;

        if (userIdStr is null || !Guid.TryParse(userIdStr, out var userId))
        {
            return TypedResults.Forbid();
        }
        var user = await dbContext.Users.FirstOrDefaultAsync(t => t.Id == userId, cancellationToken: ct);

        if (user is null)
        {
            return TypedResults.Forbid();
        }

        if ((await dbContext.GameUsers.FirstOrDefaultAsync(t => t.User.Id == userId, cancellationToken: ct)) is not null)
        {
            return TypedResults.BadRequest("用户已经加入游戏");
        }
        
        var game = new Models.GameLogic.Game
        {
            Id = Guid.CreateVersion7(),
            CreateAt = DateTimeOffset.Now,
            Users = [],
            Records = [],
            LastCardSentence = null,
            CurrentUser = null
        };
        var gameUser = new GameUser
        {
            Id = Guid.CreateVersion7(),
            Game = game,
            User = user,
            Cards = [],
            Role = GameUserRole.Undefined
        };
        game.Users.Add(gameUser);

        await dbContext.Games.AddAsync(game, ct);
        await dbContext.GameUsers.AddAsync(gameUser, ct);
        var resp = new CreateGameResponse
        {
            Id = game.Id,
            CreateAt = game.CreateAt,
        };

        return TypedResults.CreatedAtRoute(resp, $"/games/{game.Id}");
    }
}

public class CreateGameResponse
{
    public Guid Id { get; set; }
    public DateTimeOffset CreateAt { get; set; }
}