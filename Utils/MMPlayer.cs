using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using MurderMystery.Enums;
using System;
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

        internal List<CoroutineHandle> Coroutines { get; } = new List<CoroutineHandle>();

        private static Dictionary<Player, MMPlayer> Dictionary { get; } = new Dictionary<Player, MMPlayer>();
        public static IEnumerable<MMPlayer> List => Dictionary.Values;
        public static IEnumerable<MMPlayer> Spectators => List.Where(ply => ply.Role == MMRole.Spectator);
        public static IEnumerable<MMPlayer> Innocents => List.Where(ply => ply.Role == MMRole.Innocent);
        public static IEnumerable<MMPlayer> Murderers => List.Where(ply => ply.Role == MMRole.Murderer);
        public static IEnumerable<MMPlayer> Detectives => List.Where(ply => ply.Role == MMRole.Detective);
        public static IEnumerable<MMPlayer> Alive => List.Where(ply => ply.Role != MMRole.Spectator && ply.Role != MMRole.None);
        public static IEnumerable<MMPlayer> Dead => List.Where(ply => ply.Role == MMRole.Spectator);


        internal static IEnumerator<float> SetupPlayers()
        {
            yield return Timing.WaitForSeconds(0.5f);


            void CalculateRoleCounts(int playercount, out int m, out int d)
            {
                if (playercount < 1 / Plugin.Singleton.Config.MurdererPercent)
                {
                    m = 1;
                }
                else
                {
                    m = (int)Math.Floor(playercount * Plugin.Singleton.Config.MurdererPercent);
                }
                if (playercount < 1 / Plugin.Singleton.Config.DetectivePercent)
                {
                    d = 1;
                }
                else
                {
                    d = (int)Math.Floor(playercount * Plugin.Singleton.Config.DetectivePercent);
                }
            }
        }

        internal static void Add(JoinedEventArgs ev)
        {
            Dictionary.Add(ev.Player, new MMPlayer(ev.Player));
            if (Plugin.GamemodeStatus.Active || Plugin.GamemodeStatus.RoundStarted)
            {
                Get(ev.Player).Role = MMRole.Spectator;
            }
        }
        internal static void Remove(LeftEventArgs ev)
        {
            Dictionary.Remove(ev.Player);
        }

        public static bool Get(Player player, out MMPlayer mmPlayer)
        {
            bool ret = Dictionary.TryGetValue(player, out mmPlayer);
            return ret;
        }
        public static MMPlayer Get(Player player)
        {
            if (Dictionary.TryGetValue(player, out MMPlayer mmPlayer))
            {
                return mmPlayer;
            }
            else
            {
                return null;
            }
        }
    }
}