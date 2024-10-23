using HarmonyLib;
using StellarRoles.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class MurderPlayerPatch
    {
        private static bool ResetToCrewmate = false;
        private static bool ResetToDead = false;

        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (GameHistory.DeadPlayers.Any(x => x.Player.PlayerId == target.PlayerId))
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    __instance.ResetKillButton();
                }
                KillAnimationCoPerformKillPatch.HideNextAnimation = false;
                return false;
            }

            if (AmongUsClient.Instance.AmHost)
            {
                Helpers.AddDeadPlayer(target, __instance, DeathReason.Kill);
            }

            // Allow everyone to murder players
            ResetToCrewmate = !__instance.Data.Role.IsImpostor;
            ResetToDead = __instance.Data.IsDead;
            __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
            __instance.Data.IsDead = false;
            return true;
        }

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            bool isOwner = __instance.AmOwner;
            PlayerControl killer = __instance;

            // Reset killer to crewmate if resetToCrewmate
            if (ResetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            if (ResetToDead) __instance.Data.IsDead = true;

            // Remove fake tasks when player dies
            if (target.HasFakeTasks())
                target.ClearAllTasks();

            // First kill (set before lover suicide)
            if (MapOptions.FirstKillName == "") MapOptions.FirstKillName = target.Data.PlayerName;

            if (MapOptions.firstRoundWithDead)
            {
                MapOptions.FirstKillPlayersNames.Add(target.Data.PlayerName);
                if (MapOptions.FirstKillPlayersNames.Count < 3 && AmongUsClient.Instance.AmHost)
                {
                    target.RPCAddGameInfo(InfoType.FirstTwoPlayersDead);
                }
            }

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
            if (Detective.OldDeadBodies != null)
            {
                PlayerControl player = target;

                if (!Detective.KillersLinkToKills.TryGetValue(killer.PlayerId, out PlayerList kills))
                    Detective.KillersLinkToKills.Add(killer.PlayerId, kills = new());

                kills.Add(player);

                _ = Detective.KillerEscapeRoute.TryAdd(player.PlayerId, new KillerEscapeByVent(killer));

                Vector2 killerStartingPosition = killer.GetTruePosition();
                2f.DelayedAction(()=> 
                {
                    Vector2 killerEndingPosition = killer.GetTruePosition();
                    _ = Detective.KillerEscapeDirection.TryAdd(player.PlayerId, new KillerDirection(killerStartingPosition, killerEndingPosition));

                    var deadPlayer = GameHistory.DeadPlayers.Where(x => x.Player.PlayerId == target.PlayerId).FirstOrDefault();
                    Detective.FreshDeadBodies.Add((deadPlayer, target.transform.position));
                });
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
            ExtraStats.UpdateSurvivability();
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
            Helpers.HandleParasiteKillOnBodyReport();
        }
    }
}
