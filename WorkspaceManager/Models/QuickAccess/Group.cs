using System.Windows.Media;

namespace WorkspaceManagerTool.Models.QuickAccess {
    public class Group {

        private string name;
        private SolidColorBrush color;

        public string Name {
            get { return name; }
            set { name = value; }
        }
        public SolidColorBrush Color {
            get { return color; }
            set { color = value; }
        }

        public Group(string name, SolidColorBrush color) {
            Name = name;
            Color = color;
        }

        public override bool Equals(object obj) {
            Group gr = obj as Group;
            return gr != null
                && (Name ?? "").Equals(gr.Name);
        }

        public override int GetHashCode() {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}