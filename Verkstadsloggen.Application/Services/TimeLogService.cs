using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Interface.Service;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Application.Services
{
    public class TimeLogService : ITimeLogService
    {
        private readonly ITimeLogRepository _timeLogRepository;

        public TimeLogService(ITimeLogRepository timeLogRepository)
        {
            _timeLogRepository = timeLogRepository;
        }

        public async Task AddTimeLogAsync(Guid jobId, DateTime start, DateTime end, string description)
        {
            ValidateTimes(start, end);
            var log = new TimeLog
            {
                JobId = jobId,
                StartTime = start,
                EndTime = end,
                Description = description
            };

            await _timeLogRepository.AddTimeLogAsync(log);
        }

        public async Task<List<TimeLog>> GetLogsForJobAsync(Guid jobId)
        {
            return await _timeLogRepository.GetLogsForJobAsync(jobId);
        }
        public async Task<TimeSpan> GetTotalTimeForJobAsync(Guid jobId)
        {
            var logs = await _timeLogRepository.GetLogsForJobAsync(jobId);
            TimeSpan totalTime = TimeSpan.Zero;
            return TimeSpan.FromTicks(logs.Sum(log => (log.EndTime - log.StartTime).Ticks));
        }

        public async Task<TimeLog?> GetLogByIdAsync(Guid id)
        {
            return await _timeLogRepository.GetLogByIdAsync(id);
        }

        public async Task UpdateTimeLogAsync(TimeLog log)
        {
            await _timeLogRepository.UpdateTimeLogAsync(log);
        }

        public async Task DeleteTimeLogAsync(Guid id)
        {
            await _timeLogRepository.DeleteTimeLogAsync(id);
        }

        private static void ValidateTimes(DateTime start, DateTime end)
        {
            if(end <= start)
            {
                throw new ArgumentException("End time must be after start time.");
            }
        }
    }
}
