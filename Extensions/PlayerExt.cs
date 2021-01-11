using MurderMystery.Utils;
using MurderMystery.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MurderMystery.Extensions
{
    public static class PlayerExt
    {
        public static IEnumerable<MMPlayer> Nones(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.None);
        }

        public static IEnumerable<MMPlayer> Spectators(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Spectator);
        }

        public static IEnumerable<MMPlayer> Innocents(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Innocent);
        }

        public static IEnumerable<MMPlayer> Murderers(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Murderer);
        }

        public static IEnumerable<MMPlayer> Detectives(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Detective);
        }

        public static IEnumerable<MMPlayer> AliveList(this IEnumerable<MMPlayer> list)
        {
            return list.Where(ply => ply.Role == MMRole.Detective || ply.Role == MMRole.Murderer || ply.Role == MMRole.Innocent);
        }

        public static bool IsAlive(this MMPlayer ply)
        {
            return (ply.Role == MMRole.Detective || ply.Role == MMRole.Murderer || ply.Role == MMRole.Innocent);
        }
    }
}