namespace Doudizhu.Api.Models.GameLogic;

public class GameRecord
{
    public required Game Game { get; set; }
    public required GameUser GameUser { get; set; }
    public CardSentence? CardSentence { get; set; }   
}