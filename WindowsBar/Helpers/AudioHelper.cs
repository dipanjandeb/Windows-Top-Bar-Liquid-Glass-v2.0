using NAudio.CoreAudioApi;
namespace WindowsBar.Helpers;

public static class AudioHelper
{
    public static bool IsSystemMuted()
    {
        try
        {
            using var enumerator = new MMDeviceEnumerator();
            using var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            return device.AudioEndpointVolume.Mute;
        }
        catch
        {
            return false;
        }
    }
}