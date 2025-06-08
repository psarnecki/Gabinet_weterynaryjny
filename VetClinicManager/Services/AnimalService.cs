using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly AnimalMapper _animalMapper;
        private readonly IFileService _fileService;
        private readonly UserManager<User> _userManager;

        public AnimalService(ApplicationDbContext context, AnimalMapper animalMapper, IFileService fileService, UserManager<User> userManager)
        {
            _context = context;
            _animalMapper = animalMapper;
            _fileService = fileService;
            _userManager = userManager;
        }

        public async Task<IEnumerable<AnimalListUserDto>> GetAnimalsForOwnerAsync(string ownerUserId)
        {
            var animals = await _context.Animals
                                        .Where(a => a.UserId == ownerUserId)
                                        .Include(a => a.HealthRecord)
                                        .ToListAsync();

            return _animalMapper.ToUserDtos(animals);
        }

        public async Task<IEnumerable<AnimalListVetRecDto>> GetAnimalsForPersonnelAsync()
        {
            var animals = await _context.Animals
                                        .Include(a => a.User)
                                        .Include(a => a.HealthRecord)
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

        public async Task<int?> GetHealthRecordIdByAnimalIdAsync(int animalId)
        {
            var healthRecordId = await _context.Animals
                .Where(a => a.Id == animalId && a.HealthRecord != null)
                .Select(a => (int?)a.HealthRecord.Id)
                .FirstOrDefaultAsync();

            return healthRecordId;
        }

        public async Task CreateAnimalAsync(CreateAnimalDto createAnimalDto)
        {
            if (createAnimalDto.ImageFile != null)
            {
                createAnimalDto.ImageUrl = await _fileService.SaveFileAsync(createAnimalDto.ImageFile, "uploads/animals");
            }
            
            var animal = _animalMapper.ToEntity(createAnimalDto);
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();
        }

        public async Task<AnimalEditDto?> GetAnimalForEditAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null)
            {
                return null;
            }

            return _animalMapper.ToEditDto(animal);
        }

        public async Task UpdateAnimalAsync(int id, AnimalEditDto animalEditDto)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) throw new KeyNotFoundException("Animal not found for update.");
            
            var oldImageUrl = animal.ImageUrl;
            if (animalEditDto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(oldImageUrl))
                {
                    _fileService.DeleteFile(oldImageUrl);
                }
                animalEditDto.ImageUrl = await _fileService.SaveFileAsync(animalEditDto.ImageFile, "uploads/animals");
            }
            else
            {
                animalEditDto.ImageUrl = oldImageUrl;
            }

            _animalMapper.UpdateFromDto(animalEditDto, animal);
            await _context.SaveChangesAsync();
        }

        public async Task<AnimalEditDto?> GetAnimalForDeleteAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null)
            {
                return null;
            }
            
            return _animalMapper.ToEditDto(animal);
        }

        public async Task DeleteAnimalAsync(int id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
            if (animal == null) throw new KeyNotFoundException("Animal not found for deletion.");

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<SelectListItem>> GetClientUsersForSelectListAsync()
        {
            var clients = await _userManager.GetUsersInRoleAsync("Client");
            return clients.OrderBy(c => c.LastName).Select(c => new SelectListItem {
                Value = c.Id,
                Text = $"{c.FirstName} {c.LastName}"
            }).ToList();
        }
        
        public List<SelectListItem> GetEnumSelectList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(enumValue =>
            {
                var displayAttribute = typeof(TEnum).GetField(enumValue.ToString())
                    ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                    .Cast<DisplayAttribute>().FirstOrDefault();
                
                return new SelectListItem
                {
                    Value = enumValue.ToString(),
                    Text = displayAttribute?.Name ?? enumValue.ToString()
                };
            }).ToList();
        }
    }
}