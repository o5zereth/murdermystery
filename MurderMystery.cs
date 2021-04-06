using Exiled.API.Enums;
using Exiled.API.Features;
using MurderMystery.API;
using System;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery
{
    public class MurderMystery : Plugin<Config>
    {
        public override string Author { get; } = "Zereth";
        public override string Name { get; } = "MurderMystery";
        public override string Prefix { get; } = "murder_mystery";
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public override Version RequiredExiledVersion { get; } = new Version(2, 8, 0);
        public override Version Version { get; } = new Version(1, 0, 0);
        public bool Debug { get; } = true;
        public bool DebugVERBOSE { get; } = true;

        public static MurderMystery Singleton { get; private set; }
        internal static EventHandlers EventHandlers { get; private set; }
        internal static GamemodeManager GamemodeManager { get; private set; }
        internal static CoroutineManager CoroutineManager { get; private set; }
        internal static CompatibilityManager CompatabilityManager { get; private set; }
        internal static string VersionStr => $"[Version: {Singleton.Version.Major}.{Singleton.Version.Minor}.{Singleton.Version.Build}-privatealpha] (Debug: {Singleton.Debug})";

        /*public readonly string InfoStr = "\n" +
            "<color=#ffffff>" +
                "Welcome to the murder mystery gamemode! Here are some simple mechanics to get you started:\n\n" +

                "<size=30><b>Roles:</b></size>\n\n" +
                    
                    "  <i>Note: All players will spawn with painkillers by default.</i>\n\n" +
                    
                    "  <color=#00ff00>Innocent:</color>\n" +
                    "  As an innocent, your main goal is to survive! Some optional objectives would be to observe others and gain information to inform the detective, as well as finding the detectives weapon if he dies!\n" +
                  //"  Be careful when hiding! If hiding for too long in a given area you will be prompted to move from it, if not then you will be killed for camping!\n" + not implemented
                    "  <color=#00ff00>This role will spawn with no extra items.</color>\n\n" +
                    
                    "  <color=#ff0000>Murderer:</color>\n" +
                    "  As a murderer, your main goal is to kill all the others! You must cooperate with your team, you are outnumbered. <i>Create strategies, cover up deaths, lead others into traps, and kill your foes!</i>\n" +
                    "  <color=#ff0000>This role will spawn with a red keycard, silenced pistol, and an SCP-268.</color>\n\n" +
                
                    "  <color=#0000ff>Detective:</color>\n" +
                    "  As a detective, your main goal is to kill the murderers! You must watch your back, Make sure to gain as much information as possible! <i>Who can you trust?</i>\n" +
                    "  Be careful when shooting others! When killing an innocent you will become blinded and unable to pickup the gun for 30 seconds! Make sure you're shooting a murderer.\n" +
                  //"  Be careful when hiding! If hiding for too long in a given area you will be prompted to move from it, if not then you will be killed for camping!\n" + not implemented
                    "  <color=#0000ff>This role will spawn with a blue keycard, regular pistol, and a medkit.</color>\n\n" +
                
                "<size=30><b>Important mechanics:</b></size>\n\n" +
                
                    "  Some basic information you should know is that you will spawn in SCP-049's containment chamber. All doors accessible only by keycards will be locked open, and those who spawn as a murderer or detective will be given their equipment a given (configurable) time after the round starts, and will be informed of their teammates via a broadcast at the start of the round. The round will only last a given (configurable) time after the round starts before innocents win by default.\n" +
                    "  The gun can be picked up by close distance to help prevent having to stand still while picking up the gun, this makes camping the gun more difficult.\n\n" +
                
                "<size=30><b>Development Info:</b></size>\n\n" +

                    $"  The current version of murder mystery is {VersionStr}\n\n" +
                    
                    "  For more information on this plugin refer to [www.github.com/o5zereth/murdermystery].\n\n" +
                    
                    "  Developed by [Zereth#1675] on Discord, please refer to me directly in DM's or in the Exiled Discord (via ping) for inquires on the development of this project." +

            "</color>";*/

        private bool reloading = false;

        public override void OnEnabled()
        {
            Config.Validate(Config);

            if (reloading) { base.OnEnabled(); reloading = false; return; }

            Handlers.Player.Verified += MMPlayer.Add;
            Handlers.Player.Destroying += MMPlayer.Remove;
            //Handlers.Server.RestartingRound += MMPlayer.RemoveAll;

            Singleton = this;
            EventHandlers = new EventHandlers();
            GamemodeManager = new GamemodeManager();
            CoroutineManager = new CoroutineManager();
            CompatabilityManager = new CompatibilityManager();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            if (reloading) { base.OnDisabled(); return; }

            Handlers.Player.Verified -= MMPlayer.Add;
            Handlers.Player.Destroying -= MMPlayer.Remove;
            //Handlers.Server.RestartingRound -= MMPlayer.RemoveAll;

            Singleton = null;
            EventHandlers = null;
            GamemodeManager = null;
            CoroutineManager = null;
            CompatabilityManager = null;

            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            reloading = true;

            base.OnReloaded();
        }
    }
}