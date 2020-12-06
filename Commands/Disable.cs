using CommandSystem;
using System;

namespace MurderMystery.Commands
{
    public class Disable : ICommand
    {
        public string Command { get; } = "disable";

        public string[] Aliases { get; } = new string[] { "d" };

        public string Description { get; } = "Disables the murder mystery gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            throw new NotImplementedException();
        }
    }
}