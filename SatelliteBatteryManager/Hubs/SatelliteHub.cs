using SatelliteBatteryManager.Models;
using Microsoft.AspNetCore.SignalR;
using SatelliteBatteryManager.Services;

namespace SatelliteBatteryManager.Hubs
{
    public class SatelliteHub : Hub // Oбрабатывает взаимодействие с клиентами через SignalR
    {
        private readonly SatelliteService SatelliteService;

        public SatelliteHub(SatelliteService satelliteService)
        {
            SatelliteService = satelliteService;
        }

        public async Task StartCharging()
        {
            SatelliteService.ToggleCharging();// Переключаем режим зарядки
            await NotifyClients(); // Уведомляем клиентов об обновлении
        }

        public async Task StopCharging()
        {
            SatelliteService.ToggleCharging(); // Переключаем режим зарядки
            await NotifyClients(); // Уведомляем клиентов об обновлении
        }

        public async Task GetSatelliteStatus()
        {
            var satelliteStatus = SatelliteService.GetSatelliteStatus(); // Получаем текущее состояние спутника
            await Clients.All.SendAsync("UpdateSatellite", satelliteStatus); // Уведомляем всех клиентов
            //await Clients.Caller.SendAsync("UpdateSatellite", satelliteStatus);
        }

        private async Task NotifyClients()
        {
            var satelliteStatus = SatelliteService.GetSatelliteStatus(); // Текущее состояние спутника
            await Clients.All.SendAsync("UpdateSatellite", satelliteStatus);
        }
    }
}
