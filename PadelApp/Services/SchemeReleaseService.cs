using Microsoft.EntityFrameworkCore;
using PadelApp.Data;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface ISchemeReleaseService
{
    bool BracketsReleased { get; }
    bool PoulesReleased { get; }
    void ReleaseBrackets();
    void HideBrackets();
    void ReleasePoules();
    void HidePoules();
    void ResetSettings();
}

public class SchemeReleaseService : ISchemeReleaseService
{
    public static Guid Id = Guid.Parse("0199bb5c-53b3-7a48-b988-476fa5a287fd");

    private readonly PadelDbContext _dbContext;

    public SchemeReleaseService(PadelDbContext dbContext)
    {
        _dbContext = dbContext;
        LoadSettings();
    }

    // Indicates if brackets are released to all users
    public bool BracketsReleased { get; private set; }

    // Indicates if poules are released to all users
    public bool PoulesReleased { get; private set; }

    private void LoadSettings()
    {
        var settings = _dbContext.TournamentSettings.AsNoTracking().First(ts => ts.Id == Id);
        BracketsReleased = settings.BracketsReleased;
        PoulesReleased = settings.PoulesReleased;
    }

    public void ReleaseBrackets()
    {
        SetSettings(bracketsReleased: true, poulesReleased: PoulesReleased);
    }

    public void HideBrackets()
    {
        SetSettings(bracketsReleased: false, poulesReleased: PoulesReleased);
    }

    public void ReleasePoules()
    {
        SetSettings(bracketsReleased: BracketsReleased, poulesReleased: true);
    }

    public void HidePoules()
    {
        SetSettings(bracketsReleased: BracketsReleased, poulesReleased: false);
    }

    public void ResetSettings()
    {
        var settings = _dbContext.TournamentSettings.FirstOrDefault(ts => ts.Id == Id);
        if (settings is null)
        {
            settings = new TournamentSettings
            {
                Id = Id,
                BracketsReleased = false,
                PoulesReleased = false
            };
            _dbContext.TournamentSettings.Add(settings);
            _dbContext.SaveChanges();
        }
        BracketsReleased = settings.BracketsReleased;
        PoulesReleased = settings.PoulesReleased;
    }

    private void SetSettings(bool bracketsReleased, bool poulesReleased)
    {
        var settings = _dbContext.TournamentSettings.First(ts => ts.Id == Id);
        settings.BracketsReleased = bracketsReleased;
        settings.PoulesReleased = poulesReleased;
        _dbContext.SaveChanges();

        BracketsReleased = bracketsReleased;
        PoulesReleased = poulesReleased;
    }
}