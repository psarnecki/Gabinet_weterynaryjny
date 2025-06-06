using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.VisitUpdateDTOs;
using VetClinicManager.Models;

namespace VetClinicManager.Mappers;

[Mapper]
public static partial class VisitUpdateMapper
{
    // VisitUpdateCreateDto to VisitUpdate
    public static partial VisitUpdate ToVisitUpdate(this VisitUpdateCreateDto dto);
    
    // VisitUpdateEditVetDto to VisitUpdate
    public static partial VisitUpdate ToVisitUpdate(this VisitUpdateEditVetDto dto);
    
    // VisitUpdate to VisitUpdateEditVetDto
    public static partial VisitUpdateEditVetDto ToVisitUpdateEditVetDto(this VisitUpdate model);
}