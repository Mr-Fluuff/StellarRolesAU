using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using HarmonyLib;
using InnerNet;
using UnityEngine.ProBuilder;


namespace StellarRoles
{
    // This is part of the Mini.RegionInstaller, Licensed under GPLv3
    // file="RegionInstallPlugin.cs" company="miniduikboot">
    [HarmonyPatch]
    public class CustomServerManager
    {
        private static ReadOnlyDictionary<string, IRegionInfo> parsedRegions;

        public static void UpdateRegions()
        {
            ServerManager serverManager = ServerManager.Instance;
            IRegionInfo[] regions = 
            [
                new StaticHttpRegionInfo("Modded NA (MNA)", StringNames.NoTranslation,"www.aumods.org", new Il2CppReferenceArray<ServerInfo>([new ServerInfo("Http-1", "https://www.aumods.org",  443, false)])).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded EU (MEU)", StringNames.NoTranslation,"au-eu.duikbo.at", new Il2CppReferenceArray<ServerInfo>([new ServerInfo("Http-1", "https://au-eu.duikbo.at",  443, false)])).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded Asia (MAS)", StringNames.NoTranslation,"au-as.duikbo.at", new Il2CppReferenceArray<ServerInfo>([new ServerInfo("Http-1", "https://au-as.duikbo.at",  443, false)])).CastFast<IRegionInfo>(),
            ];

            Dictionary<string, IRegionInfo> dictionary = new Dictionary<string, IRegionInfo>();
            foreach (IRegionInfo regionInfo in regions)
            {
                dictionary[regionInfo.Name] = regionInfo;
            }
            parsedRegions = new ReadOnlyDictionary<string, IRegionInfo>(dictionary);

            for (var i = 0; i < serverManager.AvailableRegions.Length; i++)
            {
                IRegionInfo region = serverManager.AvailableRegions[i];
                if (region.Name.Contains("Custom") || region.Name.Contains("Om3ga"))
                {
                    Helpers.Log("Removed " +  region.Name + " Region");
                    serverManager.AvailableRegions.RemoveAt(i);
                }
            }
            IRegionInfo currentRegion = serverManager.CurrentRegion;

            Helpers.Log(LogLevel.Debug, $"Adding {regions.Length} regions");
            foreach (IRegionInfo region in regions)
            {
                if (currentRegion != null && region.Name.Equals(currentRegion.Name, StringComparison.OrdinalIgnoreCase))
                    currentRegion = region;
                serverManager.AddOrUpdateRegion(region);
            }

            // AU remembers the previous region that was set, so we need to restore it
            if (currentRegion != null)
            {
                if (serverManager.AvailableRegions.Any(x => x.Name == currentRegion.Name))
                {
                    Helpers.Log(LogLevel.Debug, "Resetting previous region");
                    serverManager.SetRegion(currentRegion);
                }
                else
                {
                    Helpers.Log("Setting Region to " + serverManager.AvailableRegions[0].Name);
                    serverManager.SetRegion(serverManager.AvailableRegions[0]);
                }
            }
        }

        public static void CorrectCurrentRegion(ServerManager instance)
        {
            IRegionInfo currentRegion = instance.CurrentRegion;
            if (parsedRegions != null && parsedRegions.ContainsKey(currentRegion.Name))
            {
                instance.CurrentRegion = parsedRegions[currentRegion.Name];
            }
        }

        [HarmonyPatch(typeof(ServerManager), "LoadServers")]
        public static class ServerManagerLoadServersPatch
        {
            public static void Postfix(ServerManager __instance)
            {
                CorrectCurrentRegion(__instance);
                __instance.CurrentUdpServer = __instance.CurrentRegion.Servers[0];
            }
        }

        [HarmonyPatch(typeof(ServerManager), "ReselectServer")]
        public static class SMReselectPatch
        {
            public static void Prefix(ServerManager __instance)
            {
                CorrectCurrentRegion(__instance);
            }
        }
    }
}
