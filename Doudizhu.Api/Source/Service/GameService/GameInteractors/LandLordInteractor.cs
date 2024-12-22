using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Doudizhu.Api.Service.GameService;

public class LandLordInteractor(
    IHubContext<GameHub, IClientNotificator> hubContext
    ) : IRegisterSelfScopedService,
        IRegisterScopedServiceFor<IGameInteractor>,
        IGameInteractor
{

    public async Task CallLandLordByPoint(Game game, GameUser gameUser, int count)
    {
        gameUser.CalledLandLordCount = count;
        await hubContext.Clients.Group(game.Id.ToString()).UserCalledLandLord(gameUser, count);
        await CurrentCancellationTokenSource.CancelAsync();
    }


    public int Index => 1;
    public CancellationTokenSource CurrentCancellationTokenSource { get; set; } = new();
    public async Task EnterInteraction(Game game)
    { 
        foreach (var gameUser in game.Users)
        {
            CurrentCancellationTokenSource = new();
            await hubContext.Clients.Group(game.Id.ToString()).RequireCallLandLord(game, gameUser);
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), CurrentCancellationTokenSource.Token);
                gameUser.CalledLandLordCount = 0;
            }
            catch (OperationCanceledException e)
            {
            }
        }
        
        var maxCount = game.Users.Max(t => t.CalledLandLordCount);
        var landLordUser = game.Users.First(t => t.CalledLandLordCount == maxCount);
        
        foreach (var gameUser in game.Users)
        {
            if (gameUser == landLordUser)
            {
                gameUser.Role = GameUserRole.Landlord;
                gameUser.Cards.AddRange(game.ReservedCards);
            }
            else
            {
                gameUser.Role = GameUserRole.Farmer;
            }
        }
    }
}