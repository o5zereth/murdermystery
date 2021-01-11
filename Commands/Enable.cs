using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands
{
    class Enable : ICommand
    {
        public string Command => "enable";

        public string[] Aliases => new string[] { "en" };

        public string Description => "Enables the gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.enable"))
            {
                response = "You don't have permission to enable the murder mystery gamemode.";
                return false;
            }

            if (MurderMystery.GamemodeStatus.Enabled && !MurderMystery.GamemodeStatus.Started)
            {
                response = "The murder mystery gamemode is already enabled.";
                return false;
            }
            else if (MurderMystery.GamemodeStatus.Started)
            {
                response = "The murder mystery gamemode is currently active.";
                return false;
            }

            MurderMystery.EventHandlers.EnableGamemode();
            response = $"The murder mystery gamemode has been enabled{(MurderMystery.Singleton.Config.RequireRoundRestart ? " for the next round" : "")}.";
            return true;
        }
    }
}