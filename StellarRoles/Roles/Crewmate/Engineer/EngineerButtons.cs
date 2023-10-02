using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class EngineerButtons
    {
        private static bool Initialized;
        public static CustomButton EngineerRepair { get; set; }

        public static void EngineerRepairButton()
        {
            EngineerRepair = new CustomButton(
                () =>
                {
                    EngineerRepair.Timer = 0f;
                    Helpers.GetActiveSabo().RepairSabo();
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    Engineer.HasFix = false;
                    SoundEffectsManager.Play(Sounds.Repair);
                },
                () => { return Engineer.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Engineer.HasFix; },
                () =>
                {
                    EngineerRepair.ActionButton.buttonLabelText.SetOutlineColor(Engineer.Color);
                    return Helpers.IsSaboActive() && PlayerControl.LocalPlayer.CanMove && !EngineerAbilities.IsRoleBlocked();
                },
                () => { },
                Engineer.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                false,
                "fix"
            );
            Initialized = true;
            SetEngineerCooldowns();
        }

        public static void SetEngineerCooldowns()
        {
            if (!Initialized)
            {
                EngineerRepairButton();
            }

            EngineerRepair.MaxTimer = 0f;
            EngineerRepair.Timer = 0f;
        }

        public static void Postfix()
        {
            Initialized = false;
            EngineerRepairButton();
        }
    }
}
