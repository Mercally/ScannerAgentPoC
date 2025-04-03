using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ScannerAgent;

internal class SignalRServer
{
    private IHost _host;

    public void Start()
    {
        _host = CreateHostBuilder().Build();
        _host.RunAsync();
        Console.WriteLine("Servidor SignalR iniciado en http://localhost:5001");
    }

    public void Stop()
    {
        _host?.StopAsync().Wait();
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowReactApp", builder =>
                        builder
                            .WithOrigins("https://localhost:53348")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials());
                });

                services.AddSignalR();
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
}
