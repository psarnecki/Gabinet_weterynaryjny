using VetClinicManager.DTOs.HealthRecords;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

public interface IHealthRecordService
{
    Task<string?> GetAnimalNameForCreateAsync(int animalId);
    Task<HealthRecordEditVetDto?> GetForEditAsync(int id);
    Task<int> CreateAsync(HealthRecordEditVetDto createDto);
    Task<bool> UpdateAsync(HealthRecordEditVetDto editDto);
    Task<HealthRecordEditVetDto?> GetForDeleteAsync(int id);
    Task<bool> DeleteAsync(int id);
}