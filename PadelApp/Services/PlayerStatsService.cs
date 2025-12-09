using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IPlayerStatsService
{
    PlayerStatsEntry GetPlayerStatsForPoule(ApplicationUser player, Guid pouleId, IReadOnlyDictionary<Guid, List<Match>>? matchesByPoule = null);
    PlayerStatsEntry GetPlayerStatsForBracket(ApplicationUser player, Guid bracketId, List<BracketMatch> bracketMatches);
}

public class PlayerStatsService : IPlayerStatsService
{
    public PlayerStatsEntry GetPlayerStatsForPoule(ApplicationUser player, Guid pouleId, IReadOnlyDictionary<Guid, List<Match>>? matchesByPoule)
    {
        List<Match> matches;
        if (matchesByPoule != null && matchesByPoule.TryGetValue(pouleId, out var pouleMatches))
        {
            matches = pouleMatches;
        }
        else
        {
            throw new ArgumentNullException(nameof(matchesByPoule));
        }

        int matchesPlayed = 0, matchesWon = 0, matchesLost = 0, draws = 0, pointsScored = 0, pointsLost = 0;

        foreach (var match in matches)
        {
            matchesPlayed = CalculateStatsForPoule(player, match, matchesPlayed, ref pointsScored, ref pointsLost, ref matchesWon, ref matchesLost, ref draws);
        }

        int totalPoints = matchesWon * 3 + draws * 2 + matchesLost * 1 + pointsScored * 1;

        return new PlayerStatsEntry
        {
            Player = player,
            MatchesPlayed = matchesPlayed,
            MatchesWon = matchesWon,
            MatchesLost = matchesLost,
            Draws = draws,
            PointsScored = pointsScored,
            PointsLost = pointsLost,
            TotalPoints = totalPoints
        };
    }

    private static int CalculateStatsForPoule(ApplicationUser player, Match match, int matchesPlayed, ref int pointsScored, ref int pointsLost, ref int matchesWon, ref int matchesLost, ref int draws)
    {
        if (!match.IsFinished)
        {
            return matchesPlayed;
        }

        bool isTeamA = match.TeamA.Any(u => u.Id == player.Id);
        bool isTeamB = match.TeamB.Any(u => u.Id == player.Id);
        if (!isTeamA && !isTeamB)
        {
            return matchesPlayed;
        }

        matchesPlayed++;

        int teamScore = isTeamA ? match.ScoreTeamA : match.ScoreTeamB;
        int opponentScore = isTeamA ? match.ScoreTeamB : match.ScoreTeamA;

        pointsScored += teamScore;
        pointsLost += opponentScore;

        if (teamScore > opponentScore)
        {
            matchesWon++;
        }
        else if (teamScore < opponentScore)
        {
            matchesLost++;
        }
        else
        {
            draws++;
        }

        return matchesPlayed;
    }

    public PlayerStatsEntry GetPlayerStatsForBracket(ApplicationUser player, Guid bracketId, List<BracketMatch> bracketMatches)
    {
        int matchesPlayed = 0, matchesWon = 0, matchesLost = 0, draws = 0, pointsScored = 0, pointsLost = 0;

        foreach (var match in bracketMatches)
        {
            matchesPlayed = CalculateStatsForBracket(player, match, matchesPlayed, ref pointsScored, ref pointsLost, ref matchesWon, ref matchesLost, ref draws);
        }

        var totalPoints = matchesWon * 3 + draws * 2 + matchesLost * 1 + pointsScored * 1;

        return new PlayerStatsEntry
        {
            Player = player,
            MatchesPlayed = matchesPlayed,
            MatchesWon = matchesWon,
            MatchesLost = matchesLost,
            Draws = draws,
            PointsScored = pointsScored,
            PointsLost = pointsLost,
            TotalPoints = totalPoints
        };
    }

    private static int CalculateStatsForBracket(ApplicationUser player, BracketMatch match, int matchesPlayed, ref int pointsScored, ref int pointsLost, ref int matchesWon, ref int matchesLost, ref int draws)
    {
        if (!match.IsFinished)
        {
            return matchesPlayed;
        }

        var isTeamA = match.TeamA.Any(u => u.Id == player.Id);
        var isTeamB = match.TeamB.Any(u => u.Id == player.Id);
        if (!isTeamA && !isTeamB)
        {
            return matchesPlayed;
        }

        matchesPlayed++;

        var teamScore = isTeamA ? match.ScoreTeamA : match.ScoreTeamB;
        var opponentScore = isTeamA ? match.ScoreTeamB : match.ScoreTeamA;

        pointsScored += teamScore;
        pointsLost += opponentScore;

        if (teamScore > opponentScore)
        {
            matchesWon++;
        }
        else if (teamScore < opponentScore)
        {
            matchesLost++;
        }
        else
        {
            draws++;
        }

        return matchesPlayed;
    }
}