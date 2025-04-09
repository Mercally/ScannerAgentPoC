using Microsoft.AspNetCore.SignalR;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using ScannerAgent.Models;
using System.Runtime.InteropServices;
using WIA;

namespace ScannerAgent;

public class ScannerManager
{
    private readonly IHubContext<ScannerHub> _hubContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public ScannerManager(IHubContext<ScannerHub> hubContext, IHttpClientFactory httpClientFactory)
    {
        _hubContext = hubContext;
        _httpClientFactory = httpClientFactory;
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

        try
        {
            SinglePageResult dataToBeSent = await PerformScanAsync(scannerName, tempFileId, null)
                .ConfigureAwait(false);

            await _hubContext.Clients.All.SendAsync("DocumentScanned", dataToBeSent).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al escanear: " + ex.Message);
        }
        finally
        {
            // Ensure the device is released
            if (scannerDevice != null)
            {
                Marshal.ReleaseComObject(scannerDevice);
            }
        }

        return outputPath;
    }

    public async ValueTask<string> PerpetualScanAsync(string scannerName, Guid tempScanFolderId, Guid tempPageId)
    {
        string outputPath = string.Empty;

        try
        {
            SinglePageResult dataToBeSent = await PerformScanAsync(scannerName, tempPageId, tempScanFolderId)
                .ConfigureAwait(false);

            await _hubContext.Clients.All.SendAsync("DocumentScanned", dataToBeSent).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al escanear: " + ex.Message);
        }

        return outputPath;
    }

    private async Task<SinglePageResult> PerformScanAsync(string scannerName, Guid tempPageId, Guid? tempFolderId)
    {
        //#region Test code

        //Thread.Sleep(TimeSpan.FromSeconds(Random.Shared.NextInt64(1, 5)));

        //string outputPath = "C:\\Users\\josue\\OneDrive\\Imágenes\\Screenshot 2025-04-08 092025.jpg";

        //byte[] imageBytes = await File.ReadAllBytesAsync(outputPath).ConfigureAwait(false);

        //return new SinglePageResult()
        //{
        //    Base64Data = Convert.ToBase64String(imageBytes),
        //    FileName = outputPath,
        //    TempPageId = tempPageId,
        //    TempFolderId = tempFolderId
        //};

        //#endregion

        Device? scannerDevice = null;
        DeviceManager deviceManager = new();

        foreach (DeviceInfo device in deviceManager.DeviceInfos)
        {
            if (device.Type == WiaDeviceType.ScannerDeviceType && device.Properties["Name"].get_Value() == scannerName)
            {
                scannerDevice = device.Connect();
                break;
            }
        }

        if (scannerDevice == null)
        {
            MessageBox.Show("Escáner no encontrado.");
            return new SinglePageResult();
        }

        Item scannerItem = scannerDevice.Items[1];

        ImageFile image = (ImageFile)scannerItem.Transfer();

        string userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string outputPath = string.Empty;
        string outputFilePath = string.Empty;

        if (tempFolderId != null)
        {
            outputPath = Path.Combine(userDocumentsPath, tempFolderId.ToString());

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            outputFilePath = Path.Combine(outputPath, $"{tempPageId}.jpg");
        }
        else
        {
            outputPath = userDocumentsPath;
            outputFilePath = Path.Combine(outputPath, $"{tempPageId}.jpg");
        }

        // Saving the image scanned to the temp folder
        byte[] imageBytes = (byte[])image.FileData.get_BinaryData();
        await File.WriteAllBytesAsync(outputFilePath, imageBytes).ConfigureAwait(false);


        // Reading the pictures as PDF from temp folder
        string pdfFilePath = ConvertFolderContentToPdf(outputPath, tempFolderId ?? Guid.NewGuid());

        byte[] pdfFile = await File.ReadAllBytesAsync(pdfFilePath);

        return new()
        {
            Base64Data = Convert.ToBase64String(pdfFile),
            FileName = outputFilePath,
            TempPageId = tempPageId,
            TempFolderId = tempFolderId
        };
    }

    private string ConvertFolderContentToPdf(string outputPath, Guid tempFolderId)
    {
        var document = new PdfDocument();

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        List<Image> scannedImages = [];

        string[] files = Directory.GetFiles(outputPath, "*.jpg");

        foreach (string file in files)
        {
            scannedImages.Add(Image.FromFile(file));
        }

        foreach (var img in scannedImages)
        {
            using var stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            var page = document.AddPage();
            page.Width = img.Width * 72 / img.HorizontalResolution;
            page.Height = img.Height * 72 / img.VerticalResolution;

            using XGraphics gfx = XGraphics.FromPdfPage(page);
            using XImage xImage = XImage.FromStream(stream);
            gfx.DrawImage(xImage, 0, 0);
        }

        string pdfFilePath = Path.Combine(outputPath, $"{tempFolderId}.pdf");

        document.Save(pdfFilePath);

        return pdfFilePath;
    }

    public async Task<string> UploadFile(Guid tempFolderId)
    {
        string userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string pdfFilePath = Path.Combine(userDocumentsPath, tempFolderId.ToString(), tempFolderId.ToString() + ".pdf");


        byte[] pdfFile = await File.ReadAllBytesAsync(pdfFilePath);

        // Upload file to the API

        var formData = new MultipartFormDataContent
        {
            { new ByteArrayContent(pdfFile), "file", tempFolderId + ".pdf" },
            { new StringContent(tempFolderId + ".pdf"), "fileName" }
        };

        HttpClient uploadApiClient = _httpClientFactory.CreateClient(Program.UPLOAD_API);

        var uploadResult = await uploadApiClient
            .PostAsync("api/uploadfiles", formData)
            .ConfigureAwait(false);

        uploadResult.EnsureSuccessStatusCode();

        await _hubContext.Clients.All.SendAsync("DocumentUploaded").ConfigureAwait(false);

        return pdfFilePath;
    }
}
