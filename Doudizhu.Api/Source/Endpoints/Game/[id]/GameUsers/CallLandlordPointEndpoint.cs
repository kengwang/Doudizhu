using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Service.GameService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Endpoints.Game.GameUsers;

public class CallLandlordPointEndpoint(GameContainer gameContainer,LandLordInteractor landLordInteractor) : Ep.Req<CallLandlordPointRequest>.Res<Results<Ok, NotFound>>
{
    public override void Configure()
    {
        Post("/games/{id:guid}/gameUsers/callLandlordPoint");
    }

    public override async Task<Results<Ok, NotFound>> ExecuteAsync(CallLandlordPointRequest req, CancellationToken ct)
    {
        // get game
        var game = gameContainer.GetGame(Route<Guid>("id"));
        if (game is null)
        {
            return TypedResults.NotFound();
        }
        var gameUsers = gameContainer.GetGames().SelectMany(t => t.Users);
        var gameUser = gameUsers.FirstOrDefault(t => t.User.Id == req.GameUser);
        await landLordInteractor.CallLandLordByPoint(game, gameUser!, req.Point);
        return TypedResults.Ok();
    }
}

public class CallLandlordPointRequest
{
    public Guid GameUser { get; set; }
    public int Point { get; set; }
}