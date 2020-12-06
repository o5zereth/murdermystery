using Exiled.API.Enums;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using MurderMystery.Utils;
using System;

namespace MurderMystery
{
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "Zereth";
        public override string Name { get; } = "MurderMystery";
        public override string Prefix { get; } = "murder_mystery";
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 18);
        public override Version Version { get; } = new Version(1, 0, 0);

        public static EventHandlers EventHandlers { get; private set; }
        public static GamemodeStatus GamemodeStatus { get; private set; }

        public override void OnEnabled()
        {
            EventHandlers = new EventHandlers(this);
            GamemodeStatus = new GamemodeStatus();

            Handlers.Player.Joined += MMPlayer.Add;
            Handlers.Player.Left += MMPlayer.Remove;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            EventHandlers = null;
            GamemodeStatus = null;

            Handlers.Player.Joined -= MMPlayer.Add;
            Handlers.Player.Left -= MMPlayer.Remove;

            base.OnDisabled();
        }
    }
}