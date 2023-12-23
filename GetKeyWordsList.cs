using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Windows.ApplicationModel;

namespace Edge
{
    public class KeyWord
    {
        public static Dictionary<string, string> LanguageTypeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(LoadFile("/Assets/LanguageType.json"))!;
        public static Dictionary<string, string> ImageTypeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(LoadFile("/Assets/ImageType.json"))!;

        public static string LoadFile(string filePath)
        {
            string fullPath = Package.Current.InstalledPath + filePath;
            using FileStream stream = new(fullPath, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}