using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class MultiPairPattern : CardPattern
{
    public override string Name => "连对";
    public override CardPatternType PatternType => CardPatternType.MultiPair;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count < 6 || card.Count % 2 != 0)
            return false;
        if (card.Any(t => t.Number is CardNumber.BigJoker or CardNumber.SmallJoker))
            return false;

        // check if its continuous
        if (card.CountBy(t => t.Number).Any(t => t.Value != 2))
            return false;

        var numbers = card.Select(t => t.Number).Distinct().OrderBy(t => t).ToList();

        return numbers.Max() - numbers.Min() + 1 == numbers.Count;
    }

    public override List<Card> Order(List<Card> card)
        => card.OrderByDescending(t => t.Number).ThenBy(t => t.Color).ToList();

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.MultiPair || last.PatternType != CardPatternType.MultiPair)
            return false;
        if (current.Cards.Count >= 6 && current.Cards.Count != last.Cards.Count)
            return false;

        return current.Cards.MinBy(t => t.Number)!.Number > last.Cards.MinBy(t => t.Number)!.Number;
    }

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(List<Card> cards,
        CardSentence? lastCardSentence)
    {
        var counts = cards.CountBy(t => t.Number).Where(t => t.Value > 2).Select(t => t.Key).Order().ToList();

        // get constants
        var targetCount = lastCardSentence?.Cards.Count / 2 ?? -1;
        var startCardNumber =
            (int?)lastCardSentence?.Cards.CountBy(t => t.Number).Where(t => t.Value == 4).MinBy(t => t.Key).Key ?? -1;
        var nextShouldBe = -1;
        var accumulate = 0;
        List<(CardNumber validStart, CardNumber end)> validRanges = [];

        for (var i = 0; i < counts.Count; i++)
        {
            var number = counts[i];
            
            if ((int)number <= startCardNumber)
            {
                continue;
            }
            
            if (nextShouldBe == -1)
            {
                accumulate++;
                nextShouldBe = (int)number + 1;

                continue;
            }

            if (nextShouldBe == (int)number)
            {
                accumulate++;
                nextShouldBe++;

                if (accumulate >= targetCount && targetCount != -1)
                {
                    validRanges.Add(((CardNumber)nextShouldBe - targetCount, number));
                }
                else
                {
                    for (var p = accumulate - 1; p >= 3; p--)
                    {
                        validRanges.Add(((CardNumber)(nextShouldBe - p), number));
                    }
                }
            }
            else
            {
                accumulate = 0;
                nextShouldBe = -1;
            }
        }

        // expand valid ranges to cards
        var avas = validRanges.Select(
            t =>
            {
                var baseCards = cards.Where(c => c.Number >= t.validStart && c.Number <= t.end)
                                     .GroupBy(c => c.Number)
                                     .SelectMany(g => g.Take(2))
                                     .ToList();

                return (baseCards, targetCount);
            }).ToList();

        return avas;
    }
}