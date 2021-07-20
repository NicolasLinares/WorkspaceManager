using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Exceptions {

    [Serializable]
    class NotScriptsInWorkspaceException : Exception {
        public NotScriptsInWorkspaceException(string message)
        : base(message) { }

    }
}
