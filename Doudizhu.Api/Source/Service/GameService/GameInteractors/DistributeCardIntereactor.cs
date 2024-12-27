using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.GameService;

public class DistributeCardIntereactor(IHubContext<GameHub, IClientNotificator> hubContext)
    : IRegisterSelfScopedService, IRegisterScopedServiceFor<IGameInteractor>, IGameInteractor

{
    public int Index => 0;
    public async Task EnterInteraction(Game game)
    {
        game.Status = GameStatus.Starting;
        var cards = Game.GetAllCards();
        cards = cards.OrderBy(_ => Random.Shared.Next()).ToList();

        for (var i = 0; i < 3; i++)
        {
            var currentUser = game.Users[i];
            currentUser.Cards = cards.Skip(i * 17).Take(17).OrderByDescending(t=>t.Number).ToList();

            await hubContext.Clients.User(currentUser.User.Id.ToString()).ReceiveCards(game.Users[i].Cards);
        }
        game.ReservedCards.AddRange(cards.Skip(51).ToList());
    }
}