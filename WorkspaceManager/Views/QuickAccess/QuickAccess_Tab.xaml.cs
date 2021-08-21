using WorkspaceManagerTool.Models.QuickAccess;
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
using FolderQuickAccess = WorkspaceManagerTool.Models.QuickAccess.QuickAccess;

namespace WorkspaceManagerTool.Views.QuickAccess {

    /// <summary>
    /// Interaction logic for QuickAccessTabWindow.xaml
    /// </summary>
    public partial class QuickAccess_Tab : UserControl, INotifyPropertyChanged {

        #region Properties and Constructor method

        private ObservableCollection<Group> groups;
        private ObservableCollection<FolderQuickAccess> quickAccess;
        private ObservableCollection<FolderQuickAccess> auxiliar;
        private Group selectedGroup;
        private FolderQuickAccess selectedQuickAccess;

        public ObservableCollection<Group> GroupItems {
            get => groups;
            set => SetProperty(ref groups, value);
        }

        public ObservableCollection<FolderQuickAccess> QuickAccessItems {
            get => quickAccess;
            set => SetProperty(ref quickAccess, value);
        }
        public ObservableCollection<FolderQuickAccess> AuxiliarItems {
            get => auxiliar;
            set => SetProperty(ref auxiliar, value);
        }
        public Group SelectedGroup {
            get => selectedGroup;
            set {
                SetProperty(ref selectedGroup, value);
            }
        }
        public Group FilterApplied { get; private set; }

        public FolderQuickAccess SelectedQuickAccessItem {
            get => selectedQuickAccess;
            set {
                SetProperty(ref selectedQuickAccess, value);
            }
        }
        public FolderQuickAccess SelectedQAToEdit { get; private set; }

        public ViewMode CurrentViewMode { get; private set; }

        private QuickAccessController QuickAccessController { get; set; }

        public QuickAccess_CreationPanel QuickAccessPanel { get; set; }



        public QuickAccess_Tab() {
            DataContext = this;
            InitializeComponent();
            // Create controller and initialize data
            QuickAccessController = new QuickAccessController();
            QuickAccessController.Init();
            // Set observable data from controller
            QuickAccessItems = QuickAccessController.QAItems;
            GroupItems = QuickAccessController.GroupItems;
            _FiltersListBox.UnselectAll();
        }
        #endregion


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

        private void CreateQuickAccess_Action(object sender, EventArgs e) {
            // new item, so empty values
            QuickAccessPanel = new QuickAccess_CreationPanel(GroupItems);
            ChangeViewMode(ViewMode.CREATION);
        }
        private void EditQuickAccess_Action(object sender, RoutedEventArgs e) {
            // set current values to edit
            QuickAccessPanel = new QuickAccess_CreationPanel(SelectedQuickAccessItem, GroupItems);
            ChangeViewMode(ViewMode.EDITION);
        }
        private void RemoveQuickAccess_Action(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el acceso directo de forma permanente?", "Eliminar acceso directo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                QuickAccessController.RemoveQA(SelectedQuickAccessItem);
                UpdateLists();
            }
        }
        private void CopyToClipboard_Action(object sender, RoutedEventArgs e) {
            Clipboard.SetText(SelectedQuickAccessItem.Path);
        }
        private void OpenQuickAccess_Action(object sender, EventArgs e) {
            if (_QuickAcessListBox.Items.Count == 0 || _QuickAcessListBox.SelectedValue == null) {
                return;
            }

            try {
                QuickAccessController.OpenQuickAccess(SelectedQuickAccessItem);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ApplyFilter_Action(object sender, MouseButtonEventArgs e) {
            ChangeViewMode(ViewMode.FILTER);
        }
        private void RemoveFilter_Action(object sender, EventArgs e) {
            ChangeViewMode(ViewMode.NORMAL);
        }


        private void AcceptButton_Click(object sender, EventArgs e) {
            FolderQuickAccess new_qa = QuickAccessPanel.GetQuickAccess();
            if (QuickAccessController.QAItems.Contains(new_qa)) {
                MessageBox.Show("El acceso directo ya existe.", "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentViewMode == ViewMode.EDITION) {
                QuickAccessController.ReplaceQA(SelectedQAToEdit, new_qa);
                SelectedQAToEdit = null;
            } else {
                QuickAccessController.AddQA(new_qa);
            }
            ChangeViewMode(ViewMode.NORMAL);
        }
        private void CancelButton_Click(object sender, EventArgs e) {
            ChangeViewMode(ViewMode.NORMAL);
        }
        #endregion



        #region GUI methods

        private void ChangeViewMode(ViewMode mode) {
            switch (mode) {
                case ViewMode.EDITION:
                    SelectedQAToEdit = SelectedQuickAccessItem;
                    OpenCreationPanel();
                    break;
                case ViewMode.CREATION:
                    OpenCreationPanel();
                    break;
                case ViewMode.FILTER:
                    ApplyFilter(SelectedGroup);
                    EnableFilterMode();
                    break;
                case (ViewMode.NORMAL):
                    UpdateLists();
                    DisableFilterMode();
                    CloseCreationPanel();
                    break;
            }

            CurrentViewMode = mode;
        }

        private void OpenCreationPanel() {
            _CreationQuickAcess_Container.Children.Add(QuickAccessPanel);
            _CreationPanel_Buttons.Visibility = Visibility.Visible;
            _CreationQuickAccess_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _FiltersListBox.IsHitTestVisible = false;
            _QuickAcessListBox.IsHitTestVisible = false;

        }

        private void CloseCreationPanel() {
            if (_CreationQuickAcess_Container.Children.Count > 0) {
                _CreationQuickAcess_Container.Children.RemoveAt(_CreationQuickAcess_Container.Children.Count - 1);
                _CreationPanel_Buttons.Visibility = Visibility.Collapsed;
                // Enable list interactions
                _FiltersListBox.IsHitTestVisible = true;
                _QuickAcessListBox.IsHitTestVisible = true;
                _QuickAcessListBox.UnselectAll();
            }
        }

        private void EnableFilterMode() {
            _CreationQuickAccess_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Visible;
        }

        private void DisableFilterMode() {
            _CreationQuickAccess_Button.Visibility = Visibility.Visible;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
        }

        #endregion


        #region Auxiliar methods

        private void UpdateLists() {
            QuickAccessItems = QuickAccessController.QAItems;
            GroupItems = QuickAccessController.GroupItems;
            _FiltersListBox.UnselectAll();
            _QuickAcessListBox.UnselectAll();
        }

        private void ApplyFilter(Group filter) {
            var list = QuickAccessController.QAItems;
            QuickAccessItems = new ObservableCollection<FolderQuickAccess>(list.Where(qa => qa.Group.Equals(filter)));
        }

        #endregion
    }
}
