using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Interface.Service;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Services
{
    public class MechanicService : IMechanicService
    {
        private readonly IMechanicRepository _mechanicRepository;

        public MechanicService(IMechanicRepository mechanicRepository)
        {
            _mechanicRepository = mechanicRepository;
        }

        public async Task<List<Mechanic>> GetAllMechanicsAsync()
        {
            return await _mechanicRepository.GetAllMechanicsAsync();
        }
    }
}
