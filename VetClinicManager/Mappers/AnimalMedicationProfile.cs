using Riok.Mapperly.Abstractions;
using VetClinicManager.Models;
using VetClinicManager.DTOs.AnimalMedicationDTOs;

namespace VetClinicManager.Mappers;

[Mapper] // Atrybut aktywujący generowanie kodu
public static partial class AnimalMedicationMapper
{
    // Mapowanie z Model na DTO (pełne)
    public static partial AnimalMedicationEditVetDto ToEditVetDto(this AnimalMedication model);

    // Mapowanie z DTO na Model (aktualizacja istniejącego obiektu)
    public static partial void ToEntity(this AnimalMedicationEditVetDto dto, AnimalMedication entity);
    
    // Custom mapping dla Medication (jeśli potrzebne)
    private static Medication MapMedication(Medication src) => src;
}