using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.DTOs.Visits.VisitBriefs;
using VetClinicManager.Models;

namespace VetClinicManager.Mappers;

[Mapper]
public partial class VisitMapper
{
    // Map from CreateVisitDto to Visit
    public partial Visit ToEntity(VisitCreateDto dto);
    
    // Map from UpdateVisitDto to existing Visit
    public partial void ToEntity(VisitEditDto dto, Visit visit);
    
    // Map from Visit to VisitDtoReceptionist
    public partial VisitListReceptionistDto ToReceptionistDto(Visit visit);
    
    // Map from Visit to VisitDtoVet
    public partial VisitListVetDto ToVetDto(Visit visit);
    
    // Map from Visit to VisitDtoUser
    public partial VisitListUserDto ToUserDto(Visit visit);
    
    public partial IEnumerable<VisitListVetDto> ToVetDtos(IEnumerable<Visit> visits);
    public partial IEnumerable<VisitListUserDto> ToUserDtos(IEnumerable<Visit> visits);
    public partial IEnumerable<VisitListReceptionistDto> ToReceptionistDtos(IEnumerable<Visit> visits);

    // Custom mappings for nested objects
    private VisitAnimalBriefDto MapAnimalToBriefDto(Animal? animal)
    {
        if (animal == null)
        {
            return new VisitAnimalBriefDto 
            { 
                Id = 0, 
                Name = string.Empty, 
                Breed = string.Empty 
            };
        }
        
        return new VisitAnimalBriefDto 
        { 
            Id = animal.Id, 
            Name = animal.Name, 
            Breed = animal.Breed ?? string.Empty 
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
        };
    }

    // Mapowanie AnimalMedication na AnimalMedicationBriefDto
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

    // Mapowanie VisitUpdate na VisitUpdateDto
    private VisitUpdateBriefDto MapVisitUpdateToDto(VisitUpdate update)
    {
        return new VisitUpdateBriefDto
        {
            Id = update.Id,
            Notes = update.Notes,
            UpdateDate = update.UpdateDate,
            ImageUrl = update.ImageUrl,
            PrescribedMedications = update.PrescribedMedications,
            UpdatedByVetName = $"{update.UpdatedBy?.FirstName} {update.UpdatedBy?.LastName}",
            Medications = update.AnimalMedications?
                .Select(MapAnimalMedicationToBriefDto)
                .ToList() ?? new List<AnimalMedicationBriefDto>()
        };
    }

    // Mapowanie listy VisitUpdate na listę VisitUpdateDto
    private List<VisitUpdateBriefDto> MapUpdates(ICollection<VisitUpdate> updates)
    {
        return updates?
            .Select(MapVisitUpdateToDto)
            .ToList() ?? new List<VisitUpdateBriefDto>();
    }
}