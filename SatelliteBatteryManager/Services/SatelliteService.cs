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
        private readonly IHubContext<SatelliteHub> _hubContext;
        private readonly ILogger<SatelliteService> _logger;
        private Timer _timer;
        private Satellite _satellite;

        public SatelliteService(IHubContext<SatelliteHub> hubContext, ILogger<SatelliteService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
            _satellite = new Satellite();
            _timer = new Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            double deltaTime = 1; // 1 секунда
            UpdateBatteryStatus(deltaTime);
            NotifyClients();
        }

        public void UpdateBatteryStatus(double deltaTime)
        {
            if (_satellite.IsCharging)
            {
                _satellite.BatteryCharge = Math.Min(100, _satellite.BatteryCharge + 0.37 * deltaTime);
                _satellite.LastChargeDuration = _satellite.LastChargeDuration.Add(TimeSpan.FromSeconds(deltaTime));
            }
            else
            {
                _satellite.BatteryCharge = Math.Max(0, _satellite.BatteryCharge - 0.51 * deltaTime);
                _satellite.ActiveTime = _satellite.ActiveTime.Add(TimeSpan.FromSeconds(deltaTime));
            }

            if (_satellite.BatteryCharge <= 5)
            {
                _logger.LogWarning("Battery charge is below 5%!");
            }
            NotifyClients();
        }

        public void ToggleCharging()
        {
            _satellite.IsCharging=!_satellite.IsCharging;
            if (_satellite.IsCharging)
            {
                _satellite.LastChargeDuration = TimeSpan.Zero;
                _logger.LogInformation("Charging started.");
            }
            else
            {
                _satellite.ChargeEndTime = DateTime.Now;
                _logger.LogInformation("Charging stopped.");
            }
            NotifyClients();
        }

        public Satellite GetSatelliteStatus()
        {
            return _satellite; // Возвращаем текущее состояние спутника
        }

        private async void NotifyClients()
        {
            await _hubContext.Clients.All.SendAsync(nameof(SatelliteHub.ReceiveBatteryUpdatedStatus), _satellite);
        }
    }
}
