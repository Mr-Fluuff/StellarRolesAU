using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class PsychicAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Psychic.Player) return;
            PsychicUpdate();
        }

        private static void PsychicUpdate()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            List<PlayerControl> InRange = new();

            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player == localPlayer || player.Data.IsDead || player.Data.Disconnected) continue;
                if (player.inVent && !Psychic.IncludeInVent) continue;
                if (player.IsInvisible() && !Psychic.IncludeInvisible) continue;

                if (Vector2.Distance(localPlayer.transform.position, player.transform.position) <= Psychic.PlayerRange)
                {
                    InRange.Add(player);
                }
            }

            Psychic.InRange = InRange.Count;
        }
    }
}
