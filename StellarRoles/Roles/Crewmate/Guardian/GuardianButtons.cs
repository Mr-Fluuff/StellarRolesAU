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

        public static void GuardianButton()
        {
            GuardianShieldButton = new CustomButton(
                () =>
                {
                    GuardianShieldButton.Timer = 0f;
                    RPCProcedure.Send(CustomRPC.GuardianSetShielded, Guardian.CurrentTarget.PlayerId);
                    RPCProcedure.GuardianSetShielded(Guardian.CurrentTarget);
                    SoundEffectsManager.Play(Sounds.Shield);
                },
                () => Guardian.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !Guardian.UsedShield,
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
            Initialized = true;
            SetGuardianCooldowns();
        }

        public static void SetGuardianCooldowns()
        {
            if (!Initialized)
            {
                GuardianButton();
            }
            GuardianShieldButton.MaxTimer = 0f;
            GuardianShieldButton.Timer = 0f;
        }

        public static void Postfix()
        {
            Initialized = false;
            GuardianButton();
            SetGuardianCooldowns();
        }
    }
}
