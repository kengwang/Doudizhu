using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Interfaces;

public interface IGameInteractor
{
    public int Index { get; }
    public CancellationTokenSource CurrentCancellationTokenSource { get; set; }
    public Task EnterInteraction(Game game);
}