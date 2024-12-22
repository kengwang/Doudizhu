using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game.GameUsers;

public class JoinGameEndpoint : EndpointWithoutRequest<
    Results<Ok<GameJoinResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHubContext<GameHub, IClientNotificator> _gameHub;

    public JoinGameEndpoint(ApplicationDbContext dbContext, IHubContext<GameHub, IClientNotificator> gameHub)
    {
        _dbContext = dbContext;
        _gameHub = gameHub;
        Post("/games/{id}/gameUser");
    }

    public override async Task<Results<Ok<GameJoinResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult>>
        ExecuteAsync(CancellationToken ct)
    {
        var userId = User.Claims.FirstOrDefault(t => t.Type == "id")?.Value;

        if (userId is null || !Guid.TryParse(userId, out var userIdGuid))
        {
            return TypedResults.Unauthorized();
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(t => t.Id == userIdGuid, cancellationToken: ct);

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }
        var gameUser = await _dbContext.GameUsers.FirstOrDefaultAsync(
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

        var game = await _dbContext.Games.Include(game => game.Users)
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
            Id = Guid.NewGuid(),
            Game = game,
            User = user,
            Cards = [],
            Role = GameUserRole.Undefined
        };

        game.Users.Add(newGameUser);
        await _dbContext.GameUsers.AddAsync(newGameUser, ct);
        await _dbContext.SaveChangesAsync(ct);
        await _gameHub.Groups.AddToGroupAsync(HttpContext.Connection.Id, game.Id.ToString(), ct);
        await _gameHub.Clients.Group(game.Id.ToString()).UserJoined(newGameUser);


        return TypedResults.Ok(
            new GameJoinResponse()
            {
                Id = game.Id,
                CreateAt = game.CreateAt,
                Users = game.Users
            });
    }
}

public class GameJoinResponse
{
    public Guid Id { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public List<GameUser> Users { get; set; }
}