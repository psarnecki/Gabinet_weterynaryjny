using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models.Enums;

namespace VetClinicManager.DTOs.Animals;

public class CreateAnimalDto
{
    [Display(Name = "Imię")]
    public string Name { get; set; }
    
    [Display(Name = "Numer Mikroczipa")]
    public string? MicrochipId { get; set; }
    
    [Display(Name = "Gatunek")]
    public string? Species { get; set; }
    
    [Display(Name = "Rasa")]
    public string? Breed { get; set; }
    
    [Display(Name = "Data Urodzenia")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    
    [Display(Name = "Waga (kg)")]
    public float BodyWeight { get; set; }
    
    [Display(Name = "Płeć")]
    public Gender Gender { get; set; }
    
    [Display(Name = "Adres URL Zdjęcia")]
    public string? ImageUrl { get; set; }
    
    [Display(Name = "Zdjęcie")]
    public IFormFile? ImageFile { get; set; }
    
    [Display(Name = "Właściciel")]
    public string? UserId { get; set; }
}