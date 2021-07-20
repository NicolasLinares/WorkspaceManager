﻿

namespace WpfApp2.Model {
    class BuildScript {

        private static BuildScript _instance;

        public static BuildScript GetInstance() {
            if (_instance == null) {
                _instance = new BuildScript();
            }
            return _instance;
        }


        public string FILENAME => @".\build.ps1";
        public string DEBUG_PARAM => "-configurations Debug";
        public string RELEASE_PARAM => "-configurations Release";
        public string ONLYDEV_PARAM => "-onlyDev";
        public string NOTJS_PARAM => "-notjs";
        public string NOTDOC_PARAM => "-notdoc";
    }
}
