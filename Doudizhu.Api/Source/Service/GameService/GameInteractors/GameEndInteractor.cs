using Doudizhu.Api.Interfaces;
using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.GameService;

public class GameEndInteractor : IRegisterSelfScopedService,IRegisterScopedServiceFor<IGameInteractor>, IGameInteractor
{
    private const int BaseCoin = 10;
    public int Index => 4;
    
    public async Task EnterInteraction(Game game)
    {
        // 结算
        var winner = game.Users.First(t => t.Cards.Count == 0);
        var baseCoin = BaseCoin;
        if (winner.Role == GameUserRole.Landlord)
        {
            winner.User.Coin += 2 * baseCoin;

            foreach (var gameUser in game.Users.Where(t=>t.Role == GameUserRole.Farmer))
            {
                gameUser.User.Coin -= baseCoin;
            }
        }
        else
        {
            foreach (var gameUser in game.Users.Where(t=>t.Role == GameUserRole.Farmer))
            {
                gameUser.User.Coin += baseCoin;
            }

            game.Users.First(t => t.Role == GameUserRole.Landlord).User.Coin -= baseCoin * 2;
        }
    }
}