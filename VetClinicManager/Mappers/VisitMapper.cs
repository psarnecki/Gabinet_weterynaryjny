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