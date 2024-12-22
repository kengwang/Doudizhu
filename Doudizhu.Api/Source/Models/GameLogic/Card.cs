using System.Text.Json.Serialization;

namespace Doudizhu.Api.Models.GameLogic;

public class Card : IComparable<Card>, IComparable
{
    public Card(CardNumber number, CardColor color)
    {
        Number = number;
        Color = color;
    }

    [JsonPropertyName("color")] public CardColor Color { get; set; }
    [JsonPropertyName("number")] public CardNumber Number { get; set; }

    public int CompareTo(Card? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var colorComparison = Color.CompareTo(other.Color);
        if (colorComparison != 0) return colorComparison;
        return Number.CompareTo(other.Number);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is Card other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Card)}");
    }
}

public enum CardColor
{
    Meihua,
    Fangkuai,
    Hongtao,
    Heitao,
    Special
}

public enum CardNumber
{
    None,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    J,
    Q,
    K,
    A,
    Two,
    SmallJoker,
    BigJoker
}