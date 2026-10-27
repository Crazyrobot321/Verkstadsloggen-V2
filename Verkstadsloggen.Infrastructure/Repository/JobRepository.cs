using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Verkstadsloggen.Domain.Enum;
using Verkstadsloggen.Domain.Models;
using Verkstadsloggen.Infrastructure.Data;
using Verkstadsloggen.Application.Interface.Repository;

namespace Verkstadsloggen.Infrastructure.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly MyDbContext _context;
        public JobRepository(MyDbContext myDbContext)
        {
            _context = myDbContext;
        }
        public async Task AddJobAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Customer)
                .Include(j => j.Mechanic)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Job>> GetJobsByStatusAsync(JobStatus status)
        {
            return await _context.Jobs
                .Include(j => j.Customer)
                .Include(j => j.Mechanic)
                .Where(j => j.Status == status)
                .ToListAsync();
        }


        public async Task<Job?> GetJobByIdAsync(Guid id)
        {
            return await _context.Jobs
                .Include(j => j.Comments)
                .Include(j => j.Customer)
                .Include(j => j.Mechanic)
                .FirstOrDefaultAsync(j => j.Id == id);
        }
        public async Task<Job?> UpdateJobByIdAsync(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();

            return await _context.Jobs
                .Include(j => j.Comments)
                .Include(j => j.Mechanic)
                .FirstOrDefaultAsync(j => j.Id == job.Id);
        }
        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}
