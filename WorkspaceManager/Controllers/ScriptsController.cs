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
            ReadData<Script>(ResourceFile);
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

    }
}
