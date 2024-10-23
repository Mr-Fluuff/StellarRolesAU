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
                   Helpers.ResetAbilityCooldown(false);

                   NeutralKiller.CurrentTarget = null;
                   PlayerControl.LocalPlayer.RPCAddGameInfo(InfoType.AddKill);
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

                   if (localPlayer == Vampire.Player && !Vampire.HasKillButton) return false;
                   if (localPlayer == Parasite.Player && !Parasite.NormalKillButton) return false;
                   return NeutralKiller.Players.Contains(localPlayer) && !PlayerControl.LocalPlayer.Data.IsDead;
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
            RogueKillButton.MaxTimer = Helpers.KillCooldown();
        }

        public static void Postfix(HudManager __instance)
        {
            Initialized = false;
            RogueImpostorKillButton();
        }
    }
}
