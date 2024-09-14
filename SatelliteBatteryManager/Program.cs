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

        // ��������� ������
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<SatelliteService>(); // ����������� �������
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        }); // ��������� ��������� SignalR

        // ��������� �����������
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        //// ��������� Serilog
        //Log.Logger = new LoggerConfiguration()
        //    .MinimumLevel.Debug() // ������� �����������
        //    .WriteTo.Console()    // ���� � �������
        //    //.WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day) // ���� � ����
        //    .CreateLogger();

        //builder.Host.UseSerilog(); //Serilog ��� ������

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // ��������� ���������
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.MapHub<SatelliteHub>("/satelliteHub"); // ����������� SignalR ����

        // ��������� ������ ���������� ���������
        StartBatteryUpdateTimer(app.Services.GetRequiredService<SatelliteService>());

        app.Run();

        void StartBatteryUpdateTimer(SatelliteService satelliteService)
        {
            Timer timer = new Timer(1000); // ���������� ������ �������
            timer.Elapsed += (sender, e) =>
            {
                satelliteService.UpdateBatteryStatus(1);
            };
            timer.Start();
        }

    }
}