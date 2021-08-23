

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkspaceManagerTool.Models.QuickAccess
{
    public class QuickAccess: INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private string path;
        private string name;
        private string description;
        private Group group;

        public string Path {
            get { return path; }
            set { SetProperty(ref path, value); }
        }
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        public string Description {
            get { return description; }
            set { SetProperty(ref description, value); }
        }
        public Group Group {
            get { return group; }
            set { SetProperty(ref group, value); }
        }

        public QuickAccess(string path, string name, string description, Group group) {
            Name = name;
            Path = path;
            Description = description;
            Group = group;
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

        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null) {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        private void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
