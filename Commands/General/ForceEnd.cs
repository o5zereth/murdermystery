using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands.General
{
    public class ForceEnd : ICommand
    {
        public string Command => "abandon";

        public string[] Aliases => new string[] { "forceend" };

        public string Description => "Forcefully ends the murder mystery gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.forceend"))
            {
                response = "You don't have permission to forcefully end the gamemode.";
                return false;
            }
			if (!MurderMystery.Singleton.GamemodeManager.Enabled)
			{
				response = "The murder mystery gamemode is not enabled.";
				return false;
			}
            if (!MurderMystery.Singleton.GamemodeManager.Started)
            {
                response = "The murder mystery gamemode has not started.";
                return false;
            }

            RoundSummary.RoundLock = false;
            MurderMystery.Singleton.GamemodeManager.ForcingRoundEnd = true;
            response = "The murder mystery gamemode has been forcefully ended.";
            return true;
        }
    }
}
