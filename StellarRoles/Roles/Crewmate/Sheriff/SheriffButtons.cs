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
                    MurderAttemptResult murderAttemptResult = Helpers.CheckMurderAttempt(Sheriff.Player, Sheriff.CurrentTarget);
                    if (murderAttemptResult == MurderAttemptResult.SuppressKill)
                        return;
                    if (murderAttemptResult == MurderAttemptResult.PerformKill)
                    {
                        PlayerControl target = null;
                        if (Sheriff.CanBeKilledBySheriff(Sheriff.CurrentTarget))
                        {
                            target = Sheriff.CurrentTarget;
                            PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddCorrectShot);
                        }
                        else if (Sheriff.MisfireKills == Misfire.Self)
                        {
                            target = PlayerControl.LocalPlayer;
                            Helpers.PlayerKilledByAbility(target);
                            PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddMisfire);
                            RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        }
                        else if (Sheriff.MisfireKills == Misfire.Target)
                        {
                            target = Sheriff.CurrentTarget;
                            Sheriff.Haskilled = true;
                            PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddMisfire);
                            RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        }
                        else if (Sheriff.MisfireKills == Misfire.Both)
                        {
                            target = Sheriff.CurrentTarget;
                            Helpers.UncheckedMurderPlayer(Sheriff.Player, PlayerControl.LocalPlayer, true);
                            Helpers.PlayerKilledByAbility(Sheriff.Player);
                            PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddMisfire);
                            RPCProcedure.Send(CustomRPC.PsychicAddCount);

                        }
                        Helpers.UncheckedMurderPlayer(Sheriff.Player, target, true);
                    }

                    SheriffKillButton.Timer = SheriffKillButton.MaxTimer;
                    Sheriff.CurrentTarget = null;
                },
                () => { return Sheriff.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && !Sheriff.Haskilled; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, SheriffKillButton, "SHOOT");
                    SheriffKillButton.ActionButton.buttonLabelText.SetOutlineColor(Sheriff.Color);
                    return Sheriff.CurrentTarget && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.IsBombedAndActive();
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

            SheriffKillButton.MaxTimer = Helpers.KillCooldown();
        }

        public static void Postfix()
        {
            Initialized = false;
            SheriffButton();
        }
    }
}
