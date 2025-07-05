using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WinAI.Plugins;
using WindowsInput;
using WindowsInput.Native;
namespace WinAI.Plugins
{
    internal class SpotifyPlugin
    {
        private static readonly object mutexLock = new();

        private bool IsSpotifyRunning()
        {
            return Process.GetProcessesByName("spotify").Any();
        }

        private bool IsSpotifyPaused()
        {
            foreach (Process p in Process.GetProcessesByName("Spotify"))
            {
                if (p.MainWindowTitle.Contains("Spotify") &&
                    p.MainWindowTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length <= 2)
                {
                    return true;
                }
            }
            return false;
        }

        private void SendMediaKey(VirtualKeyCode key)
        {
            var sim = new InputSimulator();
            sim.Keyboard.KeyPress(key);
        }


        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private const int SW_MINIMIZE = 6;

        //private const uint SWP_NOSIZE = 0x0001;

        //private const uint SWP_NOMOVE = 0x0002;

        //private const uint SWP_NOACTIVATE = 0x0010;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);


        public void FocusSpotify()
        {
            var spotifyProc = Process.GetProcessesByName("Spotify").FirstOrDefault();
            if (spotifyProc != null)
            {
                IntPtr handle = spotifyProc.MainWindowHandle;
                if (handle != IntPtr.Zero)
                {
                    ShowWindow(handle, SW_RESTORE);
                    Thread.Sleep(1000);
                    SetForegroundWindow(handle);    
                }
            }
        }

        private void MinimizeSpotify()
        {
            var spotifyProc = Process.GetProcessesByName("Spotify").FirstOrDefault();
            if (spotifyProc != null && spotifyProc.MainWindowHandle != IntPtr.Zero)
            {
                ShowWindow(spotifyProc.MainWindowHandle, SW_MINIMIZE);
            }
        }

        [KernelFunction("open_spotify")]
        [Description("Open Spotify in Windows")]
        [return: Description("Status of spotify open or not open")]
        public string OpenSpotify()
        {
            lock (mutexLock)
            {
                try
                {
                    if (IsSpotifyRunning())
                        return "Spotify already running";

                    Process.Start("spotify.exe");
                    return "Opened Spotify";
                }
                catch (Exception)
                {
                    return "Could not open Spotify";
                }
            }
        }

        [KernelFunction("play_music")]
        [Description("Play music from spotify")]
        [return: Description("Status of playing music")]
        public string PlayMusic()
        {
            lock (mutexLock)
            {
                try
                {
                    if (!IsSpotifyRunning())
                        return "Open Spotify to play or pause music";

                    if (IsSpotifyPaused())
                    {
                        SendMediaKey(VirtualKeyCode.MEDIA_PLAY_PAUSE);
                        return "Music started";
                    }

                    return "Music already in play";
                }
                catch (Exception)
                {
                    return "Could not control Spotify";
                }
            }
        }

        [KernelFunction, Description("Pause music in Spotify")]
        public string PauseMusic()
        {
            lock (mutexLock)
            {
                try
                {
                    if (!IsSpotifyRunning())
                        return "Open Spotify to play or pause music";

                    if (!IsSpotifyPaused())
                    {
                        SendMediaKey(VirtualKeyCode.MEDIA_PLAY_PAUSE);
                        return "Paused music";
                    }

                    return "Music already in pause";
                }
                catch (Exception)
                {
                    return "Could not control Spotify";
                }
            }
        }

        [KernelFunction("close_spotify")]
        [Description("Close spotify if it is open")]
        [return: Description("Status of spotify close or not close")]
        public string CloseSpotify()
        {
            lock (mutexLock)
            {
                try
                {
                    var processes = Process.GetProcessesByName("spotify");
                    foreach (var proc in processes)
                    {
                        proc.Kill();
                    }

                    return processes.Length > 0 ? "Spotify closed successfully" : "Spotify not open";
                }
                catch (Exception)
                {
                    return "Unable to close Spotify";
                }
            }
        }

        [KernelFunction("play_next_music")]
        [Description("Play the next song in Spotify")]
        [return: Description("Status of next song played")]
        public string PlayNextMusic()
        {
            lock (mutexLock)
            {
                try
                {
                    if (!IsSpotifyRunning())
                        return "Open Spotify to change the song";

                    SendMediaKey(VirtualKeyCode.MEDIA_NEXT_TRACK);
                    return "Skipped to next song";
                }
                catch (Exception)
                {
                    return "Could not skip to next song";
                }
            }
        }

        [KernelFunction("replay_music")]
        [Description("Replay the current song in Spotify")]
        [return: Description("Status of current song replay")]
        public string ReplayMusic()
        {
            lock (mutexLock)
            {
                try
                {
                    if (!IsSpotifyRunning())
                        return "Open Spotify to replay music";

                    SendMediaKey(VirtualKeyCode.MEDIA_PREV_TRACK);
                    return "Replaying current song";
                }
                catch (Exception)
                {
                    return "Could not replay the song";
                }
            }
        }

        [KernelFunction("play_previous_music")]
        [Description("Play the previous song in Spotify")]
        [return: Description("Status of previous song play")]
        public string PlayPreviousMusic()
        {
            lock (mutexLock)
            {
                try
                {
                    if (!IsSpotifyRunning())
                        return "Open Spotify to play previous music";

                    SendMediaKey(VirtualKeyCode.MEDIA_PREV_TRACK);
                    Thread.Sleep(500); // simulate double press
                    SendMediaKey(VirtualKeyCode.MEDIA_PREV_TRACK);
                    return "Played previous song";
                }
                catch (Exception)
                {
                    return "Could not go to previous song";
                }
            }
        }

        [KernelFunction, Description("Search and play a specific song on Spotify")]
        public string PlaySong(string songName)
        {
            lock (mutexLock)
            {
                try
                {
                    if (!IsSpotifyRunning())
                    {
                        this.OpenSpotify();
                        Thread.Sleep(2000); 
                    }
                        

                    if (string.IsNullOrWhiteSpace(songName))
                        return "No song name provided.";

                    var sim = new InputSimulator();
                    FocusSpotify();

                    Thread.Sleep(300); 
                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_K);

                    Thread.Sleep(300); 

                    sim.Keyboard.TextEntry(songName);

                    Thread.Sleep(1500);

                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.RETURN);
                    Thread.Sleep(1500);
                    sim.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);

                    Thread.Sleep(1000);
                    MinimizeSpotify();

                    return $"Searched and tried to play \"{songName}\" in Spotify.";
                }
                catch (Exception)
                {
                    return "Failed to search or play the song in Spotify.";
                }
            }
        }
    }
}






