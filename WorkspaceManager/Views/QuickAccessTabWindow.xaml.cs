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

        private ObservableCollection<Folder> items;
        private Folder selectedFile;

        public ObservableCollection<Folder> Items {
            get => items;
            set => SetProperty(ref items, value);
        }
        public ObservableCollection<Folder> Path {
            get => path;
            set => SetProperty(ref path, value);
        }

        public Folder SelectedFile {
            get => selectedFile;
            set {
                SetProperty(ref selectedFile, value);
            }
        }

        public QuickAccessTabWindow() {
            DataContext = this;
            InitializeComponent();

            Items = new ObservableCollection<Folder>();

            // Tests items
            Folder invox = new Folder("INVOX", "Workspace de invox");
            invox.AddSubFolders(new Folder("MisRecursos", "Carpeta de mis recursos"));
            invox.AddSubFolders(new Folder("SDK", "Workspace del sdk"));
            Items.Add(invox);

            Items.Add(new Folder("SERMAS", "Workspace de sermas"));
            Items.Add(new Folder("KALDI", "Workspace de kaldi"));
            Items.Add(new Folder("TOOLS", "Workspace del tools"));

            // Set Home item
            Path = new ObservableCollection<Folder>();
            Folder home = new Folder("HOME", "Volver al inicio");
            home.AddSubFolders(new List<Folder>(Items));
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
            Items = new ObservableCollection<Folder>();
            foreach (var item in selectedItem.Folders) {
                Items.Add(item);
            }
        }

        private void NewQuickAccess_Click(object sender, EventArgs e) {
        }

        private void NewFolder_Click(object sender, EventArgs e) {
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
            PathListBox.UnselectAll();

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
