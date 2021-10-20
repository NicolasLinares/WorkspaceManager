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
    /// Interaction logic for QuickAccessListBoxItem.xaml
    /// </summary>
    public partial class QuickAccessListBoxItem : UserControl {

        public QuickAccessListBoxItem() {
            InitializeComponent();
        }

        private void Pin_Action(object sender, EventArgs e) {
            var controller = QuickAccessController.GetInstance();
            controller.Pin(this.DataContext as QuickAccess);
            controller.UpdateChangesInView();
        }

    }
}
