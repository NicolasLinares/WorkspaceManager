

namespace WorkspaceManagerTool.Models.Deploys.Scripts {
    class ArtifactsScript {

        private static ArtifactsScript _instance;

        public static ArtifactsScript GetInstance() {
            if (_instance == null) {
                _instance = new ArtifactsScript();
            }
            return _instance;
        }

        public string FILENAME => @".\artifacts.ps1";
        public string SDK_PARAM => "-sdk";
        public string GENSDKAPIDOC_PARAM => "-genSDKApiDoc";
        public string CLIENTS_PARAM => "-clients";

    }
}
