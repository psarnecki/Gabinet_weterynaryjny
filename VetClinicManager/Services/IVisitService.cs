using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.Models;

namespace VetClinicManager.Services
{
    public interface IVisitService
    {
        // === POBIERANIE LIST ===
        Task<IEnumerable<VisitListReceptionistDto>> GetVisitsForReceptionistAsync();
        Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string vetId);
        Task<IEnumerable<VisitListUserDto>> GetVisitsForOwnerAsync(string ownerId);

        // === POBIERANIE DETALI ===
        Task<VisitListReceptionistDto?> GetDetailsForReceptionistAsync(int id);
        Task<VisitListVetDto?> GetDetailsForVetAsync(int id, string vetId);
        Task<VisitListUserDto?> GetDetailsForUserAsync(int id, string ownerId);

        // === OPERACJE CRUD ===
        Task CreateAsync(VisitCreateDto createDto);
        Task UpdateAsync(int id, VisitEditDto editDto, string userId, bool isVet);
        Task DeleteAsync(int id);

        // === DANE DLA FORMULARZY I WIDOKÓW ===
        Task<VisitEditDto?> GetForEditAsync(int id, string userId, bool isVet);
        Task<VisitListReceptionistDto?> GetForDeleteAsync(int id); 
        Task<IEnumerable<SelectListItem>> GetAnimalsSelectListAsync();
        Task<IEnumerable<User>> GetVetUsersAsync();
    }
}