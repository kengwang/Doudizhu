namespace Doudizhu.Api.Models.GameLogic;

public class GameRecord
{
    public Game Game { get; set; }
    public GameUser GameUser { get; set; }
    public List<Card> Cards { get; set; }   
}