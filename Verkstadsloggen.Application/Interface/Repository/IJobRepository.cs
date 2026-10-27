using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Repository
{
    public interface IJobRepository
    {
        Task AddJobAsync(Job job);
        Task<List<Job>> GetAllJobsAsync();
        Task<Job?> GetJobByIdAsync(Guid id);
        Task<Job?> UpdateJobByIdAsync(Job job);
        Task AddCommentAsync(Comment comment);
    }
}