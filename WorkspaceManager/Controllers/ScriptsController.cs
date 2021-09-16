using System;
using System.IO;
using WorkspaceManagerTool.Exceptions;
using System.Collections.ObjectModel;
using WorkspaceManagerTool.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WorkspaceManagerTool.Models;
using WorkspaceManagerTool.Events;

namespace WorkspaceManagerTool.Controllers
{
    class ScriptsController : LocalDataController {

        private static ScriptsController _instance;

        public static ScriptsController GetInstance() {
            if (_instance == null) {
                _instance = new ScriptsController();
            }
            return _instance;
        }

        protected override string ResourceFile {
            get {
                string file = "scripts.json";
                return Path.Combine(ResourceDirectory, file);
            }
        }

        public override void Init() {
            Items = ReadData<Script>(ResourceFile);
        }

        public void RunScript(GroupableResource script) {

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                PowerShell ps = new PowerShell();
                ps.Run(script as Script);
            }).Start();
        }


        public void DoExecution(object sender, ExecutionEvent e) {
            RunScript(e.Script);
        }

        public void DuplicateScript(Script script) {
            string name = String.Format("{0} - Copy", script.Name);
            string finalName = name;
            int counter = 1;
            while(ExistsName(finalName)) {
                finalName = String.Format("{0} ({1})", name, counter++);
            }

            Script duplicated = new Script(finalName, script.Description, script.Commands, script.Group);
            Add(duplicated);

            bool ExistsName(string itemName) {
                return Items.Any(it => it.Name.Equals(itemName));
            }
        }
    }
}
