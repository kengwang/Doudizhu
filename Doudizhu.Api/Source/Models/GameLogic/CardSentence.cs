using System.Text.Json.Serialization;

namespace Doudizhu.Api.Models.GameLogic;

public class CardSentence
{
    [JsonPropertyName("pattern")] public CardPatternType PatternType { get; set; }
    [JsonPropertyName("cards")] public required List<Card> Cards { get; set; }
}