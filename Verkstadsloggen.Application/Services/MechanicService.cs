using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Interface.Service;
using Verkstadsloggen.Domain.Enum;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Services
{
    public class MechanicService : IMechanicService
    {
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IJobRepository _jobRepository;

        public MechanicService(IMechanicRepository mechanicRepository, IJobRepository jobRepository)
        {
            _mechanicRepository = mechanicRepository;
            _jobRepository = jobRepository;
        }

        public async Task<List<Mechanic>> GetAllMechanicsAsync()
        {
            return await _mechanicRepository.GetAllMechanicsAsync();
        }
        public async Task<List<MechanicWorkload>> GetMechanicWorkloadAsync()
        {
            var mechanics = await _mechanicRepository.GetAllMechanicsAsync();
            var jobs = await _jobRepository.GetAllJobsAsync();

            var result = mechanics.Select(m => CreateWorkload(m.Id, $"{m.FirstName} {m.LastName}", jobs.Where(j => j.MechanicId == m.Id)))
                .ToList();
            var unassigned = jobs.Where(j => j.MechanicId == null).ToList();
            if (unassigned.Count > 0)
            {
                result.Add(CreateWorkload(null, "Unassigned", unassigned));
            }
            return result.OrderByDescending(w => w.Total).ToList();
        }

        public async Task<Mechanic?> GetLeastBusyMechanicAsync()
        {
            var mechanics = await _mechanicRepository.GetAllMechanicsAsync();
            if (mechanics.Count == 0) return null;

            var jobs = await _jobRepository.GetAllJobsAsync();

            return mechanics
                .OrderBy(m => jobs.Count(j => j.MechanicId == m.Id && j.Status != JobStatus.Completed))
                .First();
        }

        private static MechanicWorkload CreateWorkload(Guid? id, string name, IEnumerable<Job> jobs)
        {
            var list = jobs.ToList();
            return new MechanicWorkload(
                id,
                name,
                list.Count(j => j.Status == JobStatus.NotStarted),
                list.Count(j => j.Status == JobStatus.InProgress),
                list.Count(j => j.Status == JobStatus.Completed));
        }
    }
}
