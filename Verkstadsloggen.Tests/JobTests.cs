using Verkstadsloggen.Domain.Enum;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Tests;

public class JobTests
{
    [Fact]
    public void NewJob_Gets_Unique_Id()
    {
        var job1 = new Job();
        var job2 = new Job();

        Assert.NotEqual(Guid.Empty, job1.Id);
        Assert.NotEqual(job1.Id, job2.Id);
    }

    [Fact]
    public void NewJob_Starts_As_NotStarted()
    {
        var job = new Job();

        Assert.Equal(JobStatus.NotStarted, job.Status);
    }

    [Fact]
    public void NewJob_Is_Created_At_Current_Utc_Time()
    {
        var job = new Job();

        Assert.InRange(job.CreatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5));
    }

    [Fact]
    public void NewJob_Starts_Without_Comments()
    {
        var job = new Job();

        Assert.Empty(job.Comments);
    }
}