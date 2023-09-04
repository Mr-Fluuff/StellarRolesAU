using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch]
    public static class Impostor
    {
        public static Color color = Palette.ImpostorRed;
        public static PlayerControl CurrentTarget = null;

        public static void ClearAndReload()
        {
            CurrentTarget = null;
        }

        public static bool IsRoleAblilityBlocked()
        {
            return Helpers.IsCommsActive() && MapOptions.ImposterAbiltiesRoleBlock;
        }
    }
}
