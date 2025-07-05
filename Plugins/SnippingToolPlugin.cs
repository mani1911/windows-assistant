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
using WinAI.Helpers;

namespace WinAI.Plugins
{
    internal class SnippingToolPlugin
    {
        private static readonly object mutexLock = new();

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        [KernelFunction("capture_fullscreen_and_save_to_folder")]
        [Description("Captures a screenshot of the entire screen and saves it as an image in the path given by user")]
        [return: Description("File path of the saved screenshot or error message")]
        public string CaptureFullScreen(string path)
        {
            lock (mutexLock)
            {
                try
                {
                    string folderPath = "";
                    switch (path.ToLower())
                    {
                        case "downloads":
                            folderPath = Constants.DOWNLOADS;
                            break;
                        case "pictures":
                            folderPath = Constants.PICTURES;
                            break;
                        case "documents":
                            folderPath = Constants.DOCUMENTS;
                            break;
                        default:
                            break;

                    }


                    if (string.IsNullOrEmpty(folderPath))
                    {
                        return "Invalid path specified. Please use 'downloads' or a valid folder path.";
                    }

                    int screenWidth = GetSystemMetrics(SM_CXSCREEN);
                    int screenHeight = GetSystemMetrics(SM_CYSCREEN);

                    using Bitmap bmp = new Bitmap(screenWidth, screenHeight);
                    using Graphics g = Graphics.FromImage(bmp);
                    g.CopyFromScreen(0, 0, 0, 0, bmp.Size);

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
