using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands
{
    public class Enable : ICommand
    {
        public string Command { get; } = "enable";

        public string[] Aliases { get; } = new string[] { "e" };

        public string Description { get; } = "Enables the murder mystery gamemode.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.enable"))
            {
                response = "You don't have permission to execute this command.";
                return false;
            }
            if (!Plugin.GamemodeStatus.Enabled)
            {
                Plugin.EventHandlers.EnablePrimary();
                response = "Gamemode was enabled successfully.";
                return true;
            }
            else
            {
                response = "The gamemode is already enabled!";
                return false;
            }
        }
    }
}