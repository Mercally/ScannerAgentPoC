namespace ScannerAgent.Models;

public class SinglePageResult
{
    public Guid TempFileId { get; set; }

    public Guid TempPageId { get; set; }

    public string Base64Data { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}
