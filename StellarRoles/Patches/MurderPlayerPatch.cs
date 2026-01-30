using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using HarmonyLib;
using Reactor.Utilities.Extensions;
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
        public static bool HideNextAnimation = false;
        public static PlayerControl OverlayPlayer = null;

        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target, [HarmonyArgument(1)] MurderResultFlags resultFlags)
        {
            if (GameHistory.DeadPlayers.Any(x => x.Player.PlayerId == target.PlayerId))
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    __instance.ResetKillButton();
                }
                HideNextAnimation = false;
                return false;
            }

            Helpers.AddDeadPlayer(target, __instance, DeathReason.Kill);

            // Allow everyone to murder players
            ResetToCrewmate = !__instance.Data.Role.IsImpostor;
            ResetToDead = __instance.Data.IsDead;
            __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
            __instance.Data.IsDead = false;
            MurderPlayer(__instance, target, resultFlags);
            return false;
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
            if (Detective.Player != null)
            {
                if (!Detective.KillersLinkToKills.ContainsKey(killer.PlayerId))
                {
                    Detective.KillersLinkToKills.Add(killer.PlayerId, [target.PlayerId]);
                }
                else
                {
                    Detective.KillersLinkToKills[killer.PlayerId].Add(target);
                }

                _ = Detective.KillerEscapeRoute.TryAdd(target.PlayerId, new KillerEscapeByVent(killer));

                Vector2 killerStartingPosition = killer.GetTruePosition();
                Vector2 killerEndingPosition = killer.GetTruePosition();

                HudManager.Instance.StartCoroutine(Effects.Lerp(2f, new Action<float>((p) =>
                { // Delayed action

                    if (!MeetingHud.Instance)
                    {
                        killerEndingPosition = killer.GetTruePosition();
                    }
                    if (p == 1)
                    {
                        _ = Detective.KillerEscapeDirection.TryAdd(target.PlayerId, new KillerDirection(killerStartingPosition, killerEndingPosition));
                    }
                })));
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

                CharlatanAbilities.CheckKill(target);
            }

            Helpers.CheckPlayersAlive();

            if (RomanticAbilites.RomanticRoleUpdate(false))
            {
                VengefulRomantic.Target = killer;
            }
            RomanticAbilites.VengefulRoleUpdate(false);
            //ExtraStats.UpdateSurvivability();
        }
        public static void MurderPlayer(PlayerControl __instance, PlayerControl target, MurderResultFlags resultFlags)
        {
            __instance.isKilling = false;
            __instance.logger.Debug(string.Format("{0} trying to murder {1}", __instance.PlayerId, target.PlayerId), null);
            NetworkedPlayerInfo data = target.Data;
            if (resultFlags.HasFlag(MurderResultFlags.FailedError))
            {
                return;
            }
            if (resultFlags.HasFlag(MurderResultFlags.FailedProtected) || (resultFlags.HasFlag(MurderResultFlags.DecisionByHost) && target.protectedByGuardianId > -1))
            {
                target.protectedByGuardianThisRound = true;
                bool flag = PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.GuardianAngel;
                if (flag && (int)PlayerControl.LocalPlayer.Data.PlayerId == target.protectedByGuardianId)
                {
                    DataManager.Player.Stats.IncrementStat(StatID.Role_GuardianAngel_CrewmatesProtected);
                    DestroyableSingleton<AchievementManager>.Instance.OnProtectACrewmate();
                }
                if (__instance.AmOwner || flag)
                {
                    target.ShowFailedMurder();
                    __instance.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) / 2f);
                }
                else
                {
                    target.RemoveProtection();
                }
                __instance.logger.Debug(string.Format("{0} failed to murder {1} due to guardian angel protection", __instance.PlayerId, target.PlayerId), null);
                return;
            }
            if (resultFlags.HasFlag(MurderResultFlags.Succeeded) || resultFlags.HasFlag(MurderResultFlags.DecisionByHost))
            {
                DestroyableSingleton<DebugAnalytics>.Instance.Analytics.Kill(target.Data, __instance.Data);
                if (__instance.AmOwner)
                {
                    if (GameManager.Instance.IsHideAndSeek())
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.HideAndSeek_ImpostorKills);
                    }
                    else
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.ImpostorKills);
                    }
                    if (__instance.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.Role_Shapeshifter_ShiftedKills);
                    }
                    if (Constants.ShouldPlaySfx())
                    {
                        SoundManager.Instance.PlaySound(__instance.KillSfx, false, 0.8f, null);
                    }
                    __instance.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
                }
                DestroyableSingleton<UnityTelemetry>.Instance.WriteMurder();
                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                PlayerControl source = HideNextAnimation ? target : __instance;
                NetworkedPlayerInfo sourceData = __instance.Data;
                if (OverlayPlayer != null)
                {
                    if (OverlayPlayer.IsMorphed())
                    {
                        sourceData = Morphling.MorphTarget.Data;
                    }
                    else sourceData = OverlayPlayer.Data;
                }
                else if (__instance.IsMorphed())
                {
                    sourceData = Morphling.MorphTarget.Data;
                }
                if (target.AmOwner)
                {
                    DataManager.Player.Stats.IncrementStat(StatID.TimesMurdered);
                    if (Minigame.Instance)
                    {
                        try
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }
                        catch
                        {
                        }
                    }
                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(sourceData, data);
                    target.cosmetics.SetNameMask(false);
                    target.RpcSetScanner(false);
                }
                DestroyableSingleton<AchievementManager>.Instance.OnMurder(__instance.AmOwner, target.AmOwner, __instance.CurrentOutfitType == PlayerOutfitType.Shapeshifted, __instance.shapeshiftTargetPlayerId, (int)target.PlayerId);
                __instance.MyPhysics.StartCoroutine(__instance.KillAnimations.Random<KillAnimation>().CoPerformKill(source, target));
                __instance.logger.Debug(string.Format("{0} succeeded in murdering {1}", __instance.PlayerId, target.PlayerId), null);
                HideNextAnimation = false;
                OverlayPlayer = null;
            }
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
