using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PadelApp.Config;
using PadelApp.Data;
using PadelApp.Data.Models;
using Version = PadelApp.Data.Models.Version;

namespace PadelApp.Services;

public class DataMigrator(PadelDbContext dbContext, IDatabaseSeeder databaseSeeder, RoleManager<ApplicationRole> roleManager)
{
    public async Task MigrateData()
    {
        var version = await dbContext.DataVersions.SingleOrDefaultAsync();
        if (version is null)
        {
            version = new DataVersion { Version = Version.One };
            dbContext.DataVersions.Add(version);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        if (version.Version == Version.One)
        {
            await MigrateToVersionTwoAsync();
            version.Version = Version.Two;
            dbContext.DataVersions.Update(version);

            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        if (version.Version == Version.Two)
        {
            await MigrateToVersionThreeAsync();
            version.Version = Version.Three;
            dbContext.DataVersions.Update(version);

            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    private async Task MigrateToVersionTwoAsync()
    {
        // We updated the names of the players, so we need to reseed the user data.
        dbContext.Users.RemoveRange(dbContext.Users);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
        Console.WriteLine("Poules " + dbContext.Poules.Count());
        Console.WriteLine("brackets " + dbContext.Brackets.Count());
        Console.WriteLine("bracket matches " + dbContext.BracketMatch.Count());

        await databaseSeeder.SeedRoleDatabaseAsync();
        await databaseSeeder.SeedPlayerDatabaseAsync();
    }
    
    private async Task MigrateToVersionThreeAsync()
    {
        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = RoleConstants.ReadOnly,
            NormalizedName = "READ-ONLY"
        });
    }
}