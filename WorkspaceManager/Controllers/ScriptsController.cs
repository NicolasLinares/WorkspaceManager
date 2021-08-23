using System;
using System.IO;
using WorkspaceManagerTool.Models.Deploys;
using WorkspaceManagerTool.Exceptions;
using WorkspaceManagerTool.Models.Deploys.Scripts;
using WorkspaceManagerTool.Interfaces;
using System.Collections.ObjectModel;
using WorkspaceManagerTool.Utils;
using System.Collections.Generic;

namespace WorkspaceManagerTool.Controllers
{
    class ScriptsController : LocalDataController {

        private ObservableCollection<Deploy> scriptItems;

        public ObservableCollection<Deploy> ScriptItems {
            get {
                return OrderScripts(scriptItems);
            }
            set {
                scriptItems = value;
            }
        }



        private static ScriptsController _instance;

        private ScriptsController() {
            //DeploymentHistory = History.GetInstance();
            //workspace = new WorkSpace();
        }

        public static ScriptsController GetInstance() {
            if (_instance == null) {
                _instance = new ScriptsController();
            }
            return _instance;
        }


        //public void SetCurrentWorkspacePath(string selectedPath) {

        //    if (!Directory.Exists(selectedPath)) {
        //        throw new NullReferenceException();
        //    }

        //    if (!workspace.CheckScriptsNeeded(selectedPath)) {
        //        throw new NotScriptsInWorkspaceException("Scripts not found");
        //    }

        //    workspace.Initialize(selectedPath);
        //}

        //public string GetCurrentWorkspacePath() {
        //    return workspace.CurrentBranchInformation;
        //}

        //public void SetNewBranch(string branch) {
        //    workspace.SetBranchCommand(branch);
        //}

        //public void SetCleanOption(CleanOptions type) {
        //    workspace.SetCleanCommand(type);
        //}

        //public void SetBuildOption(BuildOptions type) {
        //    workspace.SetBuildCommand(type);
        //}

        //public string GetSummary() {
        //    return workspace.GetCommandList();
        //}

        protected override string ResourceFile {
            get {
                string file = "scripts.json";
                return Path.Combine(ResourceDirectory, file);
            }
        }

        public override void Init() {
            ReadData();
        }

        protected override void ReadData() {
            var data = JSONManager.ReadJSON<IList<Deploy>>(ResourceFile);

            if (data == null) {
                ScriptItems = new ObservableCollection<Deploy>();
            }

            ScriptItems = new ObservableCollection<Deploy>(data);
        }

        protected override void WriteData() {
            JSONManager.SaveJSON(ResourceFile, ScriptItems);
        }

        public override void Add<T>(T qa) {
            scriptItems.Add((Deploy)(object)qa);
            WriteData();
        }
        public override void Replace<T>(T old_qa, T new_qa) {
            scriptItems.Remove((Deploy)(object)old_qa);
            scriptItems.Add((Deploy)(object)new_qa);
            WriteData();
        }

        public override void Remove<T>(T qa) {
            scriptItems.Remove((Deploy)(object)qa);
            WriteData();
        }

        private ObservableCollection<Deploy> OrderScripts(ObservableCollection<Deploy> scriptsItems) {
            //TODO
            return scriptsItems;
        }
    }
}
