using HarmonyLib;
using Hazel;
using System;
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

                if (Shade.EvidenceDuration > 0f)
                {
                    Vector3 pos = PlayerControl.LocalPlayer.transform.position;
                    byte[] buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 254, SendOption.Reliable);
                    writer.Write((byte)CustomRPC.PlaceShadeTrace);
                    writer.WriteBytesAndSize(buff);
                    writer.EndMessage();
                    RPCProcedure.PlaceShadeTrace(buff);
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
