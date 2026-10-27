using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Service
{
    public interface ITimeLogService
    {
        Task AddTimeLogAsync(Guid jobId, DateTime start, DateTime end, string description);
        Task DeleteTimeLogAsync(Guid id);
        Task<TimeLog?> GetLogByIdAsync(Guid id);
        Task<TimeSpan> GetTotalTimeForJobAsync(Guid jobId);
        Task<List<TimeLog>> GetLogsForJobAsync(Guid jobId);
        Task UpdateTimeLogAsync(TimeLog log);
    }
}