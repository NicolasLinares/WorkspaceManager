using System;
using System.IO;
using WorkspaceManagerTool.Models.Deploys;
using WorkspaceManagerTool.Exceptions;
using WorkspaceManagerTool.Models.Deploys.Scripts;

namespace WorkspaceManagerTool.Controllers
{
    class DeploymentsController
    {

        private static DeploymentsController _instance;

        private DeploymentsController() {
            DeploymentHistory = History.GetInstance();
            workspace = new WorkSpace();
        }

        public static DeploymentsController GetInstance() {
            if (_instance == null) {
                _instance = new DeploymentsController();
            }
            return _instance;
        }


        private History DeploymentHistory { get; }
        private WorkSpace workspace;

        public void CreateEnvironment() {
            workspace.RunScripts();
        }

        public void SetCurrentWorkspacePath(string selectedPath) {

            if (!Directory.Exists(selectedPath)) {
                throw new NullReferenceException();
            }

            if (!workspace.CheckScriptsNeeded(selectedPath)) {
                throw new NotScriptsInWorkspaceException("Scripts not found");
            }

            workspace.Initialize(selectedPath);
        }

        public string GetCurrentWorkspacePath() {
            return workspace.CurrentBranchInformation;
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
