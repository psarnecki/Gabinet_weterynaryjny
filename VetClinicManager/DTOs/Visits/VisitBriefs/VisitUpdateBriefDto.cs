namespace VetClinicManager.DTOs.Visits.VisitBriefs;

public class VisitUpdateBriefDto
{
    public int Id { get; set; }
    public string? Notes { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? PrescribedMedications { get; set; }
    public string? UpdatedByVetName { get; set; }
    public List<AnimalMedicationBriefDto> Medications { get; set; } = new();
}