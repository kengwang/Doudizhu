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
        // notify game started
        await hubContext.Clients.Group(game.Id.ToString()).GameStarted(game);

        foreach (var gameInteractor in _interactors)
        {
            game.CurrentInteractor = gameInteractor;
            await gameInteractor.EnterInteraction(game);
        }

        // shuffle cards
    }
}