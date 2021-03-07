using HarmonyLib;
using Exiled.API.Features;
using MurderMystery.API;

namespace MurderMystery.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallRpcRoundStarted))]
    internal class RoundStartingPatch
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            if ((MurderMystery.Singleton.Config.RequireRoundRestart && !MurderMystery.GamemodeManager.WaitingForPlayers) || !MurderMystery.GamemodeManager.Enabled) { return true; }

            Log.Debug("RoundStarting prefix patch has been called.", MurderMystery.Singleton.Debug);

            foreach (MMPlayer ply in MMPlayer.List)
            {
                if (ply.Player.IsOverwatchEnabled)
                {
                    Log.Debug($"Player: {ply.Player.Nickname} is being set to spectator because of overwatch.", MurderMystery.Singleton.Debug);
                    ply.SoftlySetRole(MMRole.Spectator);
                }
            }

            return true;
        }
    }
}