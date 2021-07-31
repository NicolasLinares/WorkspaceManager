using System.Collections.Generic;

namespace INVOXWorkspaceManager.Models.QuickAccess
{

    public class Folder {

        private string name;
        private string description;
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
        public List<Folder> Folders {
            get { return folders; }
            set { folders = value; }
        }
        public List<QuickAccess> QuickAccess {
            get { return quickAccess; }
            set { quickAccess = value; }
        }

        public Folder(string name, string description) {
            Name = name;
            Description = description;
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
    }
}
