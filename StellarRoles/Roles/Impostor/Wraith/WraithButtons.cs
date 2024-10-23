using HarmonyLib;
using Hazel;
using StellarRoles.Objects;
using System;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class WraithButtons
    {
        private static bool Initialized;
        public static CustomButton WraithPhaseButton { get; set; }
        public static CustomButton WraithLanternPlaceButton { get; set; }
        public static CustomButton WraithLanternReturnButton { get; set; }

        public static void InitWraithButtons()
        {
            PhaseButton();
            LanternPlaceButton();
            LanternReturnButton();

            Initialized = true;
            SetWraithCooldowns();
        }

        public static void PhaseButton()
        {
            WraithPhaseButton = new CustomButton(
                () =>
                {
                    RPCProcedure.Send(CustomRPC.WraithPhase, true);
                    Wraith.PhaseOn = true;
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                },
                () => { return Wraith.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, WraithPhaseButton, "Dash");
                    return PlayerControl.LocalPlayer.CanMove && WraithPhaseButton.Timer < 0f && !Impostor.IsRoleAblilityBlocked();
                },
                () =>
                {
                    RPCProcedure.Send(CustomRPC.WraithPhase, false);
                    Wraith.PhaseOn = false;

                    WraithPhaseButton.Timer = WraithPhaseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    WraithPhaseButton.IsEffectActive = false;
                    WraithPhaseButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Wraith.GetPhaseButtonSprite(),
                CustomButton.ButtonPositions.LowerRow4,
                "SecondAbility",
                true,
                Wraith.CalculateDashDuration(),
                () =>
                {
                    RPCProcedure.Send(CustomRPC.WraithPhase, false);
                    Wraith.PhaseOn = false;
                    WraithPhaseButton.Timer = WraithPhaseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    ;
                }
            );
        }

        public static void LanternPlaceButton()
        {
            WraithLanternPlaceButton = new CustomButton(
                () =>
                {
                    if (Lantern.CurrentLantern == null)
                    {
                        UnityEngine.Vector3 pos = PlayerControl.LocalPlayer.transform.position;
                        byte[] buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)254, SendOption.Reliable);
                        writer.Write((byte)CustomRPC.PlaceLantern);
                        writer.WriteBytesAndSize(buff);
                        writer.EndMessage();
                        RPCProcedure.PlaceLantern(buff);
                        Wraith.LanternTimer = Wraith.CalculateWraithReturn();
                        WraithLanternReturnButton.Timer = 0.5f;
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    }
                },
                () =>
                {
                    return Wraith.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Wraith.HasLantern && Lantern.CurrentLantern == null;
                },
                () =>
                {
                    WraithLanternPlaceButton.ActionButton.graphic.sprite = Wraith.GetLanternButtonSprite();
                    Helpers.ShowTargetNameOnButtonExplicit(null, WraithLanternPlaceButton, "Place");
                    return PlayerControl.LocalPlayer.CanMove && WraithLanternPlaceButton.Timer < 0f && !Impostor.IsRoleAblilityBlocked()
;
                },
                () =>
                {
                    WraithLanternPlaceButton.Timer = WraithLanternPlaceButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    WraithLanternPlaceButton.IsEffectActive = false;
                    WraithLanternPlaceButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Wraith.GetLanternButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false,
                0f,
                () => WraithLanternPlaceButton.Timer = WraithLanternPlaceButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer)
            );
        }

        public static void LanternReturnButton()
        {
            WraithLanternReturnButton = new CustomButton(
                () =>
                {
                    if (Lantern.CurrentLantern != null)
                    {
                        RPCProcedure.Send(CustomRPC.WraithReturn);
                        RPCProcedure.WraithReturn();
                        WraithLanternPlaceButton.Timer = WraithLanternPlaceButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                        ;

                        RPCProcedure.Send(CustomRPC.SetInvisible, Wraith.Player, false);
                        RPCProcedure.SetInvisible(Wraith.Player, false);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    }
                },
                () =>
                {
                    return Wraith.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Wraith.HasLantern && Lantern.CurrentLantern != null;
                },
                () =>
                {
                    WraithLanternReturnButton.ActionButton.graphic.sprite = Wraith.GetReturnButtonSprite();
                    Helpers.ShowTargetNameOnButtonExplicit(null, WraithLanternReturnButton, $"Return - {(int)Wraith.LanternTimer}");
                    return PlayerControl.LocalPlayer.CanMove && Lantern.CurrentLantern != null;
                },
                () =>
                {
                    WraithLanternReturnButton.Timer = WraithLanternReturnButton.MaxTimer;
                    WraithLanternReturnButton.IsEffectActive = false;
                    WraithLanternReturnButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Wraith.GetReturnButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false,
                0f,
                () => WraithLanternReturnButton.Timer = WraithLanternReturnButton.MaxTimer
            );
        }

        public static void SetWraithCooldowns()
        {
            if (!Initialized)
            {
                InitWraithButtons();
            }

            WraithLanternPlaceButton.EffectDuration = Wraith.CalculateWraithReturn();
            WraithLanternPlaceButton.MaxTimer = Wraith.LanternCooldown;
            WraithLanternReturnButton.MaxTimer = 0.5f;
            WraithLanternReturnButton.Timer = 0.5f;
            WraithPhaseButton.EffectDuration = Wraith.CalculateDashDuration();
            WraithPhaseButton.MaxTimer = Wraith.PhaseCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitWraithButtons();
        }
    }
}
