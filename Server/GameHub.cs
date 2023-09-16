using Crummy.Web.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Crummy.Web.Server;

public class GameHub : Hub
{
    private readonly GameStateManager gameStateManager;

    public GameHub(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
            throw new Exception("...");

        var query = httpContext.Request.Query;
        var gameId = Guid.Parse(query["gameId"]);

        var game = gameStateManager.GetGameState(gameId);

        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
        await Clients.Caller.SendAsync("InitialState", game.Cards);
    }

    public async Task Update(int id, double x, double y)
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
            throw new Exception("...");

        var query = httpContext.Request.Query;
        var gameId = Guid.Parse(query["gameId"]);

        var game = gameStateManager.GetGameState(gameId);

        var card = game.Cards.FirstOrDefault(c => c.Id == id);

        card.X = x;
        card.Y = y;

        await Clients.Group(gameId.ToString()).SendAsync("Update", id, x, y);
    }
}

public class GameStateManager
{
    private Dictionary<Guid, GameState> gameStates;

    public GameStateManager()
    {
        gameStates = new Dictionary<Guid, GameState>();
    }

    public GameState GetGameState(Guid id)
    {
        if (gameStates.TryGetValue(id, out var gameState))
        {
            return gameState;
        }

        gameState = new GameState
        {
        };

        gameStates[id] = gameState;
        return gameState;
    }
}

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
                Id = i,
                X = i % 10 * 100,
                Y = i / 10 * 150
            });
        }
    }
}