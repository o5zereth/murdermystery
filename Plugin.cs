using Exiled.API.Enums;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using MurderMystery.Utils;
using System;
using System.Collections.Generic;
using MEC;

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
        public bool Debug { get; internal set; } = true;
        public string VersionStr => $"<size=20>[Version: v{Version.Major}.{Version.Minor}.{Version.Build}] (Debug: {Debug})</size>";

        public static EventHandlers EventHandlers { get; private set; }
        public static GamemodeStatus GamemodeStatus { get; private set; }
        internal static Plugin Singleton { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers();
            GamemodeStatus = new GamemodeStatus();

            Handlers.Player.Joined += MMPlayer.Add;
            Handlers.Player.Left += MMPlayer.Remove;

            Config.Validate();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;
            EventHandlers = null;
            GamemodeStatus = null;

            Handlers.Player.Joined -= MMPlayer.Add;
            Handlers.Player.Left -= MMPlayer.Remove;

            base.OnDisabled();
        }
    }
}