namespace VetClinicManager.DTOs.Animals;
using VetClinicManager.Models.Enums;

public class AnimalListUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? MicrochipId { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public float BodyWeight { get; set; }
    public Gender Gender { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? LastVisitDate { get; set; }
}