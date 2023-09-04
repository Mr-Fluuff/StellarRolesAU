using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TrackerAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Tracker.Player) return;
            TrackerUpdate();
            TrackerSetTarget();
        }

        static void TrackerSetTarget()
        {
            Tracker.CurrentTarget = Helpers.SetTarget(untargetablePlayers: Tracker.TrackedPlayers.GetPlayerEnumerator());
        }

        public static bool IsRoleBlocked()
        {
            return Tracker.RoleBlock && Helpers.IsCommsActive();
        }

        static void TrackerUpdate()
        {
            if (Tracker.Player.Data.IsDead)
            {
                foreach (Arrow arrow in Tracker.TrackedPlayerLocalArrows.Values)
                    UnityEngine.Object.Destroy(arrow.Object);
                Tracker.TrackedPlayerLocalArrows.Clear();
                return;
            }

            if (IsRoleBlocked() || !Tracker.TrackActive)
            {
                foreach (Arrow arrow in Tracker.TrackedPlayerLocalArrows.Values)
                    arrow.Object.SetActive(false);
                return;
            }

            // Handle player tracking
            if (Tracker.TrackedPlayerLocalArrows != null)
            {
                Tracker.TimeUntilUpdate -= Time.deltaTime;

                if (Tracker.TimeUntilUpdate <= 0f && Tracker.TrackedPlayers.Count > 0)
                {
                    foreach (PlayerControl player in Tracker.TrackedPlayers.GetPlayerEnumerator())
                        if (Tracker.TrackedPlayerLocalArrows.TryGetValue(player.PlayerId, out Arrow arrow))
                        {
                            arrow.Object.SetActive(true);
                            if (!player.Data.IsDead)
                            {
                                arrow.Update(player.transform.position);
                            }
                            else
                                foreach ((DeadPlayer deadPlayer, Vector3 position) in Detective.FeatureDeadBodies)
                                    if (deadPlayer.Player == player)
                                    {
                                        Tracker.TrackedPlayerLocalArrows[player.PlayerId].Update(position);
                                        break;
                                    }
                        }

                    Tracker.TimeUntilUpdate = 2.5f;
                }
            }
        }
    }
}
