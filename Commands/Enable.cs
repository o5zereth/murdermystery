using CommandSystem;
using System;

namespace MurderMystery.Commands
{
    public class Enable : ICommand
    {
        public string Command { get; } = "enable";

        public string[] Aliases { get; } = new string[] { "e" };

        public string Description { get; } = "Enables the murder mystery gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            throw new NotImplementedException();
        }
    }
}