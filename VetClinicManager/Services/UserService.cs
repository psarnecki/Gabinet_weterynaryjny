using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Areas.Admin.DTOs.Users;
using VetClinicManager.Areas.Admin.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserMapper _userMapper; 

    // Wstrzykiwanie zależności przez DI
    public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, UserMapper userMapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userMapper = userMapper;
    }
    
    // Dla akcji Index GET
    public async Task<List<UserListDto>> GetAllUsersWithRolesAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userListDtos = new List<UserListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userDto = _userMapper.ToUserListDtoFromUser(user);
            userDto.Roles = roles.ToList();
            userListDtos.Add(userDto);
        }

        return userListDtos;
    }

    // Dla akcji Create GET i Edit GET
    public async Task<List<string>> GetAllAvailableRolesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
    }

    public async Task<(IdentityResult result, User? user)> CreateUserAsync(UserCreateDto userDto)
    {
        var user = _userMapper.ToUser(userDto);

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (result.Succeeded)
        {
            if (userDto.SelectedRoles != null && userDto.SelectedRoles.Any())
            {
                 var validRoles = await _roleManager.Roles
                                                    .Where(r => userDto.SelectedRoles.Contains(r.Name))
                                                    .Select(r => r.Name)
                                                    .ToListAsync();

                await _userManager.AddToRolesAsync(user, validRoles);
            }
            return (result, user);
        }

        return (result, null);
    }

    // Dla akcji Edit GET
    public async Task<UserEditDto?> GetUserForEditAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        var allRoles = await GetAllAvailableRolesAsync();
        var userRoles = await _userManager.GetRolesAsync(user);

        var model = _userMapper.ToUserEditDto(user);

        model.AvailableRoles = allRoles;
        model.SelectedRoles = userRoles.ToList();

        return model;
    }

    // Dla akcji Edit POST
    public async Task<IdentityResult> UpdateUserAsync(UserEditDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id);
        if (user == null)
        {
             return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        _userMapper.UpdateUserFromDto(userDto, user);

        // Zmiana emaila/username
        if (user.Email != userDto.Email) 
        {
             var setEmailResult = await _userManager.SetEmailAsync(user, userDto.Email);
             if (!setEmailResult.Succeeded) 
                 return setEmailResult;

             var setUserNameResult = await _userManager.SetUserNameAsync(user, userDto.Email);
             if (!setUserNameResult.Succeeded) 
                 return setUserNameResult; 
        }

        var updateResult = await _userManager.UpdateAsync(user); 
        if (!updateResult.Succeeded) return updateResult; 

        // Aktualizacja roli
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = userDto.SelectedRoles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(userDto.SelectedRoles).ToList();

        if (rolesToAdd.Any())
        {
            var addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
             if (!addRolesResult.Succeeded) return addRolesResult;
        }

        if (rolesToRemove.Any())
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
             if (!removeRolesResult.Succeeded) return removeRolesResult;
        }

        return IdentityResult.Success;
    }

    // Dla akcji Delete GET
    public async Task<UserDeleteDto?> GetUserForDeleteAsync(string userId)
    {
         var user = await _userManager.FindByIdAsync(userId);
         if (user == null)
         {
             return null;
         }
         var model = _userMapper.ToUserDeleteDto(user);
         return model;
    }

    // Dla akcji Delete POST
    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Success;
        }

        // TODO: W tym miejscu można by dodać logikę walidacji przed usunięciem (teraz ogólna wersja jest w kontrolerze)

        var result = await _userManager.DeleteAsync(user);
        return result; 
    }
}