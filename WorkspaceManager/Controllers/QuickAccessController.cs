

using WorkspaceManagerTool.Models.QuickAccess;
using WorkspaceManagerTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

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

        public void SaveChanges(Folder root) {

            JSONManager.SaveJSON(QuickAccessDirectory,  root);
        }

    }
}
