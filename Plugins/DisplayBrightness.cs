using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinAI.Plugins
{
    internal class DisplayBrightnessPlugin
    {
        private static readonly object mutexLock = new();

        [DllImport("Dxva2.dll", SetLastError = true)]
        static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        [KernelFunction("change_brightness_level")]
        [Description("Change brightness to a specific level mentioned by the user")]
        [return: Description("Result of the brightness change operation")]
        public string ChangeBrightnessLevel(int level)
        {
            lock (mutexLock)
            {
                try
                {
                    var scope = new ManagementScope("root\\WMI");
                    var searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM WmiMonitorBrightness"));
                    var objects = searcher.Get();


                    foreach (ManagementObject obj in objects)
                    {
                        // Console.WriteLine($"CurrentBrightness: {obj["CurrentBrightness"]}\n");
                        byte currentBrightness = (byte)obj["CurrentBrightness"];
                        byte newBrightness = (byte)Math.Min(100, Math.Max(0, level));

                        var methods = new ManagementClass("WmiMonitorBrightnessMethods");
                        methods.Scope = scope;

                        foreach (ManagementObject method in methods.GetInstances())
                        {
                            var inParams = method.GetMethodParameters("WmiSetBrightness");
                            inParams["Timeout"] = 1;
                            inParams["Brightness"] = newBrightness;
                            method.InvokeMethod("WmiSetBrightness", inParams, null);
                        }

                        return $"Brightness increased to {newBrightness}%";
                    }

                    return "No display brightness information found.";
                }
                catch (Exception ex)
                {
                    return $"Failed to increase brightness: {ex.Message}";
                }
            }
        }

        [KernelFunction("brightness_up")]
        [Description("Increases the screen brightness by 10%")]
        [return: Description("Result of brightness up operation")]
        public string BrightnessUp()
        {
            lock (mutexLock)
            {
                try
                {
                    var scope = new ManagementScope("root\\WMI");
                    var searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM WmiMonitorBrightness"));
                    var objects = searcher.Get();

                    foreach (ManagementObject obj in objects)
                    {
                        byte currentBrightness = (byte)obj["CurrentBrightness"];
                        byte newBrightness = (byte)Math.Min(100, currentBrightness + 10);

                        var methods = new ManagementClass("WmiMonitorBrightnessMethods");
                        methods.Scope = scope;

                        foreach (ManagementObject method in methods.GetInstances())
                        {
                            var inParams = method.GetMethodParameters("WmiSetBrightness");
                            inParams["Timeout"] = 1;
                            inParams["Brightness"] = newBrightness;
                            method.InvokeMethod("WmiSetBrightness", inParams, null);
                        }

                        return $"Brightness increased to {newBrightness}%";
                    }

                    return "No display brightness information found.";
                }
                catch (Exception ex)
                {
                    return $"Failed to increase brightness: {ex.Message}";
                }
            }
        }

        [KernelFunction("brightness_down")]
        [Description("Decreases the screen brightness by 10%")]
        [return: Description("Result of brightness down operation")]
        public string BrightnessDown()
        {
            lock (mutexLock)
            {
                try
                {
                    var scope = new ManagementScope("root\\WMI");
                    var searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM WmiMonitorBrightness"));
                    var objects = searcher.Get();

                    foreach (ManagementObject obj in objects)
                    {
                        byte currentBrightness = (byte)obj["CurrentBrightness"];
                        byte newBrightness = (byte)Math.Max(0, currentBrightness - 10);

                        var methods = new ManagementClass("WmiMonitorBrightnessMethods");
                        methods.Scope = scope;

                        foreach (ManagementObject method in methods.GetInstances())
                        {
                            var inParams = method.GetMethodParameters("WmiSetBrightness");
                            inParams["Timeout"] = 1;
                            inParams["Brightness"] = newBrightness;
                            method.InvokeMethod("WmiSetBrightness", inParams, null);
                        }

                        return $"Brightness decreased to {newBrightness}%";
                    }

                    return "No display brightness information found.";
                }
                catch (Exception ex)
                {
                    return $"Failed to decrease brightness: {ex.Message}";
                }
            }
        }
    }
}
