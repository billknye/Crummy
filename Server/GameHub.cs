using Crummy.Web.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Crummy.Web.Server;

public class GameHub : Hub<IGameHubClient>
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
        var playerId = Guid.Parse(query["playerId"]);
        var name = query["name"];

        var game = gameStateManager.GetGameState(gameId);

        var player = game.Join(playerId, name);

        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());

        await Clients.Caller.InitialState(game.Cards);
        //await Clients.Caller.SendAsync("InitialState", game.Cards);
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

        card = card with
        {
            X = x,
            Y = y
        };

        await Clients.Group(gameId.ToString()).Update(id, x, y);

        //await Clients.Group(gameId.ToString()).SendAsync("Update", id, x, y);
    }
}