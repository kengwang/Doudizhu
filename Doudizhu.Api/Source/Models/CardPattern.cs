namespace Doudizhu.Api.Models;

public abstract class CardPattern
{
    public abstract string Name { get; }
    public abstract CardPatternType PatternType { get; }
    public abstract bool IsMatched(List<Card> card);
    public abstract List<Card> Order(List<Card> card);
    public abstract bool CanCover(CardSentence current, CardSentence last);
}

public enum CardPatternType
{
    Single,
    Pair,
    MultiPair,
    ThreeWithOne,
    ThreeWithPair,
    FourWithPair,
    Straight,
    Plane,
    PlaneWithPair,
    Bomb,
    JokerBomb
}