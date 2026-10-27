using Moq;
using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Services;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Tests.ServiceTests;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _repo = new();
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        _service = new PersonService(_repo.Object);
    }

    [Fact]
    public async Task CreatePerson_Customer_Creates_Customer_With_Address()
    {
        var result = await _service.CreatePersonAsync("Customer", "Anna", "Svensson", "anna@test.se", "070123", "Storgatan 1");

        var customer = Assert.IsType<Customer>(result);
        Assert.Equal("Storgatan 1", customer.Address);
        Assert.Equal("Anna", customer.FirstName);
        _repo.Verify(r => r.AddPersonAsync(customer), Times.Once);
    }

    [Fact]
    public async Task CreatePerson_Other_Type_Creates_Mechanic()
    {
        var result = await _service.CreatePersonAsync("Mechanic", "Olle", "Berg", "olle@test.se", "070456", null);

        Assert.IsType<Mechanic>(result);
        _repo.Verify(r => r.AddPersonAsync(result), Times.Once);
    }

    [Fact]
    public async Task UpdateContactInfo_Returns_Null_When_Person_Is_Missing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person?)null);

        var result = await _service.UpdateContactInfoAsync(Guid.NewGuid(), "A", "B", "a@b.se", "1");

        Assert.Null(result);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateContactInfo_Updates_Fields_And_Saves()
    {
        var existing = new Customer { FirstName = "Gammal", LastName = "Namn" };
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existing);

        var result = await _service.UpdateContactInfoAsync(Guid.NewGuid(), "Ny", "Person", "ny@test.se", "999");

        Assert.NotNull(result);
        Assert.Equal("Ny", result!.FirstName);
        Assert.Equal("ny@test.se", result.Email);
        _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePerson_Returns_False_When_Person_Is_Missing()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person?)null);

        var result = await _service.DeletePersonAsync(Guid.NewGuid());

        Assert.False(result);
        _repo.Verify(r => r.DeletePersonAsync(It.IsAny<Person>()), Times.Never);
    }

    [Fact]
    public async Task DeletePerson_Deletes_Existing_Person()
    {
        var existing = new Customer();
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existing);

        var result = await _service.DeletePersonAsync(Guid.NewGuid());

        Assert.True(result);
        _repo.Verify(r => r.DeletePersonAsync(existing), Times.Once);
    }
}