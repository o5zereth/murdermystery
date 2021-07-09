using CommandSystem;
using MurderMystery.Extensions;
using RemoteAdmin;
using System;

namespace MurderMystery.Commands.Debug
{
    public class DeveloperIdCheck : ICommand
    {
        public string Command => "checkdevuser";

        public string[] Aliases => new string[] { "devuser" };

        public string Description => "Checks if your current UserID is listed in the murdermystery developer ids.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckDebugPermission())
            {
                response = "You don't have permission to execute debug commands.";
                return false;
            }

            if (!(sender is PlayerCommandSender))
            {
                response = "Only players can run this command.";
                return false;
            }

			if (MurderMystery.DeveloperUserIds.Contains((sender as CommandSender).SenderId))
			{
				response = "Your UserID is a developer id.";
				return true;
			}
			else
			{
				response = "Your UserID is not a developer id.";
				return false;
			}
        }
    }
}