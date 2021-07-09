using CommandSystem;
using MurderMystery.Extensions;
using System;

namespace MurderMystery.Commands.Debug
{
    public class SpawnDetectiveWeapon : ICommand
    {
        public string Command => "spawndetectiveweapon";

        public string[] Aliases => new string[] { "spawndetwep", "sdw" };

        public string Description => "Spawns a detective weapon on the player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckDebugPermission())
            {
                response = "You don't have permission to execute debug commands.";
                return false;
            }

			response = "This command is not implemented.";
			return false;
        }
    }
}