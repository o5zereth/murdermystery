using Exiled.API.Enums;
using Exiled.API.Features;
using MurderMystery.API;
using System;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config>
    {
        public override string Author => "Zereth";
        public override string Name => "MurderMystery";
        public override string Prefix => "murder_mystery";
        public override PluginPriority Priority => PluginPriority.Default;
        public override Version RequiredExiledVersion => new Version(2, 10, 0);
        public override Version Version => new Version(1, 0, 0);

        public static MurderMystery Singleton { get; private set; }

        public GamemodeManager GamemodeManager { get; private set; }

        public bool DebugVersion => true;
        public bool LogDebug => DebugVersion || Config.Debug;

        public override void OnEnabled()
        {
            Singleton = this;
            GamemodeManager = new GamemodeManager();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;
            GamemodeManager = null;

            base.OnDisabled();
        }
    }
}
