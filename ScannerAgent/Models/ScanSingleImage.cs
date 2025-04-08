namespace ScannerAgent.Models;

public class ScanSingleImage
{
    public Guid TempFileId { get; set; }

    public string Base64Data { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}
