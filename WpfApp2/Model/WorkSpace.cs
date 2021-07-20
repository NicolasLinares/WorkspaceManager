using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WpfApp2.Model
{
    class WorkSpace {

        private PowerShell ps; // powershell que contiene los métodos para ejecutar los scripts
        private SetDevEnvScript setDevEnv; // representa el script setDevEnv.ps1
        private BuildScript build; // representa el script build.ps1

        private Deploy deploy; // lista de comandos que se va a ejecutar 

        public WorkSpace() {
            ps = new PowerShell();
            setDevEnv = SetDevEnvScript.GetInstance();
            build = BuildScript.GetInstance();
            deploy = new Deploy();

            // Añade el comando para revertir el workspace
            Command revert = new Command(setDevEnv.REVERT_WORKSPACE, CommandType.REVERT);
            deploy.AddCommand(revert);
        }

        public string GetCurrentBranchInformation() {

            string sentence = setDevEnv.FILENAME + " " + setDevEnv.INFO_PARAM;
            Command getBranchInfoCommand = new Command(sentence, CommandType.BRANCH);

            List<string> output = ps.RunCommandAndSaveOutput(getBranchInfoCommand);
            return FindCurrentBranch(output);

            string FindCurrentBranch(List<string> outputLines) {
                foreach(var line in outputLines) {
                    if (line != "") {
                        Match match = Regex.Match(line, @"Current branch: [\w\/\-]+");
                        if (match.Success) {
                            return line;
                        }
                    }
                }

                return "branch not found";
            }
        }

        public bool CheckScriptsNeeded(string selectedPath) {

            if (!File.Exists(Path.Combine(selectedPath, setDevEnv.FILENAME))) {
                return false;
            }

            if (!File.Exists(Path.Combine(selectedPath, build.FILENAME))) {
                return false;
            }

            //... check all files needed
            return true;
        }

        public void SetBranchCommand(string branchName) {

            string sentence = setDevEnv.FILENAME + " " + setDevEnv.BRANCH_PARAM + " " + branchName;
            Command cmmd = new Command(sentence, CommandType.BRANCH);
            deploy.AddCommand(cmmd);
        }

        public void SetCleanCommand(CleanOptions type) {
            string cleanParam;

            switch (type) {
                case CleanOptions.CLEAN:
                    cleanParam = setDevEnv.CLEAN_PARAM;
                    break;
                case CleanOptions.FULLCLEAN:
                    cleanParam = setDevEnv.FULLCLEAN_PARAM;
                    break;
                default:
                    cleanParam = null;
                    break;
            }

            if (cleanParam == null) {
                deploy.RemoveCommand(CommandType.CLEAN);
                return;
            }

            string sentence = setDevEnv.FILENAME + " " + cleanParam;
            Command cmmd = new Command(sentence, CommandType.CLEAN);
            deploy.AddCommand(cmmd);
        }

        public void SetBuildCommand(BuildOptions type) {
            string buildParam;

            switch (type) {
                case BuildOptions.NOTJS:
                    buildParam = build.NOTJS_PARAM;
                    break;
                case BuildOptions.NOTDOC:
                    buildParam = build.NOTDOC_PARAM;
                    break;
                case BuildOptions.ONLYDEV:
                    buildParam = build.ONLYDEV_PARAM;
                    break;
                default:
                    buildParam = null;
                    break;
            }

            if (buildParam == null) {
                deploy.RemoveCommand(CommandType.BUILD_DEBUG);
                return;
            }

            string sentence = build.FILENAME + " " + buildParam;
            Command cmmd = new Command(sentence, CommandType.BUILD_DEBUG);
            deploy.AddCommand(cmmd);
        }

        public void RunScripts() {
            ps.Run(deploy.FinalScript);
        }
    }
}
