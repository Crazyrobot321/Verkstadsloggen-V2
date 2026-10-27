using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Interface.Service;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task CreateJobAsync(Job job)
        {
            await _jobRepository.AddJobAsync(job);
        }

        public async Task<List<Job>> GetJobsForUserAsync(bool isAdmin, bool isMechanic, Guid? mechanicPersonId)
        {
            var allJobs = await _jobRepository.GetAllJobsAsync();

            if (isAdmin)
            {
                return allJobs;
            }

            if (isMechanic && mechanicPersonId.HasValue)
            {
                return allJobs.Where(j => j.MechanicId == mechanicPersonId.Value).ToList();
            }

            return new List<Job>();
        }

        public async Task<Job?> GetJobByIdAsync(Guid id)
        {
            return await _jobRepository.GetJobByIdAsync(id);
        }

        public async Task UpdateJobAsync(Job job)
        {
            await _jobRepository.UpdateJobByIdAsync(job);
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _jobRepository.AddCommentAsync(comment);
        }
    }
}
