using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkspaceManagerTool.Models;
using WorkspaceManagerTool.Utils;

namespace WorkspaceManagerTool.Controllers {
    public abstract class LocalDataController {

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
        protected void ReadData<T>() {
            var data = JSONManager.ReadJSON<List<T>>(ResourceFile);

            if (data == null) {
                Items = new ObservableCollection<GroupableResource>();
                return;
            }

            Items = new ObservableCollection<GroupableResource>(data.Cast<GroupableResource>());
        }
        protected void WriteData() {
            JSONManager.SaveJSON(ResourceFile, Items);
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
