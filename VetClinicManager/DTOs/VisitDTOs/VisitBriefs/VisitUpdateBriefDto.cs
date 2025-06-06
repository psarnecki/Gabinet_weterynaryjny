namespace VetClinicManager.DTOs.Visits.VisitBriefs;

public class VisitUpdateBriefDto
{
    public int Id { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime UpdateDate { get; set; }
}