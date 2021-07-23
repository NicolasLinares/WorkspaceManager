

namespace INVOXWorkspaceManager.Models.Scripts {
    class BuildScript {

        private static BuildScript _instance;

        public static BuildScript GetInstance() {
            if (_instance == null) {
                _instance = new BuildScript();
            }
            return _instance;
        }

        public string FILENAME { get; set; }
        public string DEBUG_PARAM { get; set; }
        public string RELEASE_PARAM { get; set; }
        public string BUILD_NUMBER_PARAM { get; set; }
        public string ONLYDEV_PARAM { get; set; }
        public string NOTJS_PARAM { get; set; }
        public string NOTDOC_PARAM { get; set; }

    }
}
