namespace SatelliteBatteryManager.Models
{
    public class Satellite
    {
        public double BatteryCharge { get; set; } = 100.0; // Начальный заряд 100%
        public TimeSpan ActiveTime { get; set; } = TimeSpan.Zero; // Время активной работы
        public DateTime LastChargeStartTime { get; set; }
        public TimeSpan LastChargeDuration { get; set; }
        public bool IsCharging { get; set; } = false;
        public DateTime ChargeEndTime { get; set; }

    }
}
