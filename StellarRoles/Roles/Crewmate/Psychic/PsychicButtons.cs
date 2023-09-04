using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class PsychicButtons
    {
        private static bool Initialized;

        public static CustomButton PsychicAbilitiesButton { get; set; }
        public static CustomButton PsychicPlayersButton { get; set; }


        public static void AbilitiesButton()
        {
            PsychicAbilitiesButton = new CustomButton(
                () =>
                {
                },
                () => Psychic.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Color faceColor = Color.white;
                    if (Psychic.AbilitesUsed <= 0)
                        faceColor = Color.white;
                    else if (Psychic.AbilitesUsed <= 5)
                        faceColor = Color.green;
                    else if (Psychic.AbilitesUsed <= 10)
                        faceColor = Color.yellow;
                    else
                        faceColor = Color.red;

                    PsychicAbilitiesButton.ActionButton.cooldownTimerText.SetFaceColor(faceColor);
                    PsychicAbilitiesButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    PsychicAbilitiesButton.Timer = Psychic.AbilitesUsed;
                    PsychicAbilitiesButton.ActionButton.buttonLabelText.color = Palette.EnabledColor;
                    Helpers.ShowTargetNameOnButtonExplicit(null, PsychicAbilitiesButton, $"Abilities");

                    return true;
                },
                () =>
                {
                    PsychicAbilitiesButton.Timer = 0f;
                    Psychic.AbilitesUsed = 0;
                },
                Psychic.GetPsychicAbilitiesButtonSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                "ActionQuaternary",
                false
            );
        }

        public static void PlayersButton()
        {
            PsychicPlayersButton = new CustomButton(
                () =>
                {
                },
                () => Psychic.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {

                    PsychicPlayersButton.ActionButton.buttonLabelText.SetOutlineColor(Color.cyan);
                    PsychicPlayersButton.Timer = Psychic.InRange;
                    PsychicPlayersButton.ActionButton.buttonLabelText.color = Palette.EnabledColor;
                    Helpers.ShowTargetNameOnButtonExplicit(null, PsychicPlayersButton, $"Players");

                    return true;
                },
                () =>
                {
                    PsychicPlayersButton.Timer = 0f;
                    Psychic.InRange = 0;
                },
                Psychic.GetPsychicPlayersButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                false
            );
        }

        public static void SetPsychicCooldowns()
        {
            if (!Initialized)
            {
                AddPsychicButtons();
            }

            PsychicAbilitiesButton.MaxTimer = 0f;
            PsychicAbilitiesButton.Timer = 0f;

            PsychicPlayersButton.MaxTimer = 0f;
            PsychicPlayersButton.Timer = 0f;

        }

        public static void AddPsychicButtons()
        {
            AbilitiesButton();
            PlayersButton();

            Initialized = true;
            SetPsychicCooldowns();
        }

        public static void Postfix()
        {
            Initialized = false;
            AddPsychicButtons();
        }
    }
}
