using Doudizhu.Api.Interfaces.Markers;
using Doudizhu.Api.Models.GameLogic;

namespace Doudizhu.Api.Service.CardService;

// 笨笨机器人代打
public class AutoCardMachineService(IEnumerable<CardPattern> patterns) : IRegisterSelfService
{
    private readonly Dictionary<CardPatternType, CardPattern> _patterns = patterns.ToDictionary(t => t.PatternType);
    private readonly Dictionary<Guid, GamePredictorState> _predictorStates = new();

    public async Task<int> CallLordCount(Game game, GameUser gameUser)
    {
        return gameUser.Cards.CountBy(t => t.Number).Count(t => t is { Key: > CardNumber.K, Value: 4 });
    }

    public async Task<List<Card>> FindBestMatchCard(Game game, GameUser gameUser)
    {
        if (!_predictorStates.ContainsKey(game.Id))
            _predictorStates[game.Id] = new();

        if (game.LastCardSentence is not null)
        {
            // 获取我的当前身份
            var myRole = gameUser.Role;
            bool forseFollow = false;

            // 获取我的上家的身份
            var lastUser = game.Records.LastOrDefault()?.GameUser;
            playFollowCard:

            if (lastUser?.Role != myRole || forseFollow)
            {
                Console.WriteLine("跟牌");
                // 跟牌
                var result = (await _patterns[game.LastCardSentence.PatternType]
                                  .GetBaseAndNeedle(gameUser.Cards, game.LastCardSentence))
                             .OrderBy(t => t.baseCards.FirstOrDefault()?.Number).ToList();
                var minCard = new List<Card>();
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
                        }
                    }
                    Console.WriteLine("找到了跟牌, " + minCard.Count);
                    var neededSingleCount = game.LastCardSentence.Cards.Count - minCard.Count;
                    var reservedCards = gameUser.Cards.ToList().Except(minCard).ToList();
                    Console.WriteLine("还要 " + minCard.Count + "张单牌");
                    for (var i = 0; i < neededSingleCount; i++)
                    {
                        var res = await _patterns[CardPatternType.Single]
                                      .GetBaseAndNeedle(reservedCards, game.LastCardSentence);
                        Card? minSingle = null;
                        var minSingleCount = int.MaxValue;

                        foreach ((List<Card> baseCards, int count) in res)
                        {
                            var tmpCards = reservedCards.ToList();
                            tmpCards.Remove(baseCards[0]);
                            var (singleCnt, _) = await GetCardSingleCount(tmpCards, null, []);

                            if (singleCnt < minSingleCount)
                            {
                                minSingle = baseCards[0];
                                minSingleCount = singleCnt;
                            }
                        }

                        if (minSingle is not null)
                        {
                            minCard.Add(minSingle);
                            reservedCards.Remove(minSingle);
                        }
                    }
                }
                else
                {
                    if (lastUser is { Cards: { Count: < 8 } } && gameUser.Role != lastUser.Role)
                    {
                        Console.WriteLine("要炸");
                        // 要炸!
                        var bomb = gameUser.Cards.CountBy(t => t.Number).Where(t => t.Value == 4).ToList();

                        if (bomb.Count != 0)
                        {
                            var minBoom = bomb.MinBy(t => t.Key);
                            minCard = gameUser.Cards.Where(t => t.Number == minBoom.Key).ToList();
                        }

                        // 王炸!
                        Console.WriteLine("王炸");
                        if (gameUser.Cards.Any(t => t.Number == CardNumber.SmallJoker) &&
                            gameUser.Cards.Any(t => t.Number == CardNumber.BigJoker))
                        {
                            minCard = gameUser
                                      .Cards.Where(t => t.Number is CardNumber.SmallJoker or CardNumber.BigJoker)
                                      .ToList();
                        }
                    }
                }

                return minCard;
            }
            else
            {
                // 上家和自己同一阵营
                // 记牌器, 计算场上剩下的牌

                // 正常记牌
                var leftCards = Game.GetAllCards();

                foreach (var card in game.Records.SelectMany(t => t.CardSentence?.Cards ?? []))
                {
                    leftCards.Remove(card);
                }

                foreach (var card in gameUser.Cards)
                {
                    leftCards.Remove(card);
                }

                // 计算场上剩下的牌
                var partner = game.Users.FirstOrDefault(t => t != gameUser && t.Role == GameUserRole.Farmer);
                var leftPartnerCardCount = partner?.Cards.Count ?? 0;

                if (leftPartnerCardCount < 5)
                    return [];

                forseFollow = true;

                goto playFollowCard;
            }
        }
        else
        {
            if (gameUser.Role == GameUserRole.Landlord ||
                !game.Users.Any(t => t != gameUser && t is { Role: GameUserRole.Farmer, Cards.Count: < 4 }))
            {
                // 挺好的, 正常出, 不需要考虑喂牌
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
                    var numbers = singleCards.CountBy(p => p.Number).Where(t => t.Value == 1).Select(t => t.Key)
                                             .ToList();
                    var singleCard = singleCards.Where(t => numbers.Contains(t.Number)).OrderBy(t => t.Number)
                                                .FirstOrDefault();

                    if (singleCard is not null)
                        return [singleCard];
                }
                else
                {
                    return minTrace[0];
                }
            }
            else
            {
                // 考虑喂牌
                var partner = game.Users.FirstOrDefault(t => t != gameUser && t.Role == GameUserRole.Farmer);

                if (partner?.Cards.Count == 1)
                {
                    // 最小的牌喂走
                    return [gameUser.Cards.MinBy(t => t.Number)!];
                }

                if (partner?.Cards.Count < 4)
                {
                    if (_predictorStates[game.Id].PartnerTriedPairLastCount != partner.Cards.Count)
                    {
                        return [gameUser.Cards.MinBy(t => t.Number)!];
                    }
                    else
                    {
                        _predictorStates[game.Id].PartnerTriedPairLastCount = partner.Cards.Count;
                        var res = await _patterns[CardPatternType.Pair].GetBaseAndNeedle(gameUser.Cards, null);

                        if (res.Count > 0)
                        {
                            var min = res.MinBy(t => t.baseCards[0].Number);

                            return min.baseCards;
                        }
                        else
                        {
                            return [gameUser.Cards.MinBy(t => t.Number)!];
                        }
                    }
                }
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
        List<CardPatternType> cardPatternTypes =
        [
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

public class GamePredictorState
{
    public int PartnerTriedPairLastCount { get; set; } = -1;
}