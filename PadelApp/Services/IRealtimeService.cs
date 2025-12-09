using Microsoft.AspNetCore.SignalR.Client;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IRealtimeService : IAsyncDisposable
{
    public event EventHandler<ReceiveScoreUpdate>? ReceiveScoreUpdateEventHandler;
    public event EventHandler? PoulesReleased;
    public event EventHandler<BracketsReleased>? BracketsReleased;
    void Init(Uri absoluteUri);
    public Task StartConnectionAsync();
    Task BroadcastScoreUpdateAsync(Guid matchId, int scoreA, int scoreB);
    Task BroadcastPoulesReleasedAsync();
    Task BroadcastBracketsReleasedAsync(string playerId, BracketType bracketType, string positionCode);
    Task RegisterUserAsync(string userId);
}

public class ReceiveScoreUpdate(Guid matchId, int scoreA, int scoreB) : EventArgs
{
    public Guid MatchId { get; private set; } = matchId;
    public int ScoreA { get; private set; } = scoreA;
    public int ScoreB { get; private set; } = scoreB;
}

public class BracketsReleased(string playerId, BracketType bracketType, string positionCode) : EventArgs
{
    public string PlayerId { get; private set; } = playerId;
    public BracketType BracketType { get; private set; } = bracketType;
    public string PositionCode { get; private set; } = positionCode;
}

public class SignalRService(ILogger<SignalRService> logger) : IRealtimeService
{
    private bool _disposed;
    private bool _isInitialized;
    private HubConnection? _hubConnection;
    private Uri _uri;

    public event EventHandler<ReceiveScoreUpdate>? ReceiveScoreUpdateEventHandler;
    public event EventHandler? PoulesReleased;
    public event EventHandler<BracketsReleased>? BracketsReleased;

    public void Init(Uri absoluteUri)
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;
        _uri = absoluteUri;

        try
        {
            RetryBuidling(_uri);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error initializing SignalRService: {Message}", e.Message);
        }
    }

    private void RetryBuidling(Uri absoluteUri)
    {
        // Setup SignalR connection
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(absoluteUri)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<Guid, int, int>("ReceiveScoreUpdate", (matchId, scoreA, scoreB) =>
        {
            ReceiveScoreUpdateEventHandler?.Invoke(this, new ReceiveScoreUpdate(matchId, scoreA, scoreB));
        });

        _hubConnection.On("PoulesReleased", () =>
        {
            PoulesReleased?.Invoke(this, EventArgs.Empty);
        });

        _hubConnection.On<string, BracketType, string>("BracketsReleased", (playerId, bracketType, positionCode) =>
        {
            BracketsReleased?.Invoke(this, new BracketsReleased(playerId, bracketType, positionCode));
        });
    }

    public async Task StartConnectionAsync()
    {
        try
        {
            if (_hubConnection is null)
            {
                RetryBuidling(_uri);
            }
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error starting SignalRService: {Message}", e.Message);
        }
    }

    public async Task RegisterUserAsync(string userId)
    {
        try
        {
            if (_hubConnection is null)
            {
                RetryBuidling(_uri);
            }
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("RegisterUser", userId);
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error registering user SignalRService: {Message}", e.Message);
        }
    }

    public async Task BroadcastScoreUpdateAsync(Guid matchId, int scoreA, int scoreB)
    {
        try
        {
            if (_hubConnection is null)
            {
                RetryBuidling(_uri);
            }
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("BroadcastScoreUpdate", matchId, scoreA, scoreB);
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error broadcasting score update: {Message}", e.Message);
        }
    }

    public async Task BroadcastPoulesReleasedAsync()
    {
        try
        {
            if (_hubConnection is null)
            {
                RetryBuidling(_uri);
            }
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("BroadcastPoulesReleased");
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error broadcasting poules released: {Message}", e.Message);
        }
    }

    public async Task BroadcastBracketsReleasedAsync(string playerId, BracketType bracketType, string positionCode)
    {
        try
        {
            if (_hubConnection is null)
            {
                RetryBuidling(_uri);
            }
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync("BroadcastBracketsReleased", playerId, bracketType, positionCode);
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error broadcasting brackets released: {Message}", e.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _isInitialized = false;

        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();   
        }
    }
}