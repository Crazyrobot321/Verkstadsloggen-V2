using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Verkstadsloggen.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Verkstadsloggen.Infrastructure.Identity;
using Verkstadsloggen.Application.Interface.Service;

namespace Verkstadsloggen.UI.Pages
{
    [Authorize]
    public class JobsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJobService _jobService;
        private readonly IMechanicService _mechanicService;
        private readonly IPersonService _personService;

        public JobsModel(IJobService jobService, IMechanicService mechanicService, IPersonService personService, UserManager<ApplicationUser> userManager)
        {
            _jobService = jobService;
            _mechanicService = mechanicService;
            _personService = personService;
            _userManager = userManager;
        }

        public List<Job> jobs { get; set; } = new();
        public List<Mechanic> mechanics { get; set; } = new();
        public List<Customer> customers { get; set; } = new();

        [BindProperty]
        [Required]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public Guid? MechanicId { get; set; }
        [BindProperty]
        public Guid? CustomerId { get; set; }
        [BindProperty]
        public string LicensePlate { get; set; } = string.Empty;

        public async Task OnGetAsync(string search)
        {
            await LoadAsync();

            if (User.IsInRole("Admin") && MechanicId == null)
            {
                MechanicId = (await _mechanicService.GetLeastBusyMechanicAsync())?.Id;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                jobs = jobs
                    .Where(j => j.LicensePlate != null &&
                                j.LicensePlate.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync();
                return Page();
            }

            var job = new Job
            {
                Title = Title,
                Description = Description,
                Status = Domain.Enum.JobStatus.NotStarted, // Set default status
                CustomerId = CustomerId,
                MechanicId = MechanicId,
                LicensePlate = LicensePlate,
            };

            await _jobService.CreateJobAsync(job);
            return RedirectToPage();
        }

        private async Task LoadAsync()
        {
            var isAdmin = User.IsInRole("Admin");
            var isMechanic = User.IsInRole("Mechanic");
            var user = isMechanic ? await _userManager.GetUserAsync(User) : null;

            jobs = await _jobService.GetJobsForUserAsync(isAdmin, isMechanic, user?.PersonId);

            mechanics = await _mechanicService.GetAllMechanicsAsync();
            customers = await _personService.GetAllCustomersAsync();
        }
    }
}