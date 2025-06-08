using VetClinicManager.DTOs.VisitUpdates;

namespace VetClinicManager.Services;

public interface IVisitUpdateService
{
    Task<VisitUpdateEditVetDto?> GetForEditAsync(int id, string vetId);
    Task<VisitUpdateDeleteDto?> GetForDeleteAsync(int id, string vetId);
    Task<int> CreateAsync(VisitUpdateCreateDto createDto, string vetId);
    Task<int> UpdateAsync(int id, VisitUpdateEditVetDto editDto, string vetId);
    Task<int> DeleteAsync(int id, string vetId);
}