using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Verkstadsloggen.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Verkstadsloggen.Application.Interface.Service;

namespace Verkstadsloggen.UI.Pages;

[Authorize(Roles = "Admin")]
public class PeopleModel : PageModel
{
    private readonly IPersonService _personService;

    public PeopleModel(IPersonService personService)
    {
        _personService = personService;
    }

    public List<Person> People { get; set; } = new();
    [BindProperty]
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    public string Address { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public string PersonType { get; set; } = "Customer";

    public async Task OnGetAsync()
    {
        People = await _personService.GetAllPeopleAsync();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (PersonType == "Customer" && string.IsNullOrWhiteSpace(Address))
            ModelState.AddModelError(nameof(Address), "Address is required for customers.");

        if (!ModelState.IsValid)
        {
            People = await _personService.GetAllPeopleAsync();
            return Page();
        }

        await _personService.CreatePersonAsync(PersonType, FirstName, LastName, Email, PhoneNumber, Address);

        return RedirectToPage();
    }
}