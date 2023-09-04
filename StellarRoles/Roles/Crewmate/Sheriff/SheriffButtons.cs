using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class SheriffButtons
    {
        private static bool Initialized;

        public static CustomButton SheriffKillButton { get; set; }

        public static void SheriffButton()
        {
            SheriffKillButton = new CustomButton(
                () =>
                {
                    MurderAttemptResult murderAttemptResult = Helpers.CheckMuderAttempt(Sheriff.Player, Sheriff.CurrentTarget);
                    if (murderAttemptResult == MurderAttemptResult.SuppressKill)
                        return;
                    if (murderAttemptResult == MurderAttemptResult.PerformKill)
                    {
                        PlayerControl target = null;
                        if (Sheriff.CanBeKilledBySheriff(Sheriff.CurrentTarget))
                        {
                            target = Sheriff.CurrentTarget;
                            Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddCorrectShot);
                        }
                        else if (Sheriff.MisfireKills == Misfire.Self)
                        {
                            target = PlayerControl.LocalPlayer;
                            Helpers.PlayerKilledByAbility(target);
                            Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddMisfire);
                            RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        }
                        else if (Sheriff.MisfireKills == Misfire.Target)
                        {
                            target = Sheriff.CurrentTarget;
                            Sheriff.Haskilled = true;
                            Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddMisfire);
                            RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        }
                        else if (Sheriff.MisfireKills == Misfire.Both)
                        {
                            target = Sheriff.CurrentTarget;
                            Helpers.UncheckedMurderPlayer(Sheriff.Player, PlayerControl.LocalPlayer, true);
                            Helpers.PlayerKilledByAbility(Sheriff.Player);
                            Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddMisfire);
                            RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        }
                        Helpers.UncheckedMurderPlayer(Sheriff.Player, target, true);
                    }

                    SheriffKillButton.Timer = SheriffKillButton.MaxTimer;
                    Sheriff.CurrentTarget = null;
                },
                () => { return Sheriff.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !Sheriff.Haskilled && !PlayerControl.LocalPlayer.IsBombedAndActive(); },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, SheriffKillButton, "SHOOT");
                    SheriffKillButton.ActionButton.buttonLabelText.SetOutlineColor(Sheriff.Color);
                    return Sheriff.CurrentTarget && PlayerControl.LocalPlayer.CanMove;
                },
                () => { SheriffKillButton.Timer = SheriffKillButton.MaxTimer; },
                Sheriff.GetKillButtonSprite(),
                CustomButton.ButtonPositions.UpperRow1,
                "ActionSecondary"
            );

            Initialized = true;
            SetSheriffCooldowns();
        }

        public static void SetSheriffCooldowns()
        {
            if (!Initialized)
            {
                SheriffButton();
            }

            SheriffKillButton.MaxTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            SheriffButton();
        }
    }
}
