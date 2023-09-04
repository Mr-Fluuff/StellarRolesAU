using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TrapperAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Trapper.Player) return;

            trapperSetTarget();
            VentTrap.UpdateForAscendedTrapper();
        }

        public static bool IsRoleBlocked()
        {
            return Trapper.RoleBlock && Helpers.IsCommsActive();
        }
        public static void trapperSetTarget()
        {
            Vent target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            for (int i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
            {
                Vent vent = MapUtilities.CachedShipStatus.AllVents[i];
                if (vent.gameObject.name.StartsWith("SealedVent_") || VentTrap.VentTrapMap.ContainsKey(vent.Id) || vent.gameObject.name.StartsWith("FutureSealedVent_") || (vent.gameObject.name.StartsWith("MinerVent_") && !vent.gameObject.active)) continue;
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 9) continue; // cannot seal submergeds exit only vent!
                float distance = Vector2.Distance(vent.transform.position, truePosition);
                bool anythingBetween = PhysicsHelpers.AnythingBetween(vent.transform.position, truePosition, Constants.ShipAndAllObjectsMask, false);
                if (!anythingBetween && distance <= vent.UsableDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    target = vent;
                }
            }
            Trapper.VentTarget = target;
        }
    }
}
