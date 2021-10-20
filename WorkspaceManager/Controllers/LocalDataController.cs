using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using WorkspaceManagerTool.Models;
using WorkspaceManagerTool.Utils;

namespace WorkspaceManagerTool.Controllers {
    public abstract class LocalDataController {

        public event EventHandler HandlerListImport;
        public event EventHandler UpdateListView;

        private ObservableCollection<GroupableResource> items;
        private ObservableCollection<Group> groups;

        public ObservableCollection<GroupableResource> Items {
            get {
                return items.AsQueryable().OrderBy(it => !it.Pinned).ThenBy(it => it.Group.Name).ThenBy(it => it.Name).ToObservableCollection();
            }
            set {
                items = value;
            }
        }

        public static ObservableCollection<Group> AllFilter {
            get {
                var all = new ObservableCollection<Group> {
                    CONSTANTS.AllGroup
                };
                return all;
            }
        }

        public ObservableCollection<Group> GroupItems {
            get {
                var groups = items.Select(it => it.Group).Distinct();
                groups = AllFilter.Concat(groups.OrderBy(gr => gr.Name));
                return groups.AsQueryable().ToObservableCollection();
            }
        }

        protected string ResourceDirectory {
            get {
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string directory = "WorkspaceManager";
                return Path.Combine(AppData, directory);
            }
        }
        protected abstract string ResourceFile { get; }

        #region Initialization data
        public abstract void Init();
        protected ObservableCollection<GroupableResource> ReadData<T>(string path) {
            var data = JSONManager.ReadJSON<List<T>>(path);

            if (data == null) {
                return new ObservableCollection<GroupableResource>();
            }

            return new ObservableCollection<GroupableResource>(data.Cast<GroupableResource>());
        }
        protected void WriteData() {
            JSONManager.SaveJSON(ResourceFile, items);
        }
        #endregion

        #region Lists actions
        public void Add(GroupableResource item) {
            items.Add(item);
            WriteData();
        }
        public void Remove(GroupableResource item) {
            items.Remove(item);
            WriteData();
        }
        public void Replace(GroupableResource oldItem, GroupableResource newItem) {
            items.Remove(oldItem);
            items.Add(newItem);
            WriteData();
        }
        public void Pin(GroupableResource res) {
            foreach (var item in items) {
                if (item.Equals(res)) {
                    item.Pinned = !res.Pinned;
                    WriteData();
                    return;
                }
            }
        }


        public void ChangeGroup(Group oldGroup, Group newGroup) {
            foreach (var item in items) {
                if (item.Group.Equals(oldGroup)) {
                    item.Group = newGroup;
                }
            }
            WriteData();
        }

        public void RemoveSelection(ObservableCollection<GroupableResource> selectionRemoved) {
            if (selectionRemoved.Count <= 0) {
                return;
            }
            items = items.AsQueryable().Except(selectionRemoved).ToObservableCollection(); ;
            WriteData();
        }

        public ObservableCollection<GroupableResource> SearchByName(string text) {
            return items.AsQueryable().Where(it => it.Name.ToLower().Contains(text.ToLower())).OrderBy(it => !it.Pinned).ToObservableCollection();
        }
        public ObservableCollection<GroupableResource> FilterByGroup(Group filter) {
            if (filter.Equals(CONSTANTS.AllGroup)) {
                return Items;
            }
            return items.AsQueryable().Where(qa => qa.Group.Equals(filter)).OrderBy(it => !it.Pinned).ToObservableCollection();
        }

        public void UpdateChangesInView() {
            UpdateListView?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Configuration actions
        public void Import<T>() {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "JSON File|*.json",
                Title = "Importar lista"
            };
            dialog.ShowDialog();
            if (dialog.FileName == "") {
                return;
            }
            // Confirmation
            DialogResult alert = MessageBox.Show("¿Seguro que desea sobreescribir los datos actuales?", "Importar nueva lista", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (alert == DialogResult.Cancel) {
                return;
            }
            // Read and check data 
            var tmp = ReadData<T>(dialog.FileName);
            if (!IsCorrectData<T>(tmp)) {
                MessageBox.Show("Los datos importados no son correctos", "Error al importar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            items = tmp;
            // Set changes in the view
            HandlerListImport?.Invoke(this, new EventArgs());
            // Set changes in the local file
            WriteData();
            ShowImportInformation(tmp);
        }

        public void Export(string filename) {
            SaveFileDialog dialog = new SaveFileDialog {
                Filter = "JSON File|*.json",
                Title = "Exportar lista",
                FileName = filename
            };
            dialog.ShowDialog();
            if (dialog.FileName == "") {
                return;
            }
            if (File.Exists(dialog.FileName)) {
                File.Delete(dialog.FileName);
            }
            File.Copy(ResourceFile, dialog.FileName);
        }
        public void ImportNewItems<T>() {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "JSON File|*.json",
                Title = "Importar lista"
            };
            dialog.ShowDialog();
            if (dialog.FileName == "") {
                return;
            }
            // Read and check data 
            var newItems = ReadData<T>(dialog.FileName);
            if (!IsCorrectData<T>(newItems)) {
                MessageBox.Show("Los datos importados no son correctos", "Error al importar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // joining both lists, discarding equal items
            items = new ObservableCollection<GroupableResource>(items.Concat(newItems).Distinct());
            // Set changes in the view
            HandlerListImport?.Invoke(this, new EventArgs());
            // Set changes in the local file
            WriteData();
            ShowImportInformation(newItems);
        }
        private void ShowImportInformation(ObservableCollection<GroupableResource> list) {
            var msg = String.Format("Total de elementos añadidos: {0}", list.Count);
            MessageBox.Show(msg, "Datos importados correctamente", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private bool IsCorrectData<T>(ObservableCollection<GroupableResource> list) {
            if (list == null || list.Count == 0) {
                return false;
            }
            if (typeof(T).Name == "Script") {
                return !list.Any(item => (item as Script).Commands == null);
            }
            if (typeof(T).Name == "QuickAccess") {
                return !list.Any(item => (item as QuickAccess).Path == null);
            }
            return false;
        }
        #endregion
    }
}
