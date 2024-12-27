using System.Security.Claims;
using AsyncAwaitBestPractices;
using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.GameService;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game.GameUsers;

public class JoinGameEndpoint(ApplicationDbContext dbContext, IHubContext<GameHub, IClientNotificator> gameHub, GameRoller gameRoller)
    : EndpointWithoutRequest<
        Results<Ok<GameUser>, NotFound, BadRequest<string>, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Post("/games/{id}/gameUser");
    }

    public override async Task<Results<Ok<GameUser>, NotFound, BadRequest<string>, UnauthorizedHttpResult>>
        ExecuteAsync(CancellationToken ct)
    {
        var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId is null || !Guid.TryParse(userId, out var userIdGuid))
        {
            return TypedResults.Unauthorized();
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(t => t.Id == userIdGuid, cancellationToken: ct);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }
        var gameUser = await dbContext.GameUsers.FirstOrDefaultAsync(
                           t => t.User.Id == userIdGuid,
                           cancellationToken: ct);

        if (gameUser is not null)
        {
            return TypedResults.BadRequest("用户已经加入游戏");
        }

        var gameId = Route<string>("id");

        if (!Guid.TryParse(gameId, out var gameIdGuid))
        {
            return TypedResults.BadRequest("游戏Id格式错误");
        }

        var game = await dbContext.Games.Include(game => game.Users)
                                   .FirstOrDefaultAsync(t => t.Id == gameIdGuid, cancellationToken: ct);

        if (game is null)
        {
            return TypedResults.NotFound();
        }

        if (game.Users.Count >= 3)
        {
            return TypedResults.BadRequest("游戏人数已满");
        }

        var newGameUser = new GameUser
        {
            Id = Guid.CreateVersion7(),
            GameId = game.Id,
            User = user,
            Cards = [],
            Role = GameUserRole.Undefined
        };

        game.Users.Add(newGameUser);
        await dbContext.GameUsers.AddAsync(newGameUser, ct);
        await dbContext.SaveChangesAsync(ct);
        await gameHub.Groups.AddToGroupAsync(HttpContext.Connection.Id, game.Id.ToString(), ct);
        await gameHub.Clients.Group(game.Id.ToString()).UserJoined(newGameUser);
        
        if (game.Users.Count == 3)
        {
            gameRoller.StartGameRoll(game).SafeFireAndForget();
        }

        return TypedResults.Ok(gameUser);
    }
}