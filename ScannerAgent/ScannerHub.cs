using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace ScannerAgent;

public class ScannerHub : Hub
{
    private readonly IServiceProvider _serviceProvider;

    public ScannerHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string[] GetScanners()
    {
        using var scope = _serviceProvider.CreateScope();
        var scannerManager = scope.ServiceProvider.GetRequiredService<ScannerManager>();

        return [.. scannerManager.GetScanners()];
    }

    public string ScanSingleImage(string scannerName)
    {
        Guid tempFileId = Guid.NewGuid();

        using var scope = _serviceProvider.CreateScope();
        var scannerManager = scope.ServiceProvider.GetRequiredService<ScannerManager>();

        _ = Task.Run(() => scannerManager.ScanAsync(scannerName, tempFileId));

        return tempFileId.ToString();
    }
}

public class ScanSingleImage
{
    public Guid TempFileId { get; set; }

    public string Base64Data { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}