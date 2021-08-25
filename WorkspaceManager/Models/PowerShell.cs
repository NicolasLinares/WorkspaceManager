using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace WorkspaceManagerTool.Models.Scripts {

    class PowerShell {

        public string ADMIN_PARAM => "-ExecutionPolicy Unrestricted";
        public string NO_EXIT_PARAM => "-NoExit";
        public string FILE_PARAM => "-File";

        private static string EXECUTABLE = "powershell.exe";

        private List<string> output { get; set; }

        public PowerShell() {
            output = new List<string>();
        }

        public void Run(Script script) {
            ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";
            processInfo.Arguments = ADMIN_PARAM + " " + NO_EXIT_PARAM + " " + script.Commands;
            Process process = Process.Start(processInfo);
            process.WaitForExit();
            int errorLevel = process.ExitCode;
            process.Close();
        }

        //public int RunCommand(string cmmd, bool showPSWindow) {
        //    //TODO
        //    ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
        //    processInfo.Arguments = string.Format("{0} {1} {2} {3}", 
        //        options.ADMIN_PARAM, 
        //        options.NO_EXIT_PARAM, 
        //        options.FILE_PARAM,
        //        cmmd);
        //    processInfo.CreateNoWindow = showPSWindow;
        //    processInfo.UseShellExecute = true;
        //    processInfo.Verb = "runas";

        //    Process process = Process.Start(processInfo);

        //    if (showPSWindow) {
        //        process.WaitForExit();
        //    }

        //    int errorLevel = process.ExitCode;
        //    process.Close();

        //    return errorLevel;
        //}

        //public List<string> RunCommandAndSaveOutput(string cmmd) {
        //    // TODO
        //    output = new List<string>();

        //    ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
        //    processInfo.Arguments = string.Format("{0} {1} {2}", 
        //        options.ADMIN_PARAM, 
        //        options.FILE_PARAM, 
        //        cmmd);
        //    processInfo.CreateNoWindow = true;
        //    processInfo.UseShellExecute = false;
        //    processInfo.RedirectStandardOutput = true;
        //    processInfo.Verb = "runas";

        //    Process process = new Process();
        //    process.StartInfo = processInfo;
        //    process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);

        //    process.Start();
        //    process.BeginOutputReadLine();
        //    process.WaitForExit();

        //    return output;
        //}

        //private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine) {
        //    if (string.IsNullOrEmpty(outLine.Data) || string.IsNullOrWhiteSpace(outLine.Data)) {
        //        return;
        //    }

        //    output.Add(outLine.Data);
        //}

    }
}
