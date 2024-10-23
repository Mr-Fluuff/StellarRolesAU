using HarmonyLib;
using StellarRoles.Objects;
using System;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class TrackerButtons
    {
        private static bool Initialized;

        public static CustomButton TrackerMarkButton { get; set; }
        public static CustomButton TrackerTrackButton { get; set; }

        public static void MarkButton()
        {
            TrackerMarkButton = new CustomButton(
                () =>
                {
                    RPCProcedure.Send(CustomRPC.TrackerMarkPlayer, Tracker.CurrentTarget);
                    RPCProcedure.TrackerMarkPlayer(Tracker.CurrentTarget);
                    SoundEffectsManager.Play(Sounds.TrackPlayer);
                    TrackerMarkButton.Timer = TrackerMarkButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                },
                () => Tracker.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Tracker.NumberOfTracks > 0,
                () =>
                {
                    TrackerMarkButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    Helpers.ShowTargetNameOnButtonExplicit(null, TrackerMarkButton, $"Mark - {Tracker.NumberOfTracks}");

                    return PlayerControl.LocalPlayer.CanMove && Tracker.CurrentTarget != null && !TrackerAbilities.IsRoleBlocked();
                },
                () =>
                {
                    TrackerMarkButton.Timer = 5f * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                },
                Tracker.GetMarkButtonSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                "ActionQuaternary",
                false
            );
        }
        public static void TrackButton()
        {
            TrackerTrackButton = new CustomButton(
                () =>
                {
                    RPCProcedure.Send(CustomRPC.TrackerTrackWarning);
                    TrackerTrackButton.Timer = Tracker.DelayDuration;
                    Tracker.TimeLeft = (int)Tracker.DelayDuration;
                    TrackerTrackButton.ActionButton.cooldownTimerText.SetFaceColor(Color.red);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                    HudManager.Instance.StartCoroutine(Effects.Lerp(Tracker.DelayDuration, new Action<float>((p) =>
                    {
                        int timeLeft = (int)(Tracker.DelayDuration - (Tracker.DelayDuration * p));
                        if (timeLeft <= Tracker.DelayDuration && Tracker.TimeLeft != timeLeft)
                        {
                            _ = new CustomMessage($"Tracking Players In {timeLeft} Seconds", 1f, true, Tracker.Color);
                            Tracker.TimeLeft = timeLeft;
                        }
                        if (p == 1f)
                        {
                            TrackerTrackButton.ActionButton.cooldownTimerText.SetFaceColor(Color.green);
                            Tracker.TimeUntilUpdate = 0;
                            TrackerTrackButton.Timer = Tracker.CalculateTrackDuration();
                            Tracker.TrackActive = true;
                        }
                    })));
                    (Tracker.DelayDuration + Tracker.CalculateTrackDuration()).DelayedAction(() => 
                    {
                        TrackerTrackButton.Timer = Tracker.TrackTargetCooldown;
                        TrackerTrackButton.ActionButton.cooldownTimerText.SetFaceColor(Color.white);
                        Tracker.TrackActive = false;
                    });
                },
                () => { return Tracker.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    TrackerTrackButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    Helpers.ShowTargetNameOnButtonExplicit(null, TrackerTrackButton, $"Track");

                    return PlayerControl.LocalPlayer.CanMove && Tracker.TrackedPlayers.Count > 0 && !TrackerAbilities.IsRoleBlocked();
                },
                () =>
                {
                    TrackerTrackButton.Timer = Tracker.TrackTargetCooldown * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    TrackerTrackButton.IsEffectActive = false;
                },
                Tracker.GetTrackButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                false
            );
        }

        public static void SetTrackerCooldowns()
        {
            if (!Initialized)
            {
                AddTrackerButtons();
            }

            TrackerMarkButton.MaxTimer = 5f;
            TrackerTrackButton.MaxTimer = Tracker.TrackTargetCooldown;
        }

        public static void AddTrackerButtons()
        {
            TrackButton();
            MarkButton();

            Initialized = true;
            SetTrackerCooldowns();
        }

        public static void Postfix()
        {
            Initialized = false;
            AddTrackerButtons();
        }
    }
}
