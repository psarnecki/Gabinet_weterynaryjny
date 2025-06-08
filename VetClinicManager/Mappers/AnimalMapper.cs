    using Riok.Mapperly.Abstractions;
    using VetClinicManager.DTOs.Animals;
    using VetClinicManager.DTOs.HealthRecords;
    using VetClinicManager.DTOs.Visits.VisitBriefs;
    using VetClinicManager.Models;
    using VetClinicManager.Models.Enums;

    namespace VetClinicManager.Mappers;

    [Mapper]
    public partial class AnimalMapper
    {
        // Metody mapujące na DTO 
        [MapProperty(nameof(Animal.User), nameof(AnimalListVetRecDto.Owner))] 
        public partial AnimalListVetRecDto ToVetRecDto(Animal animal);
        public partial AnimalListUserDto ToUserDto(Animal animal);
        [MapProperty(nameof(Animal.User), nameof(AnimalListVetRecDto.Owner))] 
        public partial AnimalDetailsVetRecDto ToAnimalDetailsVetRecDto(Animal animal);
        public partial AnimalDetailsUserDto ToAnimalDetailsUserDto(Animal animal);
        public partial AnimalEditDto ToEditDto(Animal animal);

        // Metody mapujące na kolekcje DTO
        public partial IEnumerable<AnimalListVetRecDto> ToVetRecDtos(IEnumerable<Animal> animals);
        public partial IEnumerable<AnimalListUserDto> ToUserDtos(IEnumerable<Animal> animals);
        
        // Metody mapujące z DTO na encję
        public partial Animal ToEntity(CreateAnimalDto dto);
        
        [MapperIgnoreTarget(nameof(Animal.LastVisitDate))]
        public partial void UpdateFromDto(AnimalEditDto dto, Animal animal);

    }