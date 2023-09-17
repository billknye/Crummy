namespace Crummy.Web.Shared;

public record GameCard
{
    public int Id { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Angle { get; set; }

    public CardValue? Value { get; set; }

    public CardSuit? Suit { get; set; }

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

public record GameStateDto
{
    public GameStage Stage { get; init; }


}



public enum GameStage
{
    Lobby = 0,
    Playing = 1,
    PostGame = 2
}

public record GamePlayerDto
{
    public int Id { get; set; }

    public required string Name { get; set; }
}