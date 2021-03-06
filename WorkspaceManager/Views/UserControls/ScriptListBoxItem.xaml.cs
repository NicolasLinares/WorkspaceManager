using System;
using System.Collections.Generic;
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

namespace WorkspaceManagerTool.Views.UserControls {
    /// <summary>
    /// Interaction logic for ScriptListBoxItem.xaml
    /// </summary>
    public partial class ScriptListBoxItem : UserControl {


        public ScriptListBoxItem() {
            InitializeComponent();
        }

        private void Execute_Action(object sender, EventArgs e) {
            var controller = ScriptsController.GetInstance();
            controller.RunScript(this.DataContext as Script);
        }

        private void Remove_Action(object sender, EventArgs e) {
            var controller = ScriptsController.GetInstance();
            controller.Remove(this.DataContext as Script);
            controller.UpdateChangesInView();
        }

        private void Pin_Action(object sender, EventArgs e) {
            var controller = ScriptsController.GetInstance();
            controller.Pin(this.DataContext as Script);
            controller.UpdateChangesInView();
        }

    }
}
