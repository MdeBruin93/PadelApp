using Microsoft.AspNetCore.SignalR;
using PadelApp.Data.Models;

namespace PadelApp;

public class PadelHub : Hub
{
    // Called by server when a match score is updated
    public async Task BroadcastScoreUpdate(Guid matchId, int scoreTeamA, int scoreTeamB)
    {
        await Clients.All.SendAsync("ReceiveScoreUpdate", matchId, scoreTeamA, scoreTeamB);
    }

    public async Task BroadcastPoulesReleased()
    {
        await Clients.All.SendAsync("PoulesReleased");
    }

    public async Task BroadcastBracketsReleased(string groupId, BracketType bracketType, string positionCode)
    {
        await Clients.Group(groupId).SendAsync("BracketsReleased", groupId, bracketType, positionCode);
    }

    public async Task RegisterUser(string userId)
    {
        // Add the connection to a group named after the userId
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
}