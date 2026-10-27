using Moq;
using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Services;
using Verkstadsloggen.Domain.Enum;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.Tests.ServiceTests;

public class MechanicServiceTests
{
    private readonly Mock<IMechanicRepository> _mechanics = new();
    private readonly Mock<IJobRepository> _jobs = new();
    private readonly MechanicService _service;

    private readonly Mechanic _olle = new() { FirstName = "Olle", LastName = "Berg" };
    private readonly Mechanic _lisa = new() { FirstName = "Lisa", LastName = "Ek" };

    public MechanicServiceTests()
    {
        _service = new MechanicService(_mechanics.Object, _jobs.Object);
    }

    private void Setup(List<Mechanic> mechanics, List<Job> jobs)
    {
        _mechanics.Setup(r => r.GetAllMechanicsAsync()).ReturnsAsync(mechanics);
        _jobs.Setup(r => r.GetAllJobsAsync()).ReturnsAsync(jobs);
    }

    [Fact]
    public async Task GetWorkload_Counts_Jobs_Per_Status()
    {
        Setup(new() { _olle }, new()
        {
            new() { MechanicId = _olle.Id, Status = JobStatus.NotStarted },
            new() { MechanicId = _olle.Id, Status = JobStatus.InProgress },
            new() { MechanicId = _olle.Id, Status = JobStatus.InProgress },
            new() { MechanicId = _olle.Id, Status = JobStatus.Completed },
        });

        var result = await _service.GetMechanicWorkloadAsync();

        var w = Assert.Single(result);
        Assert.Equal("Olle Berg", w.Name);
        Assert.Equal(1, w.NotStarted);
        Assert.Equal(2, w.InProgress);
        Assert.Equal(1, w.Completed);
        Assert.Equal(4, w.Total);
    }

    [Fact]
    public async Task GetWorkload_Includes_Mechanic_Without_Jobs()
    {
        Setup(new() { _olle, _lisa }, new()
        {
            new() { MechanicId = _olle.Id, Status = JobStatus.InProgress },
        });

        var result = await _service.GetMechanicWorkloadAsync();

        var lisa = result.Single(w => w.MechanicId == _lisa.Id);
        Assert.Equal(0, lisa.Total);
    }

    [Fact]
    public async Task GetWorkload_Adds_Unassigned_Row_Only_When_Unassigned_Jobs_Exist()
    {
        Setup(new() { _olle }, new()
        {
            new() { MechanicId = null, Status = JobStatus.NotStarted },
        });

        var result = await _service.GetMechanicWorkloadAsync();

        Assert.Contains(result, w => w.MechanicId == null && w.Name == "Unassigned" && w.NotStarted == 1);
    }

    [Fact]
    public async Task GetWorkload_Has_No_Unassigned_Row_When_All_Jobs_Are_Assigned()
    {
        Setup(new() { _olle }, new()
        {
            new() { MechanicId = _olle.Id, Status = JobStatus.NotStarted },
        });

        var result = await _service.GetMechanicWorkloadAsync();

        Assert.DoesNotContain(result, w => w.MechanicId == null);
    }

    [Fact]
    public async Task GetWorkload_Is_Sorted_By_Total_Descending()
    {
        Setup(new() { _olle, _lisa }, new()
        {
            new() { MechanicId = _lisa.Id, Status = JobStatus.NotStarted },
            new() { MechanicId = _lisa.Id, Status = JobStatus.NotStarted },
            new() { MechanicId = _olle.Id, Status = JobStatus.NotStarted },
        });

        var result = await _service.GetMechanicWorkloadAsync();

        Assert.Equal(_lisa.Id, result[0].MechanicId);
    }

    [Fact]
    public async Task GetLeastBusyMechanic_Ignores_Completed_Jobs()
    {
        Setup(new() { _olle, _lisa }, new()
        {
            new() { MechanicId = _olle.Id, Status = JobStatus.Completed },
            new() { MechanicId = _olle.Id, Status = JobStatus.Completed },
            new() { MechanicId = _lisa.Id, Status = JobStatus.InProgress },
        });

        var result = await _service.GetLeastBusyMechanicAsync();

        Assert.Equal(_olle.Id, result!.Id);
    }

    [Fact]
    public async Task GetLeastBusyMechanic_Returns_Null_When_There_Are_No_Mechanics()
    {
        Setup(new(), new());

        var result = await _service.GetLeastBusyMechanicAsync();

        Assert.Null(result);
    }
}