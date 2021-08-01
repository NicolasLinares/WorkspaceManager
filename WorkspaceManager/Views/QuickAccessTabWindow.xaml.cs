using INVOXWorkspaceManager.Models.QuickAccess;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System;
using INVOXWorkspaceManager.Controllers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ookii.Dialogs.WinForms;

namespace INVOXWorkspaceManager.Views {

    /// <summary>
    /// Interaction logic for QuickAccessTabWindow.xaml
    /// </summary>
    public partial class QuickAccessTabWindow : UserControl, INotifyPropertyChanged {

        #region Properties and Constructor method

        private ObservableCollection<Folder> path;
        public ObservableCollection<Folder> ItemsPath {
            get => path;
            set => SetProperty(ref path, value);
        }

        private ObservableCollection<Folder> itemsFolder;
        public ObservableCollection<Folder> ItemsFolder {
            get => itemsFolder;
            set => SetProperty(ref itemsFolder, value);
        }

        private ObservableCollection<QuickAccess> itemsQuickAccess;
        public ObservableCollection<QuickAccess> ItemsQuickAccess {
            get => itemsQuickAccess;
            set => SetProperty(ref itemsQuickAccess, value);
        }

        private Folder selectedFolder;
        public Folder SelectedFolderItem {
            get => selectedFolder;
            set {
                SetProperty(ref selectedFolder, value);
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

            ItemsPath = new ObservableCollection<Folder>();
            ItemsFolder = new ObservableCollection<Folder>();
            ItemsQuickAccess = new ObservableCollection<QuickAccess>();

            // Tests items
            Folder invox = new Folder("INVOX", "Workspace de invox", new Color());
            invox.AddSubFolders(new Folder("MisRecursos", "Carpeta de mis recursos", new Color()));
            invox.AddSubFolders(new Folder("SDK", "Workspace del sdk", new Color()));
            ItemsFolder.Add(invox);

            ItemsFolder.Add(new Folder("SERMAS", "Workspace de sermas", new Color()));
            ItemsFolder.Add(new Folder("KALDI", "Workspace de kaldi", new Color()));
            ItemsFolder.Add(new Folder("TOOLS", "Workspace del tools", new Color()));


            ItemsQuickAccess.Add(new QuickAccess(@"C:\REPO\invox", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));
            ItemsQuickAccess.Add(new QuickAccess(@"C:\REPO\", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));
            ItemsQuickAccess.Add(new QuickAccess(@"\\nas.cloud.local\Audios\TESTS_ES\MutuaUniversal_ES\psicologia\nicolas.linares", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));
            ItemsQuickAccess.Add(new QuickAccess(@"C:\REPO\invox3", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));


            // Set Home item

            Folder home = new Folder("HOME", "Volver al inicio", new Color());
            home.AddSubFolders(new List<Folder>(ItemsFolder));
            home.AddQuickAccess(new List<QuickAccess>(ItemsQuickAccess));
            ItemsPath.Add(home);

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

        private void DelegateMouseWheelToScrollView(object sender, MouseWheelEventArgs e) {
            ListBox listBox = sender as ListBox;

            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = listBox
            };
            listBox.RaiseEvent(eventArg);
        }

        private void CreateQuickAccess_Click(object sender, EventArgs e) {

            NewQuickAccessDialog dialog = new NewQuickAccessDialog();

            if (dialog.ShowDialog() == true) {
                QuickAccess qa = new QuickAccess(dialog.PathText, dialog.NameText, dialog.DescriptionText + "\n\nFecha de creación: " + DateTime.Now);
                ItemsQuickAccess.Add(qa);
                _ScrollView.ScrollToEnd();
            }
        }
        private void EditQuickAccess_MenuClick(object sender, RoutedEventArgs e) {

            NewQuickAccessDialog dialog = new NewQuickAccessDialog(SelectedQuickAccessItem);

            if (dialog.ShowDialog() == true) {

                foreach (var item in ItemsQuickAccess.Where(qa => qa.Equals(selectedQuickAccess))) {
                    item.Path = dialog.PathText;
                    item.Name = dialog.NameText;
                    item.Description = dialog.DescriptionText;
                }
                ItemsQuickAccess = ItemsQuickAccess;
            }
        }
        private void RemoveQuickAccess_MenuClick(object sender, RoutedEventArgs e) {

            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el acceso directo de forma permanente?", "Eliminar acceso directo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                ItemsQuickAccess.Remove(SelectedQuickAccessItem);
                // TODO: Update json
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


        private void CreateFolder_Click(object sender, EventArgs e) {

            NewFolderDialog dialog = new NewFolderDialog();

            if (dialog.ShowDialog() == true) {
                Folder folder = new Folder(dialog.NameText, dialog.DescriptionText, dialog.Color);
                ItemsFolder.Add(folder);
                _FoldersListBox.ScrollIntoView(folder);
            }

        }
        private void NavegateFolders_DoubleClick(object sender, MouseButtonEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;

            if (listBox.Items.Count == 0) {
                return;
            }

            if (listBox.SelectedValue == null) {
                return;
            }

            Folder selectedItem = (Folder)listBox.SelectedValue;

            // Forma la ruta para volver luego
            AddFolderToPath(selectedItem);

            // Se muestra el contenido de la carpeta
            SetCurrentFolder(selectedItem);
        }


        private void UpdateNavigationPath_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;

            if (listBox.Items.Count == 0 || listBox.SelectedValue == null) {
                return;
            }

            Folder selectedItem = (Folder)listBox.SelectedValue;
            _PathListBox.UnselectAll();

            if (selectedItem.Equals(ItemsPath.Last()))
                return;

            RemoveFolderFromPath(selectedItem);

            SetCurrentFolder(selectedItem);
        }

        #endregion


        #region Auxiliar methods


        private void SetCurrentFolder(Folder selectedItem) {
            ItemsFolder = new ObservableCollection<Folder>();
            foreach (var item in selectedItem.Folders) {
                ItemsFolder.Add(item);
            }
        }
        private void AddFolderToPath(Folder selectedItem) {
            ItemsPath.Add(selectedItem);
        }
        private void RemoveFolderFromPath(Folder selectedItem) {
            int index_selected = ItemsPath.IndexOf(selectedItem);
            for (int index = ItemsPath.Count; index > index_selected + 1; index--) {
                ItemsPath.RemoveAt(index - 1);
            }
        }

        /// <summary>
        /// Finds the descendant of a dependency object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                if (!(child is T childType)) {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                } else if (!string.IsNullOrEmpty(childName)) {
                    // If the child's name is set for search
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName) {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    } else {
                        foundChild = FindChild<T>(child, childName);

                        // If the child is found, break so we do not overwrite the found child. 
                        if (foundChild != null) break;
                    }
                } else {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        #endregion

    }
}
