using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class EngineerButtons
    {
        private static bool Initialized;
        public static CustomButton EngineerRepair { get; set; }
        public static CustomButton EngineerVent { get; set; }

        public static void EngineerButtonsInt()
        {
            EngineerRepairButton();
            EngineerVentButton();

            Initialized = true;
            SetEngineerCooldowns();
        }
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
        }
        public static void EngineerVentButton()
        {
            EngineerVent = new CustomButton(
                () =>
                {
                    if (Engineer.currentTarget != null)
                    {
                        if (Engineer.Player.inVent)
                        {
                            EngineerVent.CanShake = false;
                            if (Engineer.restrictVenting)
                            {
                                EngineerVent.Timer = Engineer.ventCooldown;
                            }
                        }
                        else
                        {
                            EngineerVent.CanShake = true;
                            if (Engineer.restrictVenting)
                            {
                                EngineerVent.Timer = Engineer.maxVentTime;
                                Engineer.inVentTimeRemaining = Engineer.maxVentTime;
                            }
                        }
                        Engineer.currentTarget.Use();
                    }
                },
                () => { return Engineer.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Engineer.CanVent; },
                () =>
                {
                    if (Engineer.Player.inVent)
                    {
                        float ventTime = Engineer.maxVentTime;
                        if (ventTime > 0f && Engineer.restrictVenting)
                        {
                            Engineer.inVentTimeRemaining -= Time.deltaTime;
                        }
                        if (((Engineer.inVentTimeRemaining < 0f && Engineer.restrictVenting)
                        || (Helpers.IsCommsActive() && Engineer.RoleBlock)) && Engineer.currentTarget && !Engineer.isExitVentQueued)
                        {
                            if (Engineer.restrictVenting)
                            {
                                EngineerVent.Timer = Engineer.ventCooldown;
                            }
                            EngineerVent.CanShake = false;
                            Engineer.currentTarget.Use();
                            Engineer.isExitVentQueued = true;
                        }
                    }
                    else
                    {
                        Engineer.isExitVentQueued = false;
                    }
                    EngineerVent.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    return (PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.inVent) && Engineer.currentTarget && !EngineerAbilities.IsRoleBlocked();
                },
                () =>
                {
                    if (Engineer.restrictVenting)
                    {
                        EngineerVent.Timer = Engineer.ventCooldown;
                    }
                    EngineerVent.CanShake = false;
                },
                Engineer.GetVentButtonSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                null,
                false,
                "Vent"
            );
        }

        public static void SetEngineerCooldowns()
        {
            if (!Initialized)
            {
                EngineerButtonsInt();
            }

            EngineerRepair.MaxTimer = 0f;
            EngineerRepair.Timer = 0f;
            EngineerVent.Timer = 0f;
        }

        public static void Postfix()
        {
            Initialized = false;
            EngineerButtonsInt();
        }
    }
}
