using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.VisitUpdates;
using VetClinicManager.Models;

namespace VetClinicManager.Mappers;

[Mapper]
public partial class VisitUpdateMapper
{
    [MapperIgnoreSource(nameof(VisitUpdateCreateDto.AnimalMedications))]
    public partial VisitUpdate ToVisitUpdate(VisitUpdateCreateDto dto);
    
    // VisitUpdateEditVetDto to VisitUpdate
    public partial VisitUpdate ToVisitUpdate(VisitUpdateEditVetDto dto);
    
    // VisitUpdate to VisitUpdateEditVetDto
    public partial VisitUpdateEditVetDto ToVisitUpdateEditVetDto(VisitUpdate model);
    
    public partial VisitUpdateDeleteDto ToDeleteDto(VisitUpdate update);

}