using HarmonyLib;
using Il2CppSystem;
using System.Collections.Generic;

namespace StellarRoles.Utilities;

public static class MapUtilities
{
    public static ShipStatus CachedShipStatus = ShipStatus.Instance;

    public static void MapDestroyed()
    {
        CachedShipStatus = ShipStatus.Instance;
        _systems.Clear();
    }

    private static readonly Dictionary<SystemTypes, Object> _systems = new();
    public static Dictionary<SystemTypes, Object> Systems
    {
        get
        {
            if (_systems.Count == 0) GetSystems();
            return _systems;
        }
    }

    private static void GetSystems()
    {
        if (!CachedShipStatus) return;

        Il2CppSystem.Collections.Generic.Dictionary<SystemTypes, ISystemType> systems = CachedShipStatus.Systems;
        if (systems.Count <= 0) return;

        foreach (SystemTypes systemTypes in SystemTypeHelpers.AllTypes)
        {
            if (!systems.ContainsKey(systemTypes)) continue;
            _systems[systemTypes] = systems[systemTypes].TryCast<Object>();
        }
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix(ShipStatus __instance)
    {
        AddVitals.AddVital();
        MapUtilities.CachedShipStatus = __instance;
        SubmergedCompatibility.SetupMap(__instance);
    }
}
[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
public static class ShipStatus_OnDestroy_Patch
{
    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
    public static void Postfix()
    {
        MapUtilities.CachedShipStatus = null;
        MapUtilities.MapDestroyed();
        SubmergedCompatibility.SetupMap(null);
    }
}