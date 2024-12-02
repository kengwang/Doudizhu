namespace Doudizhu.Api.Models;

public class GameUser
{
    public Guid Id { get; set; }
    public required Game Game { get; set; }
    public required User User { get; set; }
    public List<Card> Card { get; set; } = [];
    public GameUserRole Role { get; set; }

}

public enum GameUserRole
{
    Landlord,
    Farmer
} 