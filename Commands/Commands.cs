using CommandSystem;
using System;

namespace MurderMystery.Commands
{
    public class Commands : ParentCommand
    {
        public override string Command { get; } = "murdermystery";

        public override string[] Aliases { get; } = new string[] { "mm" };

        public override string Description { get; } = "Parent command for all murder mystery commands.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Enable());
            RegisterCommand(new Disable());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "You must provide a subcommand. Type mm help for a list of subcommands.";
            return false;
        }
    }
}