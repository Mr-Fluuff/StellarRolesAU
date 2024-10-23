using HarmonyLib;

namespace StellarRoles.Patches
{
    public class FlashlightModePatch
    {
        public static float ImpFlashLightRange => CustomOptionHolder.ImpFlashlightRange.GetFloat() * 0.15f;
        public static float CrewFlashLightRange => CustomOptionHolder.CrewFlashlightRange.GetFloat() * 0.25f;

        public static bool FlashLightEnabled => CustomOptionHolder.EnableFlashlightMode.GetBool();

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.IsFlashlightEnabled))]
        public static class IsFlashlightEnabledPatch
        {
            public static bool Prefix(ref bool __result)
            {
                __result = false;
                if ((Helpers.IsHideAndSeek && GameOptionsManager.Instance.currentHideNSeekGameOptions.useFlashlight) || (FlashLightEnabled && Helpers.IsNormal))
                {
                    __result = true;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
        public static class AdjustLight
        {
            public static bool Prefix(PlayerControl __instance)
            {
                if (__instance == null || PlayerControl.LocalPlayer == null || Helpers.IsHideAndSeek || !Helpers.GameStarted) return true;
                float range = (__instance.IsCrew() || __instance.IsNeutralB()) ? CrewFlashLightRange : ImpFlashLightRange;

                __instance.SetFlashlightInputMethod();
                __instance.lightSource.SetupLightingForGameplay(FlashLightEnabled, range, __instance.TargetFlashlight.transform);

                return false;
            }
        }
    }
}
