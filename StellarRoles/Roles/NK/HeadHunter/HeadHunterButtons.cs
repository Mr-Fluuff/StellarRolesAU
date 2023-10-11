using HarmonyLib;
using StellarRoles.Objects;
using System;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class HeadHunterButtons
    {
        private static bool Initialized;
        public static CustomButton HeadHunterKillButton { get; set; }
        public static CustomButton HeadHunterShuffleButton { get; set; }
        public static CustomButton HeadHunterTrackerButton { get; set; }

        public static void InitHeadHunterButtons()
        {
            TrackButton();
            KillButton();
            ShuffleButton();

            Initialized = true;
            SetHeadHunterCooldowns();
        }

        public static void TrackButton()
        {
            HeadHunterTrackerButton = new CustomButton(
                () =>
                {
                    HeadHunter.PursueCurrentTimer = HeadHunter.CalculatePursueDuration();
                    HeadHunterTrackerButton.ActionButton.cooldownTimerText.SetFaceColor(Color.green);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    HeadHunterTrackerButton.IsEffectActive = true;
                    HeadHunterTrackerButton.Timer = HeadHunter.CalculatePursueDuration();
                },
                () => { return HeadHunter.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    HeadHunterTrackerButton.ActionButton.graphic.sprite = HeadHunter.GetHeadHunterPursueSprite();
                    Helpers.ShowTargetNameOnButtonExplicit(null, HeadHunterTrackerButton, "PURSUE");
                    HeadHunterTrackerButton.ActionButton.buttonLabelText.SetOutlineColor(HeadHunter.Color);

                    return PlayerControl.LocalPlayer.CanMove && !(MapOptions.NeutralKillerRoleBlock && Helpers.IsCommsActive());
                },
                () =>
                {
                    HeadHunterTrackerButton.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
                    HeadHunterTrackerButton.Timer = HeadHunterTrackerButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    HeadHunterTrackerButton.IsEffectActive = false;
                },
                HeadHunter.GetHeadHunterPursueSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                "ActionQuaternary",
                true,
                HeadHunter.CalculatePursueDuration(),
                () =>
                {
                    HeadHunterTrackerButton.Timer = HeadHunterTrackerButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    HeadHunterTrackerButton.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
                });
        }

        public static void KillButton()
        {
            HeadHunterKillButton = new CustomButton(
                () =>
                {
                    if (Helpers.CheckMurderAttemptAndKill(HeadHunter.Player, HeadHunter.CurrentTarget) == MurderAttemptResult.SuppressKill)
                        return;
                    if (HeadHunter.Bounties.Contains(HeadHunter.CurrentTarget))
                    {
                        HeadHunter.ShuffleButton = false;
                        HeadHunter.HeadHunterCurrentCooldown -= HeadHunter.HeadHunterBenefit;
                    }
                    else
                    {
                        HeadHunter.HeadHunterCurrentCooldown += HeadHunter.HeadHunterPenalty;
                    }
                    HeadHunter.HeadHunterCurrentCooldown = Math.Max(HeadHunter.HeadHunterCurrentCooldown, 0f);

                    HeadHunterKillButton.Timer = HeadHunter.HeadHunterCurrentCooldown;
                    HeadHunter.CurrentTarget = null;
                    Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddKill);
                },
                () => { return HeadHunter.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.IsBombedAndActive(); },
                () => { return HeadHunter.CurrentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { HeadHunterKillButton.Timer = HeadHunter.HeadHunterCurrentCooldown; },
                HudManager.Instance.KillButton.graphic.sprite,
                CustomButton.ButtonPositions.UpperRow1,
                "ActionSecondary"
                );
        }

        public static void ShuffleButton()
        {
            HeadHunterShuffleButton = new CustomButton(
                () =>
                {
                    HeadHunter.Bounties.Clear();
                    foreach (PoolablePlayer pp in MapOptions.PlayerIcons.Values)
                        pp.gameObject.SetActive(false);
                    foreach (Arrow arrow in HeadHunter.PlayerLocalArrows.Values)
                        UnityEngine.Object.Destroy(arrow.Object);
                    HeadHunter.PlayerLocalArrows.Clear();
                    HeadHunterTrackerButton.Timer = 10.5f;

                },
                () => { return HeadHunter.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && HeadHunter.ShuffleButton; },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () => { },
                HeadHunter.GetHeadHunterShuffleSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "SecondAbility",
                true,
                10f,
                () =>
                {
                    HeadHunter.HeadHunterUpdate();
                    HeadHunter.ShuffleButton = false;
                },
                false,
                "Shuffle"
                );
        }

        public static void SetHeadHunterCooldowns()
        {
            if (!Initialized)
            {
                InitHeadHunterButtons();
            }

            HeadHunterKillButton.MaxTimer = Helpers.KillCooldown();
            HeadHunterTrackerButton.EffectDuration = HeadHunter.CalculatePursueDuration();
            HeadHunterTrackerButton.MaxTimer = HeadHunter.Cooldown;
            HeadHunterShuffleButton.Timer = 0;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitHeadHunterButtons();
        }
    }
}
