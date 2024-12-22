using System.Text.Json.Serialization;
using Doudizhu.Api.Interfaces;

namespace Doudizhu.Api.Models.GameLogic;

public class Game : GuidModelBase
{
    public DateTimeOffset CreateAt { get; set; }
    public List<GameUser> Users { get; init; } = [];
    public List<Card> ReservedCards { get; init; } = [];
    public List<GameRecord> Records { get; init; } = [];
    public CardSentence? LastCardSentence { get; set; }
    public GameUser? LastUser { get; set; }
    public GameStatus Status { get; set; }
    public GameUser? CurrentUser { get; set; }

    private static List<Card>? _allCards = null;
    
    [JsonIgnore]
    public IGameInteractor? CurrentInteractor;
    
    public static List<Card> GetAllCards()
    {
        if (_allCards is null)
        {
            _allCards = new();
            for (var i = 0; i < 52; i++)
            {
                _allCards.Add(new((CardNumber)(i % 13), (CardColor)(i / 4)));
            }
            _allCards.Add(new(CardNumber.SmallJoker, CardColor.Special));
            _allCards.Add(new(CardNumber.BigJoker, CardColor.Special));
        }

        return _allCards.ToList();
    }
}

public enum GameStatus
{
    Waiting,
    Starting,
    Running,
    Ended
}