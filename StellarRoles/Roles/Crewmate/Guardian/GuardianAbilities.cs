using HarmonyLib;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class GuardianAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Guardian.Player) return;
            guardianSetTarget();
        }

        public static bool IsRoleBlocked()
        {
            return Helpers.IsCommsActive() && Guardian.RoleBlock;
        }

        public static void guardianSetTarget()
        {
            Guardian.CurrentTarget = Helpers.SetTarget(targetPlayersInVents: false);
        }
    }
}
