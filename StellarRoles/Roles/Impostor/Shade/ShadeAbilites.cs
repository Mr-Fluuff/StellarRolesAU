using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ShadeAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || Shade.Player == null) return;
            ShadeBlindUpdate();

            if (PlayerControl.LocalPlayer != Shade.Player) return;
            ShadeUpdate();
        }

        static void ShadeUpdate()
        {
            Shade.InvisibleTimer -= Time.deltaTime;

            if (Shade.IsInvisble && Shade.InvisibleTimer <= 0)
            {
                RPCProcedure.Send(CustomRPC.SetInvisible, Shade.Player, true);
                RPCProcedure.SetInvisible(Shade.Player, true);

                if (Shade.EvidenceDuration > 0f && !Shade.Player.Data.IsDead)
                {
                    Vector2 pos = PlayerControl.LocalPlayer.transform.position;
                    RPCProcedure.Send(CustomRPC.PlaceShadeTrace, pos);
                    RPCProcedure.PlaceShadeTrace(pos);
                }
            }
        }

        static void ShadeBlindUpdate()
        {
            Shade.LightsOutTimer -= Time.deltaTime;

            if (PlayerControl.LocalPlayer == Shade.Player && Shade.BlindedPlayers.Count > 0 && Shade.LightsOutTimer <= 0f)
            {
                RPCProcedure.Send(CustomRPC.ShadeClearBlind);
                Shade.BlindedPlayers.Clear();
            }
        }
    }
}
