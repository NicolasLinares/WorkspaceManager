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
using FolderQuickAccess = WorkspaceManagerTool.Models.QuickAccess;
using WorkspaceManagerTool.Models;

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
            // Set observable data from controller
            QuickAccessItems = QuickAccessController.Items;
            GroupItems = QuickAccessController.GroupItems;
            _FiltersListBox.UnselectAll();
            _QuickAcessListBox.UnselectAll();
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

        #region Actions

        private void OpenCreationPanel_Action(object sender, EventArgs e) {
            // Panel creation
            if (CurrentViewMode.Equals(ViewMode.FILTER)) {
                QuickAccessPanel = new QuickAccess_CreationPanel(GroupItems, SelectedGroup);
            } else {
                QuickAccessPanel = new QuickAccess_CreationPanel(GroupItems);
            }
            ChangeViewMode(ViewMode.CREATION);
        }
        private void OpenEditionPanel_Action(object sender, RoutedEventArgs e) {
            if (_QuickAcessListBox.SelectedItem == null) {
                return;
            }
            // set current values to edit
            QuickAccessPanel = new QuickAccess_CreationPanel(SelectedQuickAccessItem, GroupItems);
            ChangeViewMode(ViewMode.EDITION);
        }
        private void ClosePanel_Action(object sender, EventArgs e) {
            ChangeViewMode(PreviousViewMode);
        }


        private void CreateItem_Action(object sender, EventArgs e) {
            GroupableResource new_qa = QuickAccessPanel.GetQuickAccess();
            if (QuickAccessController.Items.Contains(new_qa)) {
                MessageBox.Show("El acceso directo ya existe.", "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                ChangeViewMode(PreviousViewMode);
                return;
            }

            if (CurrentViewMode == ViewMode.EDITION) {
                QuickAccessController.Replace(SelectedQAToEdit, new_qa);
                GroupItems = QuickAccessController.GroupItems;
                SelectedGroup = SelectedQAToEdit.Group;
                SelectedQAToEdit = null;
            } else {
                QuickAccessController.Add(new_qa);
            }

            ChangeViewMode(PreviousViewMode);
        }
        private void RemoveItem_Action(object sender, RoutedEventArgs e) {
            if (_QuickAcessListBox.SelectedItem == null) {
                return;
            }
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el acceso directo de forma permanente?", "Eliminar acceso directo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                QuickAccessController.Remove(SelectedQuickAccessItem);
                var auxSelected = SelectedGroup;
                GroupItems = QuickAccessController.GroupItems;
                _FiltersListBox.SelectedItem = auxSelected;
                ChangeViewMode(CurrentViewMode);
            }
        }


        private void CopyToClipboard_Action(object sender, RoutedEventArgs e) {
            if (_QuickAcessListBox.SelectedItem == null) {
                return;
            }
            Clipboard.SetText((SelectedQuickAccessItem as QuickAccess).Path);
        }
        private void OpenQuickAccess_Action(object sender, EventArgs e) {
            if (_QuickAcessListBox.SelectedItem == null) {
                return;
            }

            try {
                QuickAccessController.OpenQuickAccess(SelectedQuickAccessItem);
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
            QuickAccessItems = QuickAccessController.SearchByName(_SearchText.Text);
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
        private void OpenEditionPanel() {
            SelectedQAToEdit = SelectedQuickAccessItem;
            // Open panel
            OpenCreationPanel();
        }
        private void OpenCreationPanel() {
            QuickAccessPanel.HandlerSaveChanges += CreateItem_Action;
            QuickAccessPanel.HandlerClosePanel += ClosePanel_Action;

            _CreationQuickAcess_Container.Children.Add(QuickAccessPanel);
            _Creation_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _SearchBar.IsEnabled = false;
            _FiltersListBox.IsHitTestVisible = false;
            _QuickAcessListBox.IsHitTestVisible = false;
            // Apply blur to background
            _BlurEffect.Radius = 5;
        }
        private void CloseCreationPanel() {
            if (_CreationQuickAcess_Container.Children.Count > 0) {
                QuickAccessPanel.HandlerSaveChanges -= CreateItem_Action;
                QuickAccessPanel.HandlerClosePanel -= ClosePanel_Action;
                _CreationQuickAcess_Container.Children.RemoveAt(_CreationQuickAcess_Container.Children.Count - 1);
                _Creation_Button.Visibility = Visibility.Visible;
                // Enable list interactions
                _SearchBar.IsEnabled = true;
                _FiltersListBox.IsHitTestVisible = true;
                _QuickAcessListBox.IsHitTestVisible = true;
                _QuickAcessListBox.UnselectAll();
                // Remove blur to background
                _BlurEffect.Radius = 0;
            }
        }
        private void EnableFilterMode() {
            _RemoveFilter_Button.Visibility = Visibility.Visible;
        }
        private void DisableFilterMode() {
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
        }


        //private void CollapseScrollbar(object sender, SizeChangedEventArgs e) {
        //    ScrollViewer sv = sender as ScrollViewer;

        //    if (sv.ActualHeight < sv.ScrollableHeight) {
        //        sv.BorderThickness = new Thickness(1, 1, 1, 1);
        //    } else {
        //        sv.BorderThickness = new Thickness(0, 0, 0, 0);
        //    }
        //}


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
            QuickAccessItems = QuickAccessController.Items;
            GroupItems = QuickAccessController.GroupItems;
            _FiltersListBox.UnselectAll();
            _QuickAcessListBox.UnselectAll();
        }
        private void ApplyFilter(Group group) {
            QuickAccessItems = QuickAccessController.FilterByGroup(group);
        }

        #endregion
    }
}
