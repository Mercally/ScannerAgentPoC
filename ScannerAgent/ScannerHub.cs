using Microsoft.AspNetCore.SignalR;

namespace ScannerAgent;

public class ScannerHub : Hub
{
    private readonly ScannerManager scannerManager = new();

    public string[] GetScanners()
    {
        return scannerManager.GetScanners().ToArray();
    }

    public string ScanSingleImage(string scannerName)
    {
        Guid tempFileId = Guid.NewGuid();

        Clients.All.SendAsync("DocumentScanned", false).GetAwaiter(); //.ConfigureAwait(false);

        _ = Task.Run(async () =>
        {
            try
            {
                string filePath = scannerManager.Scan(scannerName);

                ScanSingleImage dataToBeSent = new()
                {
                    Base64Data = Convert.ToBase64String(File.ReadAllBytes(filePath)),
                    FileName = filePath,
                    TempFileId = tempFileId
                };

                // Note: make sure this is thread-safe
                await Clients.All.SendAsync("DocumentScanned", dataToBeSent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log or handle errors here so they don't crash the process
                Console.WriteLine($"Error in background scan: {ex.Message}");
            }
        });

        return tempFileId.ToString();
    }
}

public class ScanSingleImage
{
    public Guid TempFileId { get; set; }

    public string Base64Data { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}