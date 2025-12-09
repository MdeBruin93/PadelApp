using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PadelApp.Components.Pages;
using PadelApp.Components.Pages.Users;
using PadelApp.Config;
using PadelApp.Data.Models;

namespace PadelApp.Services;

public interface IUserService
{
    Task<List<Crew.CrewUser>> GetAllCrewAndAdminsAsync();
    Task<List<UserViewModel>> GetAllAsync();
    Task<IdentityResult> AddAsync(UserViewModel viewModel, string password);
    Task<ApplicationUser?> GetByIdAsync(Guid id);
    Task<UserViewModel?> GetViewModelByIdAsync(Guid id);
    Task<IdentityResult> DeleteAsync(Guid id);
    Task<IdentityResult> UpdateViewModelAsync(UserViewModel viewModel);
}

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<List<Crew.CrewUser>> GetAllCrewAndAdminsAsync()
    {
        var admins = await userManager.GetUsersInRoleAsync(RoleConstants.Admin);
        var crews = await userManager.GetUsersInRoleAsync(RoleConstants.Crew);

        var crewUsers = admins.Select(admin => new Crew.CrewUser { Name = admin.Name, DiscordName = admin.DiscordName, Role = RoleConstants.Admin }).ToList();
        crewUsers.AddRange(crews.Select(crew => new Crew.CrewUser { Name = crew.Name, DiscordName = crew.DiscordName, Role = RoleConstants.Crew }));

        return crewUsers;
    }

    public async Task<List<UserViewModel>> GetAllAsync()
    {
        var users = await userManager.Users.ToListAsync().ConfigureAwait(false);
        return users.Select(user => new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                DiscordName = user.DiscordName,
                Role = userManager.GetRolesAsync(user).Result.FirstOrDefault() ?? string.Empty
            })
            .ToList();
    }

    public async Task<IdentityResult> AddAsync(UserViewModel viewModel, string password)
    {
        var appUser = new ApplicationUser
        {
            Id = viewModel.Id,
            UserName = viewModel.DiscordName,
            Name = viewModel.Name,
            DiscordName = viewModel.DiscordName
        };
        var identityResult = await userManager.CreateAsync(appUser, password);
        if (identityResult.Succeeded)
        {
            var roleResult = await userManager.AddToRoleAsync(appUser, viewModel.Role);
            return roleResult;
        }

        return identityResult;
    }

    public Task<ApplicationUser?> GetByIdAsync(Guid id)
    {
        return userManager.FindByIdAsync(id.ToString());
    }

    public async Task<UserViewModel?> GetViewModelByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
        if (user == null)
        {
            return null;
        }

        var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;

        return new UserViewModel
        {
            Id = user.Id,
            Name = user.Name,
            DiscordName = user.DiscordName,
            Role = role
        };
    }

    public async Task<IdentityResult> DeleteAsync(Guid id)
    {
        var result = await userManager.DeleteAsync((await userManager.FindByIdAsync(id.ToString()))!);
        return result;
    }

    public async Task<IdentityResult> UpdateViewModelAsync(UserViewModel viewModel)
    {
        var user = await userManager.FindByIdAsync(viewModel.Id.ToString());
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        user.Name = viewModel.Name;
        user.DiscordName = viewModel.DiscordName;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return updateResult;
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(viewModel.Role))
        {
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            var roleResult = await userManager.AddToRoleAsync(user, viewModel.Role);
            if (!roleResult.Succeeded)
            {
                return roleResult;
            }
        }

        return IdentityResult.Success;
    }
}