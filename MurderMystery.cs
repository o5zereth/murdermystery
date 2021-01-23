using Exiled.API.Enums;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using MurderMystery.Utils;
using HarmonyLib;
using System;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config>
    {
        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(2, 1, 28);
        public override Version Version => new Version(0, 1, 0);
        public bool Debug { get; } = true;

        internal static MurderMystery Singleton { get; private set; }
        internal static EventHandlers EventHandlers { get; private set; }
        internal static GamemodeStatus GamemodeStatus { get; private set; }
        internal static Harmony Harmony { get; private set; }
        internal static string VersionStr => $"[Version: {Singleton.Version.Major}.{Singleton.Version.Minor}.{Singleton.Version.Build} Public Alpha] (Debug: {Singleton.Debug})";

        private bool reloading = false;

        public override void OnEnabled()
        {
            Config.Validate(this.Config);

            if (reloading) { base.OnEnabled(); reloading = false; return; }

            Harmony = new Harmony("zereth.plugins.murdermystery");
            Harmony.PatchAll();

            Handlers.Player.Joined += MMPlayer.Add;
            Handlers.Player.Left += MMPlayer.Remove;
            Handlers.Server.RestartingRound += MMPlayer.RemoveAll;

            Singleton = this;
            EventHandlers = new EventHandlers();
            GamemodeStatus = new GamemodeStatus();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            if (reloading) { base.OnDisabled(); return; }
                
            Handlers.Player.Joined -= MMPlayer.Add;
            Handlers.Player.Left -= MMPlayer.Remove;
            Handlers.Server.RestartingRound -= MMPlayer.RemoveAll;

            Singleton = null;
            EventHandlers = null;
            GamemodeStatus = null;

            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            reloading = true;

            base.OnReloaded();
        }
    }
}