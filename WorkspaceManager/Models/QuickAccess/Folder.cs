using System.Collections.Generic;
using System.Windows.Media;


namespace INVOXWorkspaceManager.Models.QuickAccess
{

    public class Folder {

        private string name;
        private string description;
        private Color color;

        private List<Folder> folders;
        private List<QuickAccess> quickAccess;

        public string Name {
            get { return name; }
            set { name = value; }
        }
        public string Description {
            get { return description; }
            set { description = value; }
        }
        public Color Color {
            get { return color; }
            set { color = value; }
        }

        public List<Folder> Folders {
            get { return folders; }
            set { folders = value; }
        }
        public List<QuickAccess> QuickAccess {
            get { return quickAccess; }
            set { quickAccess = value; }
        }

        public Folder(string name, string description, Color color) {
            Name = name;
            Description = description;
            Color = color;
            Folders = new List<Folder>();
            QuickAccess = new List<QuickAccess>();
        }

        #region quick access methods

        public void AddQuickAccess(QuickAccess qa) {
            QuickAccess.Add(qa);
        }

        public void AddQuickAccess(List<QuickAccess> qas) {
            QuickAccess = qas;
        }

        public void RemoveQuickAccess(QuickAccess qa) {
            QuickAccess.Remove(qa);
        }

        #endregion

        #region folder methods

        public void AddSubFolders(Folder f) {
            Folders.Add(f);
        }

        public void AddSubFolders(List<Folder> f) {
            Folders = f;
        }

        public void RemoveSubFolders(Folder f) {
            Folders.Remove(f);
        }

        #endregion


        public override bool Equals(object obj) {
            return obj is Folder f
                && (Name ?? "").Equals(f.Name);
        }

        public override int GetHashCode() {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
