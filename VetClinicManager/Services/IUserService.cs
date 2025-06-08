using Microsoft.AspNetCore.Identity;
using VetClinicManager.Areas.Admin.DTOs.Users;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public interface IUserService
{
    // Dla akcji Index GET
    Task<List<UserListDto>> GetAllUsersWithRolesAsync();

    // Dla akcji Create GET i Edit GET
    Task<List<string>> GetAllAvailableRolesAsync();

    // Dla akcji Create POST
    Task<(IdentityResult result, User? user)> CreateUserAsync(UserCreateDto userDto);

    // Dla akcji Edit GET
    Task<UserEditDto?> GetUserForEditAsync(string userId);

    // Dla akcji Edit POST
    Task<IdentityResult> UpdateUserAsync(UserEditDto userDto);

    // Dla akcji Delete GET
    Task<UserDeleteDto?> GetUserForDeleteAsync(string userId);

    // Dla akcji Delete POST
    Task<IdentityResult> DeleteUserAsync(string userId);

    // TODO: Można dodać zmianę hasła użytkownika
}