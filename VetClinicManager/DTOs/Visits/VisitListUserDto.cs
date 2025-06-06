using VetClinicManager.Models.Enums;
using VetClinicManager.DTOs.Visits.VisitBriefs;

namespace VetClinicManager.DTOs.Visits;

public class VisitListUserDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public VisitStatus Status { get; set; }
    public VisitAnimalBriefDto Animal { get; set; } 
    public VisitVetBriefDto? AssignedVet { get; set; }
}