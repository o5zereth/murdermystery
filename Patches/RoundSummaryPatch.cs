using Exiled.API.Features;
using HarmonyLib;
using static RoundSummary;

namespace MurderMystery.Patches
{
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.RpcShowRoundSummary))]
    internal class RoundSummaryPatch
    {
        private static bool Prefix(RoundSummary __instance, SumInfo_ClassList list_start, SumInfo_ClassList list_finish, LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
        {
            if ((MurderMystery.Singleton.Config.RequireRoundRestart && !MurderMystery.GamemodeManager.WaitingForPlayers) || !MurderMystery.GamemodeManager.Enabled) { return true; }

            Log.Debug("RoundSummary prefix patch has been called.", MurderMystery.Singleton.Debug);

            return false;
        }
    }
}