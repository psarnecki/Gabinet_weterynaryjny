namespace VetClinicManager.DTOs.Visits.VisitBriefs;

public class AnimalMedicationBriefDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}