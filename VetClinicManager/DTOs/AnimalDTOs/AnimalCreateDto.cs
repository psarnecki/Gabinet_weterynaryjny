using VetClinicManager.Models.Enums;

namespace VetClinicManager.DTOs.AnimalDTOs;

public class CreateAnimalDto
{
    public string Name { get; set; }
    public string? MicrochipId { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public float BodyWeight { get; set; }
    public Gender Gender { get; set; }
    public string? ImageUrl { get; set; }
    public string? UserId { get; set; }
}