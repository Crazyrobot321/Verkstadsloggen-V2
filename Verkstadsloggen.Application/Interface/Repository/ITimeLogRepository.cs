using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Interface.Repository
{
    public interface ITimeLogRepository
    {
        Task AddTimeLogAsync(TimeLog log);
        Task<List<TimeLog>> GetLogsForJobAsync(Guid jobId);
        Task<TimeLog?> GetLogByIdAsync(Guid id);
        Task UpdateTimeLogAsync(TimeLog log);
        Task DeleteTimeLogAsync(Guid id);
    }
}
