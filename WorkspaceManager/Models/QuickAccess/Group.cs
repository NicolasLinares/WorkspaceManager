using System.Collections.Generic;
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
            SolidColorBrushComparer colorComparer = new SolidColorBrushComparer();
            return gr != null
                && (Name ?? "").Equals(gr.Name)
                && colorComparer.Equals(Color, gr.Color);
        }

        public override int GetHashCode() {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public class SolidColorBrushComparer : IEqualityComparer<SolidColorBrush> {
            public bool Equals(SolidColorBrush x, SolidColorBrush y) {
                return x.Color == y.Color &&
                    x.Opacity == y.Opacity;
            }

            public int GetHashCode(SolidColorBrush obj) {
                return new { C = obj.Color, O = obj.Opacity }.GetHashCode();
            }
        }
    }
}