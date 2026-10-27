using Microsoft.EntityFrameworkCore;
using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Domain.Models;
using Verkstadsloggen.Infrastructure.Data;

namespace Verkstadsloggen.Infrastructure.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly MyDbContext _context;

        public PersonRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<Person?> GetByIdAsync(Guid id)
        {
            return await _context.People.FindAsync(id);
        }

        public async Task<List<Person>> GetAllPeopleAsync()
        {
            return await _context.People
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();
        }

        public async Task AddPersonAsync(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(Person person)
        {
            _context.People.Remove(person);
            await _context.SaveChangesAsync();
        }
    }
}