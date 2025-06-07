using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.Models;

namespace VetClinicManager.Mappers;

[Mapper]
public partial class AnimalMedicationMapper
{
    // Mapowanie z AnimalMedicationCreateVetDto na AnimalMedication
    public partial AnimalMedication MapToAnimalMedication(AnimalMedicationCreateVetDto dto);

    // Mapowanie z AnimalMedicationEditVetDto na AnimalMedication
    public partial AnimalMedication MapToAnimalMedication(AnimalMedicationEditVetDto dto);

    // Mapowanie z AnimalMedication na AnimalMedicationEditVetDto
    public partial AnimalMedicationEditVetDto MapToEditDto(AnimalMedication entity);

    // Mapowanie z AnimalMedication na AnimalMedicationCreateVetDto
    public partial AnimalMedicationCreateVetDto MapToCreateDto(AnimalMedication entity);
}