using HarmonyLib;
using StellarRoles.Utilities;
using System;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class MurderPlayerPatch
    {
        private static bool ResetToCrewmate = false;
        private static bool ResetToDead = false;

        public static void Prefix(PlayerControl __instance)
        {
            // Allow everyone to murder players
            ResetToCrewmate = !__instance.Data.Role.IsImpostor;
            ResetToDead = __instance.Data.IsDead;
            __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
            __instance.Data.IsDead = false;
        }

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            bool isOwner = __instance.AmOwner;
            PlayerControl killer = __instance.IsBombed(out Bombed bombed) ? bombed.Bomber : __instance;
            DeadPlayer deadPlayer = new(target, DateTime.UtcNow, DeathReason.Kill, killer);
            // Reset killer to crewmate if resetToCrewmate
            if (ResetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            if (ResetToDead) __instance.Data.IsDead = true;

            // Remove fake tasks when player dies
            if (target.HasFakeTasks())
                target.ClearAllTasks();

            // First kill (set before lover suicide)
            if (MapOptions.FirstKillName == "") MapOptions.FirstKillName = target.Data.PlayerName;
            if (MapOptions.IsFirstRound) MapOptions.FirstKillPlayersNames.Add(target.Data.PlayerName);

            // Cutist show flash
            if (Cultist.Player != null && Follower.Player != null && !Impostor.IsRoleAblilityBlocked())
            {
                PlayerControl player = null;
                if (__instance == Cultist.Player && !Cultist.FollowerSpecialRoleAssigned) player = Follower.Player;
                else if (__instance == Follower.Player) player = Cultist.Player;

                if (player != null && !player.Data.IsDead && player == PlayerControl.LocalPlayer)
                    Helpers.ShowFlash(Palette.ImpostorRed, 1.5f);
            }

            // Detective add body
            if (Detective.DeadBodies != null)
            {
                if (deadPlayer.KillerIfExisting != null)
                {
                    PlayerControl player = deadPlayer.Player;

                    if (!Detective.KillersLinkToKills.TryGetValue(killer.PlayerId, out PlayerList kills))
                        Detective.KillersLinkToKills.Add(killer.PlayerId, kills = new());

                    kills.Add(player);

                    Detective.KillerEscapeRoute.Add(player.PlayerId, new KillerEscapeByVent(killer));

                    Vector2 killerStartingPosition = target.transform.position;
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(2, new Action<float>((p) =>
                    { // Delayed action
                        if (p == 1f)
                        {
                            Vector2 killerEndingPosition = killer.GetTruePosition();
                            Detective.KillerEscapeDirection[player.PlayerId] = new KillerDirection(killerStartingPosition, killerEndingPosition);
                        }
                    })));
                }

                Detective.FeatureDeadBodies.Add((deadPlayer, target.transform.position));
            }

            if (isOwner)
            {
                if (PlayerControl.LocalPlayer == Shade.Player)
                    Shade.Killed++;
                else if (PlayerControl.LocalPlayer == Hacker.Player)
                    Hacker.JamCharges += Hacker.JamChargesPerKill;
                else if (PlayerControl.LocalPlayer == Janitor.Player)
                    Janitor.ChargesRemaining += Janitor.ChargesPerKill;
                else if (PlayerControl.LocalPlayer == Camouflager.Player)
                    Camouflager.ChargesRemaining += Camouflager.ChargePerKill;
            }

            Helpers.CheckImpsAlive();
            Helpers.CheckPlayersAlive();
            Helpers.SetBodySize();

            if (RomanticAbilites.RomanticRoleUpdate(false))
            {
                VengefulRomantic.Target = killer;
            }
            RomanticAbilites.VengefulRoleUpdate(false);
        }
    }

    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
    class KillAnimationCoPerformKillPatch
    {
        public static bool HideNextAnimation = false;
        public static void Prefix([HarmonyArgument(0)] ref PlayerControl source, [HarmonyArgument(1)] ref PlayerControl target)
        {
            if (HideNextAnimation)
                source = target;
            HideNextAnimation = false;
        }
    }

    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement))]
    class KillAnimationSetMovementPatch
    {
        private static int? colorId = null;
        public static void Prefix(PlayerControl source)
        {
            Color color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
            if (Morphling.Player != null && source.Data.PlayerId == Morphling.Player.PlayerId)
            {
                int index = Palette.PlayerColors.IndexOf(color);
                if (index != -1) colorId = index;
            }
        }

        public static void Postfix(PlayerControl source)
        {
            if (colorId.HasValue) source.RawSetColor(colorId.Value);
            colorId = null;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    class PlayerControlCmdReportDeadBodyPatch
    {
        public static void Prefix()
        {
            Helpers.HandleVampireBiteOnBodyReport();
            Helpers.HandleBomberExplodeOnBodyReport();
        }
    }
}
