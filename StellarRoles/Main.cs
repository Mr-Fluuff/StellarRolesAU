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
using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data.Player;
using Reactor;
using Reactor.Networking;
using UnityEngine;

namespace StellarRoles;

    [BepInAutoPlugin("me.fluff.stellarroles", "StellarRoles")]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(ModFlags.RequireOnAllClients)]
    public partial class StellarRolesPlugin : BasePlugin
    {
        public static GameObject CustomLobbyPrefab { get; set; }

        public const string VersionString = "25.11.19";
        public const string UpdateString = "2025.11.19";
        public const string BetaVersion = "";
        public static Version VersionDeclared => System.Version.Parse(VersionString);
        public static Version UpdateVersion => System.Version.Parse(UpdateString);

        internal static BepInEx.Logging.ManualLogSource Logger;

        public Harmony Harmony { get; } = new Harmony(Id);
        public static StellarRolesPlugin Instance;


        public static int optionsPage = 3;

        public static ConfigEntry<bool> DebugMode { get; private set; }
        public static ConfigEntry<bool> GhostsSeeVotes { get; set; }
        public static ConfigEntry<bool> ShowRoleSummary { get; set; }
        public static ConfigEntry<bool> EnableSoundEffects { get; set; }
        public static ConfigEntry<bool> HidePetFromOthers { get; set; }

        public static ConfigEntry<string> Ip { get; set; }
        public static ConfigEntry<ushort> Port { get; set; }
        public static ConfigEntry<string> ShowPopUpVersion { get; set; }

        public static IRegionInfo[] DefaultRegions;

        // This is part of the Mini.RegionInstaller, Licensed under GPLv3
        // file="RegionInstallPlugin.cs" company="miniduikboot">
        public static void UpdateRegions()
        {
            ServerManager serverManager = ServerManager.Instance;
            IRegionInfo[] regions = new IRegionInfo[] {
                new StaticHttpRegionInfo("Om3ga Server", StringNames.NoTranslation,"om3gaserver.eastus.cloudapp.azure.com", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "http://om3gaserver.eastus.cloudapp.azure.com",  22000, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded NA (MNA)", StringNames.NoTranslation,"www.aumods.org", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://www.aumods.org",  443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded EU (MEU)", StringNames.NoTranslation,"au-eu.duikbo.at", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://au-eu.duikbo.at",  443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Modded Asia (MAS)", StringNames.NoTranslation,"au-as.duikbo.at", new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Http-1", "https://au-as.duikbo.at",  443, false) })).CastFast<IRegionInfo>(),
                new StaticHttpRegionInfo("Custom", StringNames.NoTranslation, Ip.Value, new Il2CppReferenceArray<ServerInfo>(new ServerInfo[1] { new ServerInfo("Custom", Ip.Value, Port.Value, false) })).CastFast<IRegionInfo>()
            };

            IRegionInfo currentRegion = serverManager.CurrentRegion;

/*            var regionJsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "InnerSloth\\Among Us\\regionInfo.json");
            if (File.Exists(regionJsonPath))
            {
                Helpers.Log(LogLevel.Debug, "Deleting Region");
                File.Delete(regionJsonPath);
            }*/

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
            HidePetFromOthers = Config.Bind("Custom", "Hide Pet From Others", true);

            Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
            Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
            DefaultRegions = ServerManager.DefaultRegions;

            UpdateRegions();

            AssetLoader.LoadAssets();

            Harmony.PatchAll();

            CustomColors.Load();
            CustomOptionHolder.Load();
            SubmergedCompatibility.Initialize();
            AddComponent<ModUpdater>();
        }
    }

    // Deactivate bans, since I always leave my local testing game and ban myself

    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
    public static class AmBannedPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
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

    [HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.Awake))]
    public static class FreeChatAwakePatch
    { 
        private static void Postfix(FreeChatInputField __instance)
        {
            __instance.textArea.characterLimit = 150;
            __instance.UpdateState();
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
                var playerId = playerControl.PlayerId = (byte)GameData.Instance.GetAvailableId();

                Bots.Add(playerControl);
                var data = GameData.Instance.AddDummy(playerControl);
                AmongUsClient.Instance.Spawn(data);
                AmongUsClient.Instance.Spawn(playerControl);
                playerControl.isDummy = true;

                playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
                playerControl.GetComponent<DummyBehaviour>().enabled = true;
                playerControl.NetTransform.enabled = false;

                playerControl.SetName(RandomString(10));
                playerControl.SetColor((byte)Random.Next(Palette.PlayerColors.Length));
                playerControl.SetHat(HatManager.Instance.allHats[Random.Next(HatManager.Instance.allHats.Count)].ProdId, playerControl.Data.DefaultOutfit.ColorId);
                playerControl.SetPet(HatManager.Instance.allPets[Random.Next(HatManager.Instance.allPets.Count)].ProdId);
                playerControl.SetSkin(HatManager.Instance.allSkins[Random.Next(HatManager.Instance.allSkins.Count)].ProdId, playerControl.Data.DefaultOutfit.ColorId);
                playerControl.SetVisor(HatManager.Instance.allVisors[Random.Next(HatManager.Instance.allVisors.Count)].ProdId, playerControl.Data.DefaultOutfit.ColorId);
                playerControl.SetNamePlate(HatManager.Instance.allNamePlates[Random.Next(HatManager.Instance.allNamePlates.Count)].ProdId);
                data.PlayerLevel = playerId;

                data.RpcSetTasks(new Il2CppStructArray<byte>(0));
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