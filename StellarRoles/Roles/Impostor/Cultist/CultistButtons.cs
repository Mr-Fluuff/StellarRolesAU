using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class CultistButtons
    {
        private static bool Initialized;
        public static CustomButton TurnButton { get; set; }

        public static void CultistTurnButton()
        {
            TurnButton = new CustomButton(
                () =>
                {
                    if (Cultist.CurrentFollower != null)
                    {
                        RPCProcedure.Send(CustomRPC.CultistCreateImposter, Cultist.CurrentFollower);
                        RPCProcedure.CultistCreateImposter(Cultist.CurrentFollower);
                    }
                },
                () => { return Cultist.NeedsFollower && Cultist.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(Cultist.CurrentFollower, TurnButton, "Convert");
                    return Cultist.NeedsFollower && Cultist.CurrentFollower != null && PlayerControl.LocalPlayer.CanMove;
                },
                () => { TurnButton.Timer = TurnButton.MaxTimer; },
                Cultist.GetRecruitButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary"
            );
            Initialized = true;
            SetCultistCooldowns();
        }

        public static void SetCultistCooldowns()
        {
            if (!Initialized)
            {
                CultistTurnButton();
            }

            TurnButton.MaxTimer = 0f;
            TurnButton.Timer = 0;
        }

        public static void Postfix()
        {
            Initialized = false;
            CultistTurnButton();
        }
    }
}
