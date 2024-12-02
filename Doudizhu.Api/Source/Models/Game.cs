namespace Doudizhu.Api.Models;

public class Game
{
    public Guid Id { get; set; }
    public List<GameUser> Users { get; set; } = [];
    public List<CardSentence> Records { get; set; } = [];
    public CardSentence? LastCard { get; set; }
    public User? CurrentUser { get; set; }
}