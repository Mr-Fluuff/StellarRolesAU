using HarmonyLib;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class WarlockAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Warlock.Player) return;
            WarlockSetTarget();
        }

        static void WarlockSetTarget()
        {
            if (Warlock.CurseVictim != null && (Warlock.CurseVictim.Data.Disconnected || Warlock.CurseVictim.Data.IsDead))
            {
                // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
                Warlock.ResetCurse();
            }
            if (Warlock.CurseVictim == null)
            {
                Warlock.CurrentCurseTarget = Helpers.SetTarget(false, true);
                Helpers.SetPlayerOutline(Warlock.CurrentCurseTarget, Warlock.Color);
            }
            else
            {
                Warlock.CurseVictimTarget = Helpers.SetTarget(targetOverride: Warlock.CurseVictim);
                Helpers.SetPlayerOutline(Warlock.CurseVictimTarget, Warlock.Color);
            }
        }
    }
}
