using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Edge
{
    public static class GetMoreSpecialFolder
    {
        public static List<Guid> GuidList = [
            new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7"),
            new Guid("374DE290-123F-4565-9164-39C4925E467B"),
            new Guid("4BD8D571-6D19-48D3-BE97-422220080E43"),
            new Guid("33E28130-4E1E-4676-835A-98395C3BC3BB"),
            new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4")];

        public enum SpecialFolder
        {
            Document = 0,
            Downloads = 1,
            Music = 2,
            Pictures = 3,
            SavedGames = 4
        }

        public static string GetSpecialFolder(SpecialFolder specialFolder)
        {
            return SHGetKnownFolderPath(GuidList[(int)specialFolder], 0);
        }

        [DllImport("shell32", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, nint hToken = default);

    }
}
