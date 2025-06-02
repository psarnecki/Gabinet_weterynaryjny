using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models.Enums;

namespace VetClinicManager.Models;

public class Animal {
    
    [Key] 
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(100)]
    public string? MicrochipId { get; set; } 
    
    [MaxLength(100)]
    public string? Species { get; set; }
    
    [MaxLength(100)]
    public string? Breed { get; set; }

    public DateTime? DateOfBirth { get; set; }
    
    [Range(0, 5000)]
    public float BodyWeight { get; set; }
    public Gender Gender { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public HealthRecord? HealthRecord { get; set; }
    
    public string? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    public DateTime? LastVisitDate { get; set; }
}