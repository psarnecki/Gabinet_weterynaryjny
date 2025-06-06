using VetClinicManager.DTOs.Visits.VisitBriefs;
using VetClinicManager.Models.Enums;

namespace VetClinicManager.DTOs.Visits;

public class VisitListUserDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public VisitStatus Status { get; set; }
    public VisitAnimalBriefDto Animal { get; set; } 
    public VisitVetBriefDto? AssignedVet { get; set; }
    public List<VisitUpdateBriefDto> Updates { get; set; } = new();
}