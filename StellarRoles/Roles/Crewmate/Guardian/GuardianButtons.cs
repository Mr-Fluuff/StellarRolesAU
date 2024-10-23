using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class GuardianButtons
    {
        private static bool Initialized;
        public static CustomButton GuardianShieldButton { get; set; }
        public static CustomButton GuardianSelfShieldButton { get; set; }

        public static void GuardianButton()
        {
            GuardianShieldButton = new CustomButton(
                () =>
                {
                    GuardianShieldButton.Timer = 0f;
                    RPCProcedure.Send(CustomRPC.GuardianSetShielded, Guardian.CurrentTarget);
                    RPCProcedure.GuardianSetShielded(Guardian.CurrentTarget);
                    SoundEffectsManager.Play(Sounds.Shield);
                },
                () =>
                {
                    return Guardian.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !Guardian.UsedShield;
                },
                () =>
                {
                    GuardianShieldButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    Helpers.ShowTargetNameOnButtonExplicit(null, GuardianShieldButton, "PROTECT");

                    return Guardian.CurrentTarget && PlayerControl.LocalPlayer.CanMove;
                },
                () => { },
                Guardian.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary"
            );
        }

        public static void GuardianSelfButton()
        {
            GuardianSelfShieldButton = new CustomButton(
                () =>
                {
                    GuardianSelfShieldButton.Timer = 0f;
                    RPCProcedure.Send(CustomRPC.GuardianSetShielded, PlayerControl.LocalPlayer);
                    RPCProcedure.GuardianSetShielded(PlayerControl.LocalPlayer);
                    SoundEffectsManager.Play(Sounds.Shield);
                    Guardian.SelfShieldCharges--;
                },
                () => Guardian.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !Guardian.UsedShield && Guardian.SelfShieldCharges > 0,
                () =>
                {
                    GuardianSelfShieldButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    Helpers.ShowTargetNameOnButtonExplicit(null, GuardianSelfShieldButton, "SELF\nPROTECT");

                    return PlayerControl.LocalPlayer.CanMove;
                },
                () => { },
                Guardian.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                "SecondAbility"
            );
        }

        public static void GuardianButtonsInt()
        {
            GuardianButton();
            GuardianSelfButton();

            Initialized = true;
            SetGuardianCooldowns();
        }

        public static void SetGuardianCooldowns()
        {
            if (!Initialized)
            {
                GuardianButtonsInt();
            }
            GuardianShieldButton.MaxTimer = 0f;
            GuardianShieldButton.Timer = 0f;
            GuardianSelfShieldButton.MaxTimer = 0f;
            GuardianSelfShieldButton.Timer = 5f;
        }

        public static void Postfix()
        {
            Initialized = false;
            GuardianButtonsInt();
            SetGuardianCooldowns();
        }
    }
}
