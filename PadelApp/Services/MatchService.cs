using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IMatchService
{
    Task<Match> FindByIdAsync(Guid id);
    Task SaveChangesAsync(Match match, string user, string pouleName, int orignalScoreA, int orignalScoreB);
}

public class MatchService(PadelDbContext dbContext, IAuditService auditService) : IMatchService
{
    public Task<Match> FindByIdAsync(Guid id)
    {
        return dbContext.Matches.FirstAsync(m => m.Id == id);
    }

    public async Task SaveChangesAsync(Match match, string user, string pouleName, int orignalScoreA, int orignalScoreB)
    {
        if (orignalScoreA != match.ScoreTeamA || orignalScoreB != match.ScoreTeamB)
        {
            // If scores are different, log the audit
            await auditService.LogPouleMatchScoreUpdateAsync(
                match,
                pouleName,
                user,
                orignalScoreA,
                orignalScoreB,
                match.ScoreTeamA,
                match.ScoreTeamB);
        }

        dbContext.Matches.Update(match);
        await dbContext.SaveChangesAsync();
    }
}