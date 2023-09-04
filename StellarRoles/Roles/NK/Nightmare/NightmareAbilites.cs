using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class NightmareAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || Helpers.IsHideAndSeek) return;
            if (Nightmare.PlayerToNightmare?.Keys.Count <= 0) return;

            NightMareUpdate();
            NightMareSetTarget();
        }

        static void NightMareUpdate()
        {
            foreach (Nightmare nightmare in Nightmare.PlayerToNightmare.Values)
            {
                nightmare.LightsOutTimer -= Time.deltaTime;

                if (nightmare.Player.AmOwner && nightmare.LightsOutTimer <= 0f && nightmare.BlindedPlayers.Count > 0)
                {
                    RPCProcedure.Send(CustomRPC.NightMareClear, nightmare.Player.PlayerId);
                    nightmare.BlindedPlayers.Clear();
                }
            }
        }

        static void NightMareSetTarget()
        {
            if (PlayerControl.LocalPlayer.IsNightmare(out Nightmare nightmare))
            {
                nightmare.AbilityCurrentTarget = Helpers.SetTarget(canIncrease: true, ascended: Ascended.AscendedNightmare(nightmare.Player));
                Helpers.SetPlayerOutline(nightmare.AbilityCurrentTarget, Nightmare.Color);
            }
        }
    }
}
