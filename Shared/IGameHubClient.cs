namespace Crummy.Web.Shared;

public interface IGameHubClient
{
    Task OnGameJoined(GameStage stage, int id, IEnumerable<GamePlayerDto> players);

    Task OnPlayerJoined(int id, string name);

    Task InitialState(IEnumerable<GameCard> cards);

    Task Update(int id, double x, double y);
    Task PlayerNameUpdated(int id, string name);
}
