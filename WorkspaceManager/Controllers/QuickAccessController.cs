

using WorkspaceManagerTool.Models.QuickAccess;
using WorkspaceManagerTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

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

        public void Open(QuickAccess qa) {
            Process process = Process.Start(qa.Path);
        }

        public void SaveChanges(IList<QuickAccess> list) {
            JSONManager.SaveJSON(QuickAccessDirectory,  list);
        }

        public IList<QuickAccess> ReadLocalList() {
            try {
                return JSONManager.ReadJSON<IList<QuickAccess>>(QuickAccessDirectory);
            } catch(DirectoryNotFoundException ex) {
                return null;
            }
        }

    }
}
