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

        public ObservableCollection<GroupableResource> SelectionRemoved { get; private set; }


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
            ScriptsController.HandlerListImport += SetNormalMode_Action;
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

        #region Set Mode Actions
        private void SetCreationMode_Action(object sender, EventArgs e) {
            // Joining groups from script and quickaccess tab
            var quickaccessController = QuickAccessController.GetInstance();
            var groups = quickaccessController.GroupItems;
            groups = new ObservableCollection<Group>(groups.Concat(GroupItems).Distinct());
            // Panel creation
            if (CurrentViewMode.Equals(ViewMode.FILTER)) {
                CreationPanel = new Script_CreationPanel(groups, SelectedGroup);
            } else {
                CreationPanel = new Script_CreationPanel(groups);
            }
            SetViewMode(ViewMode.CREATION);
        }
        private void SetEditionMode_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            // set current values to edit
            CreationPanel = new Script_CreationPanel(SelectedScriptItem, GroupItems);
            SetViewMode(ViewMode.EDITION);
        }
        private void SetExecutionMode_Action(object sender, EventArgs e) {
            if (SelectedScriptItem == null) {
                return;
            }
            SetViewMode(ViewMode.EXECUTION);
        }
        private void SetMultipleSelectionMode_Action(object sender, EventArgs e) {
            SetViewMode(ViewMode.MULTIPLE_SELECTION);
        }
        public void SetPreviousMode_Action(object sender, EventArgs e) {
            SetViewMode(PreviousViewMode);
        }
        public void SetNormalMode_Action(object sender, EventArgs e) {
            SetViewMode(ViewMode.NORMAL);
        }
        #endregion

        #region Multiple Selection Actions
        private void SetCounter_Action(object sender, EventArgs e) {
            if (CurrentViewMode != ViewMode.MULTIPLE_SELECTION) {
                return;
            }
            if (_ScriptsListBox.SelectedItems.Count > 0) {
                _Trash_Button.IsEnabled = true;
            } else {
                _Trash_Button.IsEnabled = false;
            }
            _SelectionCounter.Text = string.Format("{0}/{1}", _ScriptsListBox.SelectedItems.Count, _ScriptsListBox.Items.Count);
        }
        private void RemoveSelectedItems_Action(object sender, EventArgs e) {
            if (_ScriptsListBox.SelectedItems.Count > 0) {
                foreach (var item in _ScriptsListBox.SelectedItems.OfType<GroupableResource>().ToList()) {
                    ScriptsItems.Remove(item);
                    SelectionRemoved.Add(item);
                }
                _CheckMark_Button.Visibility = Visibility.Visible;
                _ScriptsListBox.UnselectAll();
            }
        }
        private void ApplySelectionChanges_Action(object sender, EventArgs e) {
            MessageBoxResult result = MessageBox.Show("¿Seguro que desea aplicar los cambios de forma permanente?", "Aplicar cambios", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel) {
                return;
            }
            ScriptsController.RemoveSelection(SelectionRemoved);
            SetViewMode(ViewMode.NORMAL);
        }
        #endregion

        #region Item Actions
        private void CreateItem_Action(object sender, EventArgs e) {
            GroupableResource new_sc = CreationPanel.GetScript();
            if (ScriptsController.Items.Contains(new_sc)) {
                MessageBox.Show("El acceso directo ya existe.", "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                SetViewMode(PreviousViewMode);
                return;
            }
            if (CurrentViewMode == ViewMode.EDITION) {
                ScriptsController.Replace(SelectedScriptToEdit, new_sc);
                GroupItems = ScriptsController.GroupItems;
                SelectedGroup = SelectedScriptToEdit.Group;
                SelectedScriptToEdit = null;
            } else {
                ScriptsController.Add(new_sc);
            }
            SetViewMode(PreviousViewMode);
        }
        public void ReplaceItem_Action(object sender, ScriptEvent e) {
            // Text script has been modified
            ScriptsController.Replace(e.OldScript, e.NewScript);
            _ScriptsListBox.UnselectAll();
        }
        private void RemoveItem_Action(object sender, RoutedEventArgs e) {
            if (_ScriptsListBox.SelectedItem == null) {
                return;
            }
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el script de forma permanente?", "Eliminar script", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                ScriptsController.Remove(SelectedScriptItem);
                UpdateFilterList();
                SetViewMode(CurrentViewMode);
            }
        }
        private void ExecuteScript_Action(object sender, EventArgs e) {
            if (CurrentViewMode == ViewMode.MULTIPLE_SELECTION || _ScriptsListBox.SelectedItem == null) {
                return;
            }
            try {
                ScriptsController.RunScript(SelectedScriptItem as Script);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DuplicateScript_Action(object sender, EventArgs e) {
            if (CurrentViewMode == ViewMode.MULTIPLE_SELECTION || _ScriptsListBox.SelectedItem == null) {
                return;
            }
            ScriptsController.DuplicateScript(SelectedScriptItem as Script);
            SetViewMode(CurrentViewMode);
        }

        private void PinScript_Action(object sender, EventArgs e) {
            if (CurrentViewMode == ViewMode.MULTIPLE_SELECTION || _ScriptsListBox.SelectedItem == null) {
                return;
            }
            ScriptsController.Pin(SelectedScriptItem as Script);
            SetViewMode(CurrentViewMode);
        }


        #endregion

        #region Searchbar & Filter
        private void SearchByName_Action(object sender, TextChangedEventArgs e) {
            if (_SearchText.Text.Length > 0) {
                _SearchRemoveButton.Visibility = Visibility.Visible;
            } else {
                _SearchRemoveButton.Visibility = Visibility.Hidden;
            }
            ScriptsItems = ScriptsController.SearchByName(_SearchText.Text);
        }
        private void ApplyFilter_Action(object sender, MouseButtonEventArgs e) {
            if (_FiltersListBox.SelectedItem == null) {
                return;
            }
            SetViewMode(ViewMode.FILTER);
        }

        #endregion

        #region GUI methods
        private void SetViewMode(ViewMode mode) {
            switch (mode) {
                case ViewMode.CREATION:
                    DoEnableCreationMode();
                    break;
                case ViewMode.EDITION:
                    DoEnableEditionMode();
                    break;
                case ViewMode.FILTER:
                    DoDisableCreationMode();
                    DoDisableExecutionMode();
                    DoDisableMultipleSelectionMode();
                    DoEnableFilterMode();
                    UpdateFilterList();
                    ApplyFilter(SelectedGroup);
                    break;
                case (ViewMode.EXECUTION):
                    DoDisableFilterMode();
                    DoEnableExecutionMode();
                    break;
                case (ViewMode.MULTIPLE_SELECTION):
                    DoEnableMultipleSelectionMode();
                    break;
                case (ViewMode.NORMAL):
                    DoDisableFilterMode();
                    DoDisableMultipleSelectionMode();
                    DoDisableCreationMode();
                    DoDisableExecutionMode();
                    DoCleanSearchBar();
                    UpdateLists();
                    break;
            }
            PreviousViewMode = CurrentViewMode;
            CurrentViewMode = mode;
        }

        private void DoEnableCreationMode() {
            CreationPanel.HandlerSaveChanges += CreateItem_Action;
            CreationPanel.HandlerClosePanel += SetPreviousMode_Action;
            // Show panel view
            _CreationPanel_Container.Children.Add(CreationPanel);
            _SelectionMultiple_Button.Visibility = Visibility.Collapsed;
            _Creation_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _FiltersListBox.IsHitTestVisible = false;
            _ScriptsListBox.IsHitTestVisible = false;
        }
        private void DoEnableEditionMode() {
            SelectedScriptToEdit = SelectedScriptItem;
            // Open panel
            DoEnableCreationMode();
        }
        private void DoEnableExecutionMode() {
            // new item, so empty values
            ExecutionPanel = new Script_ExecutionPanel(SelectedScriptItem);
            ExecutionPanel.HandlerExecution += ScriptsController.DoExecution;
            ExecutionPanel.HandlerClosePanel += SetPreviousMode_Action;
            ExecutionPanel.HandlerChanges += ReplaceItem_Action;
            // Show panel view
            _ExecutionPanel_Container.Children.Add(ExecutionPanel);
            _SelectionMultiple_Button.Visibility = Visibility.Collapsed;
            _ExecutionPanel_Container.Visibility = Visibility.Visible;
            _Creation_Button.Visibility = Visibility.Collapsed;
        }
        private void DoEnableFilterMode() {
            _RemoveFilter_Button.Visibility = Visibility.Visible;
        }
        private void DoEnableMultipleSelectionMode() {
            // Set visibility
            _SelectionMultiple_Button.Visibility = Visibility.Collapsed;
            _Creation_Button.Visibility = Visibility.Collapsed;
            _CrossMark_Button.Visibility = Visibility.Visible;
            _SelectionCounter.Visibility = Visibility.Visible;
            _Trash_Button.Visibility = Visibility.Visible;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _SearchBar.IsEnabled = false;
            _FiltersListBox.IsHitTestVisible = false;
            _ScriptsListBox.ContextMenu.Visibility = Visibility.Collapsed;
            _ScriptsListBox.UnselectAll();
            // Enable multiple selection
            _ScriptsListBox.SelectionMode = SelectionMode.Multiple;
            _SelectionCounter.Text = string.Format("{0}/{1}", _ScriptsListBox.SelectedItems.Count, _ScriptsListBox.Items.Count);
            SelectionRemoved = new ObservableCollection<GroupableResource>();
        }

        private void DoDisableCreationMode() {
            if (_CreationPanel_Container.Children.Count > 0) {
                CreationPanel.HandlerSaveChanges -= CreateItem_Action;
                CreationPanel.HandlerClosePanel -= SetPreviousMode_Action;
                _CreationPanel_Container.Children.RemoveAt(_CreationPanel_Container.Children.Count - 1);
                _SelectionMultiple_Button.Visibility = Visibility.Visible;
                _Creation_Button.Visibility = Visibility.Visible;
                // Enable list interactions
                _FiltersListBox.IsHitTestVisible = true;
                _ScriptsListBox.IsHitTestVisible = true;
                _ScriptsListBox.UnselectAll();
            }
        }
        private void DoDisableFilterMode() {
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
        }
        private void DoDisableExecutionMode() {
            if (_ExecutionPanel_Container.Children.Count > 0) {
                _ExecutionPanel_Container.Children.RemoveAt(_ExecutionPanel_Container.Children.Count - 1);
                _ExecutionPanel_Container.Visibility = Visibility.Collapsed;
                _SelectionMultiple_Button.Visibility = Visibility.Visible;
                _Creation_Button.Visibility = Visibility.Visible;
                ExecutionPanel.HandlerExecution -= ScriptsController.DoExecution;
                ExecutionPanel.HandlerClosePanel -= SetPreviousMode_Action;
                ExecutionPanel.HandlerChanges -= ReplaceItem_Action;
            }
        }
        private void DoDisableMultipleSelectionMode() {
            // Organize buttons visibility
            _SelectionMultiple_Button.Visibility = Visibility.Visible;
            _Creation_Button.Visibility = Visibility.Visible;
            _CheckMark_Button.Visibility = Visibility.Collapsed;
            _CrossMark_Button.Visibility = Visibility.Collapsed;
            _Trash_Button.Visibility = Visibility.Collapsed;
            _Trash_Button.IsEnabled = false;
            _SelectionCounter.Visibility = Visibility.Collapsed;
            // Enable list interactions
            _SearchBar.IsEnabled = true;
            _FiltersListBox.IsHitTestVisible = true;
            _ScriptsListBox.ContextMenu.Visibility = Visibility.Visible;
            // Disable multiple selection
            _ScriptsListBox.SelectionMode = SelectionMode.Single;
            _SelectionCounter.Text = string.Empty;
        }
        private void DoCleanSearchBar() {
            _SearchText.Text = string.Empty;
            _SearchRemoveButton.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Auxiliar methods
        private void UpdateFilterList() {
            var tmp = SelectedGroup;
            GroupItems = ScriptsController.GroupItems;
            SelectedGroup = tmp;
        }
        private void UpdateLists() {
            ScriptsItems = ScriptsController.Items;
            GroupItems = ScriptsController.GroupItems;
            _FiltersListBox.UnselectAll();
            _ScriptsListBox.UnselectAll();
        }
        private void ApplyFilter(Group group) {
            ScriptsItems = ScriptsController.FilterByGroup(group);
            _ScriptsListBox.UnselectAll();
        }

        private void OnPalabraGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (sender is TextBox textBox) {
                textBox.SelectAll();
            }
        }
        private void OnPalabraPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (sender is TextBox textBox) {
                if (!textBox.IsKeyboardFocusWithin) {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }
        #endregion
    }
}
