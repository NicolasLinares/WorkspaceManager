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
using WorkspaceManagerTool.Models.QuickAccess;
using WorkspaceManagerTool.Models.Scripts;

namespace WorkspaceManagerTool.Views.Scripts {
    /// <summary>
    /// Interaction logic for DeployTabWindow.xaml
    /// </summary>
    public partial class Scripts_Tab : UserControl, INotifyPropertyChanged {

        private Script selectedScript;

        private ObservableCollection<Script> scriptsItems;

        public ObservableCollection<Script> ScriptsItems {
            get => scriptsItems;
            set => SetProperty(ref scriptsItems, value);
        }

        public Script SelectedScriptItem {
            get => selectedScript;
            set {
                SetProperty(ref selectedScript, value);
            }
        }
        public Script SelectedScriptToEdit { get; private set; }

        public ViewMode CurrentViewMode { get; private set; }
        public ViewMode PreviousViewMode { get; private set; }

        private ScriptsController ScriptsController { get; set; }

        public Script_CreationPanel ScriptPanel { get; set; }


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
        private void CreateScript_Action(object sender, EventArgs e) {
            // new item, so empty values
            ScriptPanel = new Script_CreationPanel();
            ChangeViewMode(ViewMode.CREATION);
        }

        private void EditScript_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            // set current values to edit
            ScriptPanel = new Script_CreationPanel(SelectedScriptItem);
            ChangeViewMode(ViewMode.EDITION);
        }

        private void RemoveScript_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el script de forma permanente?", "Eliminar script", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                ScriptsController.Remove<Script>(SelectedScriptItem);
                ChangeViewMode(CurrentViewMode);
            }
        }

        private void RemoveSearch_Action(object sender, EventArgs e) {
            _SearchText.Text = string.Empty;
            _SearchRemoveButton.Visibility = Visibility.Hidden;
            ChangeViewMode(ViewMode.CREATION);
        }

        private void SearchByName_Action(object sender, TextChangedEventArgs e) {
            if (_SearchText.Text.Length > 0) {
                _SearchRemoveButton.Visibility = Visibility.Visible;
            } else {
                _SearchRemoveButton.Visibility = Visibility.Hidden;
            }
            ScriptsItems = ScriptsController.SearchByName(_SearchText.Text);
        }

        #endregion

        #region GUI methods

        private void ChangeViewMode(ViewMode mode) {
            switch (mode) {
                case ViewMode.CREATION:
                    OpenCreationPanel();
                    break;
                case ViewMode.EDITION:
                    SelectedScriptToEdit = SelectedScriptItem;
                    OpenCreationPanel();
                    break;
                case (ViewMode.NORMAL):
                    UpdateLists();
                    CloseCreationPanel();
                    break;
            }

            PreviousViewMode = CurrentViewMode;
            CurrentViewMode = mode;

        }
        private void OpenCreationPanel() {
            _CreationScript_Container.Children.Add(ScriptPanel);
            _CreationScript_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _ScriptsListBox.IsHitTestVisible = false;

        }
        private void CloseCreationPanel() {
            if (_CreationScript_Container.Children.Count > 0) {
                _CreationScript_Container.Children.RemoveAt(_CreationScript_Container.Children.Count - 1);
                _CreationScript_Button.Visibility = Visibility.Collapsed;
                // Enable list interactions
                _ScriptsListBox.IsHitTestVisible = true;
                _ScriptsListBox.UnselectAll();
            }
        }

        /// <summary>
        /// Select all text when textbox gets focus
        /// </summary>
        private void OnPalabraGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (sender is TextBox textBox) {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Add focus when the user clicks on the textbox
        /// </summary>
        private void OnPalabraPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (sender is TextBox textBox) {
                if (!textBox.IsKeyboardFocusWithin) {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }
        #endregion

        #region Auxiliar methods
        private void UpdateLists() {
            ScriptsItems = ScriptsController.ScriptItems;
            _ScriptsListBox.UnselectAll();
        }

        #endregion
    }
}
