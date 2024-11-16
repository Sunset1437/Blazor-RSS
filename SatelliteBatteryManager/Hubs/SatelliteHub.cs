using SatelliteBatteryManager.Models;
using Microsoft.AspNetCore.SignalR;
using SatelliteBatteryManager.Services;

namespace SatelliteBatteryManager.Hubs
{
    public class SatelliteHub : Hub // Oбрабатывает взаимодействие с клиентами через SignalR
    {

        public SatelliteHub()
        {
            
        }
        public async Task ReceiveBatteryUpdatedStatus()
        {

        }
    }
}
