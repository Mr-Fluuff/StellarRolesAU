using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.First)]
    public static class DetectiveAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Detective.Player || PlayerControl.LocalPlayer.Data.IsDead) return;

            DetectiveSetTarget();
            DetectiveVentTimer();
        }

        public static void DetectiveVentTimer()
        {
            if (Detective.KillerEscapeRoute.Count == 0) return;

            foreach (KillerEscapeByVent escapeRoute in Detective.KillerEscapeRoute.Values)
            {
                escapeRoute.Timer -= Time.deltaTime;
                if (escapeRoute.Timer < 0 || escapeRoute.UsedVent)
                    continue;

                else if (escapeRoute.Venter.inVent)
                    escapeRoute.UsedVent = true;
            }
        }

        public static bool IsRoleBlocked()
        {
            return Helpers.IsCommsActive() && Detective.RoleBlock;
        }

        public static void DetectiveSetTarget()
        {
            if (MeetingHud.Instance) return;

            List<DeadPlayer> allDeadBodies = new();
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float usableDistance = ShipStatus.Instance.AllVents.FirstOrDefault().UsableDistance;

            if (Detective.IsCrimeSceneEnabled)
            {
                foreach (var dp in Detective.OldDeadBodies)
                {
                    if (Vector2.Distance(dp.DeathPos, truePosition) <= usableDistance)
                    {
                        allDeadBodies.Add(dp);
                    }
                }
            }

            foreach (var dp in Detective.FreshDeadBodies)
            {
                if (Vector2.Distance(dp.CurrentBodyPos, truePosition) <= usableDistance)
                {
                    allDeadBodies.Add(dp);
                }
            }

            DeadPlayer target = null;
            if (allDeadBodies.Count > 0)
            {
                target = allDeadBodies.FirstOrDefault();
            }

            Detective.Target = target;
            Detective.TargetIsFresh = target != null && Detective.FreshDeadBodies.Any(t => t.Player.PlayerId == target.Player.PlayerId);
        }
    }
}
