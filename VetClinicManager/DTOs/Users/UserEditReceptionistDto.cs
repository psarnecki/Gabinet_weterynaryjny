namespace VetClinicManager.DTOs.Users;
using VetClinicManager.Models;

public class UserEditReceptionistDto
{
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    public ICollection<Visit> AssignedVisits { get; set; } = new List<Visit>();
}