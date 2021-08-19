using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkspaceManagerTool.Interfaces {
    interface IController {

        void Init();

        void ReadData();

        void WriteData();
    }
}
