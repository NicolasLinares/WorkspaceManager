using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WorkspaceManagerTool.Models
{
    public abstract class GroupableResource : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;


        private string name;
        private string description;
        private Group group;

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

        public void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null) {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        public void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
