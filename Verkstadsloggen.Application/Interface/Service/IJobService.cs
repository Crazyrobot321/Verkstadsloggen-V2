using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Service
{
    public interface IJobService
    {
        Task CreateJobAsync(Job job);
        Task<List<Job>> GetJobsForUserAsync(bool isAdmin, bool isMechanic, Guid? mechanicPersonId);
        Task<Job?> GetJobByIdAsync(Guid id);
        Task UpdateJobAsync(Job job);
        Task AddCommentAsync(Comment comment);
    }
}
