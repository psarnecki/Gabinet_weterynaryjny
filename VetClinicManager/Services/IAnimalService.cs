using VetClinicManager.DTOs.Animals;
using VetClinicManager.DTOs.HealthRecords;

namespace VetClinicManager.Services
{
    public interface IAnimalService
    {
        Task<IEnumerable<AnimalListVetRecDto>> GetAnimalsForPersonnelAsync();
        Task<AnimalDetailsVetRecDto?> GetAnimalDetailsForPersonnelAsync(int id);
        Task<IEnumerable<AnimalListUserDto>> GetAnimalsForOwnerAsync(string ownerUserId);
        Task<AnimalDetailsUserDto?> GetAnimalDetailsForOwnerAsync(int id, string userId);
        Task<int?> GetHealthRecordIdByAnimalIdAsync(int animalId);
        Task<CreateAnimalDto> GetCreateAnimalDtoAsync();
        Task CreateAnimalAsync(CreateAnimalDto createAnimalDto);
        Task<AnimalEditDto?> GetAnimalForEditAsync(int id);
        Task UpdateAnimalAsync(int id, AnimalEditDto animalEditDto);
        Task<AnimalEditDto?> GetAnimalForDeleteAsync(int id);
        Task DeleteAnimalAsync(int id);
        Task<bool> IsAnimalOwnerAsync(int animalId, string userId);
    }
}