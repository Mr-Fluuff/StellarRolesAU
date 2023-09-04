using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class RomanticButtons
    {
        private static bool Initialized;
        public static CustomButton RomanticStalkButton { get; set; }
        public static CustomButton RomanticProtectButton { get; set; }
        public static CustomButton VengefulRomanticKillButton { get; set; }

        public static void RomanticButtonsInit()
        {
            StalkButton();
            ProtectButton();
            VengKillButton();

            Initialized = true;
            SetRomanticCooldowns();
        }

        public static void StalkButton()
        {
            RomanticStalkButton = new CustomButton(
                () =>
                {
                    if (Romantic.CurrentTarget != null)
                    {
                        RPCProcedure.Send(CustomRPC.FallInLove, Romantic.CurrentTarget.PlayerId);
                        RPCProcedure.FallInLove(Romantic.CurrentTarget);
                    }
                },
                () => !Romantic.HasLover && Romantic.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    RomanticStalkButton.ActionButton.buttonLabelText.SetOutlineColor(Romantic.Color);
                    Helpers.ShowTargetNameOnButtonExplicit(Romantic.Lover, RomanticStalkButton, "Romance");
                    return !Romantic.HasLover && Romantic.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove;
                },
                () => RomanticStalkButton.Timer = RomanticStalkButton.MaxTimer,
                Romantic.GetRomanceButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary"
            );
        }

        public static void ProtectButton()
        {
            RomanticProtectButton = new CustomButton(
                () =>
                {
                    RPCProcedure.Send(CustomRPC.RomanticShield);
                    RPCProcedure.RomanticShield();
                    SoundEffectsManager.Play(Sounds.Shield);
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => Romantic.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Romantic.HasLover,
                () =>
                {
                    RomanticProtectButton.ActionButton.buttonLabelText.SetOutlineColor(Romantic.Color);
                    Helpers.ShowTargetNameOnButtonExplicit(null, RomanticProtectButton, "PROTECT");
                    RomanticProtectButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;

                    return Romantic.HasLover && PlayerControl.LocalPlayer.CanMove && !(MapOptions.NeutralRoleBlock && Helpers.IsCommsActive());
                },
                () =>
                {
                    RomanticProtectButton.Timer = RomanticProtectButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    if (PlayerControl.LocalPlayer != VengefulRomantic.Player)
                        VengefulRomanticKillButton.Timer = CustomOptionHolder.GameStartKillCD.GetFloat();
                },
                Romantic.GetProtectButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionQuaternary",
                true,
                Romantic.VestDuration,
                () => RomanticProtectButton.Timer = RomanticProtectButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer)
            );
        }

        public static void VengKillButton()
        {
            VengefulRomanticKillButton = new CustomButton(
                () =>
                {
                    MurderAttemptResult murderAttemptResult = Helpers.CheckMuderAttempt(VengefulRomantic.Player, VengefulRomantic.CurrentTarget);
                    if (murderAttemptResult == MurderAttemptResult.SuppressKill)
                        return;

                    if (murderAttemptResult == MurderAttemptResult.PerformKill)
                    {
                        PlayerControl target = null;
                        if (VengefulRomantic.CurrentTarget == VengefulRomantic.Target)
                        {
                            target = VengefulRomantic.CurrentTarget;
                            Helpers.AvengedLover();
                            Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddCorrectShot);
                        }
                        else
                        {
                            target = PlayerControl.LocalPlayer;
                            Helpers.PlayerKilledByAbility(target);
                            Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddMisfire);
                            RPCProcedure.Send(CustomRPC.PsychicAddCount);
                        }

                        Helpers.UncheckedMurderPlayer(VengefulRomantic.Player, target, true);
                    }

                    VengefulRomanticKillButton.Timer = VengefulRomanticKillButton.MaxTimer;
                    VengefulRomantic.CurrentTarget = null;
                },
                () => VengefulRomantic.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !VengefulRomantic.AvengedLover && VengefulRomantic.Target != null && !PlayerControl.LocalPlayer.IsBombedAndActive(),
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, VengefulRomanticKillButton, "Avenge");
                    VengefulRomanticKillButton.ActionButton.buttonLabelText.SetOutlineColor(VengefulRomantic.Color);
                    return VengefulRomantic.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove && !VengefulRomantic.AvengedLover;
                },
                () => VengefulRomanticKillButton.Timer = VengefulRomanticKillButton.MaxTimer,
                Romantic.GetKillButtonSprite(),
                new Vector3(0f, 1f, 0),
                "ActionSecondary"
            );
        }

        public static void SetRomanticCooldowns()
        {
            if (!Initialized)
            {
                RomanticButtonsInit();
            }

            VengefulRomanticKillButton.MaxTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
            RomanticProtectButton.MaxTimer = Romantic.Cooldown;
            RomanticStalkButton.MaxTimer = 0f;
            RomanticProtectButton.EffectDuration = Romantic.VestDuration;
            RomanticStalkButton.Timer = 0;
        }

        public static void Postfix()
        {
            Initialized = false;
            RomanticButtonsInit();
        }
    }
}
