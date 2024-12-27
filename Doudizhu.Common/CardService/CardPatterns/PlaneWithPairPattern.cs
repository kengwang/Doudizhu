using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService.CardPatterns;

public class PlaneWithPairPattern : CardPattern
{
    public override string Name => "飞机带翅膀";
    public override CardPatternType PatternType => CardPatternType.PlaneWithPair;

    public override bool IsMatched(List<Card> card)
    {
        if (card.Count < 8 || card.Count % 4 != 0)
            return false;

        var counts = card.CountBy(t => t.Number).ToList();

        if (counts.All(t => t.Value == 3))
            return false;

        if (counts.Any(t => t.Key is CardNumber.BigJoker or CardNumber.SmallJoker))
            return false;

        var longest = GetLongestTriple(counts);
        
        

        return longest.count >= 2;
    }

    public override List<Card> Order(List<Card> card)
    {
        var counts = card.CountBy(t => t.Number).ToList();
        var longest = GetLongestTriple(counts);
        var tmp = card.ToList();
        var res = new List<Card>();
        for (var i = 0; i < longest.count; i++)
        {
            var triple = tmp.Where(t => t.Number == longest.start + i).ToList().Take(3).ToList();
            res.AddRange(triple);
            tmp.RemoveAll(t => triple.Contains(t));
        }
        res.AddRange(tmp.OrderByDescending(t=>t.Number).ThenBy(t=>t.Color));

        return res;
    }

    private (CardNumber start, int count) GetLongestTriple(List<KeyValuePair<CardNumber, int>> counts)
    {
        var totalCardCount = counts.Sum(t=>t.Value);
        var maxCount = totalCardCount / 4;
        var triples = counts
                      .Where(t => t.Value >= 3)
                      .ToList()
                      .ToDictionary(t => t.Key, t => t.Value);
        
        // find constant sequence
        var sequence = triples.Keys.Order().ToList();
        Dictionary<CardNumber,int> startPoints = new();
        CardNumber? curStartPoint = null;

        for (var i = 0; i < sequence.Count; i++)
        {
            if (curStartPoint is null)
            {
                curStartPoint = sequence[i];
                startPoints[curStartPoint.Value] = 1;
                continue;
            }

            if (sequence[i] != curStartPoint + i)
            {
                curStartPoint = null;
                i--;
                continue;
            }

            startPoints[curStartPoint.Value]++;

            if (startPoints[curStartPoint.Value] > maxCount)
            {
                var count = startPoints[curStartPoint.Value];
                startPoints.Remove(curStartPoint.Value);
                curStartPoint += 1;
                startPoints[curStartPoint.Value] = count + 1;
            }
        }

        var max = startPoints.MaxBy(t => t.Value);

        return (max.Key, max.Value);
    }

    
    
    public override bool CanCover(CardSentence current, CardSentence last)
    {
        if (current.PatternType != CardPatternType.PlaneWithPair || last.PatternType != CardPatternType.PlaneWithPair)
            return false;

        if (current.Cards.Count < 8 && current.Cards.Count != last.Cards.Count)
            return false;
        
        var currentCounts = current.Cards.CountBy(t => t.Number).ToList();
        var lastCounts = last.Cards.CountBy(t => t.Number).ToList();
        
        var currentLongest = GetLongestTriple(currentCounts);
        var lastLongest = GetLongestTriple(lastCounts);
        
        if (currentLongest.count != lastLongest.count)
            return false;
        
        return currentLongest.start > lastLongest.start;
    }

    public override async Task<List<(List<Card> baseCards, int count)>> GetBaseAndNeedle(
        List<Card> cards,
        CardSentence? lastSentence)
    {
        var baseCard = -1;
        if (lastSentence?.Cards is { Count: > 1 })
        {
            baseCard = (int?)lastSentence.Cards.FirstOrDefault()?.Number ?? -1;
        }
        var counts = cards.CountBy(t => t.Number).Where(t=>(int)t.Key > baseCard).Where(t=>t.Value >= 3).Select(t=>t.Key).ToList();
        var avas = cards.Where(t => counts.Contains(t.Number)).GroupBy(t => t.Number).Select(t=>(baseCards: t.Take(3).ToList(), count: 2)).ToList();
        return avas;
    }
}