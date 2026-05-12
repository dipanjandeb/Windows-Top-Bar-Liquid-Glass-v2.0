using System.Windows;
using WindowsBar.Helpers;

namespace WindowsBar;

public partial class SettingsWindow : Window
{
    private readonly MainBar _bar;

    public SettingsWindow(MainBar bar)
    {
        InitializeComponent();
        _bar = bar;
        ChkStartup.IsChecked = StartupHelper.IsStartupEnabled();
    }

    private void ChkStartup_Changed(object sender, RoutedEventArgs e)
        => StartupHelper.SetStartup(ChkStartup.IsChecked == true);

    private void BtnRefreshColors_Click(object sender, RoutedEventArgs e)
        => _bar.RefreshColors();

    private void BtnClose_Click(object sender, RoutedEventArgs e)
        => Close();
}
