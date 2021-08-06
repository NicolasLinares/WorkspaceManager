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

namespace WorkspaceManagerTool.Views {

    /// <summary>
    /// Interaction logic for QuickAccessTabWindow.xaml
    /// </summary>
    public partial class QuickAccessTabWindow : UserControl, INotifyPropertyChanged {

        #region Properties and Constructor method

        private ObservableCollection<Group> groups;
        public ObservableCollection<Group> GroupItems {
            get => groups;
            set => SetProperty(ref groups, value);
        }


        private ObservableCollection<QuickAccess> quickAccess;
        public ObservableCollection<QuickAccess> QuickAccessItems {
            get => quickAccess;
            set => SetProperty(ref quickAccess, value);
        }

        private Group selectedGroup;
        public Group SelectedGroup {
            get => selectedGroup;
            set {
                SetProperty(ref selectedGroup, value);
            }
        }

        private QuickAccess selectedQuickAccess;
        public QuickAccess SelectedQuickAccessItem {
            get => selectedQuickAccess;
            set {
                SetProperty(ref selectedQuickAccess, value);
            }
        }

        private QuickAccessController QuickAccessController { get; set; }


        public QuickAccessTabWindow() {
            DataContext = this;
            InitializeComponent();

            QuickAccessController = new QuickAccessController();

            // Read file
            var data = QuickAccessController.ReadLocalList();

            if (data == null) {
                QuickAccessItems = new ObservableCollection<QuickAccess>();
                GroupItems = new ObservableCollection<Group>();
            } else {
                QuickAccessItems = new ObservableCollection<QuickAccess>(data);
                GroupItems = new ObservableCollection<Group>(QuickAccessItems.Select(qa => qa.Group).Distinct());
            }

        }


        #endregion


        #region Events handlers

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


        private void CreateQuickAccess_Click(object sender, EventArgs e) {

            NewQuickAccessDialog dialog = new NewQuickAccessDialog();

            if (dialog.ShowDialog() == true) {
                Group gr = new Group(dialog.GroupText, new SolidColorBrush(dialog.ColorBrush));
                QuickAccess qa = new QuickAccess(dialog.PathText, dialog.NameText, dialog.DescriptionText, gr);

                if (!GroupItems.Contains(gr)) {
                    GroupItems.Add(gr);
                }
                QuickAccessItems.Add(qa);
                QuickAccessController.SaveChanges(QuickAccessItems);
            }
        }
        private void EditQuickAccess_MenuClick(object sender, RoutedEventArgs e) {

            NewQuickAccessDialog dialog = new NewQuickAccessDialog(SelectedQuickAccessItem);

            if (dialog.ShowDialog() == true) {

                // set changes in the list
                foreach (var item in QuickAccessItems.Where(qa => qa.Equals(selectedQuickAccess))) {
                    item.Path = dialog.PathText;
                    item.Name = dialog.NameText;
                    item.Description = dialog.DescriptionText;
                    var newGroup = new Group(dialog.GroupText, new SolidColorBrush(dialog.ColorBrush));
                    if (!item.Group.Equals(newGroup)) {
                        RemoveGroupIfNotExists(item.Group);
                        item.Group = newGroup;
                    }
                }
                QuickAccessItems = QuickAccessItems;
                QuickAccessController.SaveChanges(QuickAccessItems);
            }
        }
        private void RemoveQuickAccess_MenuClick(object sender, RoutedEventArgs e) {

            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el acceso directo de forma permanente?", "Eliminar acceso directo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                var removedItem = SelectedQuickAccessItem;
                QuickAccessItems.Remove(removedItem);
                RemoveGroupIfNotExists(removedItem.Group);
                QuickAccessController.SaveChanges(QuickAccessItems);
            }
        }
        private void OpenQuickAccess_DoubleClick(object sender, MouseButtonEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;
            if (listBox.Items.Count == 0 || listBox.SelectedValue == null) {
                return;
            }

            try {
                QuickAccessController.Open((QuickAccess)listBox.SelectedValue);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        #endregion

        #region Auxiliar methods

        private void RemoveGroupIfNotExists(Group item) {
            if (!QuickAccessItems.Any(qa => qa.Group.Equals(item))) {
                GroupItems.Remove(item);
            }
        }

        #endregion

    }
}
