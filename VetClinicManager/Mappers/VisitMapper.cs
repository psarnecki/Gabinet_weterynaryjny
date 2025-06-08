using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.Users.UserBriefs;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.DTOs.Visits.VisitBriefs;
using VetClinicManager.Models;

namespace VetClinicManager.Mappers;

[Mapper]
public partial class VisitMapper
{
    // Metody mapujące z DTO na encję
    public partial Visit ToEntity(VisitCreateDto dto);
    public partial void ToEntity(VisitEditDto dto, Visit visit);
    
    // Metody mapujące na DTO
    [MapProperty(nameof(Visit.Animal.User), nameof(VisitListReceptionistDto.Owner))]
    public partial VisitListReceptionistDto ToReceptionistDto(Visit visit);
    [MapProperty(nameof(Visit.Animal.User), nameof(VisitListVetDto.Owner))]
    public partial VisitListVetDto ToVetDto(Visit visit);
    public partial VisitListUserDto ToUserDto(Visit visit);
    public partial VisitEditDto ToEditDto(Visit visit);

    
    // Metody mapujące na kolekcje DTO
    public partial IEnumerable<VisitListVetDto> ToVetDtos(IEnumerable<Visit> visits);
    public partial IEnumerable<VisitListUserDto> ToUserDtos(IEnumerable<Visit> visits);
    public partial IEnumerable<VisitListReceptionistDto> ToReceptionistDtos(IEnumerable<Visit> visits);
    
    private VisitAnimalBriefDto MapAnimalToBriefDto(Animal? animal)
    {
        if (animal == null)
        {
            return new VisitAnimalBriefDto 
            { 
                Id = 0, 
                Name = string.Empty, 
                Breed = string.Empty,
                OwnerId = string.Empty
            };
        }
        
        return new VisitAnimalBriefDto 
        { 
            Id = animal.Id, 
            Name = animal.Name, 
            Breed = animal.Breed ?? string.Empty,
            OwnerId = animal.UserId
        };
    }
    
    private VisitVetBriefDto? MapUserToVetBriefDto(User? user)
    {
        if (user == null)
        {
            return null;
        }
        
        return new VisitVetBriefDto 
        { 
            Id = user.Id, 
            FirstName = user.FirstName ?? string.Empty, 
            LastName = user.LastName ?? string.Empty, 
            Email = user.Email
        };
    }

    private AnimalMedicationBriefDto MapAnimalMedicationToBriefDto(AnimalMedication medication)
    {
        return new AnimalMedicationBriefDto
        {
            Id = medication.Id,
            Name = medication.Medication?.Name ?? "Unknown medication",
            StartDate = medication.StartDate,
            EndDate = medication.EndDate
        };
    }

    private VisitUpdateBriefDto MapVisitUpdateToDto(VisitUpdate update)
    {
        return new VisitUpdateBriefDto
        {
            Id = update.Id,
            Notes = update.Notes,
            UpdateDate = update.UpdateDate,
            ImageUrl = update.ImageUrl,
            UpdatedByVetName = $"{update.UpdatedBy?.FirstName} {update.UpdatedBy?.LastName}",
            Medications = update.AnimalMedications?
                .Select(MapAnimalMedicationToBriefDto)
                .ToList() ?? new List<AnimalMedicationBriefDto>()
        };
    }

    private UserBriefDto MapUserToBriefDto(User? user)
    {
        if (user == null)
        {
            return null;
        }
        return new UserBriefDto
        {
            Id = user.Id, 
            FirstName = user.FirstName, 
            LastName = user.LastName
        };
    }
    
    [UserMapping(Default = false)]
    private UserBriefDto? MapOwner(Visit visit) => MapUserToBriefDto(visit.Animal?.User);
    
    private List<VisitUpdateBriefDto> MapUpdates(ICollection<VisitUpdate> updates)
    {
        return updates?
            .Select(MapVisitUpdateToDto)
            .ToList() ?? new List<VisitUpdateBriefDto>();
    }
}