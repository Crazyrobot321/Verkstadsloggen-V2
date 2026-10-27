using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Repository
{
    public interface IPersonRepository
    {
        Task<Person?> GetByIdAsync(Guid id);
        Task<List<Person>> GetAllPeopleAsync();
        Task AddPersonAsync(Person person);

        Task SaveChangesAsync();

        Task DeletePersonAsync(Person person);

        Task<List<Customer>> GetAllCustomersAsync();
    }
}