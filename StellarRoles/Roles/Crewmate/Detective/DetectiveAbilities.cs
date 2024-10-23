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
            if (Detective.OldDeadBodies == null && Detective.FreshDeadBodies == null) return;

            List<(DeadPlayer, Vector3)> allDeadBodies = new();
            if (Detective.IsCrimeSceneEnabled)
                allDeadBodies.AddRange(Detective.OldDeadBodies);

            var deadbodies = Object.FindObjectsOfType<DeadBody>();

            if (Detective.FreshDeadBodies.Count > 0)
            {
                for (int i = 0; i < Detective.FreshDeadBodies.Count; i++)
                {
                    var DeadPlayer = Detective.FreshDeadBodies[i].Item1;

                    var freshbody = deadbodies.Where(x => x.ParentId == DeadPlayer.Data.PlayerId).First();
                    if (freshbody != null)
                    {
                        allDeadBodies.Add((DeadPlayer, freshbody.transform.position));
                    }
                }
            }


            DeadPlayer target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = ShipStatus.Instance.AllVents.FirstOrDefault().UsableDistance;
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
            Detective.TargetIsFresh = target != null && Detective.FreshDeadBodies.Any(t => t.Item1.Player.PlayerId == target.Player.PlayerId);
        }
    }
}
