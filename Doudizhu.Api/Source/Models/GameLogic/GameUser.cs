namespace Doudizhu.Api.Models.GameLogic;

public class GameUser : GuidModelBase
{
    public required Guid GameId { get; set; }
    public required User User { get; set; }
    public int CalledLandLordCount { get; set; }
    public List<Card> Cards { get; set; } = [];
    public GameUserRole Role { get; set; }
    public bool BotTakeOver { get; set; } = false;

}

public enum GameUserRole
{
    Undefined,
    Landlord,
    Farmer
} 