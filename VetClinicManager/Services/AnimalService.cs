using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.AnimalDTOs;
using VetClinicManager.Mappers;

namespace VetClinicManager.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly ApplicationDbContext _context;
        private readonly AnimalMapper _animalMapper;

        public AnimalService(ApplicationDbContext context, AnimalMapper animalMapper)
        {
            _context = context;
            _animalMapper = animalMapper;
        }

        public async Task<IEnumerable<AnimalListUserDto>> GetAnimalsForOwnerAsync(string ownerUserId)
        {
            var animals = await _context.Animals
                                        .Where(a => a.UserId == ownerUserId)
                                        .ToListAsync();

            return _animalMapper.ToUserDtos(animals);
        }

        public async Task<IEnumerable<AnimalListVetRecDto>> GetAnimalsForPersonnelAsync()
        {
            var animals = await _context.Animals
                                        .Include(a => a.User)
                                        .ToListAsync();

            return _animalMapper.ToVetRecDtos(animals);
        }

        public async Task<AnimalDetailsVetRecDto?> GetAnimalDetailsForPersonnelAsync(int id)
        {
            var animal = await _context.Animals
                                        .Include(a => a.User)
                                        .Include(a => a.HealthRecord)
                                        .Include(a => a.Visits)
                                            .ThenInclude(v => v.AssignedVet)
                                        .Include(a => a.AnimalMedications)
                                             .ThenInclude(am => am.Medication)
                                        // TODO: Dodaj inne ThenInclude jeśli potrzebne dla zagnieżdżonych DTOs
                                        .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return null;
            }

            return _animalMapper.ToAnimalDetailsVetRecDto(animal);
        }

        public async Task<AnimalDetailsUserDto?> GetAnimalDetailsForOwnerAsync(int id, string userId)
        {
            var animal = await _context.Animals
                                        .Include(a => a.HealthRecord)
                                        .Include(a => a.Visits)
                                        .Include(a => a.AnimalMedications)
                                             .ThenInclude(am => am.Medication)
                                        .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (animal == null)
            {
                return null;
            }

            return _animalMapper.ToAnimalDetailsUserDto(animal);
        }

        public Task<CreateAnimalDto> GetCreateAnimalDtoAsync()
        {
             return Task.FromResult(new CreateAnimalDto());
        }

        public async Task CreateAnimalAsync(CreateAnimalDto createAnimalDto)
        {
            var animal = _animalMapper.ToEntity(createAnimalDto);
            // TODO: Jeśli User tworzy swoje zwierzę, kontroler powinien przypisać UserId przed wywołaniem tej metody.
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
        }

        public async Task<AnimalEditDto?> GetAnimalForEditAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) return null;
            return _animalMapper.ToEditDto(animal);
        }

        public async Task UpdateAnimalAsync(int id, AnimalEditDto animalEditDto)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) throw new KeyNotFoundException("Animal not found for update.");
            // TODO: Logika uprawnień (jeśli User edytuje swoje) MUSI być w kontrolerze PRZED wywołaniem tej metody,
            // albo metoda powinna przyjmować userId i sprawdzać własność.

            _animalMapper.UpdateFromDto(animalEditDto, animal);
            await _context.SaveChangesAsync();
        }

        public async Task<AnimalEditDto?> GetAnimalForDeleteAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
             if (animal == null) return null;
            return _animalMapper.ToEditDto(animal);
        }

        public async Task DeleteAnimalAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) throw new KeyNotFoundException("Animal not found for deletion.");
            // TODO: Logika uprawnień (jeśli User usuwa swoje) MUSI być w kontrolerze PRZED wywołaniem tej metody

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsAnimalOwnerAsync(int animalId, string userId)
        {
            return await _context.Animals.AnyAsync(a => a.Id == animalId && a.UserId == userId);
        }
    }
}