using HarmonyLib;

namespace StellarRoles.Patches
{

    [HarmonyPatch(typeof(FungleSurveillanceMinigame), nameof(FungleSurveillanceMinigame.Update))]
    class FungleSecurityPatch
    {
        public static void Postfix(FungleSurveillanceMinigame __instance)
        {
            if (!MapOptions.CanUseCameras())
            {
                __instance.Close();
            }
        }
    }
}