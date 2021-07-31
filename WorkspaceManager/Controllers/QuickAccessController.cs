

using INVOXWorkspaceManager.Models.QuickAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace INVOXWorkspaceManager.Controllers
{
    class QuickAccessController
    {
        public void Open(QuickAccess qa) {
            Process process = Process.Start(qa.Path);
        }
    }
}
