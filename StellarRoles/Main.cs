global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.Injection;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
using AmongUs.Data;
using AmongUs.Data.Player;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor.Networking.Attributes;
using StellarRoles.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor;
using Reactor.Networking;
using UnityEngine;

namespace StellarRoles
{
    [BepInAutoPlugin("me.fluff.stellarroles", "StellarRoles")]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    [ReactorModFlags(ModFlags.RequireOnAllClients)]
    public partial class StellarRolesPlugin : BasePlugin
    {
        public static GameObject CustomLobbyPrefab { get; set; }

        public static string UpdateString = "2025.11.20";
        public static string VersionString = UpdateString.Remove(0, 2);
        public const string BetaVersion = "";

        public const string SupportedAUVersion = "2025.11.18";
        public const string SupportedAUVersionNumber = "17.1.0";

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

        public static ConfigEntry<string> ShowPopUpVersion { get; set; }

        public static IRegionInfo[] DefaultRegions;


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
            Helpers.Log(LogLevel.Info, "Stellar Roles v" + VersionString + " loaded!");

            DefaultRegions = ServerManager.DefaultRegions;

            CustomServerManager.UpdateRegions();

            AssetLoader.LoadAssets();

            Harmony.PatchAll();

            CustomColors.Load();
            CustomOptionHolder.Load();
            SubmergedCompatibility.Initialize();
            AddComponent<ModUpdater>();
        }
    }

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
}