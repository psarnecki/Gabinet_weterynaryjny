using VetClinicManager.DTOs.Users.UserBriefs;
using VetClinicManager.Models.Enums;

namespace VetClinicManager.DTOs.Animals;

public class AnimalDetailsVetRecDto
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
    public UserBriefDto? Owner { get; set; }
}