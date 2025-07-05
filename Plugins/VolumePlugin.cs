using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Runtime.InteropServices;

public class VolumePlugin
{
    private readonly object mutexLock = new();

    private const byte VK_VOLUME_UP = 0xAF;
    private const byte VK_VOLUME_DOWN = 0xAE;
    private const byte VK_VOLUME_MUTE = 0xAD;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private void PressKey(byte keyCode)
    {
        keybd_event(keyCode, 0, 0, UIntPtr.Zero);                     
        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);       
    }

    [KernelFunction("volume_up")]
    [Description("Increases the system volume by one step")]
    [return: Description("Result of volume up operation")]
    public string VolumeUp()
    {
        lock (mutexLock)
        {
            try
            {
                PressKey(VK_VOLUME_UP);
                return "Volume increased";
            }
            catch (Exception ex)
            {
                return $"Failed to increase volume";
            }
        }
    }

    [KernelFunction("volume_down")]
    [Description("Decreases the system volume by one step")]
    [return: Description("Result of volume down operation")]
    public string VolumeDown()
    {
        lock (mutexLock)
        {
            try
            {
                PressKey(VK_VOLUME_DOWN);
                return "Volume decreased";
            }
            catch (Exception ex)
            {
                return $"Failed to decrease volume";
            }
        }
    }

    [KernelFunction("set_volume_by_level")]
    [Description("Sets the system volume to a specific percentge / level")]
    [return: Description("Result of setting volume")]
    public string SetVolumeLevel(
    [Description("Desired volume level (0 to 100)")] int level)
    {
        lock (mutexLock)
        {
            try
            {
                if (level < 0 || level > 100)
                    return "Volume must be between 0 and 100";

                for (int i = 0; i < 50; i++) PressKey(VK_VOLUME_DOWN);

                int steps = (int)(level / 2.0); 
                for (int i = 0; i < steps; i++) PressKey(VK_VOLUME_UP);

                return $"Volume approximated to {level}%";
            }
            catch (Exception ex)
            {
                return $"Failed to set volume";
            }
        }
    }

    [KernelFunction("mute_volume")]
    [Description("Toggles mute for the system volume")]
    [return: Description("Result of mute toggle")]
    public string MuteVolume()
    {
        lock (mutexLock)
        {
            try
            {
                PressKey(VK_VOLUME_MUTE);
                return "Mute toggled";
            }
            catch (Exception ex)
            {
                return $"Failed to toggle mute";
            }
        }
    }
}
