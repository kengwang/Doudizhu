using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService;

// 笨笨机器人代打
public class AutoCardMachineService(IEnumerable<CardPattern> patterns) : IRegisterSelfService
{
    private readonly Dictionary<CardPatternType, CardPattern> _patterns = patterns.ToDictionary(t => t.PatternType);

    public async Task<List<Card>> FindBestMatchCard(Game game, GameUser gameUser)
    {
        if (game.LastCardSentence is not null)
        {
            // 获取我的当前身份
            var myRole = gameUser.Role;

            // 获取我的上家的身份
            var lastUser = game.LastUser;

            if (lastUser?.Role != myRole)
            {
                // 跟牌
                var result = (await _patterns[game.LastCardSentence.PatternType]
                                  .GetBaseAndNeedle(gameUser.Cards, game.LastCardSentence))
                             .OrderBy(t => t.baseCards.FirstOrDefault()?.Number).ToList();
                var minCard = new List<Card>();
                var trace = new List<List<Card>>();
                var min = int.MaxValue;

                if (result.Count != 0)
                {
                    foreach (var (baseCards, count) in result)
                    {
                        var variableCards = gameUser.Cards.ToList();

                        foreach (var card in baseCards)
                        {
                            variableCards.Remove(card);
                        }

                        var cnt = await GetCardSingleCount(variableCards, game.LastCardSentence, [baseCards]);

                        if (cnt.Item1 < min)
                        {
                            minCard = baseCards;
                            min = cnt.Item1;
                            // trace = cnt.Item2;
                        }
                    }
                }
                
                return minCard;
            }
            else
            {
                // 上家和自己同一阵营
                // 记牌器, 计算场上剩下的牌
                if (true)
                {
                    // 正常记牌
                    var leftCards = Game.GetAllCards();

                    foreach (var card in game.Records.SelectMany(t=>t.Cards))
                    {
                        leftCards.Remove(card);
                    }
                    
                    foreach (var card in gameUser.Cards)
                    {
                        leftCards.Remove(card);
                    }
                    
                    // 计算场上剩下的牌
                    var leftCardCount = leftCards.CountBy(t => t.Number).ToList();
                    
                    
                }
                
                
            }
        }
        else
        {
            var trace = new List<List<Card>>();
            var (min, minTrace) = await GetCardSingleCount(gameUser.Cards, null, trace);
            // 主动出牌
            if (min * 1.0 / gameUser.Cards.Count > 0.20) // 走不脱, 把单牌清理一下
            {
                var flattend = trace.SelectMany(t => t).ToList();
                var singleCards = gameUser.Cards.ToList();
                foreach (var card in flattend)
                {
                    singleCards.Remove(card);
                }
                // get all which CardNumber only appear once
                var numbers = singleCards.CountBy(p => p.Number).Where(t => t.Value == 1).Select(t => t.Key).ToList();
                var singleCard = singleCards.Where(t => numbers.Contains(t.Number)).OrderBy(t => t.Number)
                                            .FirstOrDefault();

                if (singleCard is not null)
                    return [singleCard];
                    
            }
            else
            {
                foreach (var curCards in minTrace)
                {
                    PrintCards(curCards);
                }

                return minTrace[0];
            }
        }
        
        return [];
    }
    

    public async Task<(int, List<List<Card>>)> GetCardSingleCount(List<Card> cards,
                                                                  CardSentence? lastCardSentence,
                                                                  List<List<Card>> trace)
    {
        // 采用深度优先搜索, 递归算出牌型的单张数, 力求得到最佳

        // 打完啦!
        if (cards.Count == 0)
        {
            return (0, trace);
        }
        var minTrace = trace;
        var min = int.MaxValue;
        List<CardPatternType> cardPatternTypes = [
            CardPatternType.Straight,
            CardPatternType.PlaneWithPair,
            CardPatternType.ThreeWithOne,
            CardPatternType.MultiPair,
            CardPatternType.FourWithPair,
            CardPatternType.MultiPair,
            CardPatternType.ThreeWithPair,
            CardPatternType.Pair,
            CardPatternType.Single,
            CardPatternType.Bomb,
        ];
       
        foreach (var cardPatternType in cardPatternTypes)
        {
            var res = await _patterns[cardPatternType].GetBaseAndNeedle(cards, lastCardSentence);
            if (res.Count != 0)
            {
                foreach (var (baseCards, count) in res)
                {
                    var variableCards = cards.ToList();

                    foreach (var card in baseCards)
                    {
                        variableCards.Remove(card);
                    }
                    var newTrace = trace.ToList();
                    newTrace.Add(baseCards);
                    var nextCount = await GetCardSingleCount(variableCards, null, newTrace);

                    if (nextCount.Item1 == 0)
                    {
                        return (0, nextCount.Item2);
                    }

                    if (nextCount.Item1 < min)
                    {
                        min = nextCount.Item1 - count;
                        minTrace = nextCount.Item2;
                    }
                }
            }
        }
        
        return (min, minTrace);
    }
    
    private static void PrintCards(List<Card> cards)
    {
        foreach (var card in cards)
        {
            Console.Write(card.Number);
            Console.Write(" ");
        }
        Console.WriteLine();
    }
}