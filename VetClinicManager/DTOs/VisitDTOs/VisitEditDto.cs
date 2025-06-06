using VetClinicManager.Models.Enums;
using VetClinicManager.Models;
using VetClinicManager.DTOs.Visits.VisitBriefs;

namespace VetClinicManager.DTOs.VisitDTOs;

public class VisitEditDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public VisitStatus Status { get; set; }
    public VisitPriority Priority { get; set; }
    public VisitAnimalBriefDto? Animal { get; set; }
    public string? AssignedVetId { get; set; }
    public VisitVetBriefDto? AssignedVet { get; set; }
    public List<VisitUpdateBriefDto> Updates { get; set; } = new();
}