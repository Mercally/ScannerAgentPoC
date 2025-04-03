using Microsoft.AspNetCore.SignalR;

namespace ScannerAgent;

public class ScannerHub : Hub
{
    private readonly ScannerManager scannerManager = new();

    public string[] GetScanners()
    {
        return scannerManager.GetScanners().ToArray();
    }

    public string ScanDocument(string scannerName)
    {
        string filePath = scannerManager.Scan(scannerName);
        Clients.All.SendAsync("DocumentScanned", filePath);
        return filePath;
    }
}
