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
using WorkspaceManagerTool.Models.QuickAccess;

namespace WorkspaceManagerTool.Controllers
{
    class ScriptsController : LocalDataController {

        private ObservableCollection<Script> scriptItems;
        private ObservableCollection<Group> grItems;

        public ObservableCollection<Script> ScriptItems {
            get {
                return OrderScripts(scriptItems);
            }
            set {
                scriptItems = value;
            }
        }
        public ObservableCollection<Group> GroupItems {
            get {
                var gr_list = scriptItems.Select(qa => qa.Group).Distinct();
                return OrderGroups(gr_list);
            }
        }


        private static ScriptsController _instance;

        public static ScriptsController GetInstance() {
            if (_instance == null) {
                _instance = new ScriptsController();
            }
            return _instance;
        }

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

        private ObservableCollection<Group> OrderGroups(IEnumerable<Group> gr_list) {
            return new ObservableCollection<Group>(gr_list.OrderBy(gr => gr.Name));
        }

        private ObservableCollection<Script> OrderScripts(ObservableCollection<Script> scriptsItems) {
            return new ObservableCollection<Script>(scriptsItems.OrderBy(sc => sc.Group.Name).ThenBy(qa => qa.Name));
        }

        public void ExecuteScript(Script selectedScriptItem) {
            Console.Write(selectedScriptItem.Commands);
        }
    }
}
