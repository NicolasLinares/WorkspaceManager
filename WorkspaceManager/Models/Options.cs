namespace WorkspaceManagerTool.Models {

    public enum ExecutionOption {
        KeepOpenAfterFinish = 0,
        CloseAfterFinish = 1,
        BackgroundExecution = 2,
    }

    public class Options {

        public ExecutionOption ExecOption { get; set; } = ExecutionOption.KeepOpenAfterFinish;

        public Options(ExecutionOption exOp) {
            ExecOption = exOp;
        }

    }
}