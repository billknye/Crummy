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
