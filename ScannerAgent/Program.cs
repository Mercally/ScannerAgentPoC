using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ScannerAgent;

internal static class Program
{
    private static IHost _host;
    public const string UPLOAD_API = "UploadAPI";

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        SignalRStart();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var initForm = _host.Services.GetRequiredService<StatusForm>();
        Application.Run(initForm);

        SignalRStop();
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // HTTP communication - CORS Configuration
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowReactApp", builder =>
                        builder
                            .WithOrigins("https://localhost:53348")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithOrigins("https://delightful-hill-00c5daa0f.6.azurestaticapps.net")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials());
                });

                services.AddSingleton<StatusForm>();

                // SignalR Configuration
                services.AddSignalR();

                // DI
                services.AddSingleton<ScannerManager>();

                services.AddHttpClient(UPLOAD_API, client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7014");
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://localhost:5001");
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseCors("AllowReactApp");
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<ScannerHub>("/scannerHub");
                    });
                });
            });



    private static void SignalRStart()
    {
        _host = CreateHostBuilder().Build();
        _host.RunAsync();
        Console.WriteLine("Servidor SignalR iniciado en http://localhost:5001");
    }

    private static void SignalRStop()
    {
        _host?.StopAsync().Wait();
    }

}