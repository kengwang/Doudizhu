using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Doudizhu.Api.Service.GameService;

public class GameRoller(IHubContext<GameHub, IClientNotificator> hubContext,
                        IEnumerable<IGameInteractor> interactors) : IRegisterSelfScopedService
{
    List<IGameInteractor> _interactors = interactors.OrderBy(t => t.Index).ToList();

    public async Task StartGameRoll(Game game)
    {
        await hubContext.Clients.Group(game.Id.ToString()).GameStarted(game);
        await Task.Delay(5000);
        for (var index = 0; index < _interactors.Count; index++)
        {
            game.CurrentInteractorIndex = index;
            var gameInteractor = _interactors[index];
            await gameInteractor.EnterInteraction(game);
        }
    }
}