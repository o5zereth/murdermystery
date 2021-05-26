using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;
using System.Collections.Generic;

namespace MurderMystery.API
{
    public class MMPlayer
    {
        internal MMPlayer(Player player) => Player = player;

        public static List<MMPlayer> List { get; } = new List<MMPlayer>();

        public Player Player { get; }
        public MMRole Role { get; internal set; }

        public MMPlayer Get(Player player)
        {
            throw new NotImplementedException();
        }

        public bool Get(Player player, out MMPlayer ply)
        {
            throw new NotImplementedException();
        }

        internal MMPlayer Verified(VerifiedEventArgs ev)
        {
            // Add broadcasts.

            if (MurderMystery.Singleton.GamemodeManager.Started)
            {
                Role = MMRole.Spectator;
            }
            else
            {
                Role = MMRole.None;
            }

            return this;
        }

        internal MMPlayer Destroying(DestroyingEventArgs ev)
        {
            return this;
        }
    }
}
