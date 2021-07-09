using Exiled.API.Features;
using HarmonyLib;
using System;
using System.Reflection;

namespace MurderMystery.API
{
    public abstract class CustomPatch
    {
        public abstract string PatchName { get; }
        public abstract MethodInfo OriginalMethod { get; }
        public abstract MethodInfo PatchMethod { get; }
        public abstract HarmonyPatchType PatchType { get; }

        public bool Patched { get; private set; } = false;

        internal void Patch(bool patching)
        {
            Log.Debug($"[CustomPatch::Patch] {(patching ? "Patching" : "Unpatching")} {PatchName}.", MurderMystery.Singleton.LogDebug);

            try
            {
                if (patching)
                {
                    if (Patched) { Log.Debug($"[CustomPatch::Patch] {PatchName} is already patched.", MurderMystery.Singleton.LogDebug); return; }

                    switch (PatchType)
                    {
                        case HarmonyPatchType.Prefix:
                            MurderMystery.Singleton.GamemodeManager.Harmony.Patch(OriginalMethod, new HarmonyMethod(PatchMethod), null, null, null);
                            break;
                        case HarmonyPatchType.Postfix:
                            MurderMystery.Singleton.GamemodeManager.Harmony.Patch(OriginalMethod, null, new HarmonyMethod(PatchMethod), null, null);
                            break;
                        case HarmonyPatchType.Transpiler:
                            MurderMystery.Singleton.GamemodeManager.Harmony.Patch(OriginalMethod, null, null, new HarmonyMethod(PatchMethod), null);
                            break;
                        case HarmonyPatchType.Finalizer:
                            MurderMystery.Singleton.GamemodeManager.Harmony.Patch(OriginalMethod, null, null, null, new HarmonyMethod(PatchMethod));
                            break;

                        case HarmonyPatchType.ReversePatch:
                        case HarmonyPatchType.All:
                            return;

                        default:
                            return;
                    }

                    Patched = true;
                }
                else
                {
                    if (!Patched) { Log.Debug($"[CustomPatch::Patch] {PatchName} is already unpatched.", MurderMystery.Singleton.LogDebug); return; }

                    MurderMystery.Singleton.GamemodeManager.Harmony.Unpatch(OriginalMethod, PatchMethod);

                    Patched = false;
                }
            }
            catch (Exception e)
            {
                Log.Error("[CustomPatch::Patch]" + e);
            }
        }
    }
}