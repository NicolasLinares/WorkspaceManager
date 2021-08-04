using System.Collections.Generic;
using System.Linq;

namespace WorkspaceManagerTool.Models.Deploys {
    class Deploy {

        public string Name => "kaldi";

        private List<Command> deployScript;

        public List<Command> FinalScript => deployScript;

        public string CommandList => string.Join("", deployScript);

        public Deploy() {
            deployScript = new List<Command>();
        }

        public void AddCommand(Command cmmd) {

            // Add or replace the command by type
            // (e.g: clean or fullclean, not both)
            if (deployScript.Any(c => c.Type == cmmd.Type)) {
                RemoveCommandByType(cmmd.Type);
            }

            // add and order by command type (execution order is important)
            deployScript.Add(cmmd);
            deployScript = deployScript.OrderBy(o => o.Type).ToList();
        }

        public void RemoveCommandByType(CommandType cmmdType) {
            deployScript.RemoveAll(c => c.Type == cmmdType);
        }

    }
}