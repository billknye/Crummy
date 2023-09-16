namespace Crummy.Web.Shared;


public class GameCard
{
    public int Id { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public Value Value => (Value)(Id % 13 + 1);

    public Suit Suit => (Suit)(Id / 13);

    public string Image => $"Classic/{Suit.ToString()[0]}{(int)Value:00}.png";
}

public enum Value
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public enum Suit : ushort
{
    Club = 0,
    Diamonds = 1,
    Heart = 2,
    Spade = 3
}