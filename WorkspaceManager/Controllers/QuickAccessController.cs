

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

        private string QuickAccessDirectory {
            get {
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string directory = @"WorkspaceManagerTool\quickaccess.json";
                return Path.Combine(AppData, directory);
            }
        }


        public void Init() {
            ReadData();
        }

        public void ReadData() {
            var data = JSONManager.ReadJSON<IList<FolderQuickAccess>>(QuickAccessDirectory);

            if (data == null) {
                QAItems = new ObservableCollection<FolderQuickAccess>();
            }

            QAItems = new ObservableCollection<FolderQuickAccess>(data);
        }

        public void WriteData() {
            JSONManager.SaveJSON(QuickAccessDirectory, QAItems);
        }

        public void AddQA(FolderQuickAccess qa) {
            qaItems.Add(qa);
            WriteData();
        }
        public void ReplaceQA(FolderQuickAccess old_qa, FolderQuickAccess new_qa) {
            qaItems.Remove(old_qa);
            qaItems.Add(new_qa);
            WriteData();
        }

        public void RemoveQA(FolderQuickAccess qa) {
            qaItems.Remove(qa);
            WriteData();
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
