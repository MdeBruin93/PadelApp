using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.SignalR.Management;
using Microsoft.EntityFrameworkCore;
using PadelApp;
using PadelApp.Components;
using PadelApp.Components.Account;
using PadelApp.Components.Pages.Users;
using PadelApp.Data;
using PadelApp.Data.Models;
using PadelApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddSingleton(_ =>
{
    return new ServiceManagerBuilder()
        .WithOptions(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("signalr");
        })
        .BuildServiceManager();
});

builder.Services.AddDbContext<PadelDbContext>(optionsBuilder =>
{
    var connectionString = builder.Configuration.GetConnectionString("database");
    optionsBuilder.UseSqlServer(connectionString);
    // optionsBuilder.UseInMemoryDatabase("PadelDb");
});

builder.EnrichSqlServerDbContext<PadelDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30; // seconds
    });
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<PadelDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ISchemeReleaseService, SchemeReleaseService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPouleService, PouleService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IBracketService, BracketService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IPlayerStatsService, PlayerStatsService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddSingleton<IRealtimeService, SignalRService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IDatabaseSeeder, OfficialDatabaseSeeder>();
    // builder.Services.AddScoped<IDatabaseSeeder, DemoDatabaseSeeder>();
}
else
{
    builder.Services.AddScoped<IDatabaseSeeder, OfficialDatabaseSeeder>();
}

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapHub<PadelHub>("/padelhub");

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Minimal API endpoint to add a user
app.MapPost("/api/users", async ([FromBody] UserViewModel viewModel, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("users-api");
    // // Check for unique AuthKey
    // if (await db.Users.AnyAsync(u => u.AuthKey == user.AuthKey))
    // {
    //     logger.LogWarning("AuthKey {AuthKey} already exists.", user.AuthKey);
    //     return Results.BadRequest("AuthKey already exists.");
    // }

    try
    {
        var user = new ApplicationUser
        {
            UserName = viewModel.DiscordName,
            Name = viewModel.Name,
            DiscordName = viewModel.DiscordName,
            EmailConfirmed = true
        };
        var pass = Guid.NewGuid().ToString("N");
        var result = await userManager.CreateAsync(user, pass);
        if (!result.Succeeded)
        {
            logger.LogError("Error while adding '{Name}'. Errors: {Errors}", user.Name, string.Join(',', result.Errors.Select(e => e.Description)));
        }
        result = await userManager.AddToRoleAsync(user, viewModel.Role);

        if (result.Succeeded)
        {
            logger.LogInformation("User created: '{pass}'", pass);
            return Results.Created($"/api/users/{user.Id}", user);
        }

        logger.LogError("Error while adding '{Name}'. Errors: {Errors}", user.Name, string.Join(',', result.Errors.Select(e => e.Description)));
        return Results.BadRequest();
    }
    catch (Exception e)
    {
        logger.LogError(e, "Error adding user.");
        return Results.Problem("Error adding user.", statusCode: 500);
    }
});

app.MapPost("/api/users/refresh", async ([FromBody] UserViewModel viewModel, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("users-api");

    try
    {
        var user = await userManager.FindByNameAsync(viewModel.DiscordName);
        if (user is null)
        {
            logger.LogInformation($"Could not find user '{viewModel.DiscordName}'");
            return Results.BadRequest($"Could not find user '{viewModel.DiscordName}'");
        }

        var pass = Guid.NewGuid().ToString("N");
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, resetToken, pass);
        if (result.Succeeded)
        {
            logger.LogInformation("User successfully refreshed '{Pass}'", pass);
            return Results.Ok();
        }

        logger.LogError("Error while refreshing user '{Name}'. Errors: {Errors}", user.DiscordName, string.Join(',', result.Errors.Select(e => e.Description)));
        return Results.BadRequest("");
    }
    catch (Exception e)
    {
        logger.LogError(e, "Error refreshing user.");
        return Results.Problem("Error refreshing user.", statusCode: 500);
    }
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PadelDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }

    if (dbContext.TournamentSettings.FirstOrDefault(ts => ts.Id == SchemeReleaseService.Id) == null)
    {
        // Create default settings if not present
        var settings = new TournamentSettings { Id = SchemeReleaseService.Id, BracketsReleased = false, PoulesReleased = false };
        dbContext.TournamentSettings.Add(settings);
        dbContext.SaveChanges();
    }

    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedRoleDatabaseAsync();
    await seeder.SeedPlayerDatabaseAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    await new DataMigrator(dbContext, seeder, roleManager).MigrateData();
}

app.Run();