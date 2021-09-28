

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkspaceManagerTool.Models
{

    public enum QuickAccessType {
        DIRECTORY,
        FILE
    }

    public class QuickAccess: GroupableResource {

        private QuickAccessType type;
        private string path;

        public string Path {
            get { return path; }
            set { SetProperty(ref path, value); }
        }

        public QuickAccessType Type {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        public QuickAccess(string path, string name, string description, Group group, QuickAccessType type, bool pinned = false) {
            Name = name;
            Path = path;
            Description = description;
            Group = group;
            Type = type;
            Pinned = pinned;
        }

        public override bool Equals(object obj) {
            QuickAccess qa = obj as QuickAccess;
            return qa != null
                && (Path ?? "").Equals(qa.Path)
                && (Name ?? "").Equals(qa.Name)
                && (Description ?? "").Equals(qa.Description)
                && Group.Equals(qa.Group);
        }

        public override int GetHashCode() {
            return (Path != null ? Path.GetHashCode() : 0);
        }
    }
}
