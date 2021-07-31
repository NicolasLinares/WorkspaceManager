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
            Folder invox = new Folder("INVOX", "Workspace de invox");
            invox.AddSubFolders(new Folder("MisRecursos", "Carpeta de mis recursos"));
            invox.AddSubFolders(new Folder("SDK", "Workspace del sdk"));
            ItemsFolder.Add(invox);

            ItemsFolder.Add(new Folder("SERMAS", "Workspace de sermas"));
            ItemsFolder.Add(new Folder("KALDI", "Workspace de kaldi"));
            ItemsFolder.Add(new Folder("TOOLS", "Workspace del tools"));


            ItemsQuickAccess.Add(new QuickAccess(@"C:\REPO\invox", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));
            ItemsQuickAccess.Add(new QuickAccess(@"C:\REPO\", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));
            ItemsQuickAccess.Add(new QuickAccess(@"\\nas.cloud.local\Audios\TESTS_ES\MutuaUniversal_ES\psicologia\nicolas.linares", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));
            ItemsQuickAccess.Add(new QuickAccess(@"C:\REPO\invox3", "Nuevo acceso directo", "Acceso directo creado el 31/07/2021"));


            // Set Home item

            Folder home = new Folder("HOME", "Volver al inicio");
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
        private void PathListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;

            if (listBox.Items.Count == 0 || listBox.SelectedValue == null) {
                return;
            }

            Folder selectedItem = (Folder)listBox.SelectedValue;
            _PathListBox.UnselectAll();

            // TODO: redefinir equals
            if (selectedItem.Name == ItemsPath.Last().Name)
                return;

            RemoveFolderFromPath(selectedItem);

            SetCurrentFolder(selectedItem);
        }
        private void FolderListBox_DoubleClick(object sender, MouseButtonEventArgs e) {
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
        private void QuickAccessListBox_DoubleClick(object sender, MouseButtonEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;
            if (listBox.Items.Count == 0 || listBox.SelectedValue == null) {
                return;
            }

            try {
                QuickAccessController.Open((QuickAccess) listBox.SelectedValue);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void QuickAccessListBox_RightClick(object sender, MouseButtonEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;
            if (listBox.Items.Count == 0 || listBox.SelectedValue == null) {
                return;
            }

            NewQuickAccessDialog dialog = new NewQuickAccessDialog((QuickAccess)listBox.SelectedValue);
            dialog.ShowDialog();

        }

        private void CreateQuickAccess_Click(object sender, EventArgs e) {

            NewQuickAccessDialog dialog = new NewQuickAccessDialog();
            dialog.ShowDialog();

            if (dialog.DialogResult == true) {
                QuickAccess qa = new QuickAccess(dialog.PathText, dialog.NameText, dialog.DescriptionText);
                ItemsQuickAccess.Add(qa);
                _ScrollView.ScrollToEnd();
            }

        }
        private void CreateFolder_Click(object sender, EventArgs e) {
            ItemsFolder.Add(new Folder("Nuevo", "carpeta nueva"));
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

        #endregion

    }
}
