using GameCore;
using HarmonyLib;
using Exiled.API.Features;

namespace MurderMystery.API
{
    /// <summary>
    /// Will later be a compatability class for other plugins and stuff, currently for internal implementations to fix base game issues.
    /// </summary>
    internal class CompatibilityManager
    {
        internal CompatibilityManager()
        {
            Harmony = new Harmony("zereth.plugins.murdermystery");
        }

        internal Harmony Harmony { get; private set; }

        internal void TogglingSecondaryEvents(bool enabling)
        {
            Exiled.API.Features.Log.Debug($"[CompatibilityManager] TogglingSecondaryEvents called. Enabling: {enabling}");

            if (enabling)
            {
                Harmony.PatchAll();

                MurderMystery.GamemodeManager.Patched = true;

                ServerConsole.FriendlyFire = true;
                CharacterClassManager.LaterJoinEnabled = false;
            }
            else
            {
                ServerConsole.FriendlyFire = ConfigFile.ServerConfig.GetBool("friendly_fire");
                CharacterClassManager.LaterJoinEnabled = ConfigFile.ServerConfig.GetBool("later_join_enabled", true);
            }
        }

        internal void Reset()
        {
            Harmony.UnpatchAll();

            MurderMystery.GamemodeManager.Patched = false;

            MurderMystery.CoroutineManager.Reset();
        }
    }
}