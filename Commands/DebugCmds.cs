using System;
using CommandSystem;

namespace MurderMystery.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DebugCmds : ParentCommand
    {
		public DebugCmds() => LoadGeneratedCommands();

        public override string Command => "murdermysterydebug2";

        public override string[] Aliases => new string[2] { "mmdebug2", "mmd2" };

        public override string Description => "Parent command for debug murder mystery commands.";

        public override void LoadGeneratedCommands()
        {
			RegisterCommand(new Debug.GamemodeManagerData());
			RegisterCommand(new Debug.DeveloperIdCheck());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand. Type help mmd2 for a list of subcommands.";
            return false;
        }
    }
}
