using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Doudizhu.Api.Service.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Doudizhu.Api.Service.GameService;

public class LandLordInteractor(
    IHubContext<GameHub, IClientNotificator> _hubContext,
    ApplicationDbContext dbContext
    ) : IRegisterSelfService
{

    private CancellationTokenSource _currentCancellationTokenSource = new();
    
    public async Task CallLandLordByPoint(Game game, GameUser gameUser, int count)
    {
        gameUser.CalledLandLordCount = count;
        await _hubContext.Clients.Group(game.Id.ToString()).UserCalledLandLord(gameUser, count);
        await _currentCancellationTokenSource.CancelAsync();
    }

    public async Task EnterLandLordSelect(Game game)
    {
       

        foreach (var gameUser in game.Users)
        {
            _currentCancellationTokenSource = new();
            await _hubContext.Clients.Group(game.Id.ToString()).RequireCallLandLord(game, gameUser);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), _currentCancellationTokenSource.Token);
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

        await dbContext.SaveChangesAsync();
    }
}