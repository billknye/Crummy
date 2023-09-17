using Crummy.Web.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Crummy.Web.Server;

public class GameState
{
    private readonly ILogger<GameState> logger;
    private readonly IHubContext<GameHub, IGameHubClient> hubContext;
    private readonly Guid gameId;

    List<GameCard> cards;
    List<Player> players;

    int playerIndex;
    private GameStage stage;

    public IEnumerable<GameCard>? Cards => cards;

    public GameState(
        ILogger<GameState> logger,
        IHubContext<GameHub, IGameHubClient> hubContext,
        Guid gameId)
    {
        cards = new List<GameCard>();
        players = new List<Player>();
        this.logger = logger;
        this.hubContext = hubContext;
        this.gameId = gameId;
    }

    public async Task Join(Guid playerId, string name, string connectionId, IGameHubClient client)
    {
        if (players.Count >= 4)
        {
            throw new InvalidOperationException("game full");
        }

        var existing = players.FirstOrDefault(n => n.PlayerId == playerId);
        if (existing != null)
        {
            existing.Client = client;
            await SendOnGameJoined(existing);
        }
        else
        {
            if (stage != GameStage.Lobby)
            {
                throw new InvalidOperationException("Can only join when in lobby stage.");
            }

            var player = new Player
            {
                PlayerId = playerId,
                Name = name,
                Client = client,
                Id = ++playerIndex,
                ConnectionId = connectionId,
            };

            players.Add(player);

            await SendOnGameJoined(player);
            await hubContext.Clients.GroupExcept(gameId.ToString(), player.ConnectionId).OnPlayerJoined(player.Id, player.Name);
        }
    }

    private async Task SendOnGameJoined(Player player)
    {
        await player.Client.OnGameJoined(stage, player.Id, players.Select(n => new GamePlayerDto
        {
            Name = n.Name,
            Id = n.Id,
        }));

        await hubContext.Groups.AddToGroupAsync(player.ConnectionId, gameId.ToString());
        await hubContext.Clients.Client(player.ConnectionId).InitialState(Cards);
    }

    public async Task UpdatePlayerName(string connectionId, string name)
    {
        if (stage != GameStage.Lobby)
        {
            throw new InvalidOperationException("Must be in lobby stage to change name.");
        }

        var player = players.FirstOrDefault(n => n.ConnectionId == connectionId);
        player.Name = name;

        await hubContext.Clients.Group(gameId.ToString()).PlayerNameUpdated(player.Id, player.Name);
    }

    public void StartGame()
    {
        if (players.Count < 3)
        {
            throw new InvalidOperationException("Not enough players, minimum 2");
        }

        if (players.Count > 4)
        {
            throw new InvalidOperationException("Too many players, maximum 4");
        }

        if (players.Any(n => !n.Ready))
        {
            throw new InvalidOperationException("Not all players ready.");
        }

        cards.Clear();

        for (int i = 0; i < 52; i++)
        {
            cards.Add(new GameCard
            {
                Suit = (CardSuit)(i / 14),
                Value = (CardValue)(i % 13 + 1)
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

    internal async Task MoveCard(int id, double x, double y)
    {
        var card = Cards.FirstOrDefault(c => c.Id == id);

        card = card with
        {
            X = x,
            Y = y
        };

        await hubContext.Clients.Group(gameId.ToString()).Update(id, x, y);
    }

    internal async Task OnDisconnect(string connectionId, Exception? exception)
    {
        var player = players.First(n => n.ConnectionId == connectionId);
        logger.LogInformation("Player {PlayerId} With Connection {ConnectionId} Disconnected From Game {GameId}", player.PlayerId, connectionId, gameId);
    }

    public class Player
    {
        /// <summary>
        /// Unique value for rejoining sessions, stored in local storage.
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// Game specific player index.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The signalr connection id.
        /// </summary>
        public required string ConnectionId { get; set; }

        public required string Name { get; set; }

        public bool Ready { get; set; }

        public required IGameHubClient Client { get; set; }
    }
}