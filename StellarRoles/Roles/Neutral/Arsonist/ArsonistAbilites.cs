using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ArsonistAbilites
    {
        public static void Postfix()
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Arsonist.Player) return;
            ArsonistSetTarget();
        }

        static PlayerList ArsoTargets { get; set; }
        public static IEnumerable<PlayerControl> GetArsonistList()
        {
            if (Arsonist.DouseTarget != null)
            {
                ArsoTargets = new();
                foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (player.PlayerId != Arsonist.DouseTarget.PlayerId)
                    {
                        ArsoTargets.Add(player);
                    }
                }
            }
            else ArsoTargets = Arsonist.DousedPlayers;

            return ArsoTargets.GetPlayerEnumerator();
        }

        public static void ArsonistSetTarget()
        {
            Arsonist.CurrentTarget = Helpers.SetTarget(untargetablePlayers: GetArsonistList());
        }
    }
}
