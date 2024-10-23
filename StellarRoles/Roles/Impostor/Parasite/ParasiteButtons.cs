using HarmonyLib;
using Hazel;
using StellarRoles;
using StellarRoles.Objects;
using System;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class ParasiteButtons
    {
        private static bool Initialized;

        public static CustomButton DecayButton { get; set; }
        public static CustomButton InfestButton { get; set; }
        public static float infesttimer => Helpers.KillCooldown() - 1f;


        public static void InitParasiteButtons()
        {
            InfestButonInit();
            DecayButtonInit();

            Initialized = true;
            SetParasiteCooldowns();
        }

        public static void InfestButonInit()
        {
            InfestButton = new CustomButton(
                () =>
                {
                    var MurderAttempt = Helpers.CheckMurderAttempt(PlayerControl.LocalPlayer, Parasite.CurrentTarget);
                    if (MurderAttempt == MurderAttemptResult.PerformKill)
                    {
                        ParasiteAbilites.ParasiteControl();
                        Helpers.SetKillerCooldown();
                        DecayButton.Timer = 1f;
                    }
                },
                () => { return Parasite.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Parasite.Controlled == null; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, InfestButton, "Infest");
                    return PlayerControl.LocalPlayer.CanMove && Parasite.CurrentTarget != null && !Impostor.IsRoleAblilityBlocked();
                },
                () =>
                {
                    InfestButton.Timer = Math.Max(Helpers.KillCooldown(), InfestButton.MaxTimer);
                    InfestButton.IsEffectActive = false;
                    InfestButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Parasite.GetControlSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "SecondAbility",
                false,
                0f,
                () => {},
                false,
                "Infest"
            );
        }


        public static void DecayButtonInit()
        {

            DecayButton = new CustomButton(
                () =>
                {
                    ParasiteAbilites.RPCKillInfected();
                    Helpers.SetKillerCooldown();
                    InfestButton.Timer = Parasite.InfestCooldown * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                },
                () => { return Parasite.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Parasite.Controlled != null; },
                () =>
                {
                    string timer = Parasite.Unlimited ? "" : $" - {(int)Parasite.ControlTimer}";
                    Helpers.ShowTargetNameOnButtonExplicit(null, DecayButton, $"Decay{timer}");
                    return PlayerControl.LocalPlayer.CanMove;
                },
                () =>
                {
                    DecayButton.Timer = 1f;
                    DecayButton.IsEffectActive = false;
                    DecayButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Parasite.GetKillInfestSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "SecondAbility",
                false,
                0f,
                () => {},
                false,
                "Decay"
            );
        }
       

        public static void SetParasiteCooldowns()
        {
            if (!Initialized)
            {
                InitParasiteButtons();
            }

            InfestButton.MaxTimer = Parasite.InfestCooldown * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
            DecayButton.MaxTimer = 1f;
            InfestButton.Timer = Math.Max(Helpers.KillCooldown(), InfestButton.MaxTimer * 0.75f);
            DecayButton.Timer = 1f;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitParasiteButtons();
        }
    }
}
