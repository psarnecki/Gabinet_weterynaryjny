using VetClinicManager.Mappers;
using VetClinicManager.Models;
using NUnit;

namespace VetClinicManager.Tests.Unit.Mappers;

public class UserMapperTests
{
    private UserMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new UserMapper();
    }

    [Test]
    public void ToUserBriefDto_ShouldCorrectlyMapUser()
    {
        var user = new User
        {
            Id = "user-123",
            FirstName = "Jan",
            LastName = "Kowalski"
        };

        var resultDto = _mapper.ToUserBriefDto(user);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.Id, Is.EqualTo("user-123"));
        Assert.That(resultDto.FirstName, Is.EqualTo("Jan"));
        Assert.That(resultDto.LastName, Is.EqualTo("Kowalski"));
    }

    [Test]
    public void ToVisitVetBriefDto_ShouldCorrectlyMapUser()
    {
        var user = new User
        {
            Id = "vet-abc",
            FirstName = "Anna",
            LastName = "Nowak",
            Email = "anna.nowak@vetclinic.com"
        };

        var resultDto = _mapper.ToVisitVetBriefDto(user);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.Id, Is.EqualTo("vet-abc"));
        Assert.That(resultDto.FirstName, Is.EqualTo("Anna"));
        Assert.That(resultDto.LastName, Is.EqualTo("Nowak"));
        Assert.That(resultDto.Email, Is.EqualTo("anna.nowak@vetclinic.com"));
    }
    
    [Test]
    public void ToUserBriefDto_WithNullNames_ShouldMapNulls()
    {
        var user = new User
        {
            Id = "user-no-name",
            FirstName = null,
            LastName = null
        };

        var resultDto = _mapper.ToUserBriefDto(user);

        Assert.That(resultDto, Is.Not.Null);
        Assert.That(resultDto.Id, Is.EqualTo("user-no-name"));
        Assert.That(resultDto.FirstName, Is.Null);
        Assert.That(resultDto.LastName, Is.Null);
    }
}