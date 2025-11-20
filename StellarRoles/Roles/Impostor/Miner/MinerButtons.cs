using HarmonyLib;
using Hazel;
using StellarRoles.Objects;
using System;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class MinerButtons
    {
        private static bool Initialized;

        public static CustomButton MineButton { get; private set; }

        public static void MinerMineButton()
        {
            MineButton = new CustomButton(
              () =>
              {
                  Vector3 pos = PlayerControl.LocalPlayer.transform.position;
                  byte[] buff = new byte[sizeof(float) * 2];
                  Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                  Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                  MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 254, SendOption.Reliable);
                  writer.Write((byte)CustomRPC.PlaceMinerVent);
                  writer.WriteBytesAndSize(buff);
                  writer.EndMessage();
                  RPCProcedure.PlaceMinerVent(buff);
                  SoundEffectsManager.Play(Sounds.Mine);
                  Miner.ChargesRemaining--;
                  MineButton.Timer = MineButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
                  RPCProcedure.Send(CustomRPC.PsychicAddCount);

              },
              () => { return Miner.Player == PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && Miner.ChargesRemaining >= 1; },
              () =>
              {
                  Helpers.ShowTargetNameOnButtonExplicit(null, MineButton, $"Mine - {Miner.ChargesRemaining}");
                  int hits = 0;

                  foreach (var c in Physics2D.OverlapBoxAll(PlayerControl.LocalPlayer.transform.position, new Vector2(1, 1), 0))
                  {
                      if (c.gameObject.layer == 8 || c.gameObject.layer == 5) continue;
                      if (c.name.Contains("Vent")) hits++;
                  }
                  return hits == 0 && PlayerControl.LocalPlayer.CanMove && !Impostor.IsRoleAblilityBlocked();
              },
              () =>
              {
                  MineButton.Timer = MineButton.MaxTimer * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer) * Helpers.ClutchMultiplier(PlayerControl.LocalPlayer);
              },
              Miner.GetMineButtonSprite(),
              CustomButton.ButtonPositions.UpperRow3,
              "ActionQuaternary"
          );

            Initialized = true;
            SetMinerCooldowns();
        }

        public static void SetMinerCooldowns()
        {
            if (!Initialized)
            {
                MinerMineButton();
            }

            MineButton.MaxTimer = Miner.Cooldown;
        }

        public static void Postfix()
        {
            Initialized = false;
            MinerMineButton();
        }
    }
}
