using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Verkstadsloggen.Application.Interface.Service;
using Verkstadsloggen.Domain.Enum;
using Verkstadsloggen.Domain.Models;

namespace Verkstadsloggen.UI.Pages;

[Authorize]
public class JobDetails : PageModel
{
    private readonly IJobService _jobService;
    private readonly ITimeLogService _timeLogService;

    public JobDetails(IJobService jobService, ITimeLogService timeLogService)
    {
        _jobService = jobService;
        _timeLogService = timeLogService;
    }

    public Job? Job { get; set; }

    // TimeLogs list for UI
    public List<TimeLog> TimeLogs { get; set; } = new();

    [BindProperty]
    public JobStatus NewStatus { get; set; }

    [BindProperty]
    public string NewComment { get; set; } = string.Empty;

    // TimeLog BindProperties
    [BindProperty]
    public DateTime LogStart { get; set; }

    [BindProperty]
    public DateTime LogEnd { get; set; }

    [BindProperty]
    public string LogDescription { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Job = await _jobService.GetJobByIdAsync(id);

        if (Job == null)
        {
            return NotFound();
        }

        // Load TimeLogs for UI
        TimeLogs = await _timeLogService.GetLogsForJobAsync(id);

        NewStatus = Job.Status;
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(Guid id)
    {
        Job = await _jobService.GetJobByIdAsync(id);

        if (Job == null)
        {
            return NotFound();
        }

        Job.Status = NewStatus;
        await _jobService.UpdateJobAsync(Job);

        return RedirectToPage("/Jobs");
    }

    public async Task<IActionResult> OnPostAddCommentAsync(Guid id)
    {
        var job = await _jobService.GetJobByIdAsync(id);

        if (job == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(NewComment))
        {
            return Page();
        }

        var comment = new Comment
        {
            Text = NewComment,
            JobId = job.Id
        };

        await _jobService.AddCommentAsync(comment);

        return RedirectToPage(new { id });
    }

    // TimeLog POST-handler
    public async Task<IActionResult> OnPostAddTimeLogAsync(Guid id)
    {
        // Prevent NULL description crash
        LogDescription ??= "";

        await _timeLogService.AddTimeLogAsync(id, LogStart, LogEnd, LogDescription);

        return RedirectToPage(new { id });
    }

    // DELETE TimeLog handler
    public async Task<IActionResult> OnPostDeleteTimeLogAsync(Guid id, Guid logId)
    {
        await _timeLogService.DeleteTimeLogAsync(logId);
        return RedirectToPage(new { id });
    }
}
