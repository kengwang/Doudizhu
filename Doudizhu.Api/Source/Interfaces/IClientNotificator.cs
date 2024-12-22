using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Interfaces;

public interface IClientNotificator
{
    Task UserJoined(GameUser user);
    Task GameStarted(Game game);
    Task ReceiveCards(List<Card> cards);
    Task RequireCallLandLord(Game game, GameUser user);
    Task UserCalledLandLord(GameUser user, int count);
    Task UserPlayCards(GameUser user, CardSentence? cardSentence);
    Task RequirePlayCards(Game game, GameUser user);
    Task EndGame(Game game, GameUser winner);

}