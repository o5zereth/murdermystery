using CommandSystem;
using System;

namespace MurderMystery.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class MurderMysteryCmds : ParentCommand
    {
        public MurderMysteryCmds() => LoadGeneratedCommands();

        public override string Command { get; } = "murdermystery";

        public override string[] Aliases { get; } = { "mm" };

        public override string Description { get; } = "Parent command for murder mystery gamemode.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Enable());
            RegisterCommand(new Disable());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand. Type help mm for a list of subcommands.";
            return false;
        }
    }
}