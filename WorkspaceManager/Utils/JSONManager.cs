using WorkspaceManagerTool.Models;
using WorkspaceManagerTool.Models.Deploys;
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

        public static T ReadJSON<T>(string jsonPath) {
            string jsonContent;
            try {
                jsonContent = File.ReadAllText(jsonPath);
            } catch(Exception) {
                throw;
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
