using HarmonyLib;
using Hazel;
using StellarRoles.Objects;
using System;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class ShadeButtons
    {
        private static bool Initialized;

        public static CustomButton BlindButton { get; set; }
        public static CustomButton VanishButton { get; set; }
        public static CustomButton EmergeButton { get; set; }

        public static void InitShadeButtons()
        {
            BlindButtonInit();
            VanishButtonInit();
            EmergeButtonInit();

            Initialized = true;
            SetShadeCooldowns();
        }

        public static void BlindButtonInit()
        {
            BlindButton = new CustomButton(
                () =>
                {
                    if (Shade.BlindRadius == 10)
                    {
                        RPCProcedure.Send(CustomRPC.ShadeGlobalBlind);
                        RPCProcedure.ShadeGlobalBlind();
                    }
                    else
                    {
                        bool isImp = PlayerControl.LocalPlayer.Data.Role.IsImpostor;
                        foreach (PlayerControl player in Helpers.FindClosestPlayers(PlayerControl.LocalPlayer, Shade.GetShadeRadius()))
                        {
                            if (isImp && player.Data.Role.IsImpostor)
                                continue;

                            RPCProcedure.Send(CustomRPC.ShadeNearBlind, player.PlayerId);
                            RPCProcedure.ShadeNearBlind(player);
                        }
                    }
                    SoundEffectsManager.Play(Sounds.Click);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => Shade.Player == PlayerControl.LocalPlayer && Shade.Killed >= Shade.KillsToGainBlind && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, BlindButton, "Blind");
                    return PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked();
                },
                () =>
                {
                    BlindButton.Timer = BlindButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    BlindButton.IsEffectActive = false;
                    BlindButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Shade.GetBlindButtonSprite(),
                CustomButton.ButtonPositions.LowerRow4,
                "SecondAbility",
                true,
                Shade.BlindDuration,
                () =>
                {
                    BlindButton.Timer = BlindButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    SoundEffectsManager.Play(Sounds.Click);
                }
            );
        }

        public static void VanishButtonInit()
        {
            VanishButton = new CustomButton(
                () =>
                {
                    if (!Shade.IsInvisble)
                    {
                        RPCProcedure.Send(CustomRPC.SetInvisible, PlayerControl.LocalPlayer.PlayerId, false);
                        RPCProcedure.SetInvisible(PlayerControl.LocalPlayer, false);
                        SoundEffectsManager.Play(Sounds.Morph);

                        if (Shade.EvidenceDuration > 0f)
                        {
                            UnityEngine.Vector3 pos = PlayerControl.LocalPlayer.transform.position;
                            byte[] buff = new byte[sizeof(float) * 2];
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                            Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                            MessageWriter writer = AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaceShadeTrace, SendOption.Reliable);
                            writer.WriteBytesAndSize(buff);
                            writer.EndMessage();
                            RPCProcedure.PlaceShadeTrace(buff);
                        }

                        EmergeButton.Timer = 0.5f;
                        VanishButton.Timer = VanishButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);

                    }
                },
                () => !Shade.IsInvisble && Shade.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, VanishButton, "VANISH");
                    return PlayerControl.LocalPlayer.CanMove && !Shade.IsInvisble && !Impostor.IsRoleAblilityBlocked()
;
                },
                () =>
                {
                    VanishButton.Timer = VanishButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    VanishButton.IsEffectActive = false;
                    VanishButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Shade.GetShadeButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false
            );
        }

        public static void EmergeButtonInit()
        {
            EmergeButton = new CustomButton(
                () =>
                {
                    if (Shade.IsInvisble)
                    {
                        SoundEffectsManager.Play(Sounds.Morph);
                        Shade.InvisibleTimer = 0f;
                        VanishButton.Timer = VanishButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    }
                },
                () => Shade.IsInvisble && Shade.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, EmergeButton, $"EMERGE - {(int)Shade.InvisibleTimer}");
                    return PlayerControl.LocalPlayer.CanMove && Shade.IsInvisble;
                },
                () => EmergeButton.Timer = 0.5f,
                Shade.GetShadeButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false
            );
        }

        public static void SetShadeCooldowns()
        {
            if (!Initialized)
            {
                InitShadeButtons();
            }

            BlindButton.EffectDuration = Shade.BlindDuration;
            BlindButton.MaxTimer = Shade.BlindCooldown * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
            VanishButton.MaxTimer = Shade.ShadeCooldown * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
            EmergeButton.MaxTimer = 0.5f;
            EmergeButton.Timer = 0.5f;
            BlindButton.Timer = 0f;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitShadeButtons();
        }
    }
}
