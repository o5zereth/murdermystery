using System;
using CommandSystem;

namespace MurderMystery.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class GeneralCmds : ParentCommand
    {
		public GeneralCmds() => LoadGeneratedCommands();

        public override string Command => "murdermystery2";

        public override string[] Aliases => new string[1] { "mm2" };

        public override string Description => "Parent command for general murder mystery commands.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new General.Enable());
			RegisterCommand(new General.Disable());
			RegisterCommand(new General.ViewPlayers());
            RegisterCommand(new General.ForceEnd());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand. Type help mm2 for a list of subcommands.";
            return false;
        }
    }
}
