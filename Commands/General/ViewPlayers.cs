using CommandSystem;
using Exiled.Permissions.Extensions;
using System.Text;
using System;
using MurderMystery.API;
using MurderMystery.Extensions;

namespace MurderMystery.Commands.General
{
	public class ViewPlayers : ICommand
	{
		public string Command { get; } = "viewplayers";

		public string[] Aliases { get; } = new string[3] { "players", "vplayers", "vplys" };

		public string Description { get; } = "Views all players and their roles.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission("mm.players") && !sender.CheckDebugPermission())
			{
				response = "You don't have permission to view players.";
				return false;
			}
			if (!MurderMystery.Singleton.GamemodeManager.Enabled)
			{
				response = "The murder mystery gamemode is not enabled.";
				return false;
			}
			if (!MurderMystery.Singleton.GamemodeManager.Started)
			{
				response = "The murder mystery gamemode is not currently active.";
				return false;
			}

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine("\n<size=25>Player roles:");

			for (int i = 0; i < MMPlayer.List.Count; i++)
			{
				stringBuilder.AppendLine($"{MMPlayer.List[i].Player.Nickname} ({MMPlayer.List[i].Player.Id}): {MMPlayer.List[i].Role.ToColoredString()}");
			}

			stringBuilder.Append("</size>");

			response = stringBuilder.ToString();
			return true;
		}
	}
}