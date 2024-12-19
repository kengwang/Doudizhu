using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class StraightPattern : CardPattern
{
    public override string Name => "顺子";
    public override CardPatternType PatternType => CardPatternType.Straight;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count < 5)
            return false;

        if (card.Any(t => t.Number is CardNumber.Two or CardNumber.SmallJoker or CardNumber.BigJoker))
            return false;

        var sortedCards = card.OrderBy(t => t.Number).ToList();

        for (var i = 0; i < sortedCards.Count - 1; i++)
        {
            if (card[i + 1].Number - card[i].Number != 1)
                return false;
        }

        return true;
    }

    public override List<Card> Order(List<Card> card)
        => card.OrderByDescending(t => t.Number).ToList();

    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.Straight || last.PatternType != CardPatternType.Straight)
            return false;
        if (current.Cards.Count != last.Cards.Count)
            return false;

        return current.Cards.MinBy(t => t.Number)!.Number > last.Cards.MinBy(t => t.Number)!.Number;
    }

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(
        List<Card> cards,
        CardSentence? lastSentence)
    {
        var baseCard = -1;

        if (lastSentence?.Cards is { Count: > 1 })
        {
            baseCard = (int?)lastSentence.Cards.OrderBy(t=>t.Number).FirstOrDefault()?.Number ?? -1;
        }

        var targetCount = lastSentence?.Cards?.Count ?? -1;

        var validCards = cards
                         .Where(
                             t => (int)t.Number > baseCard &&
                                  t.Number is not CardNumber.Two)
                         .GroupBy(t => t.Number)
                         .SelectMany(t => t.Take(1))
                         .ToList();

        var avas = new List<(List<Card> baseCards, int count)>();

        for (var i = 0; i <= validCards.Count - 5; i++)
        {
            for (var length = 5; length <= (targetCount == -1 ? validCards.Count - i : targetCount); length++)
            {
                var sequence = validCards.Skip(i).Take(length).ToList();
                var isStraight = true;

                for (var j = 0; j < sequence.Count - 1; j++)
                {
                    if (sequence[j + 1].Number - sequence[j].Number == 1)
                        continue;

                    isStraight = false;
                    break;
                }

                if (isStraight)
                {
                    avas.Add((sequence, 0));
                }
            }
        }

        return avas;
    }
}