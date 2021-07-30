using INVOXWorkspaceManager.Models.QuickAccess;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System;

namespace INVOXWorkspaceManager.Views {

    /// <summary>
    /// Interaction logic for QuickAccessTabWindow.xaml
    /// </summary>
    public partial class QuickAccessTabWindow : UserControl, INotifyPropertyChanged {

        private ObservableCollection<Folder> path;
        public ObservableCollection<Folder> Path {
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


        public QuickAccessTabWindow() {
            DataContext = this;
            InitializeComponent();

            ItemsFolder = new ObservableCollection<Folder>();

            // Tests items
            Folder invox = new Folder("INVOX", "Workspace de invox");
            invox.AddSubFolders(new Folder("MisRecursos", "Carpeta de mis recursos"));
            invox.AddSubFolders(new Folder("SDK", "Workspace del sdk"));
            ItemsFolder.Add(invox);

            ItemsFolder.Add(new Folder("SERMAS", "Workspace de sermas"));
            ItemsFolder.Add(new Folder("KALDI", "Workspace de kaldi"));
            ItemsFolder.Add(new Folder("TOOLS", "Workspace del tools"));

            // Set Home item
            Path = new ObservableCollection<Folder>();
            Folder home = new Folder("HOME", "Volver al inicio");
            home.AddSubFolders(new List<Folder>(ItemsFolder));
            Path.Add(home);

        }

        #region Folders and Quick access logic

        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
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

        private void SetCurrentFolder(Folder selectedItem) {
            ItemsFolder = new ObservableCollection<Folder>();
            foreach (var item in selectedItem.Folders) {
                ItemsFolder.Add(item);
            }
        }

        private void NewQuickAccess_Click(object sender, EventArgs e) {
            NewQuickAccessDialog dialog = new NewQuickAccessDialog();

            dialog.ShowDialog();

            if (dialog.DialogResult == true) {
                QuickAccess qa = new QuickAccess(dialog.PathText, dialog.NameText, dialog.DescriptionText);

                //TODO: add to q. a. list 
                //TODO: Save data in Json
            }
        }

        private void NewFolder_Click(object sender, EventArgs e) {
            ItemsFolder.Add(new Folder("Nuevo", "carpeta nueva"));
            _FoldersListBox.SelectedIndex = ItemsFolder.Count() -1;
            _FoldersListBox.ScrollIntoView(_FoldersListBox.Items.CurrentItem);
        }

        #endregion


        #region Button Back logic (Path)

        private void ListBoxSelectionPathChanged(object sender, SelectionChangedEventArgs e) {
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
            _PathListBox.UnselectAll();

            // TODO: redefinir equals
            if (selectedItem.Name == Path.Last().Name)
                return;

            RemoveFolderFromPath(selectedItem);

            SetCurrentFolder(selectedItem);
        }

        private void AddFolderToPath(Folder selectedItem) {
            Path.Add(selectedItem);
        }

        private void RemoveFolderFromPath(Folder selectedItem) {
            int index_selected = Path.IndexOf(selectedItem);
            for (int index = Path.Count; index > index_selected + 1; index--) {
                Path.RemoveAt(index-1);
            }
        }


        #endregion

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
    }
}
