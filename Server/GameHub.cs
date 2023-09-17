using Crummy.Web.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Crummy.Web.Server;

public class GameHub : Hub<IGameHubClient>, IGameHubServer
{
    private readonly GameStateManager gameStateManager;

    public GameHub(GameStateManager gameStateManager)
    {
        this.gameStateManager = gameStateManager;
    }

    public override async Task OnConnectedAsync()
    {
        var (game, playerId, name) = GetGamePlayerName();
        await game.Join(playerId, name, Context.ConnectionId, Clients.Caller);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var game = GetGame();
        await game.OnDisconnect(Context.ConnectionId, exception);
    }

    public async Task Update(int id, double x, double y)
    {
        var game = GetGame();
        await game.MoveCard(id, x, y);
    }

    public async Task UpdatePlayerName(string name)
    {
        var game = GetGame();
        await game.UpdatePlayerName(Context.ConnectionId, name);
    }

    private GameState GetGame()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
            throw new Exception("...");

        var query = httpContext.Request.Query;
        var gameId = Guid.Parse(query["gameId"]);

        var game = gameStateManager.GetGameState(gameId);
        return game;
    }

    private (GameState Game, Guid PlayerId, string Name) GetGamePlayerName()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
            throw new Exception("...");

        var query = httpContext.Request.Query;
        var gameId = Guid.Parse(query["gameId"]);
        var playerId = Guid.Parse(query["playerId"]);
        var name = query["name"];

        var game = gameStateManager.GetGameState(gameId);
        return (game, playerId, name);
    }
}