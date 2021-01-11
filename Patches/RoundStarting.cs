using HarmonyLib;
using MurderMystery.Utils;
using MurderMystery.Enums;
using Exiled.API.Features;

namespace MurderMystery.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallRpcRoundStarted))]
    internal class RoundStarting
    {
        private static void Prefix(CharacterClassManager __instance)
        {
            if (MurderMystery.Singleton.Config.RequireRoundRestart && !MurderMystery.GamemodeStatus.WaitingForPlayers) { return; }
            if (!MurderMystery.GamemodeStatus.Enabled) { return; }

            Log.Debug("RoundStarting prefix patch has been called.");

            foreach (MMPlayer ply in MMPlayer.List)
            {
                if (ply.Player.IsOverwatchEnabled)
                {
                    Log.Debug($"Player: {ply.Player.Nickname} is being set to spectator because of overwatch.");
                    ply.SoftlySetRole(MMRole.Spectator);
                }
            }
        }
    }
}