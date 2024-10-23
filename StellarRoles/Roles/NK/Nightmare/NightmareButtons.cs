using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class NightmareButtons
    {
        private static bool Initialized;
        public static CustomButton NightmareParalyzeButton { get; set; }
        public static CustomButton NightmareBlindButton { get; set; }

        public static void InitNightmareButtons()
        {
            BlindButton();
            ParalyzeButton();

            Initialized = true;
            SetNightmareCooldowns();
        }

        public static void BlindButton()
        {
            NightmareBlindButton = new CustomButton(
                () =>
                {
                    PlayerControl localplayer = PlayerControl.LocalPlayer;
                    foreach (PlayerControl target in localplayer.FindClosestPlayers(Nightmare.BlindRadius))
                    {
                        RPCProcedure.Send(CustomRPC.NightMareBlind, localplayer, target);
                        RPCProcedure.NightMareBlind(localplayer, target);
                    }

                    NightmareBlindButton.Timer = NightmareBlindButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                },
                () => { return PlayerControl.LocalPlayer.IsNightmare(out _) && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, NightmareBlindButton, "BLIND");
                    NightmareBlindButton.ActionButton.buttonLabelText.SetOutlineColor(Nightmare.Color);
                    return !(MapOptions.NeutralKillerRoleBlock && Helpers.IsCommsActive());
                },
                () =>
                {
                    NightmareBlindButton.Timer = NightmareBlindButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    NightmareBlindButton.IsEffectActive = false;
                    NightmareBlindButton.ActionButton.graphic.color = Palette.EnabledColor;
                },
                Nightmare.GetNightmareBlindSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                true,
                Nightmare.BlindDuration,
                () => NightmareBlindButton.Timer = NightmareBlindButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer)
            );
        }

        public static void ParalyzeButton()
        {
            NightmareParalyzeButton = new CustomButton(
                () =>
                {
                    PlayerControl target = PlayerControl.LocalPlayer.IsNightmare(out Nightmare nightmare) ? nightmare.AbilityCurrentTarget : null;
                    if (target != null)
                    {
                        RPCProcedure.Send(CustomRPC.ParalyzePlayer, PlayerControl.LocalPlayer, target);
                        RPCProcedure.ParalyzePlayer(nightmare, nightmare.AbilityCurrentTarget);
                    }
                    NightmareParalyzeButton.Timer = NightmareParalyzeButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                },
                () =>
                {
                    return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.IsNightmare(out _);
                },
                () =>
                {

                    NightmareParalyzeButton.ActionButton.graphic.sprite = Nightmare.GetNightmareParalyzeSprite();
                    Helpers.ShowTargetNameOnButtonExplicit(null, NightmareParalyzeButton, "PARALYZE");
                    NightmareParalyzeButton.ActionButton.buttonLabelText.SetOutlineColor(NeutralKiller.Color);
                    PlayerControl target = PlayerControl.LocalPlayer.IsNightmare(out Nightmare nightmare) ? nightmare.AbilityCurrentTarget : null;
                    return target != null && !(MapOptions.NeutralKillerRoleBlock && Helpers.IsCommsActive());
                },
                () =>
                {
                    NightmareParalyzeButton.Timer = NightmareParalyzeButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    NightmareParalyzeButton.IsEffectActive = false;
                    NightmareParalyzeButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Nightmare.GetNightmareParalyzeSprite(),
                CustomButton.ButtonPositions.UpperRow2,
                "SecondAbility",
                false);
        }

        public static void SetNightmareCooldowns()
        {
            if (!Initialized)
            {
                InitNightmareButtons();
            }

            NightmareBlindButton.EffectDuration = Nightmare.BlindDuration;
            NightmareBlindButton.MaxTimer = Nightmare.BlindCooldown;
            NightmareParalyzeButton.MaxTimer = Nightmare.ParalyzeCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            InitNightmareButtons();
        }
    }
}
