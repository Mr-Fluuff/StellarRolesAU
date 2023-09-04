using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class RefugeeButtons
    {
        private static bool Initialized;
        public static CustomButton RefugeeButton { get; set; }

        public static void RefugeeRefugeButton()
        {
            RefugeeButton = new CustomButton(
                () =>
                {
                    RPCProcedure.Send(CustomRPC.RefugeeShield, PlayerControl.LocalPlayer.PlayerId);
                    PlayerControl.LocalPlayer.IsRefugee(out Refugee refugee);
                    RPCProcedure.RefugeeShield(refugee);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () =>
                {
                    return PlayerControl.LocalPlayer.IsRefugee(out Refugee refugee) && !refugee.Player.Data.IsDead;
                },
                () =>
                {
                    RefugeeButton.ActionButton.buttonLabelText.SetOutlineColor(Refugee.Color);
                    Helpers.ShowTargetNameOnButtonExplicit(null, RefugeeButton, "REFUGE");
                    RefugeeButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;

                    return PlayerControl.LocalPlayer.CanMove && !(MapOptions.NeutralRoleBlock && Helpers.IsCommsActive());
                },
                () =>
                {
                    RefugeeButton.Timer = RefugeeButton.MaxTimer;
                    RefugeeButton.IsEffectActive = false;
                    RefugeeButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;

                },
                Refugee.GetRefugeeSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                true,
                Refugee.VestDuration,
                () => RefugeeButton.Timer = RefugeeButton.MaxTimer
            );

            Initialized = true;
            SetRefugeeCooldowns();
        }

        public static void SetRefugeeCooldowns()
        {
            if (!Initialized)
            {
                RefugeeRefugeButton();
            }

            RefugeeButton.EffectDuration = Refugee.VestDuration;
            RefugeeButton.MaxTimer = Refugee.VestCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            RefugeeRefugeButton();
        }
    }
}
