using Crummy.Web.Shared;

namespace Crummy.Web.Server;

public class GameState
{
    List<GameCard> cards;

    List<Player> players;

    public IEnumerable<GameCard>? Cards => cards;

    public GameState()
    {
        cards = new List<GameCard>();
        players = new List<Player>();

        StartGame();
    }

    public Player Join(Guid playerId, string name)
    {
        if (players.Count >= 4)
        {
            throw new InvalidOperationException("game full");
        }

        var existing = players.FirstOrDefault(n => n.PlayerId == playerId);
        if (existing != null)
        {
            return existing;
        }

        var player = new Player
        {
            PlayerId = playerId,
            Name = name,
        };

        players.Add(player);
        return player;
    }

    public void StartGame()
    {
        cards.Clear();

        for (int i = 0; i < 52; i++)
        {
            cards.Add(new GameCard
            {
                Suit = (Suit)(i / 14),
                Value = (Value)(i % 13 + 1)
            });
        }

        var r = new Random();
        for (int n = 0; n < 200; n++)
        {
            var i = r.Next(0, 52);
            var d = r.Next(0, 52);

            if (i == d)
            {
                n--;
                continue;
            }

            var t = cards[d];
            cards[d] = cards[i];
            cards[i] = t;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].Id = i;
            cards[i].X = 20 + i * 16;
            cards[i].Y = 100;
        }
    }

    public class Player
    {
        public Guid PlayerId { get; set; }

        public string? Name { get; set; }

        public bool Ready { get; set; }
    }
}