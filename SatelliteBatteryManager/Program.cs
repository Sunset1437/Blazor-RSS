using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SatelliteBatteryManager.Hubs;
using SatelliteBatteryManager.Services;
using Serilog;
using System.Timers;
using Timer = System.Timers.Timer;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добавляем службы
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<SatelliteService>(); // Регистрация сервиса
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        }); // Добавляем поддержку SignalR

        // Настройка логирования
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // Настройка маршрутов
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.MapHub<SatelliteHub>($"/{nameof(SatelliteHub)}"); // Регистрация SignalR хаба

        app.Run();


    }
}