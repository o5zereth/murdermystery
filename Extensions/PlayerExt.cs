using MurderMystery.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MurderMystery.Extensions
{
    public static class PlayerExt
    {
        [Obsolete("Removed for testing purposes.", true)]
        public static IEnumerable<MMPlayer> Nones(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.None);
        }

        [Obsolete("Removed for testing purposes.", true)]
        public static IEnumerable<MMPlayer> Spectators(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Spectator);
        }

        [Obsolete("Removed for testing purposes.", true)]
        public static IEnumerable<MMPlayer> Innocents(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Innocent);
        }

        [Obsolete("Removed for testing purposes.", true)]
        public static IEnumerable<MMPlayer> Murderers(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Murderer);
        }

        [Obsolete("Removed for testing purposes.", true)]
        public static IEnumerable<MMPlayer> Detectives(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Detective);
        }

        [Obsolete("Removed for testing purposes.", true)]
        public static IEnumerable<MMPlayer> AliveList(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Innocent || ply.Role == MMRole.Murderer || ply.Role == MMRole.Detective);
        }

        public static bool IsAlive(this MMPlayer ply)
        {
            return (ply.Role == MMRole.Innocent || ply.Role == MMRole.Murderer || ply.Role == MMRole.Detective);
        }

        public static List<MMPlayer> Nones(this List<MMPlayer> list)
        {
            List<MMPlayer> output = new List<MMPlayer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.None)
                {
                    output.Add(list[i]);
                }
            }

            return output;
        }

        public static List<MMPlayer> Spectators(this List<MMPlayer> list)
        {
            List<MMPlayer> output = new List<MMPlayer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Spectator)
                {
                    output.Add(list[i]);
                }
            }

            return output;
        }

        public static List<MMPlayer> Innocents(this List<MMPlayer> list)
        {
            List<MMPlayer> output = new List<MMPlayer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Innocent)
                {
                    output.Add(list[i]);
                }
            }

            return output;
        }

        public static List<MMPlayer> Murderers(this List<MMPlayer> list)
        {
            List<MMPlayer> output = new List<MMPlayer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Murderer)
                {
                    output.Add(list[i]);
                }
            }

            return output;
        }

        public static List<MMPlayer> Detectives(this List<MMPlayer> list)
        {
            List<MMPlayer> output = new List<MMPlayer>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Detective)
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

        public static int NonesCount(this List<MMPlayer> list)
        {
            int output = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.None)
                {
                    output++;
                }
            }

            return output;
        }

        public static int SpectatorsCount(this List<MMPlayer> list)
        {
            int output = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Spectator)
                {
                    output++;
                }
            }

            return output;
        }

        public static int InnocentsCount(this List<MMPlayer> list)
        {
            int output = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Innocent)
                {
                    output++;
                }
            }

            return output;
        }

        public static int MurderersCount(this List<MMPlayer> list)
        {
            int output = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Murderer)
                {
                    output++;
                }
            }

            return output;
        }

        public static int DetectivesCount(this List<MMPlayer> list)
        {
            int output = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Role == MMRole.Detective)
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