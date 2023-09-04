using AmongUs.GameOptions;
using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HeadHunterAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != HeadHunter.Player) return;

            HeadHunterSetTarget();
            HeadHunterFindBounties();
        }

        static void HeadHunterSetTarget()
        {
            HeadHunter.CurrentTarget = Helpers.SetTarget(false, true, canIncrease: true);
        }

        static void HeadHunterFindBounties()
        {
            // Handle player tracking
            if (HeadHunter.PlayerLocalArrows != null)
            {
                if (HeadHunter.PursueCurrentTimer > 0)
                {
                    HeadHunter.PursueCurrentTimer -= Time.deltaTime;
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                        if (HeadHunter.PlayerLocalArrows.TryGetValue(player.PlayerId, out Arrow arrow))
                            Helpers.TrackDeadBody(player, arrow, HeadHunter.Color);
                }
                else if (HeadHunter.PlayerLocalArrows != null)
                    foreach (Arrow arrow in HeadHunter.PlayerLocalArrows.Values)
                        arrow.Object.SetActive(false);
            }
        }
    }
}
