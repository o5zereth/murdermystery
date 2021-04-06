using Exiled.API.Features;
using HarmonyLib;
using MurderMystery.API;

namespace MurderMystery.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallRpcRoundStarted))]
    internal class RoundStartingPatch
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            Log.Debug("RoundStarting prefix patch has been called.", MurderMystery.Singleton.Debug);

            for (int i = 0; i < MMPlayer.List.Count; i++)
            {
                if (MMPlayer.List[i].Player.IsOverwatchEnabled)
                {
                    Log.Debug($"Player: {MMPlayer.List[i].Player.Nickname} is being set to spectator because of overwatch.", MurderMystery.Singleton.Debug);
                    MMPlayer.List[i].SoftlySetRole(MMRole.Spectator);
                }
            }

            return true;
        }
    }
}