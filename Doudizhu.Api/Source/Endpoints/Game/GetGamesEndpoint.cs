
using Doudizhu.Api.Service.GameService;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game;

public class GetGamesEndpoint(GameContainer gameContainer) : Ep.NoReq.Res<List<Models.GameLogic.Game>>
{
    public override void Configure()
    {
        Get("/games");
    }

    public override async Task<List<Models.GameLogic.Game>> ExecuteAsync(CancellationToken ct)
    {
        return gameContainer.GetGames();
    }
}
