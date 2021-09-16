

using WorkspaceManagerTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Linq;
using WorkspaceManagerTool.Models;

namespace WorkspaceManagerTool.Controllers {
    class QuickAccessController : LocalDataController {

        protected override string ResourceFile {
            get {
                string file = "quickaccess.json";
                return Path.Combine(ResourceDirectory, file);
            }
        }

        public ObservableCollection<GroupableResource> OrderedItems => OrderByType(Items);

        private static QuickAccessController _instance;

        public static QuickAccessController GetInstance() {
            if (_instance == null) {
                _instance = new QuickAccessController();
            }
            return _instance;
        }

        public override void Init() {
            Items = ReadData<QuickAccess>(ResourceFile);
        }

        public void OpenQuickAccess(GroupableResource qa) {
            Process process = Process.Start((qa as QuickAccess).Path);
        }

        private ObservableCollection<GroupableResource> OrderByType(ObservableCollection<GroupableResource> itms) {
            return new ObservableCollection<GroupableResource>(itms.OrderBy(it => (it as QuickAccess).Type));
        }
    }
}
