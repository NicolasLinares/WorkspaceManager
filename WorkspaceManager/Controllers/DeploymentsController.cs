using System;
using System.IO;
using System.Diagnostics;
using INVOXWorkspaceManager.Model;
using INVOXWorkspaceManager.Exceptions;
using INVOXWorkspaceManager.Models.Scripts;

namespace INVOXWorkspaceManager.Controllers
{
    class DeploymentsController
    {

        private static DeploymentsController _instance;

        private DeploymentsController() {
            deploymentHistory = History.GetInstance();
            workspace = new WorkSpace();
        }

        public static DeploymentsController GetInstance() {
            if (_instance == null) {
                _instance = new DeploymentsController();
            }
            return _instance;
        }


        private History deploymentHistory { get; }
        private WorkSpace workspace;

        public void OpenFolder() {
            Process process = Process.Start(@"C:\REPO\invox");


            /*
            
            // opens the folder in explorer
            Process.Start(@"c:\temp");
            // opens the folder in explorer
            Process.Start("explorer.exe", @"c:\temp");
            // throws exception
            Process.Start(@"c:\does_not_exist");
            // opens explorer, showing some other folder)
            Process.Start("explorer.exe", @"c:\does_not_exist");

             */


        }

        public void CreateEnvironment() {
            workspace.RunScripts();
        }

        public void SetCurrentWorkspace(string selectedPath) {

            if (!Directory.Exists(selectedPath)) {
                throw new NullReferenceException();
            }

            if (!workspace.CheckScriptsNeeded(selectedPath)) {
                throw new NotScriptsInWorkspaceException("Scripts not found");
            }

            Directory.SetCurrentDirectory(selectedPath);
        }

        public string GetCurrentWorkspace() {
            return workspace.GetCurrentBranchInformation();
        }

        public void SetNewBranch(string branch) {
            workspace.SetBranchCommand(branch);
        }

        public void SetCleanOption(CleanOptions type) {
            workspace.SetCleanCommand(type);
        }

        public void SetBuildOption(BuildOptions type) {
            workspace.SetBuildCommand(type);
        }

        public string GetSummary() {
            return workspace.GetCommandList();
        }
    }
}
