using System.Text.Json.Serialization;

namespace Doudizhu.Api.Models;

public class CardSentence
{
    [JsonPropertyName("count")] public int Count => Cards.Count;
    [JsonPropertyName("pattern")] public CardPattern Pattern { get; set; }
    [JsonPropertyName("cards")] public required List<Card> Cards { get; set; }
}

public enum CardPattern
{
    Single,
    Pair,
    MultiPair,
    ThreeWithOne,
    ThreeWithPair,
    Straight,
    Plane,
    Bomb,
    JokerBomb
}