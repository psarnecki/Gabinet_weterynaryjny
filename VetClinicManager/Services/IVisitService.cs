using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.Models;

namespace VetClinicManager.Services
{
    public interface IVisitService
    {
        // Pobieranie list
        Task<IEnumerable<VisitListReceptionistDto>> GetVisitsForReceptionistAsync();
        Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string vetId);
        Task<IEnumerable<VisitListUserDto>> GetVisitsForOwnerAsync(string ownerId);

        // Pobieranie detali
        Task<VisitListReceptionistDto?> GetDetailsForReceptionistAsync(int id);
        Task<VisitListVetDto?> GetDetailsForVetAsync(int id, string vetId);
        Task<VisitListUserDto?> GetDetailsForUserAsync(int id, string ownerId);

        // Operacje CRUD
        Task CreateAsync(VisitCreateDto createDto);
        Task UpdateAsync(int id, VisitEditDto editDto, string userId, bool isVet);
        Task DeleteAsync(int id);

        // Dane dla formularzy i widoków
        Task<VisitEditDto?> GetForEditAsync(int id, string userId, bool isVet);
        Task<VisitListReceptionistDto?> GetForDeleteAsync(int id); 
        Task<IEnumerable<SelectListItem>> GetAnimalsSelectListAsync();
        Task<IEnumerable<User>> GetVetUsersAsync();
    }
}