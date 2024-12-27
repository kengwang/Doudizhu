using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Doudizhu.Api.Service.GameService;

public class LandLordInteractor(
    IHubContext<GameHub, IClientNotificator> hubContext,
    AutoCardMachineService machineService,
    IServiceScopeFactory scopeFactory
    ) : IRegisterSelfScopedService,
        IRegisterScopedServiceFor<IGameInteractor>,
        IGameInteractor
{

    public async Task CallLandLordByPoint(Game game, GameUser gameUser, int count)
    {
        gameUser.CalledLandLordCount = count;
        await hubContext.Clients.Group(game.Id.ToString()).UserCalledLandLord(gameUser, count);
    }


    public int Index => 1;
    public async Task EnterInteraction(Game game)
    { 
        await hubContext.Clients.Group(game.Id.ToString()).RequireCallLandLord(game);
        foreach (var gameUser in game.Users)
        {
            IGameInteractor.GameCancellationTokenSource[game.Id] = new();
            try
            {
                if (gameUser.BotTakeOver)
                {
                    var cnt = await machineService.CallLordCount(game, gameUser);
                    await CallLandLordByPoint(game, gameUser, cnt);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    gameUser.CalledLandLordCount = 0;
                }

            }
            catch (Exception e)
            {
                // ignored
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
                await hubContext.Clients.User(gameUser.User.Id.ToString()).ReceiveCards(gameUser.Cards.OrderByDescending(t=>t.Number).ToList());
            }
            else
            {
                gameUser.Role = GameUserRole.Farmer;
            }
        }
        await hubContext.Clients.Group(game.Id.ToString()).LandLordSelected(landLordUser);
    }
}