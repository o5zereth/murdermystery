using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MurderMystery.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MurderMystery.Utils
{
    public class MMPlayer
    {
        private MMPlayer(Player player)
        {
            Player = player;
        }

        public Player Player { get; }
        public MMRole Role { get; private set; } = MMRole.None;

        private static Dictionary<Player, MMPlayer> Dictionary { get; } = new Dictionary<Player, MMPlayer>();
        public static IEnumerable<MMPlayer> List => Dictionary.Values;
        public static IEnumerable<MMPlayer> Spectators => List.Where(ply => ply.Role == MMRole.Spectator);
        public static IEnumerable<MMPlayer> Innocents => List.Where(ply => ply.Role == MMRole.Innocent);
        public static IEnumerable<MMPlayer> Murderers => List.Where(ply => ply.Role == MMRole.Murderer);
        public static IEnumerable<MMPlayer> Detectives => List.Where(ply => ply.Role == MMRole.Detective);
        public static IEnumerable<MMPlayer> Alive => List.Where(ply => ply.Role != MMRole.Spectator && ply.Role != MMRole.None);

        internal static void Add(JoinedEventArgs ev)
        {
            Dictionary.Add(ev.Player, new MMPlayer(ev.Player));
        }

        internal static void Remove(LeftEventArgs ev)
        {
            Dictionary.Remove(ev.Player);
        }
    }
}