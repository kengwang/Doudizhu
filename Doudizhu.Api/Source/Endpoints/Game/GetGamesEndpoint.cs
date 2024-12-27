using Doudizhu.Api.Service.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game;

public class GetGamesEndpoint(ApplicationDbContext dbContext) : Ep.NoReq.Res<List<Models.GameLogic.Game>>
{
    public override void Configure()
    {
        Get("/games");
    }

    public override async Task<List<Models.GameLogic.Game>> ExecuteAsync(CancellationToken ct)
    {
        return await dbContext.Games
                       .Include(g => g.Users)
                       .ToListAsync(cancellationToken: ct);
    }
}
