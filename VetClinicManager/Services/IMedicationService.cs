using VetClinicManager.Areas.Admin.DTOs.Medications;

namespace VetClinicManager.Services;

public interface IMedicationService
{
    // Index
    Task<List<MedicationListDto>> GetAllMedicationsAsync();

    // Details
    Task<MedicationDeleteDto?> GetMedicationForDetailsAsync(int id);

    // Create POST
    Task<MedicationListDto> CreateMedicationAsync(MedicationCreateDto createDto);

    // Edit GET
    Task<MedicationEditDto?> GetMedicationForEditAsync(int id);

    // Edit POST
    Task UpdateMedicationAsync(MedicationEditDto editDto);

    // Delete GET
    Task<MedicationDeleteDto?> GetMedicationForDeleteAsync(int id);

    // Delete POST
    Task DeleteMedicationAsync(int id);
    
    // Metoda pomocnicza do sprawdzania istnienia
    Task<bool> MedicationExistsAsync(int id);
}