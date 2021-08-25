using System;
using System.IO;
using WorkspaceManagerTool.Models.Scripts;
using WorkspaceManagerTool.Exceptions;
using WorkspaceManagerTool.Models.Scripts;
using WorkspaceManagerTool.Interfaces;
using System.Collections.ObjectModel;
using WorkspaceManagerTool.Utils;
using System.Collections.Generic;
using System.Linq;

namespace WorkspaceManagerTool.Controllers
{
    class ScriptsController : LocalDataController {

        private ObservableCollection<Script> scriptItems;

        public ObservableCollection<Script> ScriptItems {
            get {
                return OrderScripts(scriptItems);
            }
            set {
                scriptItems = value;
            }
        }


        private static ScriptsController _instance;

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
            var data = JSONManager.ReadJSON<IList<Script>>(ResourceFile);

            if (data == null) {
                ScriptItems = new ObservableCollection<Script>();
                return;
            }

            ScriptItems = new ObservableCollection<Script>(data);
        }

        protected override void WriteData() {
            JSONManager.SaveJSON(ResourceFile, ScriptItems);
        }

        public override void Add<T>(T sc) {
            scriptItems.Add((Script)(object)sc);
            WriteData();
        }
        public override void Replace<T>(T old_sc, T new_sc) {
            scriptItems.Remove((Script)(object)old_sc);
            scriptItems.Add((Script)(object)new_sc);
            WriteData();
        }

        public override void Remove<T>(T sc) {
            scriptItems.Remove((Script)(object)sc);
            WriteData();
        }


        public ObservableCollection<Script> SearchByName(string text) {
            return new ObservableCollection<Script>(ScriptItems.Where(sc => sc.Name.ToLower().Contains(text.ToLower())));
        }

        private ObservableCollection<Script> OrderScripts(ObservableCollection<Script> scriptsItems) {
            //TODO
            return scriptsItems;
        }
    }
}
