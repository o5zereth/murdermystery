using GameCore;
using HarmonyLib;

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
            if (enabling)
            {
                Harmony.PatchAll();

                ServerConsole.FriendlyFire = true;
                CharacterClassManager.LaterJoinEnabled = false;
            }
            else
            {
                Harmony.UnpatchAll();

                MurderMystery.CoroutineManager.Reset();

                ServerConsole.FriendlyFire = ConfigFile.ServerConfig.GetBool("friendly_fire");
                CharacterClassManager.LaterJoinEnabled = ConfigFile.ServerConfig.GetBool("later_join_enabled", true);
            }
        }
    }
}