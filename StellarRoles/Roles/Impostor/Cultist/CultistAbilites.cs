using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class CultistAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || Cultist.Player == null) return;
            CultistUpdate();
            FollowerUpdate();
            CultistSetFollower();
        }

        static void CultistSetFollower()
        {
            if (PlayerControl.LocalPlayer != Cultist.Player) return;

            Cultist.CurrentFollower = Helpers.SetTarget(true);
            Helpers.SetPlayerOutline(Cultist.CurrentFollower, Palette.ImpostorRed);
        }

        static void CultistUpdate()
        {
            if (PlayerControl.LocalPlayer != Cultist.Player) return;

            Arrow arrow = Cultist.LocalArrows[0];

            if (Follower.Player == null || Cultist.Player.Data.IsDead || Impostor.IsRoleAblilityBlocked())
            {
                arrow.Object.SetActive(false);
                return;
            }

            Helpers.TrackDeadBody(Follower.Player, arrow, Palette.ImpostorRed);
        }

        static void FollowerUpdate()
        {
            if (PlayerControl.LocalPlayer != Follower.Player) return;
            if (Cultist.FollowerSpecialRoleAssigned) return;

            Arrow arrow = Cultist.LocalArrows[1];

            if (Cultist.Player == null || Follower.Player.Data.IsDead || Impostor.IsRoleAblilityBlocked())
            {
                arrow.Object.SetActive(false);
                return;
            }

            Helpers.TrackDeadBody(Cultist.Player, arrow, Palette.ImpostorRed);
        }
    }
}
