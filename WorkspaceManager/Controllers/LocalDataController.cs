using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkspaceManagerTool.Models;
using WorkspaceManagerTool.Utils;

namespace WorkspaceManagerTool.Controllers {
    public abstract class LocalDataController {

        public event EventHandler HandlerListImport;

        private ObservableCollection<GroupableResource> items;
        private ObservableCollection<Group> groups;

        public ObservableCollection<GroupableResource> Items {
            get {
                return OrderGroupableResources(items);
            }
            set {
                items = value;
            }
        }
        public ObservableCollection<Group> GroupItems {
            get {
                var groups = items.Select(it => it.Group).Distinct();
                return OrderGroups(groups);
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
            JSONManager.SaveJSON(ResourceFile, Items);
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

        public ObservableCollection<GroupableResource> SearchByName(string text) {
            return new ObservableCollection<GroupableResource>(Items.Where(it => it.Name.ToLower().Contains(text.ToLower())));
        }
        public ObservableCollection<GroupableResource> FilterByGroup(Group filter) {
            return new ObservableCollection<GroupableResource>(Items.Where(qa => qa.Group.Equals(filter)));
        }

        private ObservableCollection<Group> OrderGroups(IEnumerable<Group> grps) {
            return new ObservableCollection<Group>(grps.OrderBy(gr => gr.Name));
        }
        private ObservableCollection<GroupableResource> OrderGroupableResources(ObservableCollection<GroupableResource> itms) {
            return new ObservableCollection<GroupableResource>(itms.OrderBy(it => it.Group.Name).ThenBy(e => e.Name));
        }
        #endregion

        #region Configuration actions
        public void Import<T>() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON File|*.json";
            dialog.Title = "Importar lista";
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
            Items = tmp;
            // Set changes in the view
            HandlerListImport?.Invoke(this, new EventArgs());
            // Set changes in the local file
            WriteData();
            ShowImportInformation(tmp);
        }
        public void Export(string filename) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON File|*.json";
            dialog.Title = "Exportar lista";
            dialog.FileName = filename;
            dialog.ShowDialog();
            if (dialog.FileName == "") {
                return;
            }
            File.Copy(ResourceFile, dialog.FileName);
        }
        public void ImportNewItems<T>() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON File|*.json";
            dialog.Title = "Importar lista";
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
            var newList = new ObservableCollection<GroupableResource>(Items.Concat(newItems).Distinct());
            Items = OrderGroupableResources(newList);
            // Set changes in the view
            HandlerListImport?.Invoke(this, new EventArgs());
            // Set changes in the local file
            WriteData();
            ShowImportInformation(newList);
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
