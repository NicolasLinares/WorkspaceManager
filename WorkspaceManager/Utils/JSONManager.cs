using INVOXWorkspaceManager.Model;
using Newtonsoft.Json;
using System.IO;


namespace INVOXWorkspaceManager.Utils {
    class JSONManager {

        private string DeploysDirectory {
            get {
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string directory = @"INVOXWorkspaceManager\deploys\";
                return Path.Combine(AppData, directory);
            }
        }

        //... añadir directorios necesarios

        public Deploy ReadJSON(string file) {
            string jsonPath = Path.Combine(DeploysDirectory, file);
            string jsonContent = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<Deploy>(jsonContent);
        }

        public void SaveJSON(string filename, Deploy deploy) {

            string jsonPath = Path.Combine(DeploysDirectory, filename);

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;  //<<--- tener en cuenta
            using (StreamWriter sw = new StreamWriter(jsonPath))
            using (JsonWriter writer = new JsonTextWriter(sw)) {
                serializer.Serialize(writer, deploy);
            }
        }
    }
}
