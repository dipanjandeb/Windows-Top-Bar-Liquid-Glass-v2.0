# 🪟 WindowsBar (Liquid Glass) - v2.0

A sleek, Mac-like top menu bar for Windows built with **WPF** and **C# .NET 8**. It replaces the traditional taskbar feel with a modern, translucent "liquid glass" aesthetic that dynamically adapts to your desktop wallpaper. 

Uses **~5–15MB RAM**

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/869d9f7d-a763-481f-aa9a-e853deffe073" />


## 🚀 What's New in v2.0
* **Native Virtual Desktop Integration:** Seamlessly track, switch, create, and delete virtual desktops right from the bar. Uses direct Windows Registry polling for zero-latency UI updates that stay perfectly in sync with the OS, even if you use trackpad swipes.
* **UIPI Override (System Window Support):** The bar now runs with elevated privileges by default. Interactive buttons (Start, Volume, Desktops) will now flawlessly execute even when high-privilege windows like Task Manager or the Registry Editor are in focus!

## ✨ Features

* **Virtual Desktop Manager:** An elegant, dynamically updating layout. Inactive desktops appear as subtle glass circles, while the active desktop smoothly expands into a pill. 
  * *Click* to switch desktops. 
  * *Middle-click* to delete a desktop. 
  * *Click +* to instantly create a new one.
* **Liquid Glass Aesthetic:** Uses native Windows Acrylic and dynamic color sampling to seamlessly blend the bar's translucency with your current desktop wallpaper.
* **Centered Clock:** A perfectly anchored, central clock displaying the time, day, and date.
* **Active Window Tracking:** Displays the name of the currently focused application or window on the left side, right next to the custom Start button.
* **Dynamic Network Indicator:** Actively fetches your current Wi-Fi SSID and displays signal strength via dynamically animating Wi-Fi waves.
* **Battery Monitor:** Shows battery percentage and a charging indicator.
* **Native Flyout Integration:** Seamlessly hooks into Windows 11's native flyouts. Clicking the Wi-Fi, Battery, Volume, or Bell icons opens the **Quick Settings / Action Center** just like the native taskbar.
* **System Tray Control:** Includes a hidden system tray icon to easily access settings, refresh colors, or cleanly exit the application.

## 🛠️ Tech Stack

* **Framework:** C# / WPF / .NET 8.0
* **APIs Used:** * `Microsoft.Win32` (Real-time polling of Explorer's Virtual Desktop Registry keys).
  * Windows Core Audio API (COM Interop for safe, native mute/volume status).
  * `user32.dll` (P/Invoke for keystroke simulation, window handles, and acrylic rendering).
  * `netsh` (For parsing raw Wi-Fi and signal strength data).

## 📥 How to Use

For users who just want to run the software:
1. Clone or download the project.
2. Keep only the `Windows Top Bar` folder and delete the source code files if you don't need them.
3. Navigate to **`Windows Top Bar\x64\Release\net8.0-windows`**.
4. Run the `WindowsBar.exe` file. 
5. *Note: You will be prompted by Windows UAC to run as Administrator. This is strictly required so the bar can send keyboard shortcuts while system apps (like Task Manager) are open.*

---

## 💻 For Developers

### Prerequisites
* Windows 11 (build 22000+), tested on 24H2 26100+
* Visual Studio 2022
* .NET 8.0 Desktop Runtime

### Build & Run
1. Clone the repository.
2. Open `WindowsBar.sln` in Visual Studio 2022.
3. Set the platform to **x64**.
4. Press **F5** to run. *Note: Visual Studio will prompt you to restart it with Administrator credentials to debug the UIPI manifest changes.*

### Customization
Edit `MainBar.xaml` to tweak the UI:
- `Height="36"` on the Window → change bar height.
- `GradTop` / `GradBot` GradientStop colors → override the wallpaper tint blending.
- `PillBorder` style → change pill opacity/radius for the clock and active virtual desktop.

## ⚠️ Troubleshooting

**Buttons stop working when Task Manager is open:** Ensure you did not remove the `<requestedExecutionLevel level="requireAdministrator" />` line from the `app.manifest`. The app must run as Admin to bypass Windows UIPI restrictions.

**Colors don't match the wallpaper:** Click the background of the bar to force a color refresh.

**Bar not staying on top after fullscreen games:** Some games grab exclusive fullscreen. Switch to Borderless Windowed in your game's video settings.

**AppBar not reserving space (windows overlap bar):** Restart the app once to let the AppBar register. Subsequent runs work normally.
