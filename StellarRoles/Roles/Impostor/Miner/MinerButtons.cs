using HarmonyLib;
using StellarRoles.Objects;
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
                  Vector2 pos = PlayerControl.LocalPlayer.transform.position;

                  RPCProcedure.Send(CustomRPC.PlaceMinerVent, pos);
                  RPCProcedure.PlaceMinerVent(pos);
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
