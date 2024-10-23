using HarmonyLib;
using StellarRoles.Objects;

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
                () => { return Scavenger.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
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

                    if (Helpers.BodyInRange(scavengerEatRange, out DeadBody body))
                    {
                        RPCProcedure.Send(CustomRPC.ScavengerEat, body);
                        RPCProcedure.ScavengerEat(body.ParentId);

                        ScavengerEatButton.Timer = ScavengerEatButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                        SoundEffectsManager.Play(Sounds.Eat);
                        PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddEat);
                        _ = new CustomMessage($"{Scavenger.BodiesRemainingToWin()} Bodies Left To Win", 3f, true, Scavenger.Color);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    }

                    if (Scavenger.EatenBodies == Scavenger.ScavengerNumberToWin)
                    {
                        RPCProcedure.Send(CustomRPC.ScavengerWin);
                        Scavenger.TriggerScavengerWin = true;
                        return;
                    }

                },
                () => { return Scavenger.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, ScavengerEatButton, $"EAT");
                    ScavengerEatButton.ActionButton.buttonLabelText.SetOutlineColor(Scavenger.Color);
                    float scavengerEatRange = Helpers.GetKillDistance() * .5f;
                    if (Ascended.IsAscended(Scavenger.Player))
                    {
                        scavengerEatRange = Helpers.GetKillDistance();
                    }
                    if (Helpers.BodyInRange(scavengerEatRange)) return true;
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
