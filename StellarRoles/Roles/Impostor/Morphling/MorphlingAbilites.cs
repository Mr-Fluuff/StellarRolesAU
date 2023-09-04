using HarmonyLib;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class MorphlingAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Morphling.Player) return;
            morphlingSetTarget();
        }

        static void morphlingSetTarget()
        {
            Morphling.AbilityCurrentTarget = Helpers.SetTarget();
            Helpers.SetPlayerOutline(Morphling.AbilityCurrentTarget, Morphling.Color);
        }
    }
}
