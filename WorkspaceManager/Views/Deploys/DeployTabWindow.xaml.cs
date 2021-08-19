using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace WorkspaceManagerTool.Views.Deploys {
    /// <summary>
    /// Interaction logic for DeployTabWindow.xaml
    /// </summary>
    public partial class DeployTabWindow : UserControl {


        
        public ObservableCollection<string> DeploysList { get; set; }
        public DeployTabWindow() {

            DeploysList = new ObservableCollection<string>();


            InitializeComponent();

        }


    }
}
