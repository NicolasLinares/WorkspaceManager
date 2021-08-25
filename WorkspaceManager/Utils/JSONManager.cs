using WorkspaceManagerTool.Models;
using Newtonsoft.Json;
using System;
using System.IO;


namespace WorkspaceManagerTool.Utils {
    static class JSONManager {

        private static void CreateDirectory(string path) {
            string dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir))
                return;
            Directory.CreateDirectory(dir);
        }

        private static void CreateFile(string path) {
            CreateDirectory(path);
            File.Create(path);
        }

        public static T ReadJSON<T>(string jsonPath) {
            string jsonContent = string.Empty;
            try {
                jsonContent = File.ReadAllText(jsonPath);
            } catch(FileNotFoundException) {
                CreateFile(jsonPath);
            }
            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        public static void SaveJSON<T>(string jsonPath, T obj) {
            CreateDirectory(jsonPath);

            JsonSerializer serializer = new JsonSerializer {
                NullValueHandling = NullValueHandling.Ignore  //<<--- tener en cuenta
            };
            using (StreamWriter sw = new StreamWriter(jsonPath))
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                serializer.Serialize(writer, obj);
            }
        }
    }
}
