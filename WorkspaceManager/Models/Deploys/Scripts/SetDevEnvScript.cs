

namespace INVOXWorkspaceManager.Models.Deploys.Scripts {

    public enum CleanOptions {
        CLEAN,
        FULLCLEAN,
        NOTCLEAN
    }

    public enum BranchOptions {
        INFO,
        BRANCH
    }

    public enum BuildOptions {
        ONLYDEV,
        NOTJS,
        NOTDOC
    }

    class SetDevEnvScript {

        private static SetDevEnvScript _instance;

        public static SetDevEnvScript GetInstance() {
            if (_instance == null) {
                _instance = new SetDevEnvScript();
            }
            return _instance;
        }

        public string FILENAME = @".\setDevEnv.ps1";
        public string INFO_PARAM => "-info";
        public string FULLCLEAN_PARAM => "-fullClean";
        public string CLEAN_PARAM => "-clean";
        public string BRANCH_PARAM => "-branch";

        public string REVERT_WORKSPACE => "git reset --hard";
    }
}
