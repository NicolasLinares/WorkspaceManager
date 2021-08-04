
namespace WorkspaceManagerTool.Models.Deploys {

    // Only one command for each type
    public enum CommandType {
        REVERT = 0,  
        CLEAN = 1,
        BRANCH = 2,
        BUILD_DEBUG = 3,
        BUILD_RELEASE = 4,
        INSTALLER = 5
    }

    class Command {

        private string sentence = ""; // is the complete command
        private CommandType type; // the type defines the order of execution

        public string Sentence => sentence;
        public CommandType Type => type;

        public Command(string sentence, CommandType type) {
            this.sentence = sentence;
            this.type = type;
        }

        public override string ToString() {
            return sentence + "\n";
        }
    }
}
