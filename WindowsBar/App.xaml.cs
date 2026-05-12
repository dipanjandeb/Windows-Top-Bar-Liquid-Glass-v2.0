using System.Windows;

namespace WindowsBar;

public partial class App : Application
{
    private MainBar? _bar;
    private System.Windows.Forms.NotifyIcon? _trayIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Prevent duplicate instances
        var mutex = new System.Threading.Mutex(true, "WindowsBar_SingleInstance", out bool created);
        if (!created)
        {
            Shutdown();
            return;
        }

        _bar = new MainBar();
        _bar.Show();

        SetupTrayIcon();
    }

    private void SetupTrayIcon()
    {
        _trayIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Visible = true,
            Text = "WindowsBar"
        };

        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add("Settings", null, (s, e) =>
        {
            var sw = new SettingsWindow(_bar!);
            sw.Show();
        });
        menu.Items.Add("Refresh Colors", null, (s, e) => _bar?.RefreshColors());
        menu.Items.Add("-");
        menu.Items.Add("Exit", null, (s, e) =>
        {
            _trayIcon.Visible = false;
            Shutdown();
        });
        _trayIcon.ContextMenuStrip = menu;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}
