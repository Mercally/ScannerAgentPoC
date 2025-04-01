using WIA;

namespace ScannerAgent;

internal class ScannerManager
{
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

    public string Scan(string scannerName)
    {
        try
        {
            DeviceManager deviceManager = new();
            Device scannerDevice = null;

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
                return null;
            }

            Item scannerItem = scannerDevice.Items[1]; // Primer elemento del escáner

            ImageFile image = (ImageFile)scannerItem.Transfer(); // Escanear en JPEG

            string outputPath = Path.Combine(Application.StartupPath, "ScannedDocument.jpg");

            byte[] imageBytes = (byte[])image.FileData.get_BinaryData();
            File.WriteAllBytes(outputPath, imageBytes);

            return outputPath;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al escanear: " + ex.Message);
            return null;
        }
    }
}
