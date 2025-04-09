using System.ComponentModel.DataAnnotations;

namespace ReactApp.Server;

public class UploadFileDto
{
    [Required]
    public IFormFile File { get; set; }

    [Required]
    public string FileName { get; set; } = string.Empty;
}
