using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class RogueButtons
    {
        private static bool Initialized;
        public static CustomButton RogueKillButton { get; set; }

        public static void RogueImpostorKillButton()
        {
            RogueKillButton = new CustomButton(
               () =>
               {
                   if (Helpers.CheckMurderAttemptAndKill(PlayerControl.LocalPlayer, NeutralKiller.CurrentTarget) == MurderAttemptResult.SuppressKill)
                       return;
                   RogueKillButton.Timer = RogueKillButton.MaxTimer;

                   if (PlayerControl.LocalPlayer == BountyHunter.Player)
                   {
                       if (NeutralKiller.CurrentTarget == BountyHunter.Bounty)
                       {
                           RogueKillButton.Timer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - BountyHunter.BonusTime;
                           BountyHunter.BountyUpdateTimer = 0f; // Force bounty update
                       }
                       else
                           RogueKillButton.Timer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + BountyHunter.PunishmentTime;
                   }
                   if (PlayerControl.LocalPlayer == Undertaker.Player)
                   {
                       CustomButton dragButton = UndertakerButtons.UndertakerDragButton;
                       if (dragButton.Timer < Undertaker.DraggingDelayAfterKill * 0.5f * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer))
                           dragButton.Timer = Undertaker.DraggingDelayAfterKill * 0.5f * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                   }
                   if (PlayerControl.LocalPlayer == Vampire.Player)
                   {
                       CustomButton biteButton = VampireButtons.VampireBiteButton;
                       if (biteButton.Timer < biteButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer))
                           biteButton.Timer = biteButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                   }
                   if (PlayerControl.LocalPlayer == Bomber.Player)
                   {
                       CustomButton bombButton = BomberButtons.BomberBombButton;
                       if (bombButton.Timer < bombButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer))
                           bombButton.Timer = bombButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                   }
                   if (PlayerControl.LocalPlayer == Warlock.Player)
                   {
                       CustomButton curseButton = WarlockButtons.CurseButton;
                       if (curseButton.Timer < curseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer))
                           curseButton.Timer = curseButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                   }
                   if (PlayerControl.LocalPlayer == Janitor.Player)
                   {
                       CustomButton cleanButton = JanitorButtons.CleanButton;
                       if (cleanButton.Timer < cleanButton.MaxTimer * 0.5f * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer))
                           cleanButton.Timer = cleanButton.MaxTimer * 0.5f * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                   }

                   NeutralKiller.CurrentTarget = null;
                   Helpers.AddGameInfo(PlayerControl.LocalPlayer.PlayerId, InfoType.AddKill);
               },
               () =>
               {
                   PlayerControl localPlayer = PlayerControl.LocalPlayer;
                   if (localPlayer.IsRuthlessRomantic(out _))
                   {
                       RogueKillButton.ActionButton.buttonLabelText.SetOutlineColor(RuthlessRomantic.Color);
                       RogueKillButton.ActionButton.graphic.sprite = Romantic.GetKillButtonSprite();
                   }
                   else if (NeutralKiller.Players.Contains(localPlayer))
                   {
                       RogueKillButton.ActionButton.buttonLabelText.SetOutlineColor(NeutralKiller.Color);
                   }
                   bool vampire = Vampire.Player == PlayerControl.LocalPlayer;
                   bool killers = NeutralKiller.Players.Contains(localPlayer);
                   bool vampirehasbutton = vampire && killers && Vampire.HasKillButton;
                   bool hasbutton = vampirehasbutton || (killers && !vampire);

                   return hasbutton && !PlayerControl.LocalPlayer.Data.IsDead;
               },
               () => { return NeutralKiller.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.IsBombedAndActive(); },
               () => { RogueKillButton.Timer = RogueKillButton.MaxTimer; },
               HudManager.Instance.KillButton.graphic.sprite,
               CustomButton.ButtonPositions.UpperRow1,
               "ActionSecondary"
           );

            Initialized = true;
            SetRogueCooldowns();
        }

        public static void SetRogueCooldowns()
        {
            if (!Initialized)
            {
                RogueImpostorKillButton();
            }
            RogueKillButton.MaxTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
        }

        public static void Postfix(HudManager __instance)
        {
            Initialized = false;
            RogueImpostorKillButton();
        }
    }
}
