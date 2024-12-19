using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Doudizhu.Api.Service.GameService;

public class GameCardPlayInteractor(IHubContext<GameHub, IClientNotificator> hubContext) : IRegisterSelfService
{
    public async Task EnterGamePlay(Game game)
    {
        var currentPlayerIndex = game.Users.FindIndex(t=>t.Role == GameUserRole.Landlord);
        while (true)
        {
            game.CurrentUser = game.Users[currentPlayerIndex % 3];
            
        }
    }
}