using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace INVOXWorkspaceManager.Model {

    class Params {
        public string ADMIN_PARAM => "-ExecutionPolicy Unrestricted";
        public string NO_EXIT_PARAM => "-NoExit";
        public string FILE_PARAM => "-File";
    }

    class PowerShell {

        private static string EXECUTABLE = "powershell.exe";
        private Params options;

        private List<string> output { get; set; }

        public PowerShell() {
            options = new Params();
            output = new List<string>();
        }

        public int RunCommand(Command cmmd, bool showPSWindow) {

            ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
            processInfo.Arguments = string.Format("{0} {1} {2} {3}", 
                options.ADMIN_PARAM, 
                options.NO_EXIT_PARAM, 
                options.FILE_PARAM,
                cmmd.Sentence);
            processInfo.CreateNoWindow = showPSWindow;
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";

            Process process = Process.Start(processInfo);

            if (showPSWindow) {
                process.WaitForExit();
            }

            int errorLevel = process.ExitCode;
            process.Close();

            return errorLevel;
        }

        public List<string> RunCommandAndSaveOutput(Command cmmd) {

            output = new List<string>();

            ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
            processInfo.Arguments = string.Format("{0} {1} {2}", 
                options.ADMIN_PARAM, 
                options.FILE_PARAM, 
                cmmd.Sentence);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.Verb = "runas";

            Process process = new Process();
            process.StartInfo = processInfo;
            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            return output;
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            if (string.IsNullOrEmpty(outLine.Data) || string.IsNullOrWhiteSpace(outLine.Data)) {
                return;
            }

            output.Add(outLine.Data);
        }

        public void Run(List<Command> finalScript) {

            ProcessStartInfo processInfo = new ProcessStartInfo(EXECUTABLE);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = true;
            processInfo.Verb = "runas";
        
            string args = "";

            foreach (Command c in finalScript) {
                if (c.Sentence == "git reset --hard") {
                    args = options.ADMIN_PARAM + " " + options.NO_EXIT_PARAM + " " + c.Sentence;
                } else {
                    args += " ; " + c.Sentence;
                }
            }

            MessageBox.Show(args);

            processInfo.Arguments = args;
            Process process = Process.Start(processInfo);

            process.WaitForExit();
            int errorLevel = process.ExitCode;
            process.Close();

        }
    }
}
