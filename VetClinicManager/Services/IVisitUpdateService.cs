using VetClinicManager.DTOs.VisitUpdateDTOs;
using VetClinicManager.Models;

namespace VetClinicManager.Interfaces;

public interface IVisitUpdateService
{
    Task<IEnumerable<VisitUpdate>> GetVisitUpdatesAsync();
    Task<VisitUpdate> GetVisitUpdateByIdAsync(int id);
    Task<VisitUpdate> CreateVisitUpdateAsync(VisitUpdateCreateDto createDto, string vetId);
    Task<VisitUpdate> UpdateVisitUpdateAsync(int id, VisitUpdateEditVetDto updateDto, string vetId);
    Task DeleteVisitUpdateAsync(int id);
    Task<bool> VisitUpdateExistsAsync(int id);
}