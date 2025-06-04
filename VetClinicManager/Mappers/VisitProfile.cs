using Riok.Mapperly.Abstractions;
using VetClinicManager.Dtos;
using VetClinicManager.Models;

[Mapper]
public static partial class VisitMapper
{
    // Map from CreateVisitDto to Visit
    public static partial Visit ToEntity(this CreateVisitDto dto);
    
    // Map from UpdateVisitDto to existing Visit
    public static partial void ToEntity(this UpdateVisitDto dto, Visit visit);
    
    // Map from Visit to VisitDtoReceptionist
    public static partial VisitDtoReceptionist ToReceptionistDto(this Visit visit);
    
    // Map from Visit to VisitDtoVet
    public static partial VisitDtoVet ToVetDto(this Visit visit);
    
    // Map from Visit to VisitDtoUser
    public static partial VisitDtoUser ToUserDto(this Visit visit);
    
    // Helper mappings for nested objects
    private static AnimalBriefDto Map(Animal? animal)
        => animal == null 
            ? new AnimalBriefDto { Id = 0, Name = string.Empty, Breed = string.Empty }
            : new AnimalBriefDto { Id = animal.Id, Name = animal.Name, Breed = animal.Breed ?? string.Empty };
    
    private static VetBriefDto? Map(User? user)
        => user == null 
            ? null 
            : new VetBriefDto 
            { 
                Id = user.Id, 
                FirstName = user.FirstName ?? string.Empty, 
                LastName = user.LastName ?? string.Empty, 
                Email = user.Email ?? string.Empty 
            };
    
    private static List<VisitUpdateDto> Map(ICollection<VisitUpdate>? updates)
        => updates == null 
            ? new List<VisitUpdateDto>()
            : updates.Select(update => new VisitUpdateDto
            {
                Id = update.Id,
                Notes = update.Notes,
                UpdateDate = update.UpdateDate
            }).ToList();
}