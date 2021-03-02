using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands
{
    class Disable : ICommand
    {
        public string Command { get; } = "disable";

        public string[] Aliases { get; } = { "dis" };

        public string Description { get; } = "Disables the gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.enable"))
            {
                response = "You don't have permission to disable the murder mystery gamemode.";
                return false;
            }

            if (!MurderMystery.GamemodeManager.Enabled)
            {
                response = "The murder mystery gamemode is already disabled.";
                return false;
            }

            if (MurderMystery.GamemodeManager.Started)
            {
                response = "The murder mystery gamemode is currently active, and cannot be disabled.";
                return false;
            }

            MurderMystery.GamemodeManager.EnableGamemode(false);
            response = $"The murder mystery gamemode has been disabled.";
            return true;
        }
    }
}