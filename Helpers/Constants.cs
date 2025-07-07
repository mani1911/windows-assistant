using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinAI.Helpers
{

    public enum KnownFolder
    {
        Contacts,
        Downloads,
        Favorites,
        Links,
        SavedGames,
        SavedSearches,
        Pictures,
        Documents,
        Desktop
    }

    // known folders are defined in https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/known-folder-guids-for-file-dialog-custom-places
    // can add more known folders as needed ;p
    public static class KnownFolders
    {
        private static readonly Dictionary<KnownFolder, Guid> _guids = new()
        {
            [KnownFolder.Contacts] = new("56784854-C6CB-462B-8169-88E350ACB882"),
            [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
            [KnownFolder.Favorites] = new("1777F761-68AD-4D8A-87BD-30B759FA33DD"),
            [KnownFolder.Links] = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968"),
            [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
            [KnownFolder.SavedSearches] = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA"),
            [KnownFolder.Pictures] = new("33E28130-4E1E-4676-835A-98395C3BC3BB"), // 33E28130-4E1E-4676-835A-98395C3BC3BB
            [KnownFolder.Documents] = new("FDD39AD0-238F-46AF-ADB4-6C85480369C7"),
            [KnownFolder.Desktop] = new("B4BFCC3A-DB2C-424C-B029-7FE99A87C641")
        };

        public static string GetPath(KnownFolder knownFolder)
        {
            return SHGetKnownFolderPath(_guids[knownFolder], 0);
        }

        [DllImport("shell32",
            CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
            nint hToken = 0);
    }
    internal class Constants
    {
        static public readonly string DOWNLOADS = KnownFolders.GetPath(KnownFolder.Downloads);
        static public readonly string PICTURES = KnownFolders.GetPath(KnownFolder.Pictures);
        static public readonly string DOCUMENTS = KnownFolders.GetPath(KnownFolder.Documents);
        static public readonly string DESKTOP = KnownFolders.GetPath(KnownFolder.Desktop);
    }
}
