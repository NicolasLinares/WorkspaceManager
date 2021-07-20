using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp2.Util;

namespace WpfApp2.Model {
    class Deploy : Notifier {

        private List<Command> deployScript;

        public List<Command> FinalScript => deployScript;


        private string commandList;
        public string CommandList {
            get { return commandList; }
            set { SetValue(ref commandList, value, "CommandList"); }
        }

        public Deploy() {
            deployScript = new List<Command>();
        }

        public void AddCommand(Command cmmd) {

            if (deployScript.Any(c => c.Type == cmmd.Type)) {
                RemoveCommand(cmmd.Type);
            }

            deployScript.Add(cmmd);
            CommandList = string.Join("", deployScript);

            Console.WriteLine("ADDED");
            ShowList();
        }

        public void RemoveCommand(CommandType cmmdType) {
            deployScript.RemoveAll(c => c.Type == cmmdType);
            CommandList = string.Join("", deployScript);
            Console.WriteLine("REMOVED");
            ShowList();
        }

        private void ShowList() {
            foreach (Command c in deployScript) {
                Console.Write(c);
            }
        }


    }
}