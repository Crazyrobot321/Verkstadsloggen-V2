using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Service
{
    public interface IMechanicService
    {
        Task<List<Mechanic>> GetAllMechanicsAsync();
    }
}
