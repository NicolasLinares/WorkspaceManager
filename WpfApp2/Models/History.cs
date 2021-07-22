using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVOXWorkspaceManager.Model
{
    class History
    {
        private static History _instance;
        private List<WorkSpace> deploymentHistory;

        private History() {
            deploymentHistory = new List<WorkSpace>();
        }

        public static History GetInstance() {
            if (_instance == null) {
                _instance = new History();
            }
            return _instance;
        }

        public void Add(WorkSpace d) {
            deploymentHistory.Add(d);
        }


    }
}
