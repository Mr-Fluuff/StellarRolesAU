using HarmonyLib;
using StellarRoles.Objects;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class JanitorButtons
    {
        private static bool Initialized;
        public static CustomButton CleanButton { get; set; }

        public static void JanitorCleanButton()
        {
            CleanButton = new CustomButton(
                () =>
                {
                    PlayerControl localPlayer = PlayerControl.LocalPlayer;
                    if (!localPlayer.CanMove)
                        return;

                    if (Helpers.BodyInRange(Helpers.GetKillDistance(), out DeadBody body))
                    {
                        RPCProcedure.Send(CustomRPC.CleanBody, body);
                        RPCProcedure.CleanBody(body);

                        Janitor.ChargesRemaining--;
                        SoundEffectsManager.Play(Sounds.Clean);
                        CleanButton.Timer = CleanButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                        RPCProcedure.Send(CustomRPC.PsychicAddCount);
                    }

                    Helpers.SetKillerCooldown();
                },
                () => { return Janitor.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead; },
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, CleanButton, $"Clean - {Janitor.ChargesRemaining}");

                    PlayerControl localPlayer = PlayerControl.LocalPlayer;
                    if (!localPlayer.CanMove || Impostor.IsRoleAblilityBlocked() || Janitor.ChargesRemaining <= 0)
                        return false;

                    if (Helpers.BodyInRange(Helpers.GetKillDistance())) return true;

                    return false;
                },
                () => CleanButton.Timer = CleanButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer),
                Janitor.GetButtonSprite(),
                CustomButton.ButtonPositions.UpperRow3,
                "ActionQuaternary"
            );

            Initialized = true;
            SetJanitorCooldowns();
        }

        public static void SetJanitorCooldowns()
        {
            if (!Initialized)
            {
                JanitorCleanButton();
            }

            CleanButton.MaxTimer = Janitor.CalculateJanitorCooldown();
        }

        public static void Postfix()
        {
            Initialized = false;
            JanitorCleanButton();
        }
    }
}
