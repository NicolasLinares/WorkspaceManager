using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WorkspaceManagerTool.Models.Scripts {
    public class Script : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private string description;
        private string commands;

        public string Name {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string Commands {
            get { return commands; }
            set { SetProperty(ref commands, value); }
        }

        public string Description {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        public Script(string name, string description, string commands) {
            Name = name;
            Description = description;
            Commands = commands;
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