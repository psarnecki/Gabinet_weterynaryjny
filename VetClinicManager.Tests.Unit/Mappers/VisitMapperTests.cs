using VetClinicManager.DTOs.Visits;
using VetClinicManager.Mappers;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;
using NUnit;

namespace VetClinicManager.Tests.Unit.Mappers;

[TestFixture]
public class VisitMapperTests
{
    private VisitMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new VisitMapper();
    }

    [Test]
    public void ToEntity_FromCreateDto_ShouldCorrectlyMapToVisit()
    {
        var createDto = new VisitCreateDto
        {
            Title = "Kontrolna wizyta po operacji",
            Status = VisitStatus.Scheduled,
            AnimalId = 5,
            AssignedVetId = "vet-123"
        };

        var resultVisit = _mapper.ToEntity(createDto);

        Assert.That(resultVisit, Is.Not.Null);
        Assert.That(resultVisit.Title, Is.EqualTo("Kontrolna wizyta po operacji"));
        Assert.That(resultVisit.Status, Is.EqualTo(VisitStatus.Scheduled));
        Assert.That(resultVisit.AnimalId, Is.EqualTo(5));
        Assert.That(resultVisit.AssignedVetId, Is.EqualTo("vet-123"));
    }

    [Test]
    public void ToEntity_FromEditDto_ShouldUpdateExistingVisit()
    {
        var existingVisit = new Visit
        {
            Id = 10,
            Title = "Stary Tytuł",
            Status = VisitStatus.Scheduled,
            Priority = VisitPriority.Normal
        };

        var editDto = new VisitEditDto
        {
            Id = 10,
            Title = "Nowy, zaktualizowany tytuł",
            Status = VisitStatus.InProgress,
            Priority = VisitPriority.Urgent
        };

        _mapper.ToEntity(editDto, existingVisit);

        Assert.That(existingVisit.Title, Is.EqualTo("Nowy, zaktualizowany tytuł"));
        Assert.That(existingVisit.Status, Is.EqualTo(VisitStatus.InProgress));
        Assert.That(existingVisit.Priority, Is.EqualTo(VisitPriority.Urgent));
    }
    
    [Test]
    public void ToReceptionistDto_ShouldMapNestedOwnerCorrectly()
    {
        var owner = new User { Id = "owner-1", FirstName = "Jan", LastName = "Kowalski" };
        var animal = new Animal { Id = 1, Name = "Burek", User = owner };
        var visit = new Visit { Id = 100, Title = "Szczepienie", Animal = animal };

        var resultDto = _mapper.ToReceptionistDto(visit);

        Assert.That(resultDto.Owner, Is.Not.Null);
        Assert.That(resultDto.Owner.Id, Is.EqualTo("owner-1"));
        Assert.That(resultDto.Owner.FirstName, Is.EqualTo("Jan"));
        Assert.That(resultDto.Owner.LastName, Is.EqualTo("Kowalski"));
    }
    
    [Test]
    public void ToUserDto_WhenVetIsNotAssigned_ShouldMapVetAsNull()
    {
        var visit = new Visit
        {
            Id = 200,
            Title = "Badanie krwi",
            AssignedVet = null,
            Animal = new Animal { Id = 50, Name = "Testowy Zwierz" } 

        };

        var resultDto = _mapper.ToUserDto(visit);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.AssignedVet, Is.Null);
    }
}