using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MotiZone.Models;

namespace MotiZone.Views.Financing;

public class Apply : PageModel
{
    [BindProperty] 
    public FinancingApplication Application { get; set; } = new();

    public IActionResult OnPost()
    {
       // FinancingService.SubmitApplication(Application);
        return RedirectToPage("/Dashboard");
    }
}