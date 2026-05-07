using System;
using System.IO;
using Newtonsoft.Json;

namespace RaidOverhaul.Helpers
{
    public class JsonHandler
    {
        public static T ReadFlagFile<T>(string fileName, string resourceFolderName)
        {
            var filePath = Path.Combine(Plugin.ResourcePath, resourceFolderName, fileName);
            filePath += ".json";
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<T>(json);
        }

        private static string SerializeObject(object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static void CreateFlagFile<T>(string fileName, string resourceFolderName)
            where T : new()
        {
            var filePath = Path.Combine(Plugin.ResourcePath, resourceFolderName, fileName);
            filePath += ".json";
            var jsonString = SerializeObject(new T());

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            File.Create(filePath).Dispose();

            var streamWriter = new StreamWriter(filePath);
            streamWriter.Write(jsonString);
            streamWriter.Flush();
            streamWriter.Close();
        }

        public static bool CheckFilePath(string fileName, string resourceFolderName)
        {
            var filePath = Path.Combine(Plugin.ResourcePath, resourceFolderName, fileName);
            filePath += ".json";

            return File.Exists(filePath);
        }

        public static void SaveToJson<T>(T data, string fileName, string resourceFolderName)
        {
            if (data == null)
            {
                return;
            }

            try
            {
                var filePath = Path.Combine(Plugin.ResourcePath, resourceFolderName, fileName);
                filePath += ".json";
                var jsonString = SerializeObject(data);

                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.Create(filePath).Dispose();
                var streamWriter = new StreamWriter(filePath);
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                Plugin._log.LogError(ex);
            }
        }
    }
}
