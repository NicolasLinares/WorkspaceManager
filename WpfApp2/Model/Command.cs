

using System;
using System.Collections.Generic;

namespace WpfApp2.Model {

    [Flags]
    public enum CommandType {
        REVERT = 0,
        CLEAN = 1,
        BRANCH = 2,
        BUILD_DEBUG = 3,
        BUILD_RELEASE = 4,
        INSTALLER = 5
    }

    class Command {

        private string sentence = ""; // comando completo
        private CommandType type; // usado para mantener un orden

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
