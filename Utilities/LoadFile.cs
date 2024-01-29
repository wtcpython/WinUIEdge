using System.IO;
using System.Text.Json;
using Windows.ApplicationModel;

namespace Edge.Utilities
{
    public static class Files
    {
        public static string LoadFile(string filePath)
        {
            string fullPath = Package.Current.InstalledPath + filePath;
            using FileStream stream = new(fullPath, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        public static T LoadJsonFile<T>(string filePath)
        {
            string content = LoadFile(filePath);
            T data = JsonSerializer.Deserialize<T>(content)!;
            return data;
        }
    }
}