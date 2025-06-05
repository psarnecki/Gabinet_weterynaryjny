using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.VisitDTOs;
using VetClinicManager.DTOs.VisitDTOs.VisitBriefDTOs;
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
    
    // Helper mappings for nested objects
    private VisitAnimalBriefDto Map(Animal? animal)
        => animal == null 
            ? new VisitAnimalBriefDto { Id = 0, Name = string.Empty, Breed = string.Empty }
            : new VisitAnimalBriefDto { Id = animal.Id, Name = animal.Name, Breed = animal.Breed ?? string.Empty };
    
    private VisitVetBriefDto? Map(User? user)
        => user == null 
            ? null 
            : new VisitVetBriefDto 
            { 
                Id = user.Id, 
                FirstName = user.FirstName ?? string.Empty, 
                LastName = user.LastName ?? string.Empty, 
                Email = user.Email ?? string.Empty 
            };
    
    private List<VisitUpdateBriefDto> Map(ICollection<VisitUpdate>? updates)
        => updates == null 
            ? new List<VisitUpdateBriefDto>()
            : updates.Select(update => new VisitUpdateBriefDto
            {
                Id = update.Id,
                Notes = update.Notes,
                UpdateDate = update.UpdateDate
            }).ToList();
}