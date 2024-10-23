using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class PyromaniacAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || !PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac)) return;
            PyromaniacSetTarget(pyromaniac);
        }

        public static IEnumerable<PlayerControl> PyroList(Pyromaniac pyromaniac)
        {
            if (pyromaniac.DouseTarget == null)
                return pyromaniac.DousedPlayers.GetPlayerEnumerator();
            else
                return PlayerControl.AllPlayerControls.GetFastEnumerator().Where(player => player != pyromaniac.DouseTarget);
        }

        static void PyromaniacSetTarget(Pyromaniac pyromaniac)
        {
            pyromaniac.CurrentTarget = Helpers.SetTarget(false, true, canIncrease: true);
            pyromaniac.AbilityCurrentTarget = Helpers.SetTarget(untargetablePlayers: PyroList(pyromaniac));
        }
    }
}
