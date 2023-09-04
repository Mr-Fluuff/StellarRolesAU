using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;

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
                    MurderAttemptResult murder = Helpers.CheckMuderAttempt(Vampire.Player, Vampire.AbilityCurrentTarget);
                    if (murder == MurderAttemptResult.PerformKill)
                    {
                        Vampire.Bitten = Vampire.AbilityCurrentTarget;
                        RPCProcedure.Send(CustomRPC.VampireSetBitten, Vampire.Bitten.PlayerId);
                        RPCProcedure.VampireSetBitten(Vampire.Bitten.PlayerId);
                        Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddAbilityKill, InfoType.AddKill);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Vampire.CalculateBiteDelay(), new Action<float>((p) =>
                        { // Delayed action
                            if (p == 1f)
                            {
                                // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                                Helpers.CheckMurderAttemptAndKill(Vampire.Player, Vampire.Bitten, showAnimation: false, true);
                                RPCProcedure.Send(CustomRPC.VampireResetBitten);
                                Vampire.Bitten = null;
                                RPCProcedure.Send(CustomRPC.PsychicAddCount);
                            }
                        })));
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
                () => Vampire.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.IsBombedAndActive(),
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
    }
}
