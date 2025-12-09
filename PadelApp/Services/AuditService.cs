using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IAuditService
{
    Task<List<Audit>> GetAuditLogsAsync();
    Task LogPouleMatchScoreUpdateAsync(Match match, string pouleName, string user, int oldScoreA, int oldScoreB, int newScoreA, int newScoreB);
    Task LogBracketMatchScoreUpdateAsync(BracketMatch match, string user, int oldScoreA, int oldScoreB, int newScoreA, int newScoreB);
    Task LogPoulesReleasedAsync(ApplicationUser admin);
    Task LogBracketsReleasedAsync(ApplicationUser admin);
}

public class AuditService(PadelDbContext dbContext) : IAuditService
{
    public Task<List<Audit>> GetAuditLogsAsync()
    {
        return dbContext.Audits
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task LogPouleMatchScoreUpdateAsync(Match match, string pouleName, string user, int oldScoreA, int oldScoreB, int newScoreA, int newScoreB)
    {
        var actionData = new
        {
            MatchId = match.Id,
            PouleId = match.PouleId,
            PouleName = pouleName,
            TeamA = match.TeamA.Select(p => new { p.Id, p.Name }).ToList(),
            TeamB = match.TeamB.Select(p => new { p.Id, p.Name }).ToList(),
            OldScore = new { ScoreA = oldScoreA, ScoreB = oldScoreB },
            NewScore = new { ScoreA = newScoreA, ScoreB = newScoreB }
        };

        var audit = new Audit
        {
            Id = Guid.NewGuid(),
            Action = "UpdatePouleMatchScore",
            Username = user,
            Details = $"[Poule {actionData.PouleName}] Score updated from {oldScoreA}-{oldScoreB} to {newScoreA}-{newScoreB}",
            ActionData = JsonSerializer.Serialize(actionData),
            Timestamp = DateTime.UtcNow
        };

        await dbContext.Audits.AddAsync(audit);
        await dbContext.SaveChangesAsync();
    }

    public async Task LogBracketMatchScoreUpdateAsync(BracketMatch match, string user, int oldScoreA, int oldScoreB, int newScoreA, int newScoreB)
    {
        var actionData = new
        {
            MatchId = match.Id,
            BracketType = match.BracketType.ToString(),
            RoundNumber = match.BracketRoundNumber,
            TeamA = match.TeamA.Select(p => new { p.Id, p.Name }).ToList(),
            TeamB = match.TeamB.Select(p => new { p.Id, p.Name }).ToList(),
            OldScore = new { ScoreA = oldScoreA, ScoreB = oldScoreB },
            NewScore = new { ScoreA = newScoreA, ScoreB = newScoreB }
        };

        var audit = new Audit
        {
            Id = Guid.NewGuid(),
            Action = "UpdateBracketMatchScore",
            Username = user,
            Details = $"[{match.BracketType} Round {match.BracketRoundNumber}] Score updated from {oldScoreA}-{oldScoreB} to {newScoreA}-{newScoreB}",
            ActionData = JsonSerializer.Serialize(actionData),
            Timestamp = DateTime.UtcNow
        };

        await dbContext.Audits.AddAsync(audit);
        await dbContext.SaveChangesAsync();
    }

    public async Task LogPoulesReleasedAsync(ApplicationUser admin)
    {
        var audit = new Audit
        {
            Id = Guid.NewGuid(),
            Action = "ReleasePoules",
            Username = admin.Name,
            Details = "Poules have been released",
            Timestamp = DateTime.UtcNow
        };

        dbContext.Audits.Add(audit);
        await dbContext.SaveChangesAsync();
    }

    public async Task LogBracketsReleasedAsync(ApplicationUser admin)
    {
        var audit = new Audit
        {
            Id = Guid.NewGuid(),
            Action = "ReleaseBrackets",
            Username = admin.Name,
            Details = "Brackets have been released",
            Timestamp = DateTime.UtcNow
        };

        dbContext.Audits.Add(audit);
        await dbContext.SaveChangesAsync();
    }
}