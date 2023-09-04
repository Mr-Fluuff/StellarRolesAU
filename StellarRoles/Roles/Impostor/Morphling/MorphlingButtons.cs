using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class MorphlingButtons
    {
        private static bool Initialized;

        public static CustomButton MorphlingButton { get; set; }
        public static PoolablePlayer MorphTargetDisplay { get; set; }

        public static void MorphlingMorphButton()
        {
            MorphlingButton = new CustomButton(
                () =>
                {
                    if (Morphling.SampledTarget != null)
                    {
                        RPCProcedure.Send(CustomRPC.MorphlingMorph, Morphling.SampledTarget.PlayerId);
                        RPCProcedure.MorphlingMorph(Morphling.SampledTarget);
                        Morphling.SampledTarget = null;
                        MorphlingButton.EffectDuration = Morphling.Duration;
                        SoundEffectsManager.Play(Sounds.Morph);
                    }
                    else if (Morphling.AbilityCurrentTarget != null)
                    {
                        Morphling.SampledTarget = Morphling.AbilityCurrentTarget;
                        MorphlingButton.Sprite = Morphling.GetMorphSprite();
                        MorphlingButton.EffectDuration = 1f;
                        SoundEffectsManager.Play(Sounds.Sample);

                        // Add poolable player to the button so that the target outfit is shown
                        MorphlingButton.ActionButton.cooldownTimerText.transform.localPosition = new Vector3(0, 0, -1f);  // Before the poolable player
                        MorphTargetDisplay = Object.Instantiate(Patches.IntroCutsceneOnDestroyPatch.playerPrefab, MorphlingButton.ActionButton.transform);
                        GameData.PlayerInfo data = Morphling.SampledTarget.Data;
                        Morphling.SampledTarget.SetPlayerMaterialColors(MorphTargetDisplay.cosmetics.currentBodySprite.BodySprite);
                        MorphTargetDisplay.SetSkin(data.DefaultOutfit.SkinId, data.DefaultOutfit.ColorId);
                        MorphTargetDisplay.SetHat(data.DefaultOutfit.HatId, data.DefaultOutfit.ColorId);
                        MorphTargetDisplay.cosmetics.nameText.text = "";  // Hide the name!
                        MorphTargetDisplay.transform.localPosition = new Vector3(0f, 0.0f, -0.01f);
                        MorphTargetDisplay.transform.localScale = Vector3.one * 0.4f;
                        MorphTargetDisplay.SetSemiTransparent(false);
                        MorphTargetDisplay.gameObject.SetActive(true);
                    }
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => Morphling.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    string text = "Morph";
                    if (Morphling.MorphTimer > 0)
                        text = "Morphed";
                    else if (Morphling.SampledTarget == null)
                        text = "Sample";

                    MorphlingButton.ActionButton.buttonLabelText.text = text;

                    return (Morphling.AbilityCurrentTarget || Morphling.SampledTarget) && PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked();
                },
                () =>
                {
                    MorphlingButton.Timer = MorphlingButton.MaxTimer / 2f * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    MorphlingButton.Sprite = Morphling.GetSampleSprite();
                    MorphlingButton.IsEffectActive = false;
                    MorphlingButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                    Morphling.SampledTarget = null;
                    if (MorphTargetDisplay != null)
                    {  // Reset the poolable player
                        MorphTargetDisplay.gameObject.SetActive(false);
                        Object.Destroy(MorphTargetDisplay.gameObject);
                        MorphTargetDisplay = null;
                    }
                },
                Morphling.GetSampleSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                true,
                Morphling.Duration,
                () =>
                {
                    if (Morphling.SampledTarget == null)
                    {
                        MorphlingButton.Timer = MorphlingButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                        MorphlingButton.Sprite = Morphling.GetSampleSprite();
                        SoundEffectsManager.Play(Sounds.Morph);

                        // Reset the poolable player
                        MorphTargetDisplay.gameObject.SetActive(false);
                        Object.Destroy(MorphTargetDisplay.gameObject);
                        MorphTargetDisplay = null;
                    }
                },
                false,
                "Sample"
            );

            Initialized = true;
            SetMorphlingCooldowns();
        }

        public static void SetMorphlingCooldowns()
        {
            if (!Initialized)
            {
                MorphlingMorphButton();
            }

            MorphlingButton.EffectDuration = Morphling.Duration;
            MorphlingButton.MaxTimer = Morphling.Cooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            MorphlingMorphButton();
        }
    }
}
