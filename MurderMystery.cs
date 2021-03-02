using Exiled.API.Enums;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using HarmonyLib;
using System;
using MurderMystery.API;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config>
    {
        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(2, 3, 4);
        public override Version Version => new Version(1, 0, 0);
        public bool Debug { get; } = true;

        public static MurderMystery Singleton { get; private set; }
        internal static EventHandlers EventHandlers { get; private set; }
        internal static GamemodeManager GamemodeManager { get; private set; }
        internal static CoroutineManager CoroutineManager { get; private set; }
        internal static Harmony Harmony { get; private set; }
        internal static string VersionStr => $"[Version: {Singleton.Version.Major}.{Singleton.Version.Minor}.{Singleton.Version.Build} Public Alpha] (Debug: {Singleton.Debug})";

        private bool reloading = false;

        public override void OnEnabled()
        {
            Config.Validate(Config);

            if (reloading) { base.OnEnabled(); reloading = false; return; }

            Harmony = new Harmony("zereth.plugins.murdermystery");
            Harmony.PatchAll();

            Handlers.Player.Verified += MMPlayer.Add;
            Handlers.Player.Destroying += MMPlayer.Remove;
            Handlers.Server.RestartingRound += MMPlayer.RemoveAll;

            Singleton = this;
            EventHandlers = new EventHandlers();
            GamemodeManager = new GamemodeManager();
            CoroutineManager = new CoroutineManager();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            if (reloading) { base.OnDisabled(); return; }

            Harmony.UnpatchAll();
            Harmony = null;

            Handlers.Player.Verified -= MMPlayer.Add;
            Handlers.Player.Destroying -= MMPlayer.Remove;
            Handlers.Server.RestartingRound -= MMPlayer.RemoveAll;

            Singleton = null;
            EventHandlers = null;
            GamemodeManager = null;
            CoroutineManager = null;

            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            reloading = true;

            base.OnReloaded();
        }
    }
}