using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WorkspaceManagerTool.Controllers;
using WorkspaceManagerTool.Models;

namespace WorkspaceManagerTool.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Config_Tab : UserControl, INotifyPropertyChanged {
        public Config_Tab()
        {
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        #region QuickAccess

        public void Import_QuickAccessAction(object sender, EventArgs e) {
            var controller = QuickAccessController.GetInstance();
            controller.Import<QuickAccess>();
        }
        public void Export_QuickAccessAction(object sender, EventArgs e) {
            var controller = QuickAccessController.GetInstance();
            controller.Export("quickaccess.json");
        }
        public void ImportNewItems_QuickAccessAction(object sender, EventArgs e) {
            var controller = QuickAccessController.GetInstance();
            controller.ImportNewItems<QuickAccess>();
        }

        #endregion

        #region Scripts

        public void Import_ScriptsAction(object sender, EventArgs e) {
            var controller = ScriptsController.GetInstance();
            controller.Import<Script>();
        }
        public void Export_ScriptsAction(object sender, EventArgs e) {
            var controller = ScriptsController.GetInstance();
            controller.Export("scripts.json");
        }
        public void ImportNewItems_ScriptsAction(object sender, EventArgs e) {
            var controller = ScriptsController.GetInstance();
            controller.ImportNewItems<Script>();
        }

        #endregion

    }
}
