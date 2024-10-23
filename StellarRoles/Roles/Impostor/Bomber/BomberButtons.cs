using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class BomberButtons
    {
        public static bool initialized;
        public static CustomButton BomberBombButton;
        public static CustomButton bombedKillButton;

        public static void InitBomberButtons()
        {
            BombButton();
            HasBombButton();

            initialized = true;
            SetBomberCooldowns();
        }

        public static void BombButton()
        {
            BomberBombButton = new CustomButton(
                () =>
                { /* On Use */

                    if (Helpers.CheckMurderAttempt(PlayerControl.LocalPlayer, Bomber.AbilityCurrentTarget) == MurderAttemptResult.SuppressKill)
                        return;

                    RPCProcedure.Send(CustomRPC.GiveBomb, Bomber.AbilityCurrentTarget, PlayerControl.LocalPlayer, false);
                    RPCProcedure.GiveBomb(Bomber.AbilityCurrentTarget, PlayerControl.LocalPlayer, false);

                    Helpers.SetKillerCooldown();

                    BomberBombButton.Timer = BomberBombButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                    Bomber.AbilityCurrentTarget = null;
                    RPCProcedure.Send(CustomRPC.PsychicAddCount);
                },
                () => { return Bomber.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return Bomber.AbilityCurrentTarget != null && PlayerControl.LocalPlayer.CanMove && !(MapOptions.ImposterKillAbilitiesRoleBlock && Helpers.IsCommsActive()); },
                () =>
                {  /* On Meeting End */
                    BomberBombButton.Timer = BomberBombButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    BomberBombButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                Bomber.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary",
                false,
                "Bomb"
            );
        }

        public static void HasBombButton()
        {
            bombedKillButton = new CustomButton(
                () =>
                { /* On Use */
                    PlayerControl.LocalPlayer.IsBombed(out Bombed bombed);
                    byte bombedId = bombed.Player.PlayerId;
                    byte targetId = bombed.CurrentTarget.PlayerId;
                    if (Bomber.HotPotatoMode)
                    {
                        if (Helpers.CheckMurderAttempt(bombed.Bomber, bombed.CurrentTarget) == MurderAttemptResult.SuppressKill)
                            return;

                        RPCProcedure.Send(CustomRPC.PassBomb, bombedId, targetId, bombed.TimeLeft);
                        RPCProcedure.PassBomb(bombedId, targetId, bombed.TimeLeft);
                    }
                    else if (bombed.CurrentTarget == bombed.Bomber)
                    {
                        Helpers.UncheckedMurderPlayer(bombed.Bomber, bombed.Player, false, true);
                        Helpers.PlayerKilledByAbility(bombed.Player);
                        bombed.Bomber.RPCAddGameInfo(InfoType.AddAbilityKill, InfoType.AddKill);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    }
                    else
                    {
                        if (Helpers.CheckBombedAttemptAndKill(bombed.Bomber, bombed.CurrentTarget, showAnimation: false) == MurderAttemptResult.SuppressKill)
                            return;
                        bombed.PassedBomb = true;
                        RPCProcedure.Send(CustomRPC.SnapToRpc, PlayerControl.LocalPlayer, bombed.CurrentTarget);
                        PlayerControl.LocalPlayer.NetTransform.SnapTo(bombed.CurrentTarget.transform.position);
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
                        Helpers.PlayerKilledByAbility(bombed.CurrentTarget);

                        if (!Bombed.KillerDictionary.TryGetValue(PlayerControl.LocalPlayer.PlayerId, out PlayerList players))
                            Bombed.KillerDictionary.Add(PlayerControl.LocalPlayer.PlayerId, players = new());

                        players.Add(bombed.CurrentTarget);

                        Helpers.SetKillerCooldown();
                        bombed.Bomber.RPCAddGameInfo(InfoType.AddAbilityKill, InfoType.AddKill);

                        if (PlayerControl.LocalPlayer == Scavenger.Player && ScavengerButtons.ScavengerEatButton.Timer < 5)
                            ScavengerButtons.ScavengerEatButton.Timer = 5f;
                    }

                    RPCProcedure.Send(CustomRPC.GiveBomb, bombed.Player, bombed.Bomber, true);
                    RPCProcedure.GiveBomb(bombed.Player, bombed.Bomber, true);
                },
                () => { return PlayerControl.LocalPlayer.IsBombed(out Bombed bombed) && bombed.BombActive && !bombed.Player.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.IsBombed(out Bombed bombed) && bombed.CurrentTarget != null && bombed.Player.CanMove; },
                () => {  /* On Meeting End */ },
                Bomber.GetBombSprite(),
                new Vector3(-3.5f, 1.5f, 0),
                "ActionSecondary"
            );
        }

        public static void SetBomberCooldowns()
        {
            if (!initialized)
            {
                InitBomberButtons();
            }

            BomberBombButton.MaxTimer = Bomber.Cooldown;
            bombedKillButton.MaxTimer = 0f;
            bombedKillButton.Timer = 0f;
        }

        public static void Postfix()
        {
            initialized = false;
            InitBomberButtons();
        }
    }
}
