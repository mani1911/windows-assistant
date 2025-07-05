using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;   

namespace WinAI.Plugins
{
    internal class SnippingToolPlugin
    {
        private static readonly object mutexLock = new();

        //[KernelFunction("launch_snipping_tool")]
        //[Description("Launches the Snipping Tool or Snip & Sketch on Windows for taking screenshots")]
        //[return: Description("Status of launching the Snipping Tool")]
        //public string LaunchSnippingTool()
        //{
        //    lock (mutexLock)
        //    {
        //        try
        //        {
        //            Process.Start(new ProcessStartInfo
        //            {
        //                FileName = "ms-screenclip:",
        //                UseShellExecute = true
        //            });
        //            return "Snipping Tool launched";
        //        }
        //        catch (Exception)
        //        {
        //            return "Failed to launch Snipping Tool";
        //        }
        //    }
        //}

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        [KernelFunction("capture_fullscreen")]
        [Description("Captures a screenshot of the entire screen and saves it as an image")]
        [return: Description("File path of the saved screenshot or error message")]
        public string CaptureFullScreen()
        {
            lock (mutexLock)
            {
                try
                {
                    int screenWidth = GetSystemMetrics(SM_CXSCREEN);
                    int screenHeight = GetSystemMetrics(SM_CYSCREEN);

                    using Bitmap bmp = new Bitmap(screenWidth, screenHeight);
                    using Graphics g = Graphics.FromImage(bmp);
                    g.CopyFromScreen(0, 0, 0, 0, bmp.Size);

                    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    string fileName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    string fullPath = Path.Combine(folderPath, fileName);

                    bmp.Save(fullPath, ImageFormat.Png);
                    return $"Screenshot saved at {fullPath}";
                }
                catch (Exception ex)
                {
                    return $"Failed to capture screen";
                }
            }
        }
    }
}
