using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Service
{
    public interface IPersonService
    {
        Task<Customer> CreateCustomerAsync(string firstName, string lastName, string email, string phoneNumber);
        Task<Person> CreatePersonAsync(string personType, string firstName, string lastName, string email, string phoneNumber, string? address);
        Task<bool> DeletePersonAsync(Guid id);
        Task<List<Person>> GetAllPeopleAsync();
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Person?> GetPersonAsync(Guid id);
        Task<Person?> UpdateContactInfoAsync(Guid id, string firstName, string lastName, string email, string phoneNumber);
    }
}
