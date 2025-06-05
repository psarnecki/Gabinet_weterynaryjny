using Microsoft.AspNetCore.Identity;
using VetClinicManager.Areas.Admin.DTOs.Users;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public interface IUserService
{
    // Dla akcji Index GET: Pobierz listę wszystkich użytkowników z ich rolami
    Task<List<UserListDto>> GetAllUsersWithRolesAsync();

    // Dla akcji Create GET i Edit GET: Pobierz listę wszystkich dostępnych ról
    Task<List<string>> GetAllAvailableRolesAsync();

    // Dla akcji Create POST: Utwórz nowego użytkownika
    // Zwraca IdentityResult (sukces/błąd) i opcjonalnie obiekt utworzonego użytkownika (np. dla logowania)
    Task<(IdentityResult result, User? user)> CreateUserAsync(UserCreateDto userDto);

    // Dla akcji Edit GET: Pobierz dane konkretnego użytkownika do wyświetlenia w formularzu edycji
    Task<UserEditDto?> GetUserForEditAsync(string userId);

    // Dla akcji Edit POST: Zaktualizuj dane i role istniejącego użytkownika
    // Zwraca IdentityResult (sukces/błąd)
    Task<IdentityResult> UpdateUserAsync(UserEditDto userDto);

    // Dla akcji Delete GET: Pobierz dane konkretnego użytkownika do wyświetlenia na stronie potwierdzenia usunięcia
    Task<UserDeleteDto?> GetUserForDeleteAsync(string userId);

    // Dla akcji Delete POST: Usuń użytkownika
    // Zwraca IdentityResult (sukces/błąd)
    Task<IdentityResult> DeleteUserAsync(string userId);

    // TODO: Można dodać zmianę hasła użytkownika
}