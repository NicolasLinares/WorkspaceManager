using System;

namespace WorkspaceManagerTool.Exceptions {

    [Serializable]
    class NotScriptsInWorkspaceException : Exception {
        public NotScriptsInWorkspaceException(string message)
        : base(message) { }

    }
}
