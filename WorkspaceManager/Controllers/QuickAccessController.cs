

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

namespace WorkspaceManagerTool.Controllers
{
    class QuickAccessController
    {

        private string QuickAccessDirectory {
            get {
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string directory = @"WorkspaceManagerTool\quickaccess.json";
                return Path.Combine(AppData, directory);
            }
        }

        public void Open(FolderQuickAccess qa) {
            Process process = Process.Start(qa.Path);
        }

        public void SaveChanges(IList<FolderQuickAccess> list) {
            JSONManager.SaveJSON(QuickAccessDirectory,  list);
        }

        public IList<FolderQuickAccess> ReadLocalList() {
            try {
                return JSONManager.ReadJSON<IList<FolderQuickAccess>>(QuickAccessDirectory);
            } catch(DirectoryNotFoundException ex) {
                return null;
            }
        }

    }
}
