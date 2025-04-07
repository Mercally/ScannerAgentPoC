namespace ScannerAgent;

public class FileUploaderWorker
{
    private FileSystemWatcher _watcher;
    private readonly string _scanFolder = Path.Combine(Application.StartupPath, "ScannedFiles"); // Carpeta donde se guardan los escaneos
    private readonly string _apiUrl = "https://tuapi.com/upload"; // Cambia esto por la URL de tu API

    public FileUploaderWorker()
    {
        // Crear la carpeta si no existe
        if (!Directory.Exists(_scanFolder))
            Directory.CreateDirectory(_scanFolder);

        _watcher = new FileSystemWatcher(_scanFolder)
        {
            Filter = "*.jpg", // Monitorea archivos .jpg (cambia si usas otro formato)
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        _watcher.Created += OnNewFileDetected;
        _watcher.EnableRaisingEvents = true; // Inicia la observación de la carpeta
    }

    private async void OnNewFileDetected(object sender, FileSystemEventArgs e)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));

        try
        {
            Console.WriteLine($"Nuevo archivo detectado: {e.FullPath}");
            await UploadFileToApi(e.FullPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error subiendo archivo: {ex.Message}");
        }
    }

    private Task UploadFileToApi(string filePath)
    {
        return Task.CompletedTask;

        //using (var client = new HttpClient())
        //{
        //    using (var form = new MultipartFormDataContent())
        //    {
        //        var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
        //        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // Ajusta si usas otro tipo de archivo

        //        form.Add(fileContent, "file", Path.GetFileName(filePath));

        //        Console.WriteLine($"Subiendo archivo {filePath} a {_apiUrl}");
        //        var response = await client.PostAsync(_apiUrl, form);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine("Archivo subido con éxito.");
        //            File.Delete(filePath); // Borra el archivo después de subirlo (opcional)
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Error al subir archivo: {response.StatusCode}");
        //        }
        //    }
        //}
    }
}
