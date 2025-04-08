using Microsoft.AspNetCore.SignalR;
using ScannerAgent.Models;
using WIA;

namespace ScannerAgent;

public class ScannerManager
{
    private readonly IHubContext<ScannerHub> _hubContext;

    public ScannerManager(IHubContext<ScannerHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public List<string> GetScanners()
    {
        List<string> scannerList = new List<string>();
        DeviceManager deviceManager = new DeviceManager();

        foreach (DeviceInfo device in deviceManager.DeviceInfos)
        {
            if (device.Type == WiaDeviceType.ScannerDeviceType)
            {
                scannerList.Add(device.Properties["Name"].get_Value()); // .Properties["Name"].ToString());
            }
        }

        return scannerList;
    }

    public async ValueTask<string> ScanAsync(string scannerName, Guid tempFileId)
    {
        Device? scannerDevice = null;
        string outputPath = string.Empty;

        #region Test code

        Thread.Sleep(TimeSpan.FromSeconds(Random.Shared.NextInt64(1, 5)));

        outputPath = "C:\\Users\\josue\\OneDrive\\Imágenes\\Screenshot 2025-04-08 092025.jpg";

        byte[] imageBytes = await File.ReadAllBytesAsync(outputPath).ConfigureAwait(false);

        ScanSingleImage dataToBeSent = new()
        {
            TempFileId = tempFileId,
            Base64Data = Convert.ToBase64String(imageBytes),
            FileName = outputPath,
        };

        await _hubContext.Clients.All.SendAsync("DocumentScanned", dataToBeSent).ConfigureAwait(false);

        return outputPath;

        #endregion

        //try
        //{
        //    DeviceManager deviceManager = new();

        //    foreach (DeviceInfo device in deviceManager.DeviceInfos)
        //    {
        //        if (device.Type == WiaDeviceType.ScannerDeviceType && device.Properties["Name"].get_Value() == scannerName)
        //        {
        //            scannerDevice = device.Connect();
        //            break;
        //        }
        //    }

        //    if (scannerDevice == null)
        //    {
        //        MessageBox.Show("Escáner no encontrado.");
        //        return outputPath;
        //    }

        //    Item scannerItem = scannerDevice.Items[1];

        //    ImageFile image = (ImageFile)scannerItem.Transfer();

        //    string userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        //    outputPath = Path.Combine(userDocumentsPath, $"{tempFileId}.jpg");

        //    byte[] imageBytes = (byte[])image.FileData.get_BinaryData();
        //    await File.WriteAllBytesAsync(outputPath, imageBytes).ConfigureAwait(false);

        //    ScanSingleImage dataToBeSent = new()
        //    {
        //        Base64Data = Convert.ToBase64String(imageBytes),
        //        FileName = outputPath,
        //        TempFileId = tempFileId
        //    };

        //    await _hubContext.Clients.All.SendAsync("DocumentScanned", dataToBeSent).ConfigureAwait(false);
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show("Error al escanear: " + ex.Message);
        //}
        //finally
        //{
        //    // Ensure the device is released
        //    if (scannerDevice != null)
        //    {
        //        Marshal.ReleaseComObject(scannerDevice);
        //    }
        //}

        //return outputPath;
    }

    public async ValueTask<string> PerpetualScanAsync(string scannerName, Guid tempFileId, Guid tempPageId)
    {
        Device? scannerDevice = null;
        string outputPath = string.Empty;

        #region Test code

        Thread.Sleep(TimeSpan.FromSeconds(Random.Shared.NextInt64(1, 5)));

        outputPath = "C:\\Users\\josue\\OneDrive\\Imágenes\\Screenshot 2025-04-08 092025.jpg";

        byte[] imageBytes = await File.ReadAllBytesAsync(outputPath).ConfigureAwait(false);

        SinglePageResult dataToBeSent = new()
        {
            TempFileId = tempFileId,
            TempPageId = tempPageId,
            Base64Data = Convert.ToBase64String(imageBytes),
            FileName = outputPath,
        };

        await _hubContext.Clients.All.SendAsync("NewPageScanned", dataToBeSent).ConfigureAwait(false);

        return outputPath;

        #endregion

        //try
        //{
        //    DeviceManager deviceManager = new();

        //    foreach (DeviceInfo device in deviceManager.DeviceInfos)
        //    {
        //        if (device.Type == WiaDeviceType.ScannerDeviceType && device.Properties["Name"].get_Value() == scannerName)
        //        {
        //            scannerDevice = device.Connect();
        //            break;
        //        }
        //    }

        //    if (scannerDevice == null)
        //    {
        //        MessageBox.Show("Escáner no encontrado.");
        //        return outputPath;
        //    }

        //    Item scannerItem = scannerDevice.Items[1];

        //    ImageFile image = (ImageFile)scannerItem.Transfer();

        //    string userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        //    outputPath = Path.Combine(userDocumentsPath, $"{tempFileId}.jpg");

        //    byte[] imageBytes = (byte[])image.FileData.get_BinaryData();
        //    await File.WriteAllBytesAsync(outputPath, imageBytes).ConfigureAwait(false);

        //    ScanSingleImage dataToBeSent = new()
        //    {
        //        Base64Data = Convert.ToBase64String(imageBytes),
        //        FileName = outputPath,
        //        TempFileId = tempFileId
        //    };

        //    await _hubContext.Clients.All.SendAsync("DocumentScanned", dataToBeSent).ConfigureAwait(false);
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show("Error al escanear: " + ex.Message);
        //}
        //finally
        //{
        //    // Ensure the device is released
        //    if (scannerDevice != null)
        //    {
        //        Marshal.ReleaseComObject(scannerDevice);
        //    }
        //}

        //return outputPath;
    }
}
