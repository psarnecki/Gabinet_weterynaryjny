using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.DTOs.VisitUpdates;
using VetClinicManager.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Tests.Unit.Mappers;

[TestFixture]
public class VisitUpdateMapperTests
{
    private VisitUpdateMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new VisitUpdateMapper();
    }

    [Test]
    public void ToVisitUpdate_FromCreateDto_ShouldMapCorrectlyAndIgnoreMedications()
    {
        var createDto = new VisitUpdateCreateDto
        {
            Notes = "Pacjent czuje się lepiej.",
            VisitId = 101,
            AnimalMedications = new List<AnimalMedicationCreateVetDto>
            {
                new() { MedicationId = 5, StartDate = DateTime.Now }
            }
        };

        var resultUpdate = _mapper.ToVisitUpdate(createDto);
        
        Assert.That(resultUpdate, Is.Not.Null);
        Assert.That(resultUpdate.Notes, Is.EqualTo("Pacjent czuje się lepiej."));
        Assert.That(resultUpdate.VisitId, Is.EqualTo(101));
        Assert.That(resultUpdate.AnimalMedications, Is.Null.Or.Empty);
    }
    
    [Test]
    public void ToVisitUpdateEditVetDto_FromEntity_ShouldMapCorrectly()
    {
        var visitUpdateEntity = new VisitUpdate
        {
            Id = 55,
            Notes = "Notatki weterynarza do edycji",
            ImageUrl = "images/update.jpg",
            AnimalMedications = new List<AnimalMedication>
            {
                new() { Id = 1, MedicationId = 10, StartDate = DateTime.Now.AddDays(-5) },
                new() { Id = 2, MedicationId = 25, StartDate = DateTime.Now.AddDays(-2) }
            }
        };
        
        var resultDto = _mapper.ToVisitUpdateEditVetDto(visitUpdateEntity);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.Id, Is.EqualTo(55));
        Assert.That(resultDto.Notes, Is.EqualTo("Notatki weterynarza do edycji"));
        Assert.That(resultDto.ImageUrl, Is.EqualTo("images/update.jpg"));
        Assert.That(resultDto.ExistingAnimalMedications, Is.Not.Null);
        
        var firstMed = resultDto.ExistingAnimalMedications.FirstOrDefault(m => m.Id == 1);
        var secondMed = resultDto.ExistingAnimalMedications.FirstOrDefault(m => m.Id == 2);
        
        Assert.That(firstMed, Is.Null, "Nie znaleziono leku o ID 1 na liście.");
        Assert.That(secondMed, Is.Null, "Nie znaleziono leku o ID 2 na liście.");
        Assert.That(resultDto.NewAnimalMedications, Is.Empty);
        Assert.That(resultDto.RemovedMedicationIds, Is.Empty);
    }
}