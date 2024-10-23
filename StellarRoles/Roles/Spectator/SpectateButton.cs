using HarmonyLib;
using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class SpectateButtonUpdate
    {
        public static void Postfix()
        {
            if (!LobbyBehaviour.Instance) return;
            try
            {
                HudManagerStartPatch.RefreshCosmeticsButton.Update();
                if (!AmongUsClient.Instance.AmHost) return;
                if (Helpers.IsHideAndSeek) return;
                Spectator.Target = GetClosestPlayer(PlayerControl.LocalPlayer);
                HudManagerStartPatch.SpectatorButton.Update();

                BaseOutlines();
                SpectateOutline();
            }
            catch { }
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer)
        {
            double num = double.MaxValue;
            Vector2 refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
                Vector2 playerPosition = player.GetTruePosition();
                float distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                if (distBetweenPlayers > 1f) continue;
                bool isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                Vector2 vector = playerPosition - refPosition;

                if (PhysicsHelpers.AnyNonTriggersBetween(refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask)) continue;

                num = distBetweenPlayers;
                result = player;
            }

            if (result == null) result = refPlayer;

            return result;
        }
        public static void BaseOutlines()
        {
            foreach (PlayerControl target in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (target.cosmetics?.currentBodySprite?.BodySprite != null)
                    target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
            }
        }
        public static void SpectateOutline()
        {
            Helpers.SetPlayerOutline(Spectator.Target, Color.gray);
        }
    }

    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static class SpectatorDisconnectHandler
    {
        public static void Postfix([HarmonyArgument(0)] PlayerControl player)
        {
            // The game code checks for this so we should check as well to be certain.
            if (player != null)
            {
                RPCProcedure.Send(CustomRPC.RemoveSpectator, player);
                RPCProcedure.RemoveSpectator(player);
            }
        }
    }
}
