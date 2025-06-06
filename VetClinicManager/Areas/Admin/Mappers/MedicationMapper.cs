using Riok.Mapperly.Abstractions;
using VetClinicManager.Areas.Admin.DTOs.Medications;
using VetClinicManager.Models;

namespace VetClinicManager.Areas.Admin.Mappers;

[Mapper]
public partial class MedicationMapper
{
    // Mapowanie z Medication na MedicationListDto - pojedyńczy obiekt encji (Index)
    public partial MedicationListDto ToMedicationListDto(Medication medication);

    // Mapowanie z Medication na MedicationEditDto (Edit GET)
    public partial MedicationEditDto ToMedicationEditDto(Medication medication);

    // Mapowanie z Medication na MedicationDeleteDto (Delete GET)
    public partial MedicationDeleteDto ToMedicationDeleteDto(Medication medication);

    // Mapowanie z MedicationCreateDto na Medication (Create POST)
    public partial Medication ToMedication(MedicationCreateDto createDto);

    // Mapowanie z MedicationEditDto na istniejący Medication (Edit POST)
    [MapperIgnoreSource(nameof(MedicationEditDto.Id))]
    public partial void UpdateMedicationFromDto(MedicationEditDto editDto, Medication medication);

    // Mapowanie kolekcji Medication na kolekcję MedicationListDto - lista obiektów encji (Index)
    public partial IEnumerable<MedicationListDto> ToMedicationListDtos(IEnumerable<Medication> medications);
}