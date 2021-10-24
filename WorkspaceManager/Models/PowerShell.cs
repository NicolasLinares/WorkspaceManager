using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace WorkspaceManagerTool.Models {

    class PowerShell {

        public string ADMIN_PARAM => " -ExecutionPolicy Unrestricted ";
        public string NO_EXIT_PARAM => " -NoExit ";
        public string FILE_PARAM => " -File ";

        private static string EXECUTABLE = "powershell.exe";

        public void Run(Script script) {

            var options = script.Options;

            ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = options.ExecOption != ExecutionOption.BackgroundExecution;
            processInfo.Verb = "runas";

            var args = ADMIN_PARAM;
            if (options.ExecOption == ExecutionOption.KeepOpenAfterFinish) {
                args += NO_EXIT_PARAM;
            }
            processInfo.Arguments = args + script.Commands;
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
