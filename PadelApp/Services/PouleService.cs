using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IPouleService
{
    Task<List<Poule>> GetAllPoulesAndMatchesAsync();
    Task<List<Tuple<Poule, List<Match>>>> GetAllPouleInfo(bool includePlayers = false);
    Task<List<Tuple<Poule, List<Match>>>> GetAllPouleInfoForCurrentUser(Guid userId);
}

public class PouleService(PadelDbContext dbContext) : IPouleService
{
    public Task<List<Poule>> GetAllPoulesAndMatchesAsync()
    {
        return dbContext.Poules.ToListAsync();
    }

    public async Task<List<Tuple<Poule, List<Match>>>> GetAllPouleInfo(bool includePlayers = false)
    {
        var poules = await dbContext.Poules.Include(p => p.Players).ToListAsync();
        var pouleIds = poules.Select(p => p.Id).ToList();

        var matches = await dbContext.Matches.Where(m => pouleIds.Contains(m.PouleId))
            .Include(m => m.TeamA)
            .Include(m => m.TeamB)
            .ToListAsync();

        if (includePlayers)
        {
            var players = await dbContext.Users.ToListAsync();
            foreach (var poule in poules)
            {
                poule.Players.AddRange(players.Where(p => p.PouleId == poule.Id));
            }
        }

        return poules.Select(poule =>
                new Tuple<Poule, List<Match>>(poule, matches.Where(m => m.PouleId == poule.Id).ToList()))
            .ToList();
    }

    public async Task<List<Tuple<Poule, List<Match>>>> GetAllPouleInfoForCurrentUser(Guid userId)
    {
        var user = await dbContext.Users.Include(u => u.Poule).FirstOrDefaultAsync(u => u.Id == userId);
        if (user?.PouleId == null)
        {
            return [];
        }

        var poule = await dbContext.Poules.FirstOrDefaultAsync(p => p.Id == user.PouleId);
        if (poule == null)
        {
            return [];
        }

        var matches = await dbContext.Matches.Where(m => m.PouleId == poule.Id)
            .Include(m => m.TeamA)
            .Include(m => m.TeamB)
            .ToListAsync();

        return [new(poule, matches)];
    }
}