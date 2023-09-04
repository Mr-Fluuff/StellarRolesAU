using HarmonyLib;
using System.Collections.Generic;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class VampireAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Vampire.Player) return;
            vampireSetBiteTarget();
        }

        static void vampireSetBiteTarget()
        {
            PlayerControl target;
            if (Vampire.IsNeutralKiller)
            {
                target = Helpers.SetTarget(false, true);
            }

            else if (Spy.Player != null)
            {
                if (Spy.ImpostorsCanKillAnyone)
                {
                    target = Helpers.SetTarget(false, true);
                }
                else
                {
                    target = Helpers.SetTarget(true, true, new List<PlayerControl>() { Spy.Player });
                }
            }

            else
            {
                target = Helpers.SetTarget(true, true);
            }

            Vampire.AbilityCurrentTarget = target;
            Helpers.SetPlayerOutline(Vampire.AbilityCurrentTarget, Vampire.Color);
        }
    }
}
