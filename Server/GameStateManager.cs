namespace Crummy.Web.Server;

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
