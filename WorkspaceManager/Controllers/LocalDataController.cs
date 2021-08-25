using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkspaceManagerTool.Controllers {
    public abstract class LocalDataController {

        protected string ResourceDirectory {
            get {
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string directory = "WorkspaceManagerTool";
                return Path.Combine(AppData, directory);
            }
        }

        protected abstract string ResourceFile { get; }

        public abstract void Init();

        protected abstract void ReadData();
        protected abstract void WriteData();

        public abstract void Add<T>(T obj);
        public abstract void Remove<T>(T obj);
        public abstract void Replace<T>(T oldObj, T newObj);
    }
}
