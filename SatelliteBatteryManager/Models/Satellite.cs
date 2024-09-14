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

        public void StartCharging()
        {
            IsCharging = true;
            LastChargeStartTime = DateTime.Now;
        }

        public void StopCharging()
        {
            IsCharging = false;
            LastChargeDuration = DateTime.Now - LastChargeStartTime;
        }

        public void UpdateBatteryStatus(double deltaTime)
        {
            if (IsCharging)
            {
                BatteryCharge += 0.37 * deltaTime; // Увеличиваем заряд
                if (BatteryCharge > 100) BatteryCharge = 100; // Ограничиваем максимум
            }
            else
            {
                BatteryCharge -= 5 * deltaTime; // Уменьшаем заряд
                if (BatteryCharge < 0) BatteryCharge = 0; // Ограничиваем минимум
            }

            ActiveTime += TimeSpan.FromSeconds(deltaTime);
        }
    }
}
