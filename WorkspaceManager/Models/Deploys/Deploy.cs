using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WorkspaceManagerTool.Models.Deploys {
    public class Deploy {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private string description;
        private List<string> commands;

        public string Name {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public List<string> Script {
            get { return commands; }
            set { SetProperty(ref commands, value); }
        }

        public string Description {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        public Deploy(string name, List<string> commands) {
            Name = name;
            Script = commands;
        }

        public void AddCommand(string cmmd) {
            Script.Add(cmmd);
        }

        public void RemoveCommand(string cmmd) {
            Script.Remove(cmmd);
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