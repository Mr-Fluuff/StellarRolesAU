global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.Injection;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
using AmongUs.Data;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor.Networking.Attributes;
using StellarRoles.Modules;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    [BepInPlugin(Id, "StellarRoles", VersionString)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("Among Us.exe")]
    [ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
    public class StellarRolesPlugin : BasePlugin
    {
        public static GameObject CustomLobbyPrefab { get; set; }
        public const string Id = "me.fluff.stellarroles";
        public const string VersionString = "23.10.11";
        public const string UpdateString = "2023.10.11";
        public const string BetaVersion = "";
        public static Version Version => Version.Parse(VersionString);
        internal static BepInEx.Logging.ManualLogSource Logger;

        public Harmony Harmony { get; } = new Harmony(Id);
        public static StellarRolesPlugin Instance;

        public static int optionsPage = 2;

        public static ConfigEntry<bool> DebugMode { get; private set; }
        public static ConfigEntry<bool> GhostsSeeVotes { get; set; }
        public static ConfigEntry<bool> ShowRoleSummary { get; set; }
        public static ConfigEntry<bool> EnableSoundEffects { get; set; }
        public static ConfigEntry<bool> SetProcessAffinityTo1 { get; set; }
        public static ConfigEntry<bool> HidePetFromOthers { get; set; }

        public static ConfigEntry<string> Ip { get; set; }
        public static ConfigEntry<ushort> Port { get; set; }
        public static ConfigEntry<string> ShowPopUpVersion { get; set; }

        public static IRegionInfo[] DefaultRegions;

        // This is part of the Mini.RegionInstaller, Licensed under GPLv3
        // file="RegionInstallPlugin.cs" company="miniduikboot">
        public static void UpdateRegions()
        {
            ServerManager serverManager = FastDestroyableSingleton<ServerManager>.Instance;
            IRegionInfo[] regions = new IRegionInfo[] {
                new StaticHttpRegionInfo("Om3ga Server", StringNames.NoTranslation,"om3gaserver.eastus.cloudapp.azure.com",new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "http://om3gaserver.eastus.cloudapp.azure.com",  22000, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded NA (MNA)", StringNames.NoTranslation,"www.aumods.us",new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://www.aumods.us",  443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded EU (MEU)", StringNames.NoTranslation,"au-eu.duikbo.at",new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://au-eu.duikbo.at",  443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded Asia (MAS)", StringNames.NoTranslation,"au-as.duikbo.at",new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://au-as.duikbo.at",  443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("<color=#00FFFF>小猫服<color=#FFFF00>[青色北京]", StringNames.NoTranslation,"au.3q.fan",new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://au.3q.fan",  22020, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("<color=#FF1493>小猫服<color=#FFFF00>[粉色北京]", StringNames.NoTranslation,"au.3q.fan",new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://au.3q.fan",  22020, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("<color=#800080>小猫服<color=#FFFF00>[紫色北京]", StringNames.NoTranslation,"au.3q.fan",new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://au.3q.fan",  22020, false) })).CastFast<IRegionInfo>(),//注意 猫服可能出现安装bug，毕竟第一次做这！
                new StaticHttpRegionInfo("Custom", StringNames.NoTranslation, Ip.Value,new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Custom", Ip.Value,  Port.Value, false) })).CastFast<IRegionInfo>()
            };

            IRegionInfo currentRegion = serverManager.CurrentRegion;

            var regionJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "InnerSloth\\Among Us\\regionInfo.json");
            if (File.Exists(regionJsonPath))
            {
                Helpers.Log(LogLevel.Debug, "Deleting Region");
                File.Delete(regionJsonPath);
            }

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
                Helpers.Log(LogLevel.Debug, "Resetting previous region");
                serverManager.SetRegion(currentRegion);
            }
        }

        public override void Load()
        {
            Logger = Log;
            Instance = this;

            DebugMode = Config.Bind("Custom", "Enable Debug Mode", false);
            GhostsSeeVotes = Config.Bind("Custom", "Ghosts See Votes", true);
            ShowRoleSummary = Config.Bind("Custom", "Show Role Summary", true);
            EnableSoundEffects = Config.Bind("Custom", "Enable Sound Effects", true);
            ShowPopUpVersion = Config.Bind("Custom", "Show PopUp", "0");
            HidePetFromOthers = Config.Bind("Custom", "Hide Pet From Others", false);
            SetProcessAffinityTo1 = Config.Bind("Custom", "Set Process Affinity to 1", false);

            Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
            Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
            DefaultRegions = ServerManager.DefaultRegions;

            UpdateRegions();

            AssetLoader.LoadAssets();

            Harmony.PatchAll();

            CustomColors.Load();
            CustomOptionHolder.Load();
            SubmergedCompatibility.Initialize();
            AddComponent<ModUpdateBehaviour>();

        }
    }

    // Deactivate bans, since I always leave my local testing game and ban myself
    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
    public static class AmBannedPatch
    {
        public static void Postfix(out bool __result)
        {
            __result = false;
        }
    }
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
    public static class ChatControllerAwakePatch
    {
        private static void Prefix()
        {
            if (!EOSManager.Instance.isKWSMinor)
            {
                DataManager.Settings.Multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
            }
        }
    }

    // Debugging tools
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class DebugManager
    {
        private static readonly System.Random Random = new((int)DateTime.Now.Ticks);
        private static readonly List<PlayerControl> Bots = new();

        public static void Postfix()
        {
            if (!StellarRolesPlugin.DebugMode.Value)
                return;

            // Spawn dummys
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerControl playerControl = UnityEngine.Object.Instantiate(AmongUsClient.Instance.PlayerPrefab);

                Bots.Add(playerControl);
                GameData.Instance.AddPlayer(playerControl);
                AmongUsClient.Instance.Spawn(playerControl, -2, InnerNet.SpawnFlags.None);

                playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
                playerControl.GetComponent<DummyBehaviour>().enabled = true;
                playerControl.NetTransform.enabled = false;
                playerControl.SetName(RandomString(10));
                playerControl.SetColor((byte)Random.Next(Palette.PlayerColors.Length));
                GameData.Instance.RpcSetTasks(playerControl.PlayerId, Array.Empty<byte>());
            }

            // Terminate round
            if (Input.GetKeyDown(KeyCode.L) && AmongUsClient.Instance.AmHost)
            {
                RPCProcedure.Send(CustomRPC.ForceEnd);
                RPCProcedure.ForceEnd();
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}