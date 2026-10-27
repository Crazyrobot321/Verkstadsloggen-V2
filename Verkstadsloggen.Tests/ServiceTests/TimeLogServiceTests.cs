using Moq;
using Verkstadsloggen.Application;
using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Services;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Tests.ServiceTests;

public class TimeLogServiceTests
{
    private readonly Mock<ITimeLogRepository> _repo = new();
    private readonly TimeLogService _service;

    public TimeLogServiceTests()
    {
        _service = new TimeLogService(_repo.Object);
    }

    [Fact]
    public async Task AddTimeLog_Throws_When_End_Is_Before_Start()
    {
        var start = new DateTime(2026, 10, 27, 10, 0, 0);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddTimeLogAsync(Guid.NewGuid(), start, start.AddHours(-1), "Fel"));

        _repo.Verify(r => r.AddTimeLogAsync(It.IsAny<TimeLog>()), Times.Never);
    }

    [Fact]
    public async Task AddTimeLog_Throws_When_End_Equals_Start()
    {
        var start = new DateTime(2026, 10, 27, 10, 0, 0);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddTimeLogAsync(Guid.NewGuid(), start, start, "Noll tid"));
    }

    [Fact]
    public async Task AddTimeLog_Saves_Log_With_Correct_Fields_And_Trimmed_Description()
    {
        TimeLog? saved = null;
        _repo.Setup(r => r.AddTimeLogAsync(It.IsAny<TimeLog>()))
             .Callback<TimeLog>(l => saved = l)
             .Returns(Task.CompletedTask);

        var jobId = Guid.NewGuid();
        var start = new DateTime(2026, 10, 27, 8, 0, 0);
        var end = start.AddHours(2);

        await _service.AddTimeLogAsync(jobId, start, end, "  Bytte bromsar  ");

        Assert.NotNull(saved);
        Assert.Equal(jobId, saved!.JobId);
        Assert.Equal(start, saved.StartTime);
        Assert.Equal(end, saved.EndTime);
        Assert.Equal("Bytte bromsar", saved.Description);
    }

    [Fact]
    public async Task UpdateTimeLog_Throws_When_Times_Are_Invalid()
    {
        var start = new DateTime(2026, 10, 27, 10, 0, 0);
        var log = new TimeLog { StartTime = start, EndTime = start.AddMinutes(-5) };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateTimeLogAsync(log));

        _repo.Verify(r => r.UpdateTimeLogAsync(It.IsAny<TimeLog>()), Times.Never);
    }

    [Fact]
    public async Task GetTotalTimeForJob_Sums_All_Logs()
    {
        var jobId = Guid.NewGuid();
        var day = new DateTime(2026, 10, 27, 8, 0, 0);
        _repo.Setup(r => r.GetLogsForJobAsync(jobId)).ReturnsAsync(new List<TimeLog>
        {
            new() { StartTime = day, EndTime = day.AddHours(1) },
            new() { StartTime = day.AddHours(3), EndTime = day.AddHours(3).AddMinutes(30) },
        });

        var total = await _service.GetTotalTimeForJobAsync(jobId);

        Assert.Equal(TimeSpan.FromMinutes(90), total);
    }

    [Fact]
    public async Task GetTotalTimeForJob_Returns_Zero_When_There_Are_No_Logs()
    {
        _repo.Setup(r => r.GetLogsForJobAsync(It.IsAny<Guid>())).ReturnsAsync(new List<TimeLog>());

        var total = await _service.GetTotalTimeForJobAsync(Guid.NewGuid());

        Assert.Equal(TimeSpan.Zero, total);
    }
}