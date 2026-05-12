using System.Net.NetworkInformation;
using System.Windows.Threading;

namespace WindowsBar.Helpers;

public class SystemInfoProvider : IDisposable
{
    private readonly DispatcherTimer _timer;
    private bool _disposed;

    public event Action? DataUpdated;

    public int BatteryPercent { get; private set; }
    public bool IsCharging { get; private set; }
    public bool HasBattery { get; private set; }
    public bool IsNetworkConnected { get; private set; }
    public bool IsWifi { get; private set; }

    public SystemInfoProvider()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _timer.Tick += (s, e) => Update();
        _timer.Start();
        Update();
    }

    private void Update()
    {
        // Battery (no WMI needed — pure .NET)
        var power = System.Windows.Forms.SystemInformation.PowerStatus;
        HasBattery = power.BatteryChargeStatus != System.Windows.Forms.BatteryChargeStatus.NoSystemBattery &&
                     power.BatteryChargeStatus != System.Windows.Forms.BatteryChargeStatus.Unknown;
        if (HasBattery)
        {
            BatteryPercent = (int)(power.BatteryLifePercent * 100);
            IsCharging = power.BatteryChargeStatus.HasFlag(System.Windows.Forms.BatteryChargeStatus.Charging) ||
                         power.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Online;
        }

        // Network
        IsNetworkConnected = NetworkInterface.GetIsNetworkAvailable();
        IsWifi = false;
        if (IsNetworkConnected)
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    IsWifi = true;
                    break;
                }
            }
        }

        DataUpdated?.Invoke();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _timer.Stop();
    }
}
