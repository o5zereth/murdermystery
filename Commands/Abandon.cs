using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands
{
    public class Abandon : ICommand
    {
        public string Command { get; } = "abandon";

        public string[] Aliases { get; } = { "forceend", "fend", "end" };

        public string Description { get; } = "Forcefully ends the murder mystery gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.forceend"))
            {
                response = "You don't have permission to forcefully end the murder mystery gamemode.";
                return false;
            }

            if (!MurderMystery.GamemodeManager.Enabled || !MurderMystery.GamemodeManager.Started)
            {
                response = "The murder mystery gamemode is not currently active.";
                return false;
            }

            MurderMystery.GamemodeManager.ForceRoundEnd = true;
            response = $"The murder mystery gamemode has been forcefully ended.";
            return true;
        }
    }
}