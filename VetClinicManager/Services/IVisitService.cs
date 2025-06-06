using VetClinicManager.DTOs.Visits;
using VetClinicManager.Models;

namespace VetClinicManager.Services
{
    public interface IVisitService
    {
        Task<IEnumerable<VisitListReceptionistDto>> GetVisitsForReceptionistAsync();
        Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string userId);
        Task<VisitListReceptionistDto> GetVisitDetailsForReceptionistAsync(int id);
        Task<VisitListVetDto> GetVisitDetailsForVetAsync(int id, string userId);
        Task<VisitListUserDto> GetVisitDetailsForUserAsync(int id, string userId);
        Task CreateVisitAsync(VisitCreateDto createVisitDto);
        Task UpdateVisitAsync(int id, VisitEditDto visitEditDto, string currentUserId, bool isVet);
        Task DeleteVisitAsync(int id);
        Task<VisitEditDto> GetVisitForEditAsync(int id, string currentUserId);
        Task<IEnumerable<User>> GetVetUsersAsync();
    }
}