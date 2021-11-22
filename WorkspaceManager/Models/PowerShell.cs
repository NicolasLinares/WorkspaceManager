using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WorkspaceManagerTool.Models {

    class PowerShell {

        private const string ADMIN_PARAM = " -ExecutionPolicy Unrestricted ";
        private const string NO_EXIT_PARAM = " -NoExit ";
        private const string FILE_PARAM = " -File ";
        private const string SCRIPT_TEMP_FILE = "temp.ps1";

        private const string EXECUTABLE = "powershell.exe";

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
            File.WriteAllText(SCRIPT_TEMP_FILE, script.Commands);
            var script_temp_file = Path.Combine(Directory.GetCurrentDirectory(), SCRIPT_TEMP_FILE);
            processInfo.Arguments = args + script_temp_file;
            try {
                Process process = Process.Start(processInfo);
                process.WaitForExit();
                File.Delete(script_temp_file);
                int errorLevel = process.ExitCode;
                process.Close();
            } catch(Win32Exception) {
                return;
            }
        }
    }
}
