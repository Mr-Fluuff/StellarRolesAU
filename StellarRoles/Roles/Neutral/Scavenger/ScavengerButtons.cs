using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class ScavengerButtons
    {
        private static bool Initialized;
        public static CustomButton ScavengerCorpsesButton { get; set; }
        public static CustomButton ScavengerEatButton { get; set; }

        public static void InitScavengerButtons()
        {
            CorpesButton();
            EatButton();

            Initialized = true;
            SetScavengerCooldowns();
        }

        public static void CorpesButton()
        {
            ScavengerCorpsesButton = new CustomButton(
                () =>
                {
                    Scavenger.CorpsesTrackingTimer = Scavenger.Duration;
                    SoundEffectsManager.Play(Sounds.TrackCorpses);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => Scavenger.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, ScavengerCorpsesButton, $"SCAVENGE");
                    ScavengerCorpsesButton.ActionButton.buttonLabelText.SetOutlineColor(Scavenger.Color);

                    return PlayerControl.LocalPlayer.CanMove && !(MapOptions.NeutralRoleBlock && Helpers.IsCommsActive());
                },
                () =>
                {
                    ScavengerCorpsesButton.Timer = ScavengerCorpsesButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    ScavengerCorpsesButton.IsEffectActive = false;
                    ScavengerCorpsesButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Scavenger.GetTrackCorpsesButtonSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                "ActionQuaternary",
                true,
                Scavenger.Duration,
                () => ScavengerCorpsesButton.Timer = ScavengerCorpsesButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer));
        }

        public static void EatButton()
        {
            ScavengerEatButton = new CustomButton(
                () =>
                {
                    float scavengerEatRange = Helpers.GetKillDistance() * .5f;
                    if (Ascended.IsAscended(Scavenger.Player))
                    {
                        scavengerEatRange = Helpers.GetKillDistance();
                    }
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), scavengerEatRange, Constants.PlayersOnlyMask))
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component?.Reported == false)
                            {
                                Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= scavengerEatRange && PlayerControl.LocalPlayer.CanMove && !PhysicsHelpers.AnythingBetween(truePosition, truePosition2, Constants.ShadowMask, false))
                                {
                                    RPCProcedure.Send(CustomRPC.ScavengerEat, component.ParentId);
                                    RPCProcedure.ScavengerEat(component.ParentId);

                                    ScavengerEatButton.Timer = ScavengerEatButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                                    SoundEffectsManager.Play(Sounds.Eat);
                                    Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddEat);
                                    _ = new CustomMessage($"{Scavenger.BodiesRemainingToWin()} Bodies Left To Win", 3f, true, Scavenger.Color);
                                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                                    break;
                                }
                            }
                        }

                    if (Scavenger.EatenBodies == Scavenger.ScavengerNumberToWin)
                    {
                        RPCProcedure.Send(CustomRPC.ScavengerWin);
                        Scavenger.TriggerScavengerWin = true;
                        return;
                    }

                },
                () => Scavenger.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, ScavengerEatButton, $"EAT");
                    ScavengerEatButton.ActionButton.buttonLabelText.SetOutlineColor(Scavenger.Color);
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), Helpers.GetKillDistance() * 0.5f, Constants.PlayersOnlyMask))
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component?.Reported == false)
                            {
                                Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                                Vector2 truePosition2 = component.TruePosition;
                                if (Vector2.Distance(truePosition2, truePosition) <= Helpers.GetKillDistance() * 0.5f && PlayerControl.LocalPlayer.CanMove)
                                    return true;
                            }
                        }

                    return false;
                },
                () => ScavengerEatButton.Timer = ScavengerEatButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer),
                Scavenger.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary"
            );
        }

        public static void SetScavengerCooldowns()
        {
            if (!Initialized)
            {
                InitScavengerButtons();
            }

            ScavengerCorpsesButton.EffectDuration = Scavenger.Duration;
            ScavengerCorpsesButton.MaxTimer = Scavenger.CorpsesTrackingCooldown;
            ScavengerEatButton.MaxTimer = Scavenger.Cooldown;
            ScavengerCorpsesButton.Timer = Scavenger.CorpsesTrackingCooldown * 0.5f + 5;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitScavengerButtons();
        }
    }
}
