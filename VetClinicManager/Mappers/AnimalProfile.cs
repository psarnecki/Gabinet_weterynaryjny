using Riok.Mapperly.Abstractions;
using VetClinicManager.DTOs;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;

namespace VetClinicManager.Mappers;

[Mapper]
public static partial class AnimalMapper
{
    // Mapowanie z Animal na AnimalDto
    public static partial AnimalDto ToDto(this Animal animal);
    
    // Mapowanie z CreateAnimalDto na Animal
    public static partial Animal ToEntity(this CreateAnimalDto dto);
    
    // Mapowanie z UpdateAnimalDto na Animal
    public static partial Animal ToEntity(this UpdateAnimalDto dto);
    
    // Aktualizacja istniejącego Animal z UpdateAnimalDto
    public static partial void UpdateFromDto(this UpdateAnimalDto dto, Animal animal);
    
    public static partial UpdateAnimalDto ToUpdateDto(this Animal animal);
    
    // Mapowanie kolekcji Animal na AnimalDto
    public static partial IEnumerable<AnimalDto> ToDtos(this IEnumerable<Animal> animals);
    
    // Metody pomocnicze dla specjalnych przypadków (jeśli potrzebne)
    private static string? MapUserId(string? userId) => userId;
    private static Gender MapGender(Gender gender) => gender;
}