using MurderMystery.API;
using System.Collections.Generic;

namespace MurderMystery.Extensions
{
    public static class PlayerExt
    {
        public static List<MMPlayer> OfRole(this List<MMPlayer> list, MMRole role)
        {
            List<MMPlayer> output = new List<MMPlayer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == role)
                {
                    output.Add(list[i]);
                }
            }

            return output;
        }

        public static List<MMPlayer> AliveList(this List<MMPlayer> list)
        {
            List<MMPlayer> output = new List<MMPlayer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Innocent || list[i].Role == MMRole.Murderer || list[i].Role == MMRole.Detective)
                {
                    output.Add(list[i]);
                }
            }

            return output;
        }

        public static int OfRoleCount(this List<MMPlayer> list, MMRole role)
        {
            int output = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == role)
                {
                    output++;
                }
            }

            return output;
        }

        public static int AliveListCount(this List<MMPlayer> list)
        {
            int output = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Innocent || list[i].Role == MMRole.Murderer || list[i].Role == MMRole.Detective)
                {
                    output++;
                }
            }

            return output;
        }

        public static bool IsAlive(this MMPlayer ply)
        {
            return (ply.Role == MMRole.Innocent || ply.Role == MMRole.Murderer || ply.Role == MMRole.Detective);
        }

        public static string GetRoleAsColoredString(this MMRole role)
        {
            switch (role)
            {
                case MMRole.None:
                    return "<color=#ffffff>None</color>";
                case MMRole.Spectator:
                    return "<color=#7f7f7f>Spectator</color>";
                case MMRole.Innocent:
                    return "<color=#00ff00>Innocent</color>";
                case MMRole.Murderer:
                    return "<color=#ff0000>Murderer</color>";
                case MMRole.Detective:
                    return "<color=#0000ff>Detective</color>";
                default:
                    return null;
            }
        }
    }
}