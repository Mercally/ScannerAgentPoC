using Microsoft.AspNetCore.Mvc;

namespace ReactApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadFilesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileDto requestDto)
    {
        if (requestDto.File == null || requestDto.File.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Use a temporary directory, such as Path.GetTempPath(), or define your own
        var tempDirectory = Path.Combine(Path.GetTempPath(), "UploadedFiles");

        // Ensure the temp directory exists
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        var filePath = Path.Combine(tempDirectory, requestDto.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await requestDto.File.CopyToAsync(stream);
        }

        return Ok(new { message = "Archivo subido con éxito.", filePath });
    }
}
