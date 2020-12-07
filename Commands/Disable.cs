using CommandSystem;
using Exiled.Permissions.Extensions;
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
            if (!sender.CheckPermission("mm.enable"))
            {
                response = "You don't have permission to execute this command.";
                return false;
            }
            if (!Plugin.GamemodeStatus.Enabled)
            {
                response = "The gamemode is already disabled!";
                return false;
            }
            else if (!Plugin.GamemodeStatus.RoundStarted && !Plugin.GamemodeStatus.Active)
            {
                Plugin.EventHandlers.EnablePrimary(false);
                response = "Gamemode was disabled successfully.";
                return true;
            }
            else
            {
                response = "You can't disable the gamemode if it has already started!";
                return false;
            }
        }
    }
}