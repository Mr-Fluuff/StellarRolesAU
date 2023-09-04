using HarmonyLib;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SheriffAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Sheriff.Player) return;
            sheriffSetTarget();
        }

        static void sheriffSetTarget()
        {
            Sheriff.CurrentTarget = Helpers.SetTarget(canIncrease: true);
        }
    }
}
