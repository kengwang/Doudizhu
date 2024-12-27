using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Interfaces;

public interface IGameInteractor
{
    public int Index { get; }
    public static Dictionary<Guid, CancellationTokenSource> GameCancellationTokenSource { get; set; } = new();
    public Task EnterInteraction(Game game);
}