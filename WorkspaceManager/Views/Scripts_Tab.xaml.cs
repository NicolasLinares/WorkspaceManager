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

        private ObservableCollection<Group> groups;
        private Group selectedGroup;


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
        public ObservableCollection<Group> GroupItems {
            get => groups;
            set => SetProperty(ref groups, value);
        }
        public Group SelectedGroup {
            get => selectedGroup;
            set {
                SetProperty(ref selectedGroup, value);
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
            GroupItems = ScriptsController.GroupItems;
            _FiltersListBox.UnselectAll();
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
        private void OpenCreationPanel_Action(object sender, EventArgs e) {
            // new item, so empty values
            ScriptPanel = new Script_CreationPanel(GroupItems);
            ChangeViewMode(ViewMode.CREATION);
        }
        private void OpenEditionPanel_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            // set current values to edit
            ScriptPanel = new Script_CreationPanel(SelectedScriptItem, GroupItems);
            ChangeViewMode(ViewMode.EDITION);
        }
        private void ClosePanel_Action(object sender, EventArgs e) {
            ChangeViewMode(PreviousViewMode);
        }


        private void CreateItem_Action(object sender, EventArgs e) {
            Script new_sc = ScriptPanel.GetScript();
            if (ScriptsController.ScriptItems.Contains(new_sc)) {
                MessageBox.Show("El acceso directo ya existe.", "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentViewMode == ViewMode.EDITION) {
                ScriptsController.Replace<Script>(SelectedScriptToEdit, new_sc);
                GroupItems = ScriptsController.GroupItems;
                SelectedGroup = SelectedScriptToEdit.Group;
                SelectedScriptToEdit = null;
            } else {
                ScriptsController.Add<Script>(new_sc);
            }

            ChangeViewMode(PreviousViewMode);
        }
        private void RemoveItem_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el script de forma permanente?", "Eliminar script", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                ScriptsController.Remove<Script>(SelectedScriptItem);
                ChangeViewMode(CurrentViewMode);
            }
        }


        private void ExecuteScript_Action(object sender, EventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            try {
                ScriptsController.ExecuteScript(SelectedScriptItem);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SearchByName_Action(object sender, TextChangedEventArgs e) {
            if (_SearchText.Text.Length > 0) {
                _SearchRemoveButton.Visibility = Visibility.Visible;
            } else {
                _SearchRemoveButton.Visibility = Visibility.Hidden;
            }
            ScriptsItems = ScriptsController.SearchByName(_SearchText.Text);
        }
        private void RemoveSearch_Action(object sender, EventArgs e) {
            _SearchText.Text = string.Empty;
            _SearchRemoveButton.Visibility = Visibility.Hidden;
            ChangeViewMode(ViewMode.NORMAL);
        }
        private void ApplyFilter_Action(object sender, MouseButtonEventArgs e) {
            if (_FiltersListBox.SelectedItem == null) {
                return;
            }
            ChangeViewMode(ViewMode.FILTER);
        }
        private void RemoveFilter_Action(object sender, EventArgs e) {
            ChangeViewMode(ViewMode.NORMAL);
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
                case ViewMode.FILTER:
                    ApplyFilter(SelectedGroup);
                    EnableFilterMode();
                    CloseCreationPanel();
                    break;
                case (ViewMode.NORMAL):
                    UpdateLists();
                    DisableFilterMode();
                    CloseCreationPanel();
                    break;
            }
            PreviousViewMode = CurrentViewMode;
            CurrentViewMode = mode;
        }
        private void OpenCreationPanel() {
            _CreationScript_Container.Children.Add(ScriptPanel);
            _CreationPanel_Buttons.Visibility = Visibility.Visible;
            _Creation_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _FiltersListBox.IsHitTestVisible = false;
            _ScriptsListBox.IsHitTestVisible = false;

        }
        private void CloseCreationPanel() {
            if (_CreationScript_Container.Children.Count > 0) {
                _CreationScript_Container.Children.RemoveAt(_CreationScript_Container.Children.Count - 1);
                _CreationPanel_Buttons.Visibility = Visibility.Collapsed;
                // Enable list interactions
                _FiltersListBox.IsHitTestVisible = true;
                _ScriptsListBox.IsHitTestVisible = true;
                _ScriptsListBox.UnselectAll();
            }
        }
        private void EnableFilterMode() {
            _Creation_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Visible;
        }
        private void DisableFilterMode() {
            _Creation_Button.Visibility = Visibility.Visible;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
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
            GroupItems = ScriptsController.GroupItems;
            _FiltersListBox.UnselectAll();
            _ScriptsListBox.UnselectAll();
        }
        private void ApplyFilter(Group filter) {
            var list = ScriptsController.ScriptItems;
            ScriptsItems = new ObservableCollection<Script>(list.Where(qa => qa.Group.Equals(filter)));
        }


        #endregion
    }
}
