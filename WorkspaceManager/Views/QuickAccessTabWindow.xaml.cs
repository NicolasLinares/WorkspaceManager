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
        private ObservableCollection<QuickAccess> quickAccess;
        private ObservableCollection<QuickAccess> auxiliar;
        private Group selectedGroup;
        private QuickAccess selectedQuickAccess;

        public ObservableCollection<Group> GroupItems {
            get => groups;
            set => SetProperty(ref groups, value);
        }
        public ObservableCollection<QuickAccess> QuickAccessItems {
            get => quickAccess;
            set => SetProperty(ref quickAccess, value);
        }
        public ObservableCollection<QuickAccess> AuxiliarItems {
            get => auxiliar;
            set => SetProperty(ref auxiliar, value);
        }
        public Group SelectedGroup {
            get => selectedGroup;
            set {
                SetProperty(ref selectedGroup, value);
            }
        }
        public QuickAccess SelectedQuickAccessItem {
            get => selectedQuickAccess;
            set {
                SetProperty(ref selectedQuickAccess, value);
            }
        }
        public QuickAccess SelectedQAToEdit { get; private set; }
        public bool EditingMode { get; private set; }

        private QuickAccessController QuickAccessController { get; set; }

        public QuickAccessCreationPanel QuickAccessPanel { get; set; }



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
                QuickAccessItems = new ObservableCollection<QuickAccess>(OrderList(data));
                UpdateGroupList();
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
            OpenCreationPanel();
        }
        private void EditQuickAccess_MenuClick(object sender, RoutedEventArgs e) {
            SelectedQAToEdit = SelectedQuickAccessItem;
            EditingMode = true;
            OpenCreationPanel(SelectedQAToEdit);
        }
        private void RemoveQuickAccess_MenuClick(object sender, RoutedEventArgs e) {

            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el acceso directo de forma permanente?", "Eliminar acceso directo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                var removedItem = SelectedQuickAccessItem;
                RemoveItem(removedItem);
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

        private void ApplyFilter_Click(object sender, MouseButtonEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;
            if (listBox.Items.Count <= 1 || listBox.SelectedValue == null) {
                return;
            }

            _CreationQuickAccess_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Visible;

            ApplyFilter((Group)listBox.SelectedValue);
        }



        private void AcceptButton_Click(object sender, EventArgs e) {

            // Validación de inputs
            if (string.IsNullOrWhiteSpace(QuickAccessPanel.NameText)) {
                QuickAccessPanel.NameText = QuickAccessCreationPanel.DefaultName;
            }
            if (string.IsNullOrWhiteSpace(QuickAccessPanel.PathText)) {
                QuickAccessPanel.PathText = QuickAccessCreationPanel.DefaultPath;
            }
            if (string.IsNullOrWhiteSpace(QuickAccessPanel.GroupText)) {
                QuickAccessPanel.GroupText = QuickAccessCreationPanel.DefaultGroup;
                QuickAccessPanel.ColorBrush = QuickAccessCreationPanel.DefaultColor;
            }
            if (string.IsNullOrWhiteSpace(QuickAccessPanel.DescriptionText)) {
                QuickAccessPanel.DescriptionText = QuickAccessCreationPanel.DefaultDescription;
            }
            if (!QuickAccessPanel._ColorPicker.SelectedColor.HasValue) {
                QuickAccessPanel.ColorBrush = QuickAccessCreationPanel.DefaultColor;
            }

            Group group = new Group(QuickAccessPanel.GroupText, new SolidColorBrush(QuickAccessPanel.ColorBrush));
            QuickAccess new_qa = new QuickAccess(QuickAccessPanel.PathText, QuickAccessPanel.NameText, QuickAccessPanel.DescriptionText, group);

            if (EditingMode) {
                QuickAccessItems.Remove(SelectedQAToEdit);
            }

            // Comprueba que existe para evitar duplicidad
            if (!QuickAccessItems.Contains(new_qa)) {
                // comprueba si el grupo ha cambiado para actualizar el color si el usuario quiere
                if (EditingMode && !SelectedQAToEdit.Group.Equals(new_qa.Group) && QuickAccessItems.Select(qa => qa.Group).Any(gr => gr.Name == new_qa.Group.Name)) {
                    MessageBoxResult result = MessageBox.Show("¿Desea actualizar el color del grupo \"" + new_qa.Group.Name + "\" en el resto de accesos directos?", "Grupo modificado", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) {
                        foreach (var it in QuickAccessItems) {
                            if (it.Group.Name == new_qa.Group.Name) {
                                it.Group.Color = new_qa.Group.Color;
                            }
                        }
                    }
                }
                AddItem(new_qa);
                CloseCreationPanel();
            } else {
                MessageBox.Show("El acceso directo ya existe.", "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void CancelButton_Click(object sender, EventArgs e) {
            CloseCreationPanel();
            EditingMode = false;
            SelectedQAToEdit = null;
        }


        private void RemoveFilter_Click(object sender, EventArgs e) {
            QuickAccessItems = AuxiliarItems;
            AuxiliarItems = null;
            _CreationQuickAccess_Button.Visibility = Visibility.Visible;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            _FiltersListBox.UnselectAll();
        }

        #endregion

        #region Interface methods
        private void OpenCreationPanel(QuickAccess qa = null) {
            if (EditingMode) {
                QuickAccessPanel = new QuickAccessCreationPanel(qa);
            } else {
                QuickAccessPanel = new QuickAccessCreationPanel();
            }
            _CreationQuickAcess_Container.Children.Add(QuickAccessPanel);
            _CreationPanel_Buttons.Visibility = Visibility.Visible;
            _CreationQuickAccess_Button.Visibility = Visibility.Collapsed;
        }

        private void CloseCreationPanel() {

            _CreationQuickAcess_Container.Children.RemoveAt(_CreationQuickAcess_Container.Children.Count - 1);
            _CreationPanel_Buttons.Visibility = Visibility.Collapsed;
            _CreationQuickAccess_Button.Visibility = Visibility.Visible;
        }


        #endregion


        #region Auxiliar methods

        private void RemoveGroupIfNotExists(Group item) {
            if (!QuickAccessItems.Any(qa => qa.Group.Equals(item))) {
                GroupItems.Remove(item);
            }
        }

        private void AddItem(QuickAccess newItem) {
            QuickAccessItems.Add(newItem);
            QuickAccessItems = new ObservableCollection<QuickAccess>(OrderList(QuickAccessItems));
            UpdateGroupList();
            SaveChanges();
        }

        private void RemoveItem(QuickAccess item) {
            QuickAccessItems.Remove(item);
            UpdateGroupList();
            SaveChanges();
        }

        private void UpdateGroupList() {
            GroupItems = new ObservableCollection<Group>(QuickAccessItems.Select(qa => qa.Group).Distinct().OrderBy(gr => gr.Name));
        }

        private void ApplyFilter(Group filter) {

            if (AuxiliarItems == null) {
                AuxiliarItems = QuickAccessItems;
            }

            QuickAccessItems = new ObservableCollection<QuickAccess>(AuxiliarItems.Where(qa => qa.Group.Equals(filter)));
        }


        private void SaveChanges() {

            // TODO: control de los cambios ya que ahora se usan dos listas

            if (AuxiliarItems != null) {
                QuickAccessController.SaveChanges(AuxiliarItems);
                return;
            }
            QuickAccessController.SaveChanges(QuickAccessItems);
        }

        private IOrderedEnumerable<QuickAccess> OrderList(IList<QuickAccess> list) {
            return list.OrderBy(qa => qa.Group.Name).ThenBy(qa => qa.Name);

        }

        #endregion

    }
}
