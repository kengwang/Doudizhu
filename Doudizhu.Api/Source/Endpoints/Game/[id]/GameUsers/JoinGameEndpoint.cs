using System.Security.Claims;
using AsyncAwaitBestPractices;
using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.GameService;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game.GameUsers;

public class JoinGameEndpoint(IHubContext<GameHub, IClientNotificator> gameHub, GameRoller gameRoller, GameContainer gameContainer)
    : EndpointWithoutRequest<
        Results<Ok<GameUser>, NotFound, BadRequest<string>, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Post("/games/{id:guid}/gameUser");
    }

    public override async Task<Results<Ok<GameUser>, NotFound, BadRequest<string>, UnauthorizedHttpResult>>
        ExecuteAsync(CancellationToken ct)
    {
        var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId is null || !Guid.TryParse(userId, out var userIdGuid))
        {
            return TypedResults.Unauthorized();
        }

        var user = gameContainer.Users.FirstOrDefault(t => t.Id == userIdGuid);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }
        var gameUser = gameContainer.GetGames().SelectMany(t=>t.Users)
                                      .FirstOrDefault(t => t.User.Id == userIdGuid);
        var gameId = Route<Guid>("id");
        if (gameUser is not null && gameUser.GameId != gameId)
        {
            return TypedResults.BadRequest("用户已经加入游戏");
        }

        var game = gameContainer.GetGame(gameId);

        if (game is null)
        {
            return TypedResults.NotFound();
        }
        
        if (gameUser is null)
        {
            if (game.Users.Count >= 3)
            {
                return TypedResults.BadRequest("游戏人数已满");
            }
            gameUser = new()
            {
                Id = Guid.CreateVersion7(),
                GameId = game.Id,
                User = user,
                Cards = [],
                Role = GameUserRole.Undefined
            };

            game.Users.Add(gameUser);
        }


        await gameHub.Groups.AddToGroupAsync(HttpContext.Connection.Id, game.Id.ToString(), ct);
        await gameHub.Clients.Group(game.Id.ToString()).UserJoined(gameUser);
        
        if (game.Users.Count == 3 && game.Status == GameStatus.Waiting)
        {
            game.Status = GameStatus.Starting;
            gameRoller.StartGameRoll(game).SafeFireAndForget();
        }

        return TypedResults.Ok(gameUser);
    }
}