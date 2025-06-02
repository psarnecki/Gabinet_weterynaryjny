using VetClinicManager.Models.Enums;

namespace VetClinicManager.Models;

public class Animal {
    public int Id { get; set; }
    public string Name { get; set; }
    public string? MicrochipId { get; set; } 
    public string Species { get; set; }
    public string Breed { get; set; }
    public DateTime DateOfBirth { get; set; }
    public float BodyWeight { get; set; }
    public Gender Gender { get; set; }
    public string? ImageUrl { get; set; }
    public HealthRecord? HealthRecord { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    public DateTime? LastVisitDate { get; set; }
}