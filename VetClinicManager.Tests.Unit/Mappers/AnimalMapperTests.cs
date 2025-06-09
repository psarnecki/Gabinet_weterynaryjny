using VetClinicManager.DTOs.Animals;
using VetClinicManager.Mappers;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;
using NUnit;

namespace VetClinicManager.Tests.Unit.Mappers;

[TestFixture]
public class AnimalMapperTests
{
    private AnimalMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new AnimalMapper();
    }

    [Test]
    public void ToEditDto_ShouldCorrectlyMapAnimalToDto()
    {
        var animal = new Animal
        {
            Id = 1,
            Name = "Burek",
            Species = "Pies",
            Breed = "Mieszaniec",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(2020, 5, 15),
            ImageUrl = "images/burek.jpg"
        };

        var resultDto = _mapper.ToEditDto(animal);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.Id, Is.EqualTo(1));
        Assert.That(resultDto.Name, Is.EqualTo("Burek"));
        Assert.That(resultDto.Gender, Is.EqualTo(Gender.Male));
        Assert.That(resultDto.DateOfBirth, Is.EqualTo(new DateTime(2020, 5, 15)));
        Assert.That(resultDto.ImageUrl, Is.EqualTo("images/burek.jpg"));
    }

    [Test]
    public void ToEntity_ShouldCorrectlyMapCreateDtoToAnimal()
    {
        var createDto = new CreateAnimalDto
        {
            Name = "Mruczek",
            Species = "Kot",
            Breed = "Europejski",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(2019, 3, 10),
            UserId = "user-123",
            ImageUrl = "images/mruczek.jpg"
        };

        var resultAnimal = _mapper.ToEntity(createDto);

        Assert.That(resultAnimal, Is.Not.Null);
        Assert.That(resultAnimal.Name, Is.EqualTo("Mruczek"));
        Assert.That(resultAnimal.Species, Is.EqualTo("Kot"));
        Assert.That(resultAnimal.UserId, Is.EqualTo("user-123"));
        Assert.That(resultAnimal.ImageUrl, Is.EqualTo("images/mruczek.jpg"));
    }

    [Test]
    public void UpdateFromDto_ShouldUpdateExistingAnimal()
    {
        var existingAnimal = new Animal
        {
            Id = 5,
            Name = "Stare Imię",
            Species = "Chomik",
            Breed = "Syryjski",
            ImageUrl = "old-image.jpg"
        };

        var editDto = new AnimalEditDto
        {
            Id = 5,
            Name = "Nowe Imię Po Edycji",
            Species = "Chomik",
            Breed = "Dżungarski",
            ImageUrl = "new-image.jpg"
        };

        _mapper.UpdateFromDto(editDto, existingAnimal);

        Assert.That(existingAnimal.Name, Is.EqualTo("Nowe Imię Po Edycji"));
        Assert.That(existingAnimal.Breed, Is.EqualTo("Dżungarski"));
        Assert.That(existingAnimal.ImageUrl, Is.EqualTo("new-image.jpg"));
        Assert.That(existingAnimal.Id, Is.EqualTo(5));
    }

    [Test]
    public void ToAnimalDetailsUserDto_ShouldMapCorrectly()
    {
        var animal = new Animal
        {
            Id = 10,
            Name = "Azor",
            Species = "Pies",
            HealthRecord = new HealthRecord { Id = 100 },
            Visits = new List<Visit> { new Visit { Id = 1 } }
        };

        var resultDto = _mapper.ToAnimalDetailsUserDto(animal);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.Id, Is.EqualTo(10));
        Assert.That(resultDto.Name, Is.EqualTo("Azor"));
        Assert.That(resultDto.HealthRecordId, Is.EqualTo(100));
    }
}