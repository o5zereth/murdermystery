using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands.General
{
    public class Disable : ICommand
    {
        public string Command => "disable";

        public string[] Aliases => new string[] { "dis" };

        public string Description => "Disables the murder mystery gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.enable"))
            {
                response = "You don't have permission to disable the gamemode.";
                return false;
            }
            if (MurderMystery.Singleton.GamemodeManager.Started)
            {
                response = "The murder mystery gamemode is already active and cannot be disabled.";
                return false;
            }
            else if (!MurderMystery.Singleton.GamemodeManager.Enabled)
            {
                response = "The murder mystery gamemode is already disabled.";
                return false;
            }

            MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
            response = "The murder mystery gamemode has been disabled.";
            return true;
        }
    }
}
