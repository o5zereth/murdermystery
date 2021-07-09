using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands.General
{
    public class Enable : ICommand
    {
        public string Command => "enable";

        public string[] Aliases => new string[] { "en" };

        public string Description => "Enables the murder mystery gamemode for the next round.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.enable"))
            {
                response = "You don't have permission to enable the gamemode.";
                return false;
            }
            if (MurderMystery.Singleton.GamemodeManager.Started)
            {
                response = "The murder mystery gamemode is already active.";
                return false;
            }
            else if (MurderMystery.Singleton.GamemodeManager.Enabled)
            {
                response = "The murder mystery gamemode is already enabled.";
                return false;
            }

            MurderMystery.Singleton.GamemodeManager.ToggleGamemode(true);
            response = "The murder mystery gamemode has been enabled for the next round.";
            return true;
        }
    }
}
