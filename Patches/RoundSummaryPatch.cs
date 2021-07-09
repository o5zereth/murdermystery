using Exiled.API.Features;
using HarmonyLib;
using MurderMystery.API;
using System.Reflection;
using static RoundSummary;

namespace MurderMystery.Patches
{
    public class RoundSummaryPatch : CustomPatch
	{
		public static RoundSummaryPatch Singleton { get; } = new RoundSummaryPatch();

		public override string PatchName { get; } = "RoundSummary";
        public override MethodInfo OriginalMethod { get; } = typeof(RoundSummary).GetMethod(nameof(RoundSummary.RpcShowRoundSummary), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		public override MethodInfo PatchMethod { get; } = typeof(RoundSummaryPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static);
		public override HarmonyPatchType PatchType { get; } = HarmonyPatchType.Prefix;

        private static bool Prefix(RoundSummary __instance, SumInfo_ClassList list_start, SumInfo_ClassList list_finish, LeadingTeam leadingTeam, int e_ds, int e_sc, int scp_kills, int round_cd)
		{
			Log.Debug("[RoundSummaryPatch::Prefix] Patch called.", MurderMystery.Singleton.LogDebug);

			Singleton.Patch(false);

			return false;
		}
	}
}