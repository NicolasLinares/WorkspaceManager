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
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string directory = "WorkspaceManagerTool";
                return Path.Combine(AppData, directory);
            }
        }
        protected abstract string ResourceFile { get; }

        public abstract void Init();
        protected void ReadData<T>(string path) {
            var data = JSONManager.ReadJSON<List<T>>(path);

            if (data == null) {
                Items = new ObservableCollection<GroupableResource>();
                return;
            }

            Items = new ObservableCollection<GroupableResource>(data.Cast<GroupableResource>());
        }
        protected void WriteData() {
            JSONManager.SaveJSON(ResourceFile, Items);
        }

        public void Import<T>() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON File|*.json";
            dialog.Title = "Importar lista";
            dialog.ShowDialog();
            if (dialog.FileName != "") {
                DialogResult alert = MessageBox.Show("¿Seguro que desea sobreescribir los datos actuales?", "Importar nueva lista", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (alert == DialogResult.OK) {
                    ReadData<T>(dialog.FileName);
                    // Set changes in the view
                    HandlerListImport?.Invoke(this, new EventArgs());
                    // Set changes in the local file
                    File.Replace(dialog.FileName, ResourceFile, Path.GetTempFileName());
                }
            }
        }

        public void Export(string filename) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON File|*.json";
            dialog.Title = "Exportar lista";
            dialog.FileName = filename;
            dialog.ShowDialog();
            if (dialog.FileName != "") {
                File.Copy(ResourceFile, dialog.FileName);
            }
        }

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
    }
}
