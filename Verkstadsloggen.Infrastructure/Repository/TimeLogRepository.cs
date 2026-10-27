using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Verkstadsloggen.Domain.Models;
using Verkstadsloggen.Infrastructure.Data;
using Verkstadsloggen.Application.Interface.Repository;

namespace Verkstadsloggen.Infrastructure.Repository
{
    public class TimeLogRepository : ITimeLogRepository
    {
        private readonly MyDbContext _context;

        public TimeLogRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddTimeLogAsync(TimeLog log)
        {
            _context.TimeLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TimeLog>> GetLogsForJobAsync(Guid jobId)
        {
            return await _context.TimeLogs
                .Where(t => t.JobId == jobId)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<TimeLog?> GetLogByIdAsync(Guid id)
        {
            return await _context.TimeLogs
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateTimeLogAsync(TimeLog log)
        {
            _context.TimeLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimeLogAsync(Guid id)
        {
            var log = await _context.TimeLogs.FindAsync(id);
            if (log != null)
            {
                _context.TimeLogs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }
    }
}
