using System.Runtime.InteropServices;

namespace WindowsBar.Helpers;

public static class NativeMethods
{
    // Window styles
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TOOLWINDOW = 0x00000080;
    public const int WS_EX_NOACTIVATE = 0x08000000;
    public const int WS_EX_TOPMOST = 0x00000008;
    public const int WS_EX_LAYERED = 0x00080000;

    // SetWindowPos flags
    public static readonly IntPtr HWND_TOPMOST = new(-1);
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_SHOWWINDOW = 0x0040;

    // DWM
    public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    public const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
    public const int DWMWA_MICA_EFFECT = 1029;

    public const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    [DllImport("user32.dll")]
    public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("Shell32.dll")]
    public static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("shcore.dll")]
    public static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    [DllImport("user32.dll")]
    public static extern uint GetDpiForWindow(IntPtr hwnd);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int Left, Right, Top, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public IntPtr lParam;
    }

    // AppBar messages
    public const uint ABM_NEW = 0x00000000;
    public const uint ABM_REMOVE = 0x00000001;
    public const uint ABM_QUERYPOS = 0x00000002;
    public const uint ABM_SETPOS = 0x00000003;
    public const uint ABM_GETSTATE = 0x00000004;
    public const uint ABE_TOP = 1;

    // DWM Backdrop types
    public const int DWMSBT_DISABLE = 1;
    public const int DWMSBT_MAINWINDOW = 2; // Mica
    public const int DWMSBT_TRANSIENTWINDOW = 3; // Acrylic
    public const int DWMSBT_TABBEDWINDOW = 4; // Mica Alt

    /// <summary>
    /// Registers this window as an AppBar so Windows reserves screen space.
    /// </summary>
    //public static void RegisterAppBar(IntPtr hwnd, int height, int screenWidth, uint callbackMsg)
    public static void RegisterAppBar(IntPtr hwnd, int heightLogical, int screenWidthLogical, uint callbackMsg)
    {
        // Get physical DPI scale for this window
        uint dpi = GetDpiForWindow(hwnd);
        if (dpi == 0) dpi = 96; // fallback
        float scale = dpi / 96f;

        int heightPhysical = (int)Math.Round(heightLogical * scale);
        int screenWidthPhysical = (int)Math.Round(screenWidthLogical * scale);

        var abd = new APPBARDATA
        {
            cbSize = (uint)Marshal.SizeOf<APPBARDATA>(),
            hWnd = hwnd,
            uCallbackMessage = callbackMsg,
            uEdge = ABE_TOP
        };
        SHAppBarMessage(ABM_NEW, ref abd);

        //abd.rc = new RECT { Left = 0, Top = 0, Right = screenWidth, Bottom = height };
        //SHAppBarMessage(ABM_QUERYPOS, ref abd);
        //abd.rc.Bottom = abd.rc.Top + height;
        //SHAppBarMessage(ABM_SETPOS, ref abd);
        abd.rc = new RECT { Left = 0, Top = 0, Right = screenWidthPhysical, Bottom = heightPhysical };
        SHAppBarMessage(ABM_QUERYPOS, ref abd);
        abd.rc.Bottom = abd.rc.Top + heightPhysical; // re-pin height after QUERYPOS may adjust Top
        SHAppBarMessage(ABM_SETPOS, ref abd);
    }

    public static void UnregisterAppBar(IntPtr hwnd)
    {
        var abd = new APPBARDATA
        {
            cbSize = (uint)Marshal.SizeOf<APPBARDATA>(),
            hWnd = hwnd
        };
        SHAppBarMessage(ABM_REMOVE, ref abd);
    }

    /// <summary>
    /// Apply Windows 11 acrylic/blur-behind effect.
    /// </summary>
    public static void EnableAcrylic(IntPtr hwnd)
    {
        // Try DWMSBT_TRANSIENTWINDOW (Acrylic) first — Windows 11 22H2+
        int backdrop = DWMSBT_TRANSIENTWINDOW;
        DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
    }

    public static void SetDarkMode(IntPtr hwnd)
    {
        int dark = 1;
        DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
    }

    public static void ExtendGlass(IntPtr hwnd)
    {
        var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }
}
