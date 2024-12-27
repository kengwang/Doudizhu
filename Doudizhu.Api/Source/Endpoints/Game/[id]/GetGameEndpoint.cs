using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game;

public class GetGameEndpoint(ApplicationDbContext dbContext)
    : EndpointWithoutRequest<Results<Ok<Models.GameLogic.Game>, NotFound>>
{
    public override void Configure()
    {
        Get("/games/{id:guid}");
    }

    public override async Task<Results<Ok<Models.GameLogic.Game>, NotFound>> ExecuteAsync(CancellationToken ct)
    {
        var gameId = Route<Guid>("id");

        var game = await dbContext.Games.Include(game => game.Users)
                                  .Include(t => t.Records)
                                  .FirstOrDefaultAsync(t => t.Id == gameId, cancellationToken: ct);

        if (game is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(game);
    }
}