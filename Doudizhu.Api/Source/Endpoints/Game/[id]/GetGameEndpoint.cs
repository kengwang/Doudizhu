using Doudizhu.Api.Service.GameService;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Doudizhu.Api.Endpoints.Game;

public class GetGameEndpoint(GameContainer gameContainer)
    : EndpointWithoutRequest<Results<Ok<Models.GameLogic.Game>, NotFound>>
{
    public override void Configure()
    {
        Get("/games/{id:guid}");
    }

    public override async Task<Results<Ok<Models.GameLogic.Game>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        var gameId = Route<Guid>("id");
        var game = gameContainer.GetGame(gameId);

        if (game is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(game);
    }
}