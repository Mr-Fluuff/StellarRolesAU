using AmongUs.Data.Player;
using AmongUs.Data.Settings;
using HarmonyLib;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class SaveFilePatches
    {
        [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.FileName), MethodType.Getter)]
        [HarmonyPatch(typeof(SettingsData), nameof(SettingsData.FileName), MethodType.Getter)]
        public static void Postfix(ref string __result)
        {
            __result += "_SR";
        }
    }
}