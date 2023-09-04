using AmongUs.GameOptions;
using HarmonyLib;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ParityCopAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || !PlayerControl.LocalPlayer.IsParityCop(out ParityCop parityCop)) return;
            ParityCopSetTarget(parityCop);
        }

        public static bool IsRoleBlocked()
        {
            return ParityCop.RoleBlock && Helpers.IsCommsActive();
        }

        static void ParityCopSetTarget(ParityCop parityCop)
        {
            PlayerControl target = Helpers.SetTarget();
            parityCop.CurrentTarget = parityCop.IsValidTarget(target) ? target : null;
        }
    }
}
