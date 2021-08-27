using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkspaceManagerTool.Models;

namespace WorkspaceManagerTool.Events {
    public class ScriptEvent : EventArgs {
        public GroupableResource OldScript { get; set; }
        public GroupableResource NewScript { get; set; }

    }
}
