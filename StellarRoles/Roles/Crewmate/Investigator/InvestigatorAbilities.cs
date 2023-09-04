using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class InvestigatorAbilities
    {
        public static void Postfix()
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Investigator.Player || IsRoleBlocked()) return;

            InvestigatorUpdateFootPrints();
        }

        public static bool IsRoleBlocked()
        {
            return Investigator.RoleBlock && Helpers.IsCommsActive();
        }
        static void InvestigatorUpdateFootPrints()
        {
            if (Investigator.Player.Data.IsDead) return;
            Investigator.Timer -= Time.deltaTime;
            if (Investigator.Timer <= 0f)
            {
                Investigator.Timer = Investigator.CalculateFootprintInterval();
                foreach (PlayerControl player in Investigator.AllPlayers.GetPlayerEnumerator())
                {
                    if (player.inVent
                        || player.Data.IsDead
                        || (player == Shade.Player && Shade.IsInvisble)
                        || (player == Wraith.Player && Wraith.IsInvisible)
                        || player == Investigator.Player) continue;

                    FootprintHolder.Instance.MakeFootprint(player);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public static class BodyReportPatch
    {
        public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo target)
        {
            if (Investigator.Player != PlayerControl.LocalPlayer)
                return;

            DeadPlayer deadPlayer = GameHistory.DeadPlayers.FirstOrDefault(x => x.Player.PlayerId == target.PlayerId);

            if (deadPlayer != null && deadPlayer.KillerIfExisting != null)
            {
                float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;
                string msg = $"Body Report: Killed {Math.Round(timeSinceDeath / 1000)}s ago!";

                if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);
            }
        }
    }
}
