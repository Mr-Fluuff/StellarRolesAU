using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class RuthlessButtons
    {
        private static bool Initialized;
        public static CustomButton ShieldButton { get; set; }

        public static void RuthlessRomanticShield()
        {
            ShieldButton = new CustomButton(
                    () =>
                    {
                        RPCProcedure.Send(CustomRPC.RuthlessRomanticShield, PlayerControl.LocalPlayer);
                        PlayerControl.LocalPlayer.IsRuthlessRomantic(out RuthlessRomantic ruthlessRomantic);
                        RPCProcedure.RuthlessRomanticShield(ruthlessRomantic);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    },
                    () =>
                    {
                        return PlayerControl.LocalPlayer.IsRuthlessRomantic(out RuthlessRomantic ruthlessRomantic) && !ruthlessRomantic.Player.Data.IsDead;
                    },
                    () =>
                    {
                        ShieldButton.ActionButton.buttonLabelText.SetOutlineColor(RuthlessRomantic.Color);
                        Helpers.ShowTargetNameOnButtonExplicit(null, ShieldButton, "SHIELD");
                        ShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;

                        return PlayerControl.LocalPlayer.CanMove && !(MapOptions.NeutralKillerRoleBlock && Helpers.IsCommsActive());
                    },
                    () =>
                    {
                        ShieldButton.Timer = ShieldButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                        ShieldButton.IsEffectActive = false;
                        ShieldButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;

                    },
                    RuthlessRomantic.GetRuthlessRomanticSprite(),
                    CustomButton.ButtonPositions.UpperRow2,
                    "ActionQuaternary",
                    true,
                    RuthlessRomantic.VestDuration,
                    () => ShieldButton.Timer = ShieldButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer)
                );

            Initialized = true;
            SetRuthlessRomanticCooldowns();
        }

        public static void SetRuthlessRomanticCooldowns()
        {
            if (!Initialized)
            {
                RuthlessRomanticShield();
            }

            ShieldButton.EffectDuration = RuthlessRomantic.VestDuration;
            ShieldButton.MaxTimer = RuthlessRomantic.VestCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            RuthlessRomanticShield();
        }
    }
}
