using System.Net;
using Crummy.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Crummy.Web.Client.Pages;

public partial class Play : IAsyncDisposable, IGameHubClient
{
    [Parameter]
    public Guid GameId { get; set; }

    List<GameCard>? cards;
    private HubConnection? hubConnection;
    private IGameHubServer? server;
    private IDisposable? connectionRegistration;

    protected override async Task OnInitializedAsync()
    {
        var playerDetails = await localStorage.GetItemAsync<PlayerDetails>("player-details");

        if (playerDetails is null)
        {
            playerDetails = new PlayerDetails
            {
                PlayerId = Guid.NewGuid(),
                Name = "player"
            };

            await localStorage.SetItemAsync("player-details", playerDetails);
        }

        hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri($"/hub?gameId={GameId}&playerId={playerDetails.PlayerId}&name={WebUtility.UrlEncode(playerDetails.Name)}"))
            .Build();

        server = hubConnection.CreateHubProxy<IGameHubServer>();
        connectionRegistration = hubConnection.Register<IGameHubClient>(this);
        await hubConnection.StartAsync();
    }

    public async Task InitialState(IEnumerable<GameCard> cards)
    {
        this.cards = new List<GameCard>(cards);
        StateHasChanged();
    }

    public async Task Update(int id, double x, double y)
    {
        var i = this.cards.FindIndex(n => n.Id == id);
        cards[i].X = x;
        cards[i].Y = y;

        StateHasChanged();
    }

    private Task OnCardMoved(GameCard card)
    {
        return Send(card);
    }

    private async Task Send(GameCard card)
    {
        if (server is not null)
        {
            await server.Update(card.Id, card.X, card.Y);
        }
    }

    public bool IsConnected =>
        hubConnection?.State == HubConnectionState.Connected;

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }

        connectionRegistration?.Dispose();
    }

    class PlayerDetails
    {
        public Guid PlayerId { get; set; }
        public required string Name { get; set; }
    }
}
