using CommandSystem;
using Exiled.API.Features;
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

            if (MurderMystery.GamemodeManager.ForceRoundEnd)
            {
                Log.Debug("ForceRoundEnd command is being used twice, this may cause unexpected behavior!");
                MurderMystery.GamemodeManager.EnableSecondary(false);
                response = "Attempting to disable the event mid-round... (Note: This is a last resort to forcefully end the gamemode.)";
                return true;
            }

            MurderMystery.GamemodeManager.ForceRoundEnd = true;
            response = $"The murder mystery gamemode should be forcefully ended.\n(Ensure that roundlock is disabled.)\n[If this fails and roundlock is disabled, use the command again.]";
            return true;
        }
    }
}