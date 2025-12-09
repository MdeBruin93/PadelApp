using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PadelApp.Config;
using PadelApp.Data;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IDatabaseSeeder
{
    Task SeedRoleDatabaseAsync();
    Task SeedPlayerDatabaseAsync();
    Task SeedPouleDatabaseAsync();
    Task SeedBracketDatabaseAsync(BracketType bracketType, List<OfficialDatabaseSeeder.BracketSeedPlayer> players);
}

public class OfficialDatabaseSeeder(ILogger<OfficialDatabaseSeeder> logger, PadelDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IDatabaseSeeder
{
    public async Task SeedRoleDatabaseAsync()
    {
        logger.LogInformation("Checking user roles");
        if (await dbContext.Roles.AnyAsync())
        {
            logger.LogInformation("User roles already exists");
            return;
        }
        logger.LogInformation("Seeding user roles");

        var roles = new List<ApplicationRole>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.Admin
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.Crew
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.Player
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = RoleConstants.Guest
            }
        };

        foreach (var role in roles)
        {
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                logger.LogError("Error while adding '{Name}'. Errors: {Errors}", role.Name, string.Join(',', result.Errors));
            }
        }
        logger.LogInformation("Seeding user roles done");
    }

    public async Task SeedPlayerDatabaseAsync()
    {
        logger.LogInformation("Checking users");
        if (dbContext.Users.Any())
        {
            logger.LogInformation("Users already exists");
            return;
        }

        logger.LogInformation("Seeding user roles");
        var json = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "Data/SeedData/official_players.json"));
        var players = JsonSerializer.Deserialize<List<SeedPlayer>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        foreach (var player in players!)
        {
            var pass = Guid.NewGuid().ToString("N");
            var user = new ApplicationUser
            {
                Name = player.Name,
                DiscordName = player.DiscordName,
                UserName = player.DiscordName,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, pass);

            if (!result.Succeeded)
            {
                logger.LogError("Error while adding player '{Name}'. Errors: {Errors}", player.Name, string.Join(',', result.Errors));
            }

            result = await userManager.AddToRoleAsync(user, player.Role);
            if (!result.Succeeded)
            {
                logger.LogError("Error while adding player '{Name}' to role. Errors: {Errors}", player.Name, string.Join(',', result.Errors));
            }
        }
    }

    public async Task SeedPouleDatabaseAsync()
    {
        if (dbContext.Poules.Any())
        {
            logger.LogInformation("Poules already exists");
            return;
        }

        // Create 5 poules with codes A-E
        for (int i = 0; i < 5; i++)
        {
            var code = ((char)('A' + i)).ToString();
            var poule = new Poule { Name = $"Poule {i + 1}", Code = code };
            dbContext.Poules.Add(poule);
        }
        await dbContext.SaveChangesAsync();

        var pouleA = await dbContext.Poules.FirstAsync(p => p.Name == "Poule 1");
        pouleA.Players =
        [
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "MalseMakkert".ToLower()).ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "krekelmoss").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "sebasya97").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "vaagjunior").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "bullz").ConfigureAwait(false)
        ];
        dbContext.Poules.Update(pouleA);
        await SeedPouleAMatchesDatabaseAsync(pouleA).ConfigureAwait(false);

        var pouleB = await dbContext.Poules.FirstAsync(p => p.Name == "Poule 2");
        pouleB.Players =
        [
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "suikerspinmb").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "marnixwesthuis").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "coifmonkey").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "jeffskye").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "woefie").ConfigureAwait(false)
        ];
        dbContext.Poules.Update(pouleB);
        await SeedPouleBMatchesDatabaseAsync(pouleB).ConfigureAwait(false);

        var pouleC = await dbContext.Poules.FirstAsync(p => p.Name == "Poule 3");
        pouleC.Players =
        [
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "eliasmeteengrotejas").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "lejon92").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "jeroenmaaas").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "mrflintstonekamo").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "wannabee101").ConfigureAwait(false)
        ];
        dbContext.Poules.Update(pouleC);
        await SeedPouleCMatchesDatabaseAsync(pouleC).ConfigureAwait(false);

        var pouleD = await dbContext.Poules.FirstAsync(p => p.Name == "Poule 4");
        pouleD.Players =
        [
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "streamwithlien").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "rick").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "dreamrrboy").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "darkreaperss").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "VGeNesiS_".ToLower()).ConfigureAwait(false)
        ];
        dbContext.Poules.Update(pouleD);
        await SeedPouleDMatchesDatabaseAsync(pouleD).ConfigureAwait(false);

        var pouleE = await dbContext.Poules.FirstAsync(p => p.Name == "Poule 5");
        pouleE.Players =
        [
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "BruteBeef".ToLower()).ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "vertomme").ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "Bartje".ToLower()).ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "vanDiek058".ToLower()).ConfigureAwait(false),
            await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "th3glenn".ToLower()).ConfigureAwait(false)
        ];
        dbContext.Poules.Update(pouleE);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
        await SeedPouleEMatchesDatabaseAsync(pouleE).ConfigureAwait(false);
    }

    private async Task SeedPouleAMatchesDatabaseAsync(Poule poule)
    {
        var sam = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "MalseMakkert".ToLower()).ConfigureAwait(false);
        var justin = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "krekelmoss").ConfigureAwait(false);
        var sebas = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "sebasya97").ConfigureAwait(false);
        var vaag = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "vaagjunior").ConfigureAwait(false);
        var bullz = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "bullz").ConfigureAwait(false);

        var startTime = new DateTime(2025, 10, 18, 12, 36, 00);

        var matches = new List<Match>();
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [sam, justin, sebas, vaag],
            TeamA = [sam, justin],
            TeamB = [sebas, vaag],
            StartTime = startTime,
            Court = 2
        });

        var update = startTime.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [bullz, sam, justin, sebas],
            TeamA = [bullz, sam],
            TeamB = [justin, sebas],
            StartTime = update,
            Court = 2
        });

        update = update.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [vaag, bullz, sam, sebas],
            TeamA = [vaag, bullz],
            TeamB = [sam, sebas],
            StartTime = update,
            Court = 2
        });

        update = update.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [justin, bullz, sam, vaag],
            TeamA = [justin, bullz],
            TeamB = [sam, vaag],
            StartTime = update,
            Court = 2
        });

        update = update.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [sebas, bullz, justin, vaag],
            TeamA = [sebas, bullz],
            TeamB = [justin, vaag],
            StartTime = update,
            Court = 2
        });

        await dbContext.Matches.AddRangeAsync(matches);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task SeedPouleBMatchesDatabaseAsync(Poule poule)
    {
        var marrit = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "suikerspinmb".ToLower()).ConfigureAwait(false);
        var marnix = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "marnixwesthuis").ConfigureAwait(false);
        var nathan = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "coifmonkey").ConfigureAwait(false);
        var jeff = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "jeffskye").ConfigureAwait(false);
        var lars = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "woefie").ConfigureAwait(false);

        var startTime = new DateTime(2025, 10, 18, 12, 36, 00);

        var matches = new List<Match>();
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [marrit, marnix, nathan, jeff],
            TeamA = [marrit, marnix],
            TeamB = [nathan, jeff],
            StartTime = startTime,
            Court = 3
        });

        var updated = startTime.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [lars, marrit, marnix, nathan],
            TeamA = [lars, marrit],
            TeamB = [marnix, nathan],
            StartTime = updated,
            Court = 3
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [jeff, lars, marrit, nathan],
            TeamA = [jeff, lars],
            TeamB = [marrit, nathan],
            StartTime = updated,
            Court = 3
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [marnix, jeff, lars, nathan],
            TeamA = [marnix, jeff],
            TeamB = [lars, nathan],
            StartTime = updated,
            Court = 3
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [marnix, lars, marrit, jeff],
            TeamA = [marnix, lars],
            TeamB = [marrit, jeff],
            StartTime = updated,
            Court = 3
        });

        await dbContext.Matches.AddRangeAsync(matches);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task SeedPouleCMatchesDatabaseAsync(Poule poule)
    {
        var elias = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "eliasmeteengrotejas".ToLower()).ConfigureAwait(false);
        var lejon = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "lejon92").ConfigureAwait(false);
        var jeroen = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "jeroenmaaas").ConfigureAwait(false);
        var colin = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "mrflintstonekamo").ConfigureAwait(false);
        var edwin = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "wannabee101").ConfigureAwait(false);

        var startTime = new DateTime(2025, 10, 18, 12, 36, 00);

        var matches = new List<Match>();
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [elias, lejon, jeroen, colin],
            TeamA = [elias, lejon],
            TeamB = [jeroen, colin],
            StartTime = startTime,
            Court = 4
        });

        var updated = startTime.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [edwin, elias, lejon, jeroen],
            TeamA = [edwin, elias],
            TeamB = [lejon, jeroen],
            StartTime = updated,
            Court = 4
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [colin, edwin, jeroen, elias],
            TeamA = [colin, edwin],
            TeamB = [jeroen, elias],
            StartTime = updated,
            Court = 4
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [lejon, colin, edwin, jeroen],
            TeamA = [lejon, colin],
            TeamB = [edwin, jeroen],
            StartTime = updated,
            Court = 4
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [lejon, edwin, elias, colin],
            TeamA = [lejon, edwin],
            TeamB = [elias, colin],
            StartTime = updated,
            Court = 4
        });

        await dbContext.Matches.AddRangeAsync(matches);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task SeedPouleDMatchesDatabaseAsync(Poule poule)
    {
        var lien = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "streamwithlien".ToLower()).ConfigureAwait(false);
        var rick = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "rick").ConfigureAwait(false);
        var brian = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "dreamrrboy").ConfigureAwait(false);
        var kevin = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "darkreaperss").ConfigureAwait(false);
        var viet = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "VGeNesiS_").ConfigureAwait(false);

        var startTime = new DateTime(2025, 10, 18, 12, 36, 00);

        var matches = new List<Match>();
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [lien, rick, brian, kevin],
            TeamA = [lien, rick],
            TeamB = [brian, kevin],
            StartTime = startTime,
            Court = 5
        });

        var updated = startTime.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [viet, lien, rick, brian],
            TeamA = [viet, lien],
            TeamB = [rick, brian],
            StartTime = updated,
            Court = 5
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [kevin, viet, brian, lien],
            TeamA = [kevin, viet],
            TeamB = [brian, lien],
            StartTime = updated,
            Court = 5
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [viet, brian, rick, kevin],
            TeamA = [viet, brian],
            TeamB = [rick, kevin],
            StartTime = updated,
            Court = 5
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [lien, kevin, rick, viet],
            TeamA = [lien, kevin],
            TeamB = [rick, viet],
            StartTime = updated,
            Court = 5
        });

        await dbContext.Matches.AddRangeAsync(matches);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task SeedPouleEMatchesDatabaseAsync(Poule poule)
    {
        var david = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "BruteBeef".ToLower()).ConfigureAwait(false);
        var tom = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "vertomme").ConfigureAwait(false);
        var bart = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "Bartje").ConfigureAwait(false);
        var dennis = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "vanDiek058").ConfigureAwait(false);
        var glenn = await dbContext.Users.FirstAsync(u => u.DiscordName.ToLower() == "th3glenn").ConfigureAwait(false);

        var startTime = new DateTime(2025, 10, 18, 12, 36, 00);

        var matches = new List<Match>();
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [david, tom, bart, dennis],
            TeamA = [david, tom],
            TeamB = [bart, dennis],
            StartTime = startTime,
            Court = 6
        });

        var updated = startTime.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [glenn, david, tom, bart],
            TeamA = [glenn, david],
            TeamB = [tom, bart],
            StartTime = updated,
            Court = 6
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [dennis, david, glenn, tom],
            TeamA = [dennis, david],
            TeamB = [glenn, tom],
            StartTime = updated,
            Court = 6
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [bart, david, dennis, glenn],
            TeamA = [bart, david],
            TeamB = [dennis, glenn],
            StartTime = updated,
            Court = 6
        });

        updated = updated.AddMinutes(11);
        matches.Add(new Match
        {
            PouleId = poule.Id,
            Players = [bart, glenn, tom, dennis],
            TeamA = [bart, glenn],
            TeamB = [tom, dennis],
            StartTime = updated,
            Court = 6
        });

        await dbContext.Matches.AddRangeAsync(matches);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task SeedBracketDatabaseAsync(BracketType bracketType, List<BracketSeedPlayer> players)
    {
        logger.LogInformation("Checking brackets");

        if (await dbContext.Brackets.AnyAsync(b => b.BracketType == bracketType))
        {
            logger.LogInformation("BracketType {BracketType} already exist", bracketType);
            return;
        }

        logger.LogInformation("Seeding brackets");

        var bracket = new Bracket
        {
            BracketType = bracketType,
            Entries = [],
            Matches = []
        };

        SeedBracketEntries(bracket, players);

        if (bracketType == BracketType.Upper)
        {
            SeedUpperBracketMatches(bracket, players);
        }
        else
        {
            SeedLowerBracketMatches(bracket, players);
        }

        dbContext.Brackets.Add(bracket);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeding brackets done");
    }

    private void SeedBracketEntries(Bracket bracket, List<BracketSeedPlayer> bracketSeedPlayer)
    {
        foreach (var seedPlayer in bracketSeedPlayer)
        {
            bracket.Entries.Add(
                new BracketEntry
                {
                    BracketType = bracket.BracketType,
                    PositionCode = seedPlayer.PositionCode,
                    UserId = seedPlayer.User.Id
                });
        }
    }

    private void SeedUpperBracketMatches(Bracket bracket, List<BracketSeedPlayer> bracketSeedPlayer)
    {
        var startTime = new DateTime(2025, 10, 18, 13, 40, 00);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 1,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = startTime,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 1,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = startTime,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ]
        });

        var updated = startTime.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 2,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 2,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 3,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 3,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 4,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 4,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 5,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 5,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 6,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 6,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 7,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 7,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 8,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 8,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 9,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 9,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 10,
            BracketType = BracketType.Upper,
            Court = 5,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C1").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A1").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 10,
            BracketType = BracketType.Upper,
            Court = 6,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E1").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A2").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C2").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E2").User,
            ]
        });
    }

    private void SeedLowerBracketMatches(Bracket bracket, List<BracketSeedPlayer> bracketSeedPlayer)
    {
        var startTime = new DateTime(2025, 10, 18, 13, 40, 00);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 1,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = startTime,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 1,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = startTime,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 1,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = startTime,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
            ]
        });

        var updated = startTime.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 2,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 2,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 2,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 3,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 3,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 3,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 4,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 4,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 4,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 5,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 5,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 5,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 6,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 6,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 6,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 7,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 7,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 7,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 8,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 8,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 8,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 9,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 9,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 9,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
            ]
        });

        updated = updated.AddMinutes(11);
        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 10,
            BracketType = BracketType.Lower,
            Court = 2,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E4").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 10,
            BracketType = BracketType.Lower,
            Court = 3,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "D3").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C3").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "C4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "E3").User,
            ]
        });

        bracket.Matches.Add(new BracketMatch
        {
            BracketRoundNumber = 10,
            BracketType = BracketType.Lower,
            Court = 4,
            StartTime = updated,
            Players =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ],
            TeamA =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "E5").User,
                bracketSeedPlayer.First(p => p.PositionCode == "C5").User,
            ],
            TeamB =
            [
                bracketSeedPlayer.First(p => p.PositionCode == "B4").User,
                bracketSeedPlayer.First(p => p.PositionCode == "A3").User,
            ]
        });
    }

    private class SeedPlayer
    {
        public string Name { get; set; } = string.Empty;
        public string DiscordName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class BracketSeedPlayer
    {
        public string PositionCode { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}