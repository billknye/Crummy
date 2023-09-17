namespace Crummy.Web.Shared;

public interface IGameHubServer
{
    Task Update(int id, double x, double y);

    Task UpdatePlayerName(string name);
}