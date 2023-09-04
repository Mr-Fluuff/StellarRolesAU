using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class DetectiveAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Detective.Player) return;

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
            if (Detective.DeadBodies == null && Detective.FeatureDeadBodies == null) return;
            List<DeadPlayer> freshBodies = new();

            foreach ((DeadPlayer deadPlayer, _) in Detective.FeatureDeadBodies)
                freshBodies.Add(deadPlayer);

            List<(DeadPlayer, Vector3)> allDeadBodies = new();
            if (Detective.IsCrimeSceneEnabled)
                allDeadBodies.AddRange(Detective.DeadBodies);
            allDeadBodies.AddRange(Detective.FeatureDeadBodies);


            DeadPlayer target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
            foreach ((DeadPlayer dp, Vector3 ps) in allDeadBodies)
            {
                float distance = Vector2.Distance(ps, truePosition);
                if (distance <= usableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = dp;
                }
            }
            Detective.Target = target;
            Detective.TargetIsFresh = target != null && freshBodies.Any(t => t.Player.PlayerId == target.Player.PlayerId);
        }
    }
}
