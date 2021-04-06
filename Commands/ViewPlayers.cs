using CommandSystem;
using Exiled.Permissions.Extensions;
using MurderMystery.API;
using MurderMystery.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MurderMystery.Commands
{
    public class ViewPlayers : ICommand
    {
        public string Command { get; } = "viewplayers";

        public string[] Aliases { get; } = { "players", "plys" };

        public string Description { get; } = "Views all players and their roles when the gamemode is active.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.players"))
            {
                response = "You don't have permission to view players.";
                return false;
            }

            if (!MurderMystery.GamemodeManager.Enabled || !MurderMystery.GamemodeManager.Started)
            {
                response = "This command cannot be used unless the murder mystery gamemode is active.";
                return false;
            }

            List<MMPlayer> Nones = MMPlayer.List.Nones();
            List<MMPlayer> Spectators = MMPlayer.List.Spectators();
            List<MMPlayer> Innocents = MMPlayer.List.Innocents();
            List<MMPlayer> Murderers = MMPlayer.List.Murderers();
            List<MMPlayer> Detectives = MMPlayer.List.Detectives();

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("\nPlayers roles are listed below:");

            for (int i = 0; i < Nones.Count; i++)
            {
                stringBuilder.Append($"\n<size=25>[{MMRole.None.GetRoleAsColoredString()}] {Nones[i].Player.Nickname} ({Nones[i].Player.Id})</size>");
            }
            for (int i = 0; i < Spectators.Count; i++)
            {
                stringBuilder.Append($"\n<size=25>[{MMRole.Spectator.GetRoleAsColoredString()}] {Spectators[i].Player.Nickname} ({Spectators[i].Player.Id})</size>");
            }
            for (int i = 0; i < Innocents.Count; i++)
            {
                stringBuilder.Append($"\n<size=25>[{MMRole.Innocent.GetRoleAsColoredString()}] {Innocents[i].Player.Nickname} ({Innocents[i].Player.Id})</size>");
            }
            for (int i = 0; i < Murderers.Count; i++)
            {
                stringBuilder.Append($"\n<size=25>[{MMRole.Murderer.GetRoleAsColoredString()}] {Murderers[i].Player.Nickname} ({Murderers[i].Player.Id})</size>");
            }
            for (int i = 0; i < Detectives.Count; i++)
            {
                stringBuilder.Append($"\n<size=25>[{MMRole.Detective.GetRoleAsColoredString()}] {Detectives[i].Player.Nickname} ({Detectives[i].Player.Id})</size>");
            }

            response = stringBuilder.ToString();
            return true;
        }
    }
}