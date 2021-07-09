using CommandSystem;
using Exiled.Permissions.Extensions;
using MurderMystery.Extensions;
using System.Text;
using System;

namespace MurderMystery.Commands.Debug
{
    public class GamemodeManagerData : ICommand
    {
        public string Command => "gamemodemanager";

        public string[] Aliases => new string[] { "gamemodemng", "gmm" };

        public string Description => "Views data values of the GamemodeManager class.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckDebugPermission())
            {
                response = "You don't have permission to execute debug commands.";
                return false;
            }
            
            response = MurderMystery.Singleton.GamemodeManager.GetData();
			return true;
        }
    }
}