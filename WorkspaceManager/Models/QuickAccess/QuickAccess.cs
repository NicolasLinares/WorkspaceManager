

namespace INVOXWorkspaceManager.Models.QuickAccess
{
    public class QuickAccess {
        private string path;
        private string name;
        private string description;

        public string Path {
            get { return path; }
            set { path = value; }
        }
        public string Name {
            get { return name; }
            set { name = value; }
        }
        public string Description {
            get { return description; }
            set { description = value; }
        }

        public QuickAccess(string path, string name, string description) {
            Name = name;
            Path = path;
            Description = description;
        }
    }
}
