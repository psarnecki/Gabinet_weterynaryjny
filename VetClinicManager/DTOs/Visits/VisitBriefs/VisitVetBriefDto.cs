using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.DTOs.Visits.VisitBriefs;

public class VisitVetBriefDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}