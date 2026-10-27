using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Repository
{
    public interface IMechanicRepository
    {
        Task<List<Mechanic>> GetAllMechanicsAsync();
    }
}
