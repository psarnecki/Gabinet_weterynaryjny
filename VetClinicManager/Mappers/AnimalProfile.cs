using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs.Animals;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;

namespace VetClinicManager.Mappers;

[Mapper]
public partial class AnimalMapper
{
    // Mapowanie z Animal na AnimalListVetRecDto (dla weterynarzy/recepcji)
    public partial AnimalListVetRecDto ToVetRecDto(Animal animal);
    
    // Mapowanie z Animal na AnimalListUserDto (dla klientów)
    public partial AnimalListUserDto ToUserDto(Animal animal);
    
    // Mapowanie z CreateAnimalDto na Animal
    public partial Animal ToEntity(CreateAnimalDto dto);
    
    // Mapowanie z AnimalEditDto na Animal (tworzenie nowej instancji)
    public partial Animal ToEntity(AnimalEditDto dto);
    
    // Aktualizacja istniejącego Animal z AnimalEditDto
    public partial void UpdateFromDto(AnimalEditDto dto, Animal animal);
    
    // Mapowanie z Animal na AnimalEditDto
    public partial AnimalEditDto ToEditDto(Animal animal);
    
    // Mapowanie kolekcji Animal na AnimalListVetRecDto
    public partial IEnumerable<AnimalListVetRecDto> ToVetRecDtos(IEnumerable<Animal> animals);
    
    // Mapowanie kolekcji Animal na AnimalListUserDto
    public partial IEnumerable<AnimalListUserDto> ToUserDtos(IEnumerable<Animal> animals);

    // Metody pomocnicze dla specjalnych przypadków
    private string? MapMicrochipId(string? microchipId) => microchipId;
    private string? MapSpecies(string? species) => species;
    private string? MapBreed(string? breed) => breed;
    private string? MapImageUrl(string? imageUrl) => imageUrl;
    private string? MapUserId(string? userId) => userId;
    private Gender MapGender(Gender gender) => gender;
    
    // Metoda dla LastVisitDate - można dodać logikę jeśli potrzebna
    private DateTime? MapLastVisitDate(DateTime? date) => date;
}