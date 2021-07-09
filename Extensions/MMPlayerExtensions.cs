using System.Collections.Generic;
using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MurderMystery.API;

namespace MurderMystery.Extensions
{
    public static class MMPlayerExtensions
    {
		public static List<MMPlayer> GetRole(this List<MMPlayer> list, MMRole role)
		{
			List<MMPlayer> ret = new List<MMPlayer>();

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Role == role)
				{
					ret.Add(list[i]);
				}
			}

			return ret;
		}

		public static int GetRoleCount(this List<MMPlayer> list, MMRole role)
		{
			int ret = 0;

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Role == role)
				{
					ret++;
				}
			}

			return ret;
		}

		public static List<MMPlayer> GetRoles(this List<MMPlayer> list, params MMRole[] roles)
		{
			if (roles.Length == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(roles));
			}

			List<MMPlayer> ret = new List<MMPlayer>();

			for (int i = 0; i < list.Count; i++)
			{
				for (int role = 0; role < roles.Length; role++)
				{
					if (list[i].Role == roles[role])
					{
						ret.Add(list[i]);
						break;
					}
				}
			}

			return ret;
		}

		public static int GetRolesCount(this List<MMPlayer> list, params MMRole[] roles)
		{
			if (roles.Length == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(roles));
			}

			int ret = 0;

			for (int i = 0; i < list.Count; i++)
			{
				for (int role = 0; role < roles.Length; role++)
				{
					if (list[i].Role == roles[role])
					{
						ret++;
						break;
					}
				}
			}

			return ret;
		}

		public static bool CheckDebugPermission(this ICommandSender sender)
		{
			CommandSender commandSender = sender as CommandSender;

			return commandSender.CheckPermission("mm.debug") || ((MurderMystery.Singleton.DebugVersion || MurderMystery.Singleton.Config.DeveloperDebugAccess) && MurderMystery.DeveloperUserIds.Contains(commandSender.SenderId));
		}

		public static string GetRoleInfo(this MMRole role)
		{
			switch (role)
			{
				case MMRole.None:
					return "<size=30>You have no role.</size>";
				case MMRole.Spectator:
					return "<size=30>You are in overwatch and have been set to spectator.</size>";
				case MMRole.Innocent:
					return "<size=30>You are an <color=#00ff00>Innocent</color>.\nYou must <color=#ff00ff>survive</color>, and avoid <color=#ff0000>Murderers</color>.</size>";
				case MMRole.Murderer:
					return "<size=30>You are a <color=#ff0000>Murderer</color>.\nYou must <color=#ff0000>kill all</color> <color=#00ff00>innocents</color> and <color=#0000ff>detectives</color>.</size>";
				case MMRole.Detective:
					return "<size=30>You are a <color=#0000ff>Detective</color>.\nYou must <color=#ff0000>kill all murderers</color> and <color=#00ff00>protect innocents</color>.</size>";
				default:
					return null;
			}
		}

		public static string ToColoredString(this MMRole role)
		{
			switch (role)
			{
				case MMRole.None:
					return "<color=#000000>None</color>";
				case MMRole.Spectator:
					return "<color=#7f7f7f>Spectator</color>";
				case MMRole.Innocent:
					return "<color=#00ff00>Innocent</color>";
				case MMRole.Murderer:
					return "<color=#ff0000>Murderer</color>";
				case MMRole.Detective:
					return "<color=#0000ff>Detective</color>";
				default:
					return "Unknown Role";
			}
		}
    }
}