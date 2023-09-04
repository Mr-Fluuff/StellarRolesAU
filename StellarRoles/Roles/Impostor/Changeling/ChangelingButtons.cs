using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class ChangelingButtons
    {
        private static bool Initialized;
        public static CustomButton ChangelingButton { get; set; }

        public static void ChanglingChangeButton()
        {
            ChangelingButton = new CustomButton(
                () =>
                {
                    ChangelingUtils.ChanglingButton();
                },
                () => { return Changeling.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, ChangelingButton, "Transform");
                    return PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked();
                },
                () => { ChangelingButton.Timer = ChangelingButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer); },
                Changeling.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary"
            );
            Initialized = true;
            SetChanglingCooldowns();
        }

        public static void SetChanglingCooldowns()
        {
            if (!Initialized)
            {
                ChanglingChangeButton();
            }
            ChangelingButton.MaxTimer = 15f;
        }

        public static void Postfix()
        {
            Initialized = false;
            ChanglingChangeButton();
        }
    }
}
