using Microsoft.AspNetCore.Identity;

namespace VetClinicManager.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Specialization { get; set; }
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    public ICollection<Visit> AssignedVisits { get; set; } = new List<Visit>();
}