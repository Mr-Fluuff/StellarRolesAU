using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class CamouflagerButtons
    {
        public static bool initialized;
        public static CustomButton CamouflagerButton;

        public static void CamouflagerCamoButton()
        {
            CamouflagerButton = new CustomButton(
                () =>
                {
                    RPCProcedure.Send(CustomRPC.CamouflagerCamouflage);
                    RPCProcedure.CamouflagerCamouflage();
                    SoundEffectsManager.Play(Sounds.Morph);
                    Camouflager.ChargesRemaining--;
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => Camouflager.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, CamouflagerButton, $"CAMO - {Camouflager.ChargesRemaining}");
                    return PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked() && Camouflager.ChargesRemaining > 0;
                },
                () =>
                {
                    CamouflagerButton.Timer = CamouflagerButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    CamouflagerButton.IsEffectActive = false;
                    CamouflagerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Camouflager.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                true,
                Camouflager.Duration,
                () =>
                {
                    CamouflagerButton.Timer = CamouflagerButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    SoundEffectsManager.Play(Sounds.Morph);
                }
            );

            initialized = true;
            SetCamouflagerCooldowns();
        }

        public static void SetCamouflagerCooldowns()
        {
            if (!initialized)
            {
                CamouflagerCamoButton();
            }

            CamouflagerButton.EffectDuration = Camouflager.Duration;
            CamouflagerButton.MaxTimer = Camouflager.Cooldown;
        }

        public static void Postfix()
        {
            initialized = false;
            CamouflagerCamoButton();
        }
    }
}
