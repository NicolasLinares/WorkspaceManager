using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace WorkspaceManagerTool.Models {

    class PowerShell {

        public string ADMIN_PARAM => "-ExecutionPolicy Unrestricted";
        public string NO_EXIT_PARAM => "-NoExit";
        public string FILE_PARAM => "-File";

        private static string EXECUTABLE = "powershell.exe";

        public PowerShell() {
        }

        public void Run(Script script) {
            ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";
            processInfo.Arguments = ADMIN_PARAM + " " + NO_EXIT_PARAM + " " + script.Commands;
            try {
                Process process = Process.Start(processInfo);
                process.WaitForExit();
                int errorLevel = process.ExitCode;
                process.Close();
            } catch(Win32Exception) {
                return;
            }
        }
    }
}
