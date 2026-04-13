using Microsoft.AspNetCore.Mvc;
using iysSite.Models;
using iysSite.Services;

namespace iysSite.Controllers;

[ApiController]
[Route("[controller]")]
public class CommissionsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _environment;

    public CommissionsController(IEmailService emailService, IWebHostEnvironment environment)
    {
        _emailService = emailService;
        _environment = environment;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CommissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads");
        Directory.CreateDirectory(uploadsFolder);

        var savedFiles = new List<string>();

        foreach (var file in request.InspirationImages.Where(file => file.Length > 0))
        {
            var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            savedFiles.Add(filePath);
        }

        await _emailService.SendCommissionRequestEmailAsync(request, savedFiles);
        await _emailService.SendClientConfirmationEmailAsync(request.Email, request.FirstName, request.CommissionType);

        return Ok(new
        {
            message = "Commission request received successfully.",
            uploadedFileCount = savedFiles.Count
        });
    }
}
