using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.GameService;

public class GameContainer()
{
    
    private readonly Dictionary<Guid, Game> _games = new();
    public readonly List<User> Users = new();
    
    public void AddGame(Game game)
    {
        _games[game.Id] = game;
    }
    
    public Game? GetGame(Guid gameId)
    {
        return _games.GetValueOrDefault(gameId);
    }
    
    public List<Game> GetGames()
    {
        return _games.Values.ToList();
    }
}