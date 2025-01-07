using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Doudizhu.Api.Service.CardService;
using Doudizhu.Api.Service.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.GameService;

public class GameCardPlayIntereactor(IHubContext<GameHub, IClientNotificator> hubContext,
                                     AutoCardMachineService autoCardMachineService,
                                     CardSentenizer cardSentenizer)
    : IRegisterSelfScopedService, IRegisterScopedServiceFor<IGameInteractor>, IGameInteractor
{
    public async Task PlayCard(Game game, GameUser gameUser, CardSentence? cardSentence)
    {
        if (cardSentence is not null)
        {
            game.LastCardSentence = cardSentence;
            game.LastUser = gameUser;
        }
        game.Records.Add(
            new()
            {
                Game = game,
                GameUser = gameUser,
                CardSentence = cardSentence
            });
        var cards = cardSentence?.Cards;
        if (cards is { Count: > 0 })
            gameUser.Cards.RemoveAll(t=>cards.Any(c=>t.Number == c.Number && t.Number == c.Number));
        await hubContext.Clients.Group(game.Id.ToString()).UserPlayCards(gameUser, cardSentence);
        await IGameInteractor.GameCancellationTokenSource[game.Id].CancelAsync();
    }

    public int Index => 3;
    
    public async Task EnterInteraction(Game game)
    {
        var currentPlayerIndex = game.Users.FindIndex(t => t.Role == GameUserRole.Landlord);

        while (true)
        {
            game.CurrentUser = game.Users[currentPlayerIndex % game.Users.Count];
            IGameInteractor.GameCancellationTokenSource[game.Id] = new();
            try
            {
                await hubContext.Clients.Group(game.Id.ToString())
                                .RequirePlayCards(game.CurrentUser);
                if (!game.CurrentUser.BotTakeOver)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), IGameInteractor.GameCancellationTokenSource[game.Id].Token);
                }
                var cards = await autoCardMachineService.FindBestMatchCard(game, game.CurrentUser);
                var cardSentence = cards.Count == 0 ? null : cardSentenizer.Sentenize(cards);
                await PlayCard(game, game.CurrentUser, cardSentence);
            }
            catch (Exception e)
            {
                // ignored
            }

            if (game.CurrentUser.Cards.Count == 0)
            {
                break;
            }
            currentPlayerIndex++;
        }
    }
}