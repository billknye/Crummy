using Crummy.Web.Shared;

namespace Crummy.Web.Server;

public class GameState
{
    List<GameCard>? cards;

    public IEnumerable<GameCard>? Cards => cards;

    public GameState()
    {
        cards = new List<GameCard>();

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
            cards[i].X = i % 10 * 100;
            cards[i].Y = i / 10 * 150;
        }
    }
}