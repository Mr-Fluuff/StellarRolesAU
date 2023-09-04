using PowerTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Objects
{
    public class VentTrap
    {
        public static readonly Dictionary<int, VentTrap> VentTrapMap = new();
        public static readonly Dictionary<byte, float> PlayerTimerMap = new();
        public static Dictionary<int, VentTrap> ascendedVentTimeKeeper = new();
        public readonly List<PlayerControl> PlayersVisibleTo = new();
        public int VentId;
        public Vent VentTrapped;
        public bool Used;
        public float Duration = 0f;
        private Arrow arrow;
        private SpriteAnim Animator;

        public VentTrap(Vent vent)
        {
            VentId = vent.Id;
            Used = false;
            VentTrapped = vent;
            PlayersVisibleTo = new()
            {
                Trapper.Player
            };
            VentTrapMap.Add(vent.Id, this);
        }

        private bool IsPlayerNearTrap(PlayerControl player)
        {
            return Vector2.Distance(VentTrapped.transform.position, player.GetTruePosition()) <= 1f;
        }

        public void DecorateVent()
        {
            VentTrapped.myRend.color = new Color32(10, 200, 10, byte.MaxValue);
            Animator = VentTrapped.GetComponent<SpriteAnim>();
        }

        public void RestoreVent()
        {
            VentTrapped.myRend.color = Color.white;
            Animator?.Resume();
        }

        public static void UpdateVentTrapPerPlayer()
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            if (player.Data.IsDead || !Helpers.RoleCanUseVents(player) || TrapperAbilities.IsRoleBlocked()) return; //Vent Trap only displays for roles that can vent
            foreach (VentTrap ventTrap in VentTrapMap.Values)
            {
                if (
                    //If vent trap is used, nothing unique to display
                    ventTrap.Used ||
                    ventTrap.PlayersVisibleTo.Any(p => p.PlayerId == player.PlayerId) ||
                    TrapperAbilities.IsRoleBlocked() ||
                    !ventTrap.IsPlayerNearTrap(player)
                ) continue;

                if (!PlayerTimerMap.ContainsKey(player.PlayerId))
                    PlayerTimerMap.Add(player.PlayerId, 3f);

                float timer = PlayerTimerMap[player.PlayerId] -= Time.deltaTime;
                if (timer <= 0)
                {
                    ventTrap.PlayersVisibleTo.Add(player);
                    ventTrap.DecorateVent();
                    PlayerTimerMap.Remove(player.PlayerId);
                }
            }

        }

        private void ArrowForVentTrap()
        {
            Arrow Arrow = new(Trapper.Color);
            Arrow.Update(VentTrapped.transform.position);
            Arrow.Object.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer == Trapper.Player);
            this.arrow = Arrow;
            Duration = 5f;
            ascendedVentTimeKeeper.Add(VentId, this);
        }

        public static void UpdateForAscendedTrapper()
        {
            //Decrease time on Vent Trap Arrows
            float dt = Time.deltaTime;
            List<VentTrap> toBeRemoved = new();
            foreach (VentTrap t in ascendedVentTimeKeeper.Values)
            {
                t.Duration -= dt;
                if (t.Duration <= 0)
                {
                    toBeRemoved.Add(t);
                }
            }
            //Remove Vent Trap arrows that have run out of time
            foreach (VentTrap t in toBeRemoved)
            {
                if (t.arrow != null)
                {
                    t.arrow.Object.SetActive(false);
                    Object.Destroy(t.arrow.Object);
                }
                ascendedVentTimeKeeper.Remove(t.VentId);
            }

        }

        public void useVentTrap()
        {
            Used = true;
            if (PlayersVisibleTo.Any(p => p.PlayerId == PlayerControl.LocalPlayer.PlayerId)) RestoreVent();
            if (Ascended.IsAscended(Trapper.Player))
            {
                ArrowForVentTrap();
            }
            VentTrapMap.Remove(VentId);
        }


        public static void ClearAllVentTraps()
        {
            VentTrapMap.Clear();
            PlayerTimerMap.Clear();
            ascendedVentTimeKeeper.Clear();
        }

    }
}
