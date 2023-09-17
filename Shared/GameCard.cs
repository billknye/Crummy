namespace Crummy.Web.Shared;


public record GameCard
{
    public int Id { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Angle { get; set; }

    public Value? Value { get; set; }

    public Suit? Suit { get; set; }

    public string Image
    {
        get
        {
            if (Value is not null && Suit is not null)
            {
                return $"Classic/{Suit.ToString()[0]}{(int)Value:00}.png";
            }

            return $"Card-Back-03.png";
        }
    }
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

public enum Suit
{
    Club = 0,
    Diamonds = 1,
    Heart = 2,
    Spade = 3
}

public interface IGameHubClient
{
    Task InitialState(IEnumerable<GameCard> cards);
    Task Update(int id, double x, double y);
}

public interface IGameHubServer
{
    Task Update(int id, double x, double y);
}