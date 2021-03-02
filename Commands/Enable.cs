using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands
{
    class Enable : ICommand
    {
        public string Command { get; } = "enable";

        public string[] Aliases { get; } = { "en" };

        public string Description { get; } = "Enables the gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.enable"))
            {
                response = "You don't have permission to enable the murder mystery gamemode.";
                return false;
            }

            if (MurderMystery.GamemodeManager.Enabled && !MurderMystery.GamemodeManager.Started)
            {
                response = "The murder mystery gamemode is already enabled.";
                return false;
            }
            else if (MurderMystery.GamemodeManager.Started)
            {
                response = "The murder mystery gamemode is currently active.";
                return false;
            }

            MurderMystery.GamemodeManager.EnableGamemode();
            response = $"The murder mystery gamemode has been enabled{(MurderMystery.Singleton.Config.RequireRoundRestart ? " for the next round" : "")}.";
            return true;
        }
    }
}