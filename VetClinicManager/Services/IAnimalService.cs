using VetClinicManager.DTOs.AnimalDTOs;
using VetClinicManager.Models;

namespace VetClinicManager.Services
{
    public interface IAnimalService
    {
        Task<IEnumerable<AnimalListVetRecDto>> GetAnimalsForPersonnelAsync();
        Task<AnimalListVetRecDto> GetAnimalDetailsForPersonnelAsync(int id);
        Task<AnimalListUserDto> GetAnimalDetailsForOwnerAsync(int id, string userId);
        Task<AnimalEditDto> GetAnimalForDeleteAsync(int id);
        Task CreateAnimalAsync(CreateAnimalDto createAnimalDto);
        Task UpdateAnimalAsync(int id, AnimalEditDto animalEditDto);
        Task DeleteAnimalAsync(int id);
        Task<AnimalEditDto> GetAnimalForEditAsync(int id);
        Task<bool> IsAnimalOwnerAsync(int animalId, string userId);
        
    }
}