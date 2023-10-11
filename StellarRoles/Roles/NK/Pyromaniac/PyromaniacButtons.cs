using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class PyromaniacButtons
    {
        private static bool Initialized;
        public static CustomButton PyromaniacKillButton { get; set; }
        public static CustomButton PyromaniacDouseButton { get; set; }

        public static void InitPyromaniacButtons(HudManager __instance)
        {
            KillButton(__instance);
            DouseButton();

            Initialized = true;
            SetPyromaniacCooldowns();
        }

        public static void KillButton(HudManager __instance)
        {
            PyromaniacKillButton = new CustomButton(
                () =>
                {
                    PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac);
                    if (Helpers.CheckMurderAttemptAndKill(pyromaniac.Player, pyromaniac.CurrentTarget) == MurderAttemptResult.SuppressKill)
                        return;

                    PyromaniacKillButton.Timer = pyromaniac.DousedPlayers.Contains(pyromaniac.CurrentTarget) ? Pyromaniac.DouseKillCooldown : PyromaniacKillButton.MaxTimer;

                    pyromaniac.CurrentTarget = null;

                    if (PyromaniacDouseButton.Timer < PyromaniacDouseButton.MaxTimer * 0.5f)
                        PyromaniacDouseButton.Timer = PyromaniacDouseButton.MaxTimer * 0.5f;
                    Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddKill);
                },
                () => PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac) && !pyromaniac.Player.Data.IsDead && !pyromaniac.Player.IsBombedAndActive(),
                () =>
                {
                    PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac);

                    if (pyromaniac.CurrentTarget != null)
                    {
                        bool isDoused = pyromaniac.DousedPlayers.Contains(pyromaniac.CurrentTarget);
                        PyromaniacKillButton.ActionButton.graphic.sprite = isDoused ? Pyromaniac.GetPyroIgniteSprite() : __instance.KillButton.graphic.sprite;
                        Helpers.ShowTargetNameOnButtonExplicit(null, PyromaniacKillButton, isDoused ? "BURN" : "KILL");
                    }
                    else
                    {
                        PyromaniacKillButton.ActionButton.graphic.sprite = __instance.KillButton.graphic.sprite;
                        Helpers.ShowTargetNameOnButtonExplicit(null, PyromaniacKillButton, "KILL");
                    }
                    PyromaniacKillButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                    PyromaniacKillButton.ActionButton.buttonLabelText.SetOutlineColor(Pyromaniac.Color);

                    return pyromaniac.CurrentTarget && PlayerControl.LocalPlayer.CanMove;
                },
                () =>
                {
                    PyromaniacKillButton.Timer = PyromaniacKillButton.MaxTimer;
                },
                __instance.KillButton.graphic.sprite,
                CustomButton.ButtonPositions.UpperRow1,
                "ActionSecondary"
            );
        }

        public static void DouseButton()
        {
            PyromaniacDouseButton = new CustomButton(
                () =>
                {
                    PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac);
                    if (pyromaniac.AbilityCurrentTarget != null)
                    {
                        pyromaniac.DouseTarget = pyromaniac.AbilityCurrentTarget;
                        PyromaniacDouseButton.HasEffect = true;
                        SoundEffectsManager.Play(Sounds.Douse);
                    }
                },
                () => PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac) && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac);
                    Helpers.ShowTargetNameOnButtonExplicit(null, PyromaniacDouseButton, "DOUSE");
                    PyromaniacDouseButton.ActionButton.buttonLabelText.SetOutlineColor(Pyromaniac.Color);

                    if (PyromaniacDouseButton.IsEffectActive && pyromaniac.DouseTarget != pyromaniac.AbilityCurrentTarget)
                    {
                        pyromaniac.DouseTarget = null;
                        PyromaniacDouseButton.Timer = 0f;
                        PyromaniacDouseButton.IsEffectActive = false;
                    }

                    return PlayerControl.LocalPlayer.CanMove && pyromaniac.AbilityCurrentTarget != null && !(MapOptions.NeutralKillerRoleBlock && Helpers.IsCommsActive());
                },
                () =>
                {
                    if (PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac))
                    {
                        PyromaniacDouseButton.Timer = PyromaniacDouseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                        PyromaniacDouseButton.IsEffectActive = false;
                        pyromaniac.DouseTarget = null;
                    }
                },
                Pyromaniac.GetPyromaniacDouse(),
                CustomButton.ButtonPositions.UpperRow2,
                "ActionQuaternary",
                true,
                Pyromaniac.DouseDuration,
                () =>
                {
                    PlayerControl.LocalPlayer.IsPyromaniac(out Pyromaniac pyromaniac);
                    if (pyromaniac.DouseTarget != null)
                    {
                        RPCProcedure.Send(CustomRPC.PyromaniacDouse, pyromaniac.Player.PlayerId, pyromaniac.DouseTarget.PlayerId);
                        pyromaniac.DousedPlayers.Add(pyromaniac.DouseTarget);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    }
                    pyromaniac.DouseTarget = null;
                    PyromaniacDouseButton.Timer = PyromaniacDouseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);

                    if (PyromaniacKillButton.Timer < PyromaniacKillButton.MaxTimer * 0.5f)
                        PyromaniacKillButton.Timer = PyromaniacKillButton.MaxTimer * 0.5f;
                }
            );
        }

        public static void SetPyromaniacCooldowns()
        {
            if (!Initialized)
            {
                InitPyromaniacButtons(HudManager.Instance);
            }

            PyromaniacDouseButton.EffectDuration = Pyromaniac.DouseDuration;
            PyromaniacDouseButton.MaxTimer = Pyromaniac.DouseCooldown;
            PyromaniacKillButton.MaxTimer = Helpers.KillCooldown();
        }

        public static void Postfix(HudManager __instance)
        {
            Initialized = false;
            InitPyromaniacButtons(__instance);
        }
    }
}
