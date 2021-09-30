using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System;
using WorkspaceManagerTool.Controllers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ookii.Dialogs.WinForms;
using WorkspaceManagerTool.Models;
using System.Windows.Media.Effects;
using System.IO;

namespace WorkspaceManagerTool.Views {

    /// <summary>
    /// Interaction logic for QuickAccessTabWindow.xaml
    /// </summary>
    public partial class QuickAccess_Tab : UserControl, INotifyPropertyChanged {

        #region Properties and Constructor method

        private ObservableCollection<Group> groups;
        private ObservableCollection<GroupableResource> quickAccess;
        private ObservableCollection<GroupableResource> auxiliar;
        private Group selectedGroup;
        private GroupableResource selectedQuickAccess;

        public ObservableCollection<Group> GroupItems {
            get => groups;
            set => SetProperty(ref groups, value);
        }

        public ObservableCollection<GroupableResource> QuickAccessItems {
            get => quickAccess;
            set => SetProperty(ref quickAccess, value);
        }

        public Group SelectedGroup {
            get => selectedGroup;
            set {
                SetProperty(ref selectedGroup, value);
            }
        }

        public GroupableResource SelectedQuickAccessItem {
            get => selectedQuickAccess;
            set {
                SetProperty(ref selectedQuickAccess, value);
            }
        }

        public GroupableResource SelectedQAToEdit { get; private set; }

        public ObservableCollection<GroupableResource> SelectionRemoved { get; private set; }

        public ViewMode CurrentViewMode { get; private set; }
        public ViewMode PreviousViewMode { get; private set; }

        private QuickAccessController QuickAccessController { get; set; }

        public QuickAccess_CreationPanel QuickAccessPanel { get; set; }

        public QuickAccess_Tab() {
            DataContext = this;
            InitializeComponent();
            // Create controller and initialize data
            QuickAccessController = QuickAccessController.GetInstance();
            QuickAccessController.Init();
            QuickAccessController.HandlerListImport += SetNormalMode_Action;

            // Set observable data from controller
            UpdateLists();
        }
        #endregion

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
            var scriptController = ScriptsController.GetInstance();
            var groups = scriptController.GroupItems;
            groups = new ObservableCollection<Group>(groups.Concat(GroupItems).Distinct());
            // Panel creation
            if (CurrentViewMode.Equals(ViewMode.FILTER)) {
                QuickAccessPanel = new QuickAccess_CreationPanel(groups, SelectedGroup);
            } else {
                QuickAccessPanel = new QuickAccess_CreationPanel(groups);
            }
            SetViewMode(ViewMode.CREATION);
        }
        private void SetEditionMode_Action(object sender, RoutedEventArgs e) {
            if (_QuickAcessListBox.SelectedItem == null) {
                return;
            }
            // set current values to edit
            QuickAccessPanel = new QuickAccess_CreationPanel(SelectedQuickAccessItem, GroupItems);
            SetViewMode(ViewMode.EDITION);
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
            if (_QuickAcessListBox.SelectedItems.Count > 0) {
                _Trash_Button.IsEnabled = true;
            } else {
                _Trash_Button.IsEnabled = false;
            }
            _SelectionCounter.Text = string.Format("{0}/{1}", _QuickAcessListBox.SelectedItems.Count, _QuickAcessListBox.Items.Count);
        }
        private void RemoveSelectedItems_Action(object sender, EventArgs e) {
            if (_QuickAcessListBox.SelectedItems.Count > 0) {
                foreach (var item in _QuickAcessListBox.SelectedItems.OfType<GroupableResource>().ToList()) {
                    QuickAccessItems.Remove(item);
                    SelectionRemoved.Add(item);
                }
                _CheckMark_Button.Visibility = Visibility.Visible;
                _QuickAcessListBox.UnselectAll();
            }
        }
        private void ApplySelectionChanges_Action(object sender, EventArgs e) {
            MessageBoxResult result = MessageBox.Show("¿Seguro que desea aplicar los cambios de forma permanente?", "Aplicar cambios", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel) {
                return;
            }
            QuickAccessController.RemoveSelection(SelectionRemoved);
            SetViewMode(ViewMode.NORMAL);
        }

        private void OpenGroupEditionWindow_Action(object sender, RoutedEventArgs e) {
            Group_CreationDialog dialog = new Group_CreationDialog(SelectedGroup);
            if (dialog.ShowDialog() == true) {
                var editedGroup = dialog.GetGroup();
                if (GroupItems.Contains(editedGroup)) {
                    MessageBox.Show("El grupo creado ya existe.", "Grupo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                QuickAccessController.ChangeGroup(SelectedGroup, editedGroup);
                SelectedGroup = editedGroup;
                SetViewMode(ViewMode.FILTER);
            }
        }

        #endregion

        #region Item Actions
        private void CreateItem_Action(object sender, EventArgs e) {
            GroupableResource new_qa = QuickAccessPanel.GetQuickAccess();
            if (QuickAccessController.Items.Contains(new_qa)) {
                MessageBox.Show(CurrentViewMode == ViewMode.EDITION ? "No se han realizado modificaciones en el acceso directo." : "El acceso directo ya existe.",
                    CurrentViewMode == ViewMode.EDITION ? "Acceso directo no modificado" : "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                SetViewMode(PreviousViewMode);
                return;
            }
            if (CurrentViewMode == ViewMode.EDITION) {
                QuickAccessController.Replace(SelectedQAToEdit, new_qa);
                SelectedGroup = SelectedQAToEdit.Group;
                SelectedQAToEdit = null;
            } else {
                QuickAccessController.Add(new_qa);
            }
            SetViewMode(PreviousViewMode);
        }
        private void RemoveItem_Action(object sender, RoutedEventArgs e) {
            if (_QuickAcessListBox.SelectedItem == null) {
                return;
            }
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el acceso directo de forma permanente?", "Eliminar acceso directo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                QuickAccessController.Remove(SelectedQuickAccessItem);
                UpdateFilterList();
                SetViewMode(CurrentViewMode);
            }
        }
        private void CopyToClipboard_Action(object sender, RoutedEventArgs e) {
            if (_QuickAcessListBox.SelectedItem == null) {
                return;
            }
            Clipboard.SetText((SelectedQuickAccessItem as QuickAccess).Path);
        }
        private void OpenQuickAccess_Action(object sender, EventArgs e) {
            if (CurrentViewMode == ViewMode.MULTIPLE_SELECTION || _QuickAcessListBox.SelectedItem == null) {
                return;
            }

            try {
                QuickAccessController.OpenPath((SelectedQuickAccessItem as QuickAccess).Path);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PinScript_Action(object sender, EventArgs e) {
            if (CurrentViewMode == ViewMode.MULTIPLE_SELECTION || _QuickAcessListBox.SelectedItem == null) {
                return;
            }
            QuickAccessController.Pin(SelectedQuickAccessItem as QuickAccess);
            SetViewMode(CurrentViewMode);
        }

        private void OpenFileFolder_Action(object sender, EventArgs e) {
            if (CurrentViewMode == ViewMode.MULTIPLE_SELECTION || _QuickAcessListBox.SelectedItem == null) {
                return;
            }
            try {
                var qa = SelectedQuickAccessItem as QuickAccess;
                string folderContent = Path.GetDirectoryName(qa.Path);
                QuickAccessController.OpenPath(folderContent);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DefineContextMenuOptionsByItemType_Action(object sender, EventArgs e) {
            if (CurrentViewMode == ViewMode.MULTIPLE_SELECTION || _QuickAcessListBox.SelectedItem == null) {
                return;
            }
            var element = _QuickAcessListBox.SelectedItem as QuickAccess;
            if (element.Type is QuickAccessType.FILE) {
                _MenuItemOpenFolder.Visibility = Visibility.Visible;
            } else {
                _MenuItemOpenFolder.Visibility = Visibility.Collapsed;
            }
        }



        #endregion

        #region Searchbar & Filter
        private void SearchByName_Action(object sender, TextChangedEventArgs e) {
            if (_SearchText.Text.Length > 0) {
                _SearchRemoveButton.Visibility = Visibility.Visible;
            } else {
                _SearchRemoveButton.Visibility = Visibility.Hidden;
            }
            QuickAccessItems = QuickAccessController.SearchByName(_SearchText.Text);
        }
        private void ApplyFilter_Action(object sender, EventArgs e) {
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
                    DoDisableMultipleSelectionMode();
                    DoEnableFilterMode();
                    UpdateFilterList();
                    ApplyFilter(SelectedGroup);
                    break;
                case (ViewMode.MULTIPLE_SELECTION):
                    DoEnableMultipleSelectionMode();
                    break;
                case (ViewMode.NORMAL):
                    DoDisableFilterMode();
                    DoDisableMultipleSelectionMode();
                    DoDisableCreationMode();
                    DoCleanSearchBar();
                    UpdateLists();
                    break;
            }
            PreviousViewMode = CurrentViewMode;
            CurrentViewMode = mode;
        }


        private void DoEnableCreationMode() {
            QuickAccessPanel.HandlerSaveChanges += CreateItem_Action;
            QuickAccessPanel.HandlerClosePanel += SetPreviousMode_Action;

            _CreationQuickAcess_Container.Children.Add(QuickAccessPanel);
            _SelectionMultiple_Button.Visibility = Visibility.Collapsed;
            _Creation_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _SearchBar.IsEnabled = false;
            _FiltersListBox.IsHitTestVisible = false;
            _QuickAcessListBox.IsHitTestVisible = false;
            // Apply blur to background
            BlurEffect effect = new BlurEffect() { Radius = 5 };
            _BlurEffect.Effect = effect;
        }
        private void DoEnableEditionMode() {
            SelectedQAToEdit = SelectedQuickAccessItem;
            // Open panel
            DoEnableCreationMode();
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
            _QuickAcessListBox.ContextMenu.Visibility = Visibility.Collapsed;
            _QuickAcessListBox.UnselectAll();
            // Enable multiple selection
            _QuickAcessListBox.SelectionMode = SelectionMode.Multiple;
            _SelectionCounter.Text = string.Format("{0}/{1}", _QuickAcessListBox.SelectedItems.Count, _QuickAcessListBox.Items.Count);
            SelectionRemoved = new ObservableCollection<GroupableResource>();
        }

        private void DoDisableCreationMode() {
            if (_CreationQuickAcess_Container.Children.Count > 0) {
                QuickAccessPanel.HandlerSaveChanges -= CreateItem_Action;
                QuickAccessPanel.HandlerClosePanel -= SetPreviousMode_Action;
                _CreationQuickAcess_Container.Children.RemoveAt(_CreationQuickAcess_Container.Children.Count - 1);
                _SelectionMultiple_Button.Visibility = Visibility.Visible;
                _Creation_Button.Visibility = Visibility.Visible;
                // Enable list interactions
                _SearchBar.IsEnabled = true;
                _FiltersListBox.IsHitTestVisible = true;
                _QuickAcessListBox.IsHitTestVisible = true;
                _QuickAcessListBox.UnselectAll();
                // Remove blur to background
                _BlurEffect.Effect = null;
            }
        }
        private void DoDisableFilterMode() {
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
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
            _QuickAcessListBox.ContextMenu.Visibility = Visibility.Visible;
            // Disable multiple selection
            _QuickAcessListBox.SelectionMode = SelectionMode.Single;
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
            GroupItems = QuickAccessController.GroupItems;
            SelectedGroup = tmp;
        }
        private void UpdateLists() {
            QuickAccessItems = QuickAccessController.Items;
            GroupItems = QuickAccessController.GroupItems;
            _FiltersListBox.UnselectAll();
            _QuickAcessListBox.UnselectAll();
        }
        private void ApplyFilter(Group group) {
            QuickAccessItems = QuickAccessController.FilterByGroup(group);
            _QuickAcessListBox.UnselectAll();
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
