using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class TrapperButtons
    {
        private static bool Initialized;

        public static CustomButton TrapperCoverButton { get; set; }
        public static CustomButton TrapperSetTrapButton { get; set; }

        public static void InitTrapperButtons()
        {
            CoverButton();
            SetTrapButton();

            Initialized = true;
            SetTrapperCooldowns();
        }

        public static void CoverButton()
        {
            TrapperCoverButton = new CustomButton(
                () =>
                {
                    if (Trapper.VentTarget != null)
                    { // Seal vent
                        RPCProcedure.Send(CustomRPC.SealVent, Trapper.VentTarget.Id);
                        RPCProcedure.SealVent(Trapper.VentTarget.Id);
                        Trapper.VentTarget = null;
                        Trapper.CoverCount--;

                    }
                    SoundEffectsManager.Play(Sounds.PlaceTrap);  // Same sound used for both types (cam or vent)!
                    TrapperCoverButton.Timer = TrapperCoverButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    TrapperSetTrapButton.Timer = TrapperSetTrapButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => Trapper.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Trapper.CoverCount > 0,
                () =>
                {
                    TrapperCoverButton.ActionButton.graphic.sprite = Trapper.GetVentBoardButton();
                    Helpers.ShowTargetNameOnButtonExplicit(null, TrapperCoverButton, $"Cover - {(int)Trapper.CoverCount}");
                    TrapperCoverButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                    TrapperCoverButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);

                    return Trapper.VentTarget != null && Trapper.CoverCount > 0 && PlayerControl.LocalPlayer.CanMove && !TrapperAbilities.IsRoleBlocked();
                },
                () =>
                {
                    TrapperCoverButton.Timer = TrapperCoverButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                },
                Trapper.GetVentBoardButton(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary"
            );
        }

        public static void SetTrapButton()
        {
            TrapperSetTrapButton = new CustomButton(
                () =>
                {
                    if (Trapper.VentTarget != null)
                    { // Seal vent
                        RPCProcedure.Send(CustomRPC.SetTrap, Trapper.VentTarget.Id);
                        RPCProcedure.SetVentTrap(Trapper.VentTarget.Id);
                        Trapper.VentTarget = null;
                        Trapper.TrapCount--;

                    }
                    SoundEffectsManager.Play(Sounds.PlaceTrap);  // Same sound used for both types (cam or vent)!
                    TrapperSetTrapButton.Timer = TrapperSetTrapButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    TrapperCoverButton.Timer = TrapperCoverButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => Trapper.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Trapper.TrapCount > 0,
                () =>
                {
                    TrapperSetTrapButton.ActionButton.graphic.sprite = Trapper.GetCloseVentButtonSprite();
                    Helpers.ShowTargetNameOnButtonExplicit(null, TrapperSetTrapButton, $"Trap - {(int)Trapper.TrapCount}");
                    TrapperSetTrapButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                    TrapperSetTrapButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);

                    return Trapper.VentTarget != null && Trapper.TrapCount > 0 && PlayerControl.LocalPlayer.CanMove && !TrapperAbilities.IsRoleBlocked();
                },
                () => TrapperSetTrapButton.Timer = TrapperSetTrapButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer),
                Trapper.GetCloseVentButtonSprite(),
               CustomButton.ButtonPositions.UpperRow2,
                "SecondAbility"
            );
        }

        public static void SetTrapperCooldowns()
        {
            if (!Initialized)
            {
                InitTrapperButtons();
            }

            TrapperCoverButton.MaxTimer = Trapper.CoverCooldown;
            TrapperSetTrapButton.MaxTimer = Trapper.TrapCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitTrapperButtons();
        }
    }
}
