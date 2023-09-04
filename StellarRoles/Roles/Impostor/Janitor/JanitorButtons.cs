using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

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
                    Vector2 playerPosition = localPlayer.GetTruePosition();
                    float killDistance = Helpers.GetKillDistance();

                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(playerPosition, killDistance, Constants.PlayersOnlyMask))
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component?.Reported == false)
                            {
                                Vector2 bodyPosition = component.TruePosition;
                                if (Vector2.Distance(bodyPosition, playerPosition) <= killDistance && !PhysicsHelpers.AnythingBetween(playerPosition, bodyPosition, Constants.ShipAndObjectsMask, false))
                                {
                                    RPCProcedure.Send(CustomRPC.CleanBody, component.ParentId);
                                    RPCProcedure.CleanBody(component);

                                    Janitor.ChargesRemaining--;
                                    SoundEffectsManager.Play(Sounds.Clean);
                                    CleanButton.Timer = CleanButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                                    RPCProcedure.Send(CustomRPC.PsychicAddCount);

                                    break;
                                }
                            }
                        }

                    Helpers.SetKillerCooldown();
                },
                () => Janitor.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead,
                () =>
                {
                    Helpers.ShowTargetNameOnButtonExplicit(null, CleanButton, $"Clean - {Janitor.ChargesRemaining}");

                    PlayerControl localPlayer = PlayerControl.LocalPlayer;
                    if (!localPlayer.CanMove || Impostor.IsRoleAblilityBlocked() || Janitor.ChargesRemaining <= 0)
                        return false;

                    Vector2 playerPosition = localPlayer.GetTruePosition();
                    float killDistance = Helpers.GetKillDistance();
                    foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(playerPosition, killDistance, Constants.PlayersOnlyMask))
                        if (collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component?.Reported == false)
                            {
                                Vector2 bodyPosition = component.TruePosition;
                                if (Vector2.Distance(bodyPosition, playerPosition) <= killDistance && !PhysicsHelpers.AnythingBetween(playerPosition, bodyPosition, Constants.ShipAndObjectsMask, false))
                                    return true;
                            }
                        }

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
