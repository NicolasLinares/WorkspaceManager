using System;

namespace INVOXWorkspaceManager.Exceptions {

    [Serializable]
    class NotScriptsInWorkspaceException : Exception {
        public NotScriptsInWorkspaceException(string message)
        : base(message) { }

    }
}
