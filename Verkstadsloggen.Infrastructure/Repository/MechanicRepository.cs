using Microsoft.EntityFrameworkCore;
using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Domain.Models;
using Verkstadsloggen.Infrastructure.Data;

namespace Verkstadsloggen.Infrastructure.Repository
{
    public class MechanicRepository : IMechanicRepository
    {
        private readonly MyDbContext _myDbContext;

        public MechanicRepository(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        public async Task<List<Mechanic>> GetAllMechanicsAsync()
        {
            return await _myDbContext.Set<Mechanic>()
                .OrderBy(m => m.FirstName)
                .ToListAsync();
        }
    }
}
