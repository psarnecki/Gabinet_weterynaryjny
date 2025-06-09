using VetClinicManager.Areas.Admin.DTOs.Medications;
using VetClinicManager.Areas.Admin.Mappers;
using VetClinicManager.Models;
using NUnit;

namespace VetClinicManager.Tests.Unit.Mappers;

[TestFixture]
public class MedicationMapperTests
{
    private MedicationMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new MedicationMapper();
    }

    [Test]
    public void ToMedication_FromCreateDto_ShouldCorrectlyMapToEntity()
    {
        var createDto = new MedicationCreateDto
        {
            Name = "Apap",
        };

        var resultMedication = _mapper.ToMedication(createDto);

        Assert.That(resultMedication, Is.Not.Null);
        Assert.That(resultMedication.Name, Is.EqualTo("Apap"));
    }

    [Test]
    public void UpdateMedicationFromDto_ShouldUpdateExistingMedication()
    {
        var existingMedication = new Medication
        {
            Id = 15,
            Name = "Ibuprofen",
        };

        var editDto = new MedicationEditDto
        {
            Id = 15,
            Name = "Ibuprofen Forte"
        };

        _mapper.UpdateMedicationFromDto(editDto, existingMedication);

        Assert.That(existingMedication.Name, Is.EqualTo("Ibuprofen Forte"));
        Assert.That(existingMedication.Id, Is.EqualTo(15));
    }
}