

using WorkspaceManagerTool.Models.QuickAccess;
using WorkspaceManagerTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using FolderQuickAccess = WorkspaceManagerTool.Models.QuickAccess.QuickAccess;
using System.Collections.ObjectModel;
using System.Linq;

namespace WorkspaceManagerTool.Controllers {
    class QuickAccessController : LocalDataController {

        private ObservableCollection<FolderQuickAccess> qaItems;
        private ObservableCollection<Group> grItems;

        public ObservableCollection<FolderQuickAccess> QAItems {
            get {
                return OrderFolders(qaItems);
            }
            set {
                qaItems = value;
            }
        }
        public ObservableCollection<Group> GroupItems {
            get {
                var gr_list = qaItems.Select(qa => qa.Group).Distinct();
                return OrderGroups(gr_list);
            }
        }

        protected override string ResourceFile {
            get {
                string file = "quickaccess.json";
                return Path.Combine(ResourceDirectory, file);
            }
        }


        private static QuickAccessController _instance;

        public static QuickAccessController GetInstance() {
            if (_instance == null) {
                _instance = new QuickAccessController();
            }
            return _instance;
        }


        public override void Init() {
            ReadData();
        }

        protected override void ReadData() {
            var data = JSONManager.ReadJSON<IList<FolderQuickAccess>>(ResourceFile);

            if (data == null) {
                QAItems = new ObservableCollection<FolderQuickAccess>();
                return;
            }

            QAItems = new ObservableCollection<FolderQuickAccess>(data);
        }

        protected override void WriteData() {
            JSONManager.SaveJSON(ResourceFile, QAItems);
        }

        public override void Add<T>(T qa) {
            qaItems.Add((FolderQuickAccess)(object) qa);
            WriteData();
        }
        public override void Replace<T>(T old_qa, T new_qa) {
            qaItems.Remove((FolderQuickAccess)(object) old_qa);
            qaItems.Add((FolderQuickAccess)(object) new_qa);
            WriteData();
        }
        public override void Remove<T>(T qa) {
            qaItems.Remove((FolderQuickAccess)(object) qa);
            WriteData();
        }

        public ObservableCollection<FolderQuickAccess> SearchByName(string text) {
            return new ObservableCollection<FolderQuickAccess>(QAItems.Where(qa => qa.Name.ToLower().Contains(text.ToLower())));
        }

        private ObservableCollection<FolderQuickAccess> OrderFolders(IList<FolderQuickAccess> qa_list) {
            return new ObservableCollection<FolderQuickAccess>(qa_list.OrderBy(qa => qa.Group.Name).ThenBy(qa => qa.Name));
        }
        private ObservableCollection<Group> OrderGroups(IEnumerable<Group> gr_list) {
            return new ObservableCollection<Group>(gr_list.OrderBy(gr => gr.Name));
        }

        public void OpenQuickAccess(FolderQuickAccess qa) {
            Process process = Process.Start(qa.Path);
        }

    }
}
