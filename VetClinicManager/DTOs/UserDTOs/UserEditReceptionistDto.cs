namespace VetClinicManager.DTOs.UserDTOs;
using VetClinicManager.Models;

public class UserEditReceptionistDto
{
    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
    public ICollection<Visit> AssignedVisits { get; set; } = new List<Visit>();
}