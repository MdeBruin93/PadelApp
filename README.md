# PadelApp

This is a Blazor Server application for managing a padel tournament, including poule phase, bracket phase, user management, and match results.  
The project targets **.NET 9** and uses **C# 13**.

## Project Structure

- **PadelApp/Components/Pages/**: Main Blazor pages (e.g., Matches, Poules, Brackets, Login, Crew, AdminUI, etc.)
- **PadelApp/Models/**: Data models (User, Poule, Match, BracketBreak, TournamentSettings, etc.)
- **PadelApp/Services/**: Application services (UserService, PouleService, BracketSettingsService, BracketScheduler, etc.)
- **PadelApp/Persistence/**: Entity Framework Core DbContext (`PadelDbContext`)
- **PadelApp/wwwroot/**: Static files, custom JS, CSS

## Setup

1. **.NET 9 SDK** required.
2. The app uses Entity Framework Core for persistence.
3. Run the app with `dotnet run` from the root directory.

## Key Features

- **User Management**: CRUD for users, roles (Admin, Crew, Player, Guest), Discord name, AuthKey (readonly, auto-generated for new users).
- **Login**: AuthKey-based login, supports query parameter `authCode` for direct login links.
- **Poules**: Group phase, player sorting, score editing (disabled after brackets are released), release control via admin panel.
- **Brackets**: Upper and lower bracket phases, match scheduling, score editing (admin/crew only), release control via admin panel.
- **Matches**: All matches overview, filter by poule, mobile-friendly score editing via modal.
- **Crew Page**: Overview of all users with role Admin or Crew.
- **Rules Page**: Tournament rules, sorting logic, golden point explanation.
- **Mobile Friendly**: Uses Bootstrap responsive tables, modals for editing on mobile, and custom CSS for better mobile experience.

## Admin Workflow

- **Poules Generation & Release**:  
  - Go to `/admin-ui` as an admin.
  - Click "Poules vrijgeven" to generate poules and persist them to the database.
  - Poule matches are also generated and persisted at this step.
  - Poule release state is managed via the admin panel.

- **Brackets Generation & Release**:  
  - After poules are released and matches played, click "Brackets vrijgeven" in the admin panel.
  - This generates upper and lower brackets, persists bracket matches, and stores break players for each round.
  - Bracket release state is managed via the admin panel.

- **Persistence**:  
  - All poule and bracket matches are persisted in the database.
  - Break players for each bracket round are stored in the `BracketBreak` table.

## Data Model Notes

- **Match**: Contains all info for poule and bracket matches, including teams, scores, start time, bracket type, and round number.
- **BracketBreak**: Stores which players have a break in which round and bracket.
- **TournamentSettings**: Stores global release states for poules and brackets.

## Development Guidelines for GitHub Copilot

- **Blazor Server patterns** only (no Razor Pages or MVC).
- **Use dependency injection** for all services.
- **Persist all tournament state** (matches, poules, brackets, breaks) in the database.
- **AdminUI** is the central place for generating and releasing poules and brackets.
- **Do not generate or persist poules/brackets in the bracket pages**; only load and display.
- **When loading bracket rounds, reconstruct break players from the database using BracketBreak.**
- **All pages must be mobile friendly** (use Bootstrap, responsive tables, modals).
- **For new features, check for existing services and models before creating new ones.**
- **Use the `PadelDbContext` for all data persistence.**
- **Authentication/authorization**: Use the existing role system and session service.

## Bracket generation algorithms

 - **Upper Bracket algorithm**
   - **Teams** Players should not team up with other players more then 2 times.
   - **Breaks** Every player should have an equal amount of breaks. A break means 1 round where the player does not have any matches.
   - **No sequential rounds for breaks** A player cannot have a break in 2 sequental rounds. There should be atleast 2 rounds in between breaks.
   - **No sequential rounds for same teams** If a player has to team up with another player again, there should be alteast 1 round in between the precending team up.

 - **Lower Bracket algorithm**
   - **Teams** Players should not team up with other players more then once.
   - **Breaks** Every player should have an equal amount of breaks. A break means 1 round where the player does not have any matches.
   - **No sequential rounds for breaks** A player cannot have a break in 2 sequental rounds. There should be atleast 2 rounds in between breaks.
   - **No sequential rounds for same teams** If a player has to team up with another player again, there should be alteast 1 round in between the precending team up.

## Useful References

- [Blazor documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Bootstrap documentation](https://getbootstrap.com/docs/5.0/getting-started/introduction/)
- [Entity Framework Core documentation](https://learn.microsoft.com/en-us/ef/core/)

---

**If you are a GitHub Copilot instance, please follow these instructions and respect the established patterns and architecture.**