using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data.Models;

namespace PadelApp.Data;

public class PadelDbContext(DbContextOptions<PadelDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<TournamentSettings> TournamentSettings => Set<TournamentSettings>();
    public DbSet<Poule> Poules => Set<Poule>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Bracket> Brackets => Set<Bracket>();
    public DbSet<BracketEntry> BracketEntries => Set<BracketEntry>();
    public DbSet<BracketMatch> BracketMatch => Set<BracketMatch>();

    public DbSet<DataVersion> DataVersions => Set<DataVersion>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Audit>(typeBuilder =>
        {
            typeBuilder.Property(e => e.Action).HasMaxLength(150);
            typeBuilder.Property(e => e.Username).HasMaxLength(150);
            typeBuilder.Property(e => e.Details).HasMaxLength(250);
        });

        builder.Entity<ApplicationUser>(typeBuilder =>
        {
            typeBuilder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            typeBuilder.Property(e => e.DiscordName)
                .IsRequired()
                .HasMaxLength(150);
        });

        builder.Entity<Audit>(typeBuilder =>
        {
            typeBuilder.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();
        });

        builder.Entity<TournamentSettings>(typeBuilder =>
        {
            typeBuilder.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();
        });

        builder.Entity<Poule>(typeBuilder =>
        {
            typeBuilder.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();
        });

        builder.Entity<Match>(typeBuilder =>
        {
            typeBuilder.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();
        });

        builder.Entity<Bracket>(typeBuilder =>
        {
            typeBuilder.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();
        });

        builder.Entity<BracketEntry>(typeBuilder =>
        {
            typeBuilder.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();
        });

        builder.Entity<BracketMatch>(typeBuilder =>
        {
            typeBuilder.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();
        });

        builder.Entity<Match>()
            .HasMany(m => m.Players)
            .WithMany()
            .UsingEntity(j => j.ToTable("MatchPlayers"));
        builder.Entity<Match>()
            .HasMany(m => m.TeamA)
            .WithMany()
            .UsingEntity(j => j.ToTable("MatchTeamA"));
        builder.Entity<Match>()
            .HasMany(m => m.TeamB)
            .WithMany()
            .UsingEntity(j => j.ToTable("MatchTeamB"));

        builder.Entity<BracketMatch>()
            .HasMany(m => m.Players)
            .WithMany()
            .UsingEntity(j => j.ToTable("BracketMatchPlayers"));
        builder.Entity<BracketMatch>()
            .HasMany(m => m.TeamA)
            .WithMany()
            .UsingEntity(j => j.ToTable("BracketMatchTeamA"));
        builder.Entity<BracketMatch>()
            .HasMany(m => m.TeamB)
            .WithMany()
            .UsingEntity(j => j.ToTable("BracketMatchTeamB"));
    }
}