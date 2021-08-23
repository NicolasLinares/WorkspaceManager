using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkspaceManagerTool.Controllers;
using WorkspaceManagerTool.Models.Deploys;

namespace WorkspaceManagerTool.Views.Scripts {
    /// <summary>
    /// Interaction logic for DeployTabWindow.xaml
    /// </summary>
    public partial class Scripts_Tab : UserControl, INotifyPropertyChanged {

        private ObservableCollection<Deploy> scriptsItems;

        public ObservableCollection<Deploy> ScriptsItems {
            get => scriptsItems;
            set => SetProperty(ref scriptsItems, value);
        }

        private ScriptsController ScriptsController { get; set; }

        public Scripts_Tab() {
            DataContext = this;
            InitializeComponent();

            // Create controller and initialize data
            ScriptsController = ScriptsController.GetInstance();
            ScriptsController.Init();
            // Set observable data from controller
            ScriptsItems = ScriptsController.ScriptItems;
            _ScriptsListBox.UnselectAll();
        }

        #region Property Changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null) {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        private void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Actions
        #endregion

        #region GUI methods
        #endregion

        #region Auxiliar methods

        #endregion
    }
}
