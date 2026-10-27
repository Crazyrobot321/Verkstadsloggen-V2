using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Interface.Service;
using Verkstadsloggen.Application.Services;
using Verkstadsloggen.Domain.Enum;
using Verkstadsloggen.Domain.Models;
using Verkstadsloggen.Infrastructure.Data;
using Verkstadsloggen.Infrastructure.Repository;
using Verkstadsloggen.UI.Pages;

namespace Verkstadsloggen.Tests;

public class JobDetailsPageTests
{
    private static DbContextOptions<MyDbContext> NewOptions() =>
        new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private static JobDetails CreatePage(MyDbContext context, ITimeLogService? timeLogService = null)
    {
        var jobService = new JobService(new JobRepository(context));

        return new JobDetails(jobService, timeLogService ?? Mock.Of<ITimeLogService>())
        {
            PageContext = new PageContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new PageActionDescriptor()))
            {
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            }
        };
    }

    private static async Task<Job> AddJobAsync(MyDbContext context, JobStatus status = JobStatus.NotStarted)
    {
        var job = new Job { Title = "Bromsbyte", Description = "Byte av bromsar", Status = status };
        context.Jobs.Add(job);
        await context.SaveChangesAsync();
        return job;
    }

    // ---------- Comments ----------

    [Fact]
    public async Task AddComment_WithValidText_SavesComment()
    {
        await using var context = new MyDbContext(NewOptions());
        var job = await AddJobAsync(context);
        var page = CreatePage(context);
        page.NewComment = "Kunden kontaktad";

        var result = await page.OnPostAddCommentAsync(job.Id);

        Assert.IsType<RedirectToPageResult>(result);
        var saved = await context.Comments.SingleAsync();
        Assert.Equal("Kunden kontaktad", saved.Text);
        Assert.Equal(job.Id, saved.JobId);
    }

    [Fact]
    public async Task AddComment_WithWhitespace_DoesNotSaveComment()
    {
        await using var context = new MyDbContext(NewOptions());
        var job = await AddJobAsync(context);
        var page = CreatePage(context);
        page.NewComment = "   ";

        var result = await page.OnPostAddCommentAsync(job.Id);

        Assert.IsType<PageResult>(result);
        Assert.Empty(await context.Comments.ToListAsync());
    }

    [Fact]
    public async Task AddComment_WhenJobDoesNotExist_ReturnsNotFound()
    {
        await using var context = new MyDbContext(NewOptions());
        var page = CreatePage(context);
        page.NewComment = "Kunden kontaktad";

        var result = await page.OnPostAddCommentAsync(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
        Assert.Empty(await context.Comments.ToListAsync());
    }

    // ---------- Status ----------

    [Fact]
    public async Task UpdateStatus_SavesNewStatusToDatabase()
    {
        var options = NewOptions();
        await using var context = new MyDbContext(options);
        var job = await AddJobAsync(context, JobStatus.NotStarted);
        var page = CreatePage(context);
        page.NewStatus = JobStatus.InProgress;

        var result = await page.OnPostUpdateStatusAsync(job.Id);

        Assert.IsType<RedirectToPageResult>(result);

        // Read from a new context to ensure the change was saved to the database
        await using var verifyContext = new MyDbContext(options);
        var saved = await verifyContext.Jobs.SingleAsync();
        Assert.Equal(JobStatus.InProgress, saved.Status);
    }

    [Fact]
    public async Task UpdateStatus_WhenJobDoesNotExist_ReturnsNotFound()
    {
        await using var context = new MyDbContext(NewOptions());
        var page = CreatePage(context);

        var result = await page.OnPostUpdateStatusAsync(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    // ---------- TimeLogs ----------

    private static (Mock<ITimeLogRepository> repo, TimeLogService service) CreateTimeLogService()
    {
        var repo = new Mock<ITimeLogRepository>();
        repo.Setup(r => r.GetLogsForJobAsync(It.IsAny<Guid>())).ReturnsAsync(new List<TimeLog>());
        return (repo, new TimeLogService(repo.Object));
    }

    [Fact]
    public async Task AddTimeLog_WithEndBeforeStart_ShowsErrorAndDoesNotSave()
    {
        await using var context = new MyDbContext(NewOptions());
        var job = await AddJobAsync(context);
        var (repo, service) = CreateTimeLogService();
        var page = CreatePage(context, service);
        page.LogStart = new DateTime(2026, 10, 27, 10, 0, 0);
        page.LogEnd = new DateTime(2026, 10, 27, 9, 0, 0);

        var result = await page.OnPostAddTimeLogAsync(job.Id);

        Assert.IsType<PageResult>(result);
        Assert.Equal(1, page.ModelState.ErrorCount);
        repo.Verify(r => r.AddTimeLogAsync(It.IsAny<TimeLog>()), Times.Never);
    }

    [Fact]
    public async Task AddTimeLog_WithValidTimes_SavesAndRedirects()
    {
        await using var context = new MyDbContext(NewOptions());
        var job = await AddJobAsync(context);
        var (repo, service) = CreateTimeLogService();
        var page = CreatePage(context, service);
        page.LogStart = new DateTime(2026, 10, 27, 9, 0, 0);
        page.LogEnd = new DateTime(2026, 10, 27, 10, 30, 0);
        page.LogDescription = "Bytte bromsar";

        var result = await page.OnPostAddTimeLogAsync(job.Id);

        Assert.IsType<RedirectToPageResult>(result);
        repo.Verify(r => r.AddTimeLogAsync(It.Is<TimeLog>(l => l.JobId == job.Id && l.Description == "Bytte bromsar")), Times.Once);
    }
}