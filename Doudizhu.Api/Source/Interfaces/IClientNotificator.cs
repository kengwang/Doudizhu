using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Interfaces;

public interface IClientNotificator
{
    Task UserJoined(GameUser user);
    Task ReceiveCards(List<Card> cards);
    Task RequireCallLandLord(Game game, GameUser user);
    Task UserCalledLandLord(GameUser user, int count);
    
}