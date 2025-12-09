using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IBracketService
{
    Task<Bracket> GetLowerBracketInfoAsync();
    Task<Bracket> GetUpperBracketInfoAsync();
    Task UpdateBracketMatchScoreAsync(BracketMatch bracketMatch, string user);
    Task<List<BracketMatch>> GetBracketRoundsForUserAsync(Guid userId);
    Task<Dictionary<Guid, string>> GetAllPositionCodes();
}

public class BracketService(PadelDbContext dbContext, IAuditService auditService) : IBracketService
{
    public Task<Bracket> GetLowerBracketInfoAsync()
    {
        return GetBracketInfoAsync(BracketType.Lower);
    }

    public Task<Bracket> GetUpperBracketInfoAsync()
    {
        return GetBracketInfoAsync(BracketType.Upper);
    }

    private async Task<Bracket> GetBracketInfoAsync(BracketType bracketType)
    {
        var bracket = await dbContext.Brackets
            .Include(bracket => bracket.Entries)
            .Include(bracket => bracket.Matches)
            .Include("Matches.Players")
            .Include("Matches.TeamA")
            .Include("Matches.TeamB")
            .Include("Entries.User")
            .FirstAsync(b => b.BracketType == bracketType);

        return bracket;
    }

    public async Task UpdateBracketMatchScoreAsync(BracketMatch bracketMatch, string user)
    {
        var originalMatch = await dbContext.BracketMatch.FirstAsync(bm => bm.Id == bracketMatch.Id);

        await auditService.LogBracketMatchScoreUpdateAsync(bracketMatch,
            user,
            originalMatch.ScoreTeamA,
            originalMatch.ScoreTeamB,
            bracketMatch.ScoreTeamA,
            bracketMatch.ScoreTeamB);

        dbContext.BracketMatch.Update(bracketMatch);
        await dbContext.SaveChangesAsync();
    }

    public Task<List<BracketMatch>> GetBracketRoundsForUserAsync(Guid userId)
    {
        return dbContext.BracketMatch
            .Where(match => match.Players.Any(player => player.Id == userId))
            .Include(m => m.Players)
            .Include(m => m.TeamA)
            .Include(m => m.TeamB)
            .ToListAsync();
    }

    public Task<Dictionary<Guid, string>> GetAllPositionCodes()
    {
        return dbContext.BracketEntries.ToDictionaryAsync(be => be.UserId, be => be.PositionCode);
    }
}