using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class VampireButtons
    {
        private static bool Initialized;
        public static CustomButton VampireBiteButton { get; set; }

        public static void VampireButton()
        {
            VampireBiteButton = new CustomButton(
                () =>
                {
                    MurderAttemptResult murder = Helpers.CheckMurderAttempt(Vampire.Player, Vampire.AbilityCurrentTarget);
                    if (murder == MurderAttemptResult.PerformKill)
                    {
                        Vampire.Bitten = Vampire.AbilityCurrentTarget;
                        RPCProcedure.Send(CustomRPC.VampireSetBitten, Vampire.Bitten);
                        RPCProcedure.VampireSetBitten(Vampire.Bitten.PlayerId);
                        PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddAbilityKill, InfoType.AddKill);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        HudManager.Instance.StartCoroutine(DelayVampire());
                        SoundEffectsManager.Play(Sounds.Bite);

                        VampireBiteButton.EffectDuration = Vampire.CalculateBiteDelay();
                        VampireBiteButton.HasEffect = true; // Trigger effect on this click

                        Helpers.SetKillerCooldown();
                    }
                    else
                    {
                        VampireBiteButton.HasEffect = false;
                    }
                },
                () =>
                {
                    return Vampire.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.IsBombedAndActive();
                },
                () =>
                {
                    VampireBiteButton.ActionButton.graphic.sprite = Vampire.GetButtonSprite();

                    return Vampire.AbilityCurrentTarget != null
                        && PlayerControl.LocalPlayer.CanMove
                        && (!(MapOptions.ImposterKillAbilitiesRoleBlock && Helpers.IsCommsActive()) || !Vampire.HasKillButton);
                },
                () =>
                {
                    VampireBiteButton.Timer = VampireBiteButton.MaxTimer;
                    VampireBiteButton.IsEffectActive = false;
                    VampireBiteButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Vampire.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false,
                0f,
                () => VampireBiteButton.Timer = VampireBiteButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer),
                false,
                "Bite"
            );

            Initialized = true;
            SetVampireCooldowns();
        }

        public static void SetVampireCooldowns()
        {
            if (!Initialized)
            {
                VampireButton();
            }

            VampireBiteButton.EffectDuration = Vampire.CalculateBiteDelay();
            VampireBiteButton.MaxTimer = Vampire.BiteCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            VampireButton();
        }

        private static readonly WaitForSeconds delay = new WaitForSeconds(0.25f);

        public static IEnumerator DelayVampire()
        {
            yield return new WaitForSeconds(Vampire.CalculateBiteDelay());
            while (Vampire.Bitten.inMovingPlat || Vampire.Bitten.onLadder)
            {
                yield return delay;
            }
            RPCKillBitten();
        }

        public static void RPCKillBitten()
        {
            Helpers.CheckMurderAttemptAndKill(Vampire.Player, Vampire.Bitten, showAnimation: false, true);
            RPCProcedure.Send(CustomRPC.VampireResetBitten);
            Vampire.Bitten = null;
            RPCProcedure.Send(CustomRPC.PsychicAddCount);
        }
    }
}
