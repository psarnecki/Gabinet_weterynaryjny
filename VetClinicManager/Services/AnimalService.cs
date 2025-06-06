using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.Animals;
using VetClinicManager.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly ApplicationDbContext _context;
        private readonly AnimalMapper _mapper;

        public AnimalService(ApplicationDbContext context, AnimalMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AnimalListVetRecDto>> GetAnimalsForPersonnelAsync()
        {
            var animals = await _context.Animals
                .Include(a => a.User)
                .ToListAsync();
            return _mapper.ToVetRecDtos(animals);
        }

        public async Task<AnimalListVetRecDto> GetAnimalDetailsForPersonnelAsync(int id)
        {
            var animal = await GetAnimalByIdAsync(id);
            return _mapper.ToVetRecDto(animal);
        }

        public async Task<AnimalListUserDto> GetAnimalDetailsForOwnerAsync(int id, string userId)
        {
            var animal = await GetAnimalByIdAsync(id);
            
            if (animal.UserId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this animal");

            return _mapper.ToUserDto(animal);
        }
        
        public async Task<AnimalEditDto> GetAnimalForDeleteAsync(int id)
        {
            var animal = await GetAnimalByIdAsync(id);
            return _mapper.ToEditDto(animal);
        }

        public async Task CreateAnimalAsync(CreateAnimalDto createAnimalDto)
        {
            var animal = _mapper.ToEntity(createAnimalDto);
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnimalAsync(int id, AnimalEditDto animalEditDto)
        {
            var animal = await GetAnimalByIdAsync(id);
            _mapper.UpdateFromDto(animalEditDto, animal);
            _context.Animals.Update(animal);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnimalAsync(int id)
        {
            var animal = await GetAnimalByIdAsync(id);
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
        }

        public async Task<AnimalEditDto> GetAnimalForEditAsync(int id)
        {
            var animal = await GetAnimalByIdAsync(id);
            return _mapper.ToEditDto(animal);
        }

        public async Task<bool> IsAnimalOwnerAsync(int animalId, string userId)
        {
            var animal = await _context.Animals.FindAsync(animalId);
            return animal?.UserId == userId;
        }

        private async Task<Animal> GetAnimalByIdAsync(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
                throw new KeyNotFoundException("Animal not found");

            return animal;
        }
    }
}