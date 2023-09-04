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
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    EngineerRepair.Timer = 0f;
                    SoundEffectsManager.Play(Sounds.Repair);
                    Engineer.HasFix = false;
                    SabatageTypes Sabotage = Helpers.GetActiveSabo();
                    if (Sabotage == SabatageTypes.Lights)
                    {
                        RPCProcedure.Send(CustomRPC.EngineerFixLights);
                        RPCProcedure.EngineerFixLights();
                    }
                    else if (Sabotage == SabatageTypes.O2)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                    }
                    else if (Sabotage == SabatageTypes.Reactor)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 16);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);

                    }
                    else if (Sabotage == SabatageTypes.Reactor)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Laboratory, 16);
                    }
                    else if (Sabotage == SabatageTypes.Comms)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                    }
                    else if (Sabotage == SabatageTypes.OxyMask)
                    {
                        SubmergedCompatibility.RepairOxygen();
                        RPCProcedure.Send(CustomRPC.EngineerFixSubmergedOxygen);
                    }
                },
                () => { return Engineer.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Engineer.HasFix; },
                () =>
                {
                    EngineerRepair.ActionButton.buttonLabelText.SetOutlineColor(Engineer.Color);
                    return Helpers.GetActiveSabo() != SabatageTypes.None && PlayerControl.LocalPlayer.CanMove && !EngineerAbilities.IsRoleBlocked();
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
