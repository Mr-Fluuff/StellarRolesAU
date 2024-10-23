using HarmonyLib;
using Il2CppSystem;
using System.Collections.Generic;

namespace StellarRoles.Utilities;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix(ShipStatus __instance)
    {
        AddAssets.AddObjects();
        SubmergedCompatibility.SetupMap(__instance);
    }
}
[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
public static class ShipStatus_OnDestroy_Patch
{
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix()
    {
        SubmergedCompatibility.SetupMap(null);
    }
}