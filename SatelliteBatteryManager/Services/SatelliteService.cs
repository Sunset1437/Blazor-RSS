using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using SatelliteBatteryManager.Hubs;
using SatelliteBatteryManager.Models;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SatelliteBatteryManager.Services
{
    public class SatelliteService // Взаимодействие с бизнес-логикой
    {
        private readonly IHubContext<SatelliteHub> HubContext;
        private readonly ILogger<SatelliteService> Logger;
        private Timer Timer;
        private Satellite Satellite;

        public SatelliteService(IHubContext<SatelliteHub> hubContext, ILogger<SatelliteService> logger)
        {
            HubContext = hubContext;
            Logger = logger;
            Satellite = new Satellite();
            Timer = new Timer(1000);
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            double deltaTime = 1; // 1 секунда
            UpdateBatteryStatus(deltaTime);
            NotifyClients();
        }

        public void UpdateBatteryStatus(double deltaTime)
        {
            if (Satellite.IsCharging)
            {
                Satellite.BatteryCharge = Math.Min(100, Satellite.BatteryCharge + 0.37 * deltaTime);
                Satellite.LastChargeDuration = Satellite.LastChargeDuration.Add(TimeSpan.FromSeconds(deltaTime));
            }
            else
            {
                Satellite.BatteryCharge = Math.Max(0, Satellite.BatteryCharge - 0.51 * deltaTime);
                Satellite.ActiveTime = Satellite.ActiveTime.Add(TimeSpan.FromSeconds(deltaTime));
            }

            if (Satellite.BatteryCharge <= 5)
            {
                Logger.LogWarning("Battery charge is below 5%!");
            }
            NotifyClients();
        }

        public void ToggleCharging()
        {
            Satellite.IsCharging=!Satellite.IsCharging;
            if (Satellite.IsCharging)
            {
                Satellite.LastChargeDuration = TimeSpan.Zero;
                Logger.LogInformation("Charging started.");
            }
            else
            {
                Satellite.ChargeEndTime = DateTime.Now;
                Logger.LogInformation("Charging stopped.");
            }
        }

        public Satellite GetSatelliteStatus()
        {
            return Satellite; // Возвращаем текущее состояние спутника
        }

        private async void NotifyClients()
        {
            await HubContext.Clients.All.SendAsync("UpdateSatellite", Satellite);
        }
    }
}
