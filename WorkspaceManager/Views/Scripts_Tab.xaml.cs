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
using WorkspaceManagerTool.Events;
using WorkspaceManagerTool.Models;

namespace WorkspaceManagerTool.Views {
    /// <summary>
    /// Interaction logic for DeployTabWindow.xaml
    /// </summary>
    public partial class Scripts_Tab : UserControl, INotifyPropertyChanged {

        private GroupableResource selectedScript;
        private ObservableCollection<GroupableResource> scriptsItems;

        private ObservableCollection<Group> groups;
        private Group selectedGroup;


        public ObservableCollection<GroupableResource> ScriptsItems {
            get => scriptsItems;
            set => SetProperty(ref scriptsItems, value);
        }

        public GroupableResource SelectedScriptItem {
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

        public GroupableResource SelectedScriptToEdit { get; private set; }

        public ViewMode CurrentViewMode { get; private set; }
        public ViewMode PreviousViewMode { get; private set; }

        private ScriptsController ScriptsController { get; set; }

        public Script_CreationPanel CreationPanel { get; set; }
        public Script_ExecutionPanel ExecutionPanel { get; set; }


        public Scripts_Tab() {
            DataContext = this;
            InitializeComponent();

            // Create controller and initialize data
            ScriptsController = ScriptsController.GetInstance();
            ScriptsController.Init();
            // Set observable data from controller
            ScriptsItems = ScriptsController.Items;
            GroupItems = ScriptsController.GroupItems;
            _FiltersListBox.UnselectAll();
            _ScriptsListBox.UnselectAll();
        }

        #region Notify Preperties changes
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

        private void OpenExecutionPanel_Action(object sender, EventArgs e) {
            if (SelectedScriptItem == null) {
                return;
            }
            ChangeViewMode(ViewMode.EXECUTION);
        }

        private void OpenCreationPanel_Action(object sender, EventArgs e) {
            // Panel creation
            if (CurrentViewMode.Equals(ViewMode.FILTER)) {
                CreationPanel = new Script_CreationPanel(GroupItems, SelectedGroup);
            } else {
                CreationPanel = new Script_CreationPanel(GroupItems);
            }
            ChangeViewMode(ViewMode.CREATION);
        }
        private void OpenEditionPanel_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            // set current values to edit
            CreationPanel = new Script_CreationPanel(SelectedScriptItem, GroupItems);
            ChangeViewMode(ViewMode.EDITION);
        }
        private void ClosePanel_Action(object sender, EventArgs e) {
            ChangeViewMode(PreviousViewMode);
        }


        private void CreateItem_Action(object sender, EventArgs e) {
            GroupableResource new_sc = CreationPanel.GetScript();
            if (ScriptsController.Items.Contains(new_sc)) {
                MessageBox.Show("El acceso directo ya existe.", "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentViewMode == ViewMode.EDITION) {
                ReplaceItem(SelectedScriptToEdit, new_sc);
            } else {
                ScriptsController.Add(new_sc);
            }

            ChangeViewMode(PreviousViewMode);
        }
        private void RemoveItem_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el script de forma permanente?", "Eliminar script", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                ScriptsController.Remove(SelectedScriptItem);
                ChangeViewMode(CurrentViewMode);
            }
        }
        public void DoSaveChangesInScript(object sender, ScriptEvent e) {
            ReplaceItem(e.OldScript, e.NewScript);
        }


        private void ExecuteScript_Action(object sender, EventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            try {
                ScriptsController.RunScript(SelectedScriptItem as Script);
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
                    OpenEditionPanel();
                    break;
                case ViewMode.FILTER:
                    ApplyFilter(SelectedGroup);
                    EnableFilterMode();
                    CloseCreationPanel();
                    CloseExecutionPanel();
                    break;
                case (ViewMode.EXECUTION):
                    DisableFilterMode();
                    OpenExecutionPanel();
                    break;
                case (ViewMode.NORMAL):
                    UpdateLists();
                    DisableFilterMode();
                    CloseCreationPanel();
                    CloseExecutionPanel();
                    break;
            }
            PreviousViewMode = CurrentViewMode;
            CurrentViewMode = mode;
        }

        private void OpenExecutionPanel() {
            // new item, so empty values
            ExecutionPanel = new Script_ExecutionPanel(SelectedScriptItem);
            ExecutionPanel.HandlerExecution += ScriptsController.DoExecution;
            ExecutionPanel.HandlerClosePanel += DoCloseExecutionPanel;
            ExecutionPanel.HandlerChanges += DoSaveChangesInScript;
            // Show panel view
            _ExecutionPanel_Container.Children.Add(ExecutionPanel);
            _ExecutionPanel_Container.Visibility = Visibility.Visible;
            _Creation_Button.Visibility = Visibility.Collapsed;
        }


        private void CloseExecutionPanel() {
            if (_ExecutionPanel_Container.Children.Count > 0) {
                _ExecutionPanel_Container.Children.RemoveAt(_ExecutionPanel_Container.Children.Count - 1);
                _ExecutionPanel_Container.Visibility = Visibility.Collapsed;
                ExecutionPanel.HandlerExecution -= ScriptsController.DoExecution;
                ExecutionPanel.HandlerClosePanel -= DoCloseExecutionPanel;
                ExecutionPanel.HandlerChanges -= DoSaveChangesInScript;
            }
        }


        public void DoCloseExecutionPanel(object sender, EventArgs e) {
            ChangeViewMode(PreviousViewMode);
        }

        private void OpenEditionPanel() {
            SelectedScriptToEdit = SelectedScriptItem;
            // Open panel
            OpenCreationPanel();
        }

        private void OpenCreationPanel() {
            // Show panel view
            _CreationPanel_Container.Children.Add(CreationPanel);
            _CreationPanel_Buttons.Visibility = Visibility.Visible;
            _Creation_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _FiltersListBox.IsHitTestVisible = false;
            _ScriptsListBox.IsHitTestVisible = false;
        }
        private void CloseCreationPanel() {
            if (_CreationPanel_Container.Children.Count > 0) {
                _CreationPanel_Container.Children.RemoveAt(_CreationPanel_Container.Children.Count - 1);
                _CreationPanel_Buttons.Visibility = Visibility.Collapsed;
                _Creation_Button.Visibility = Visibility.Visible;
                // Enable list interactions
                _FiltersListBox.IsHitTestVisible = true;
                _ScriptsListBox.IsHitTestVisible = true;
                _ScriptsListBox.UnselectAll();
            }
        }
        private void EnableFilterMode() {
            _RemoveFilter_Button.Visibility = Visibility.Visible;
        }
        private void DisableFilterMode() {
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


        private void ReplaceItem(GroupableResource olditem, GroupableResource newitem) {
            ScriptsController.Replace(olditem, newitem);
            GroupItems = ScriptsController.GroupItems;
            SelectedGroup = olditem.Group;
            SelectedScriptToEdit = null;
        }

        private void UpdateLists() {
            ScriptsItems = ScriptsController.Items;
            GroupItems = ScriptsController.GroupItems;
            _FiltersListBox.UnselectAll();
            _ScriptsListBox.UnselectAll();
        }
        private void ApplyFilter(Group group) {
            ScriptsItems = ScriptsController.FilterByGroup(group);
        }


        #endregion
    }
}
