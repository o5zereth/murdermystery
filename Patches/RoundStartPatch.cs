using Exiled.API.Features;
using HarmonyLib;
using MurderMystery.API;
using System.Reflection;

namespace MurderMystery.Patches
{
    public class RoundStartPatch : CustomPatch
	{
        public static RoundStartPatch Singleton { get; internal set; } = new RoundStartPatch();

        public override string PatchName { get; } = "RoundStart";
        public override MethodInfo OriginalMethod { get; } = typeof(CharacterClassManager).GetMethod(nameof(CharacterClassManager.CallRpcRoundStarted), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		public override MethodInfo PatchMethod { get; } = typeof(RoundStartPatch).GetMethod(nameof(Prefix), BindingFlags.NonPublic | BindingFlags.Static);
        public override HarmonyPatchType PatchType { get; } = HarmonyPatchType.Prefix;

        private static bool Prefix(CharacterClassManager __instance)
		{
			Log.Debug("[RoundStartPatch::Prefix] Patch called.", MurderMystery.Singleton.LogDebug);

            for (int i = 0; i < MMPlayer.List.Count; i++)
            {
                if (MMPlayer.List[i].Player.IsOverwatchEnabled)
                {
                    Log.Debug($"Player: {MMPlayer.List[i].Player.Nickname} is being set to spectator because of overwatch.", MurderMystery.Singleton.LogDebug);
                    MMPlayer.List[i].Role = MMRole.Spectator;
                }
            }

			Singleton.Patch(false);

			return true;
		}
	}
}