

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
using WorkspaceManagerTool.Interfaces;
using System.Linq;

namespace WorkspaceManagerTool.Controllers {
    class QuickAccessController : IController {

        private string QuickAccessDirectory {
            get {
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string directory = @"WorkspaceManagerTool\quickaccess.json";
                return Path.Combine(AppData, directory);
            }
        }

        public ObservableCollection<FolderQuickAccess> QAItems { get; set; }
        public ObservableCollection<Group> GroupItems { get; set; }


        public void Init() {
            ReadData();
            UpdateGroupList();
        }

        public void ReadData() {
            var data = JSONManager.ReadJSON<IList<FolderQuickAccess>>(QuickAccessDirectory);

            if (data == null) {
                QAItems = new ObservableCollection<FolderQuickAccess>();
            }

            QAItems = OrderFolders(data);
        }

        public void WriteData() {
            JSONManager.SaveJSON(QuickAccessDirectory, QAItems);
        }

        public void SaveChanges(ObservableCollection<FolderQuickAccess> qa_list) {
            QAItems = qa_list;
            WriteData();
        }

        public void UpdateGroupList() {
            var gr_list = QAItems.Select(qa => qa.Group).Distinct();
            GroupItems = OrderGroups(gr_list);
        }
        private ObservableCollection<FolderQuickAccess> OrderFolders(IEnumerable<FolderQuickAccess> qa_list) {
            return (ObservableCollection<FolderQuickAccess>) qa_list.OrderBy(qa => qa.Group.Name).ThenBy(qa => qa.Name);
        }
        private ObservableCollection<Group> OrderGroups(IEnumerable<Group> gr_list) {
            return (ObservableCollection<Group>)gr_list.OrderBy(gr => gr.Name);
        }

        public void OpenQuickAccess(FolderQuickAccess qa) {
            Process process = Process.Start(qa.Path);
        }
    }
}
