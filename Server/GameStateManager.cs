using Crummy.Web.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Crummy.Web.Server;

public class GameStateManager
{
    private readonly IHubContext<GameHub, IGameHubClient> hubContext;
    private readonly ILogger<GameState> gameStateLogger;
    private Dictionary<Guid, GameState> gameStates;

    public GameStateManager(IHubContext<GameHub, IGameHubClient> hubContext, ILogger<GameState> gameStateLogger)
    {
        gameStates = new Dictionary<Guid, GameState>();
        this.hubContext = hubContext;
        this.gameStateLogger = gameStateLogger;
    }

    public GameState GetGameState(Guid gameId)
    {
        if (gameStates.TryGetValue(gameId, out var gameState))
        {
            return gameState;
        }

        gameState = new GameState(gameStateLogger, hubContext, gameId);

        gameStates[gameId] = gameState;
        return gameState;
    }
}
