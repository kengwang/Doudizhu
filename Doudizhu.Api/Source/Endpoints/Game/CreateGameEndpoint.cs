using System.Security.Claims;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.GameService;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game;
public class CreateGameEndpoint(GameContainer gameContainer)
    : EndpointWithoutRequest<Results<CreatedAtRoute<Models.GameLogic.Game>, BadRequest<string>, ForbidHttpResult>>
{
    public override void Configure()
    {
        Post("/games");
    }

    public override async Task<Results<CreatedAtRoute<Models.GameLogic.Game>, BadRequest<string>, ForbidHttpResult>> ExecuteAsync(
        CancellationToken ct)
    {
        var userIdStr = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdStr is null || !Guid.TryParse(userIdStr, out var userId))
        {
            return TypedResults.Forbid();
        }
        var user = gameContainer.Users.FirstOrDefault(t => t.Id == userId);

        if (user is null)
        {
            return TypedResults.Forbid();
        }

        if ((gameContainer.GetGames().SelectMany(t=>t.Users).FirstOrDefault(t => t.User.Id == userId)) is not null)
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
            GameId = game.Id,
            User = user,
            Cards = [],
            Role = GameUserRole.Undefined
        };
        game.Users.Add(gameUser);

        gameContainer.AddGame(game);

        return TypedResults.CreatedAtRoute(game, nameof(GetGameEndpoint), new {id = game.Id});
    }
}
