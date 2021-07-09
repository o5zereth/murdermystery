using HarmonyLib;
using MurderMystery.API;
using System.Reflection;

namespace MurderMystery.Patches
{
    public class CustomInfoPatch : CustomPatch
    {
        public static CustomInfoPatch Singleton { get; } = new CustomInfoPatch();

        public override string PatchName { get; } = "CustomInfo";
        public override MethodInfo OriginalMethod { get; } = typeof(NicknameSync).GetMethod("set_Network_customPlayerInfoString", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        public override MethodInfo PatchMethod { get; } = typeof(CustomInfoPatch).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic);
        public override HarmonyPatchType PatchType { get; } = HarmonyPatchType.Prefix;

        private static bool Prefix(NicknameSync __instance, bool value)
        {
            return false;
        }
    }
}
