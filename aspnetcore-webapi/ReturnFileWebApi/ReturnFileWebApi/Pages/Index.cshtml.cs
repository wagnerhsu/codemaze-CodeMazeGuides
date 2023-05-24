using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using ReturnFileWebApi.Models;

namespace ReturnFileWebApi.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetHandleStringAsync()
    {
        await Task.CompletedTask;
        return new OkObjectResult(new
            DemoDto { Name = "Wagner" });
    }

    public async Task<IActionResult> OnGetHandleFileAsync()
    {
        await Task.CompletedTask;
        return new FileContentResult(Encoding.UTF8.GetBytes("Wagner"), "text/html"){ FileDownloadName = "FileDemo.key"};
    }
}
