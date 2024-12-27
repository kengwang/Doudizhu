using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Interfaces;

public interface IClientNotificator
{
    Task UserJoined(GameUser user);
    Task GameStarted(Game game);
    Task ReceiveCards(List<Card> cards);
    Task RequireCallLandLord(Game game);
    Task UserCalledLandLord(GameUser user, int count);
    Task LandLordSelected(GameUser user);
    Task UserPlayCards(GameUser user, CardSentence? cardSentence);
    Task RequirePlayCards(GameUser user);
    Task EndGame(Game game, GameUser winner);

}