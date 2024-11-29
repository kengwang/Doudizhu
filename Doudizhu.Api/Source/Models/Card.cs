using System.Text.Json.Serialization;

namespace Doudizhu.Api.Models;

public class Card
{
    [JsonPropertyName("color")] public CardColor Color { get; set; }
    [JsonPropertyName("number")] public CardNumber Number { get; set; }
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