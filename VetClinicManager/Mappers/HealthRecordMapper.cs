using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.HealthRecords;
using VetClinicManager.Models;

namespace VetClinicManager.Mappers;

[Mapper]
public partial class HealthRecordMapper
{
    // Konwertuje DTO na NOWĄ encję (używane przy tworzeniu)
    public partial HealthRecord ToEntity(HealthRecordEditVetDto dto);
    
    // Konwertuje encję na DTO (używane do wyświetlania w formularzu edycji)
    public partial HealthRecordEditVetDto ToEditDto(HealthRecord entity);
}