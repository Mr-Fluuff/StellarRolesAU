using HarmonyLib;
using Hazel;
using Reactor.Utilities.Extensions;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static StellarRoles.Patches.GameStartManagerPatch;
using static UnityEngine.UI.Button;

namespace StellarRoles.Patches
{
    public class GameStartManagerPatch
    {
        public static readonly Dictionary<int, PlayerVersion> PlayerVersions = new();
        public static float Timer = 600f;
        private static float KickingTimer = 0f;
        private static bool VersionSent = false;

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public class AmongUsClientOnPlayerJoinedPatch
        {
            public static void Postfix()
            {
                if (PlayerControl.LocalPlayer != null)
                {
                    Helpers.ShareGameVersion();
                }
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public class GameStartManagerStartPatch
        {
            public static void Postfix()
            {
                // Trigger version refresh
                VersionSent = false;
                // Reset lobby countdown timer
                Timer = 600f;
                // Reset kicking timer
                KickingTimer = 0f;
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class GameStartManagerUpdatePatch
        {
            public static float StartingTimer = 0;
            private static bool update = false;
            public static bool sendGamemode = true;

            public static void Prefix(GameStartManager __instance)
            {
                if (!GameData.Instance) return; // No instance
                __instance.MinPlayers = 1;
                update = GameData.Instance.PlayerCount != __instance.LastPlayerCount;
            }

            public static void Postfix(GameStartManager __instance)
            {
                // Send version as soon as CachedPlayer.LocalPlayer.PlayerControl exists
                if (PlayerControl.LocalPlayer != null && !VersionSent)
                {
                    VersionSent = true;
                    Helpers.ShareGameVersion();
                }


                // Check version handshake infos
                bool versionMismatch = false;
                string message = "";
                foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.ToArray())
                {
                    if (client.Character == null) continue;
                    else if (!PlayerVersions.ContainsKey(client.Id))
                    {
                        versionMismatch = true;
                        message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a different or no version of StellarRoles\n</color>";
                    }
                    else
                    {
                        PlayerVersion PV = PlayerVersions[client.Id];
                        int diff = StellarRolesPlugin.Version.CompareTo(PV.version);
                        if (diff > 0)
                        {
                            message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has an older version of StellarRoles (v20{PlayerVersions[client.Id].version.ToString()})\n</color>";
                            versionMismatch = true;
                        }
                        else if (diff < 0)
                        {
                            message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a newer version of StellarRoles (v20{PlayerVersions[client.Id].version.ToString()})\n</color>";
                            versionMismatch = true;
                        }
                        else if (!PV.GuidMatches())
                        { // version presumably matches, check if Guid matches
                            message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a modified version of SR v20{PlayerVersions[client.Id].version.ToString()} <size=30%>({PV.guid.ToString()})</size>\n</color>";
                            versionMismatch = true;
                        }
                    }
                }
                // Display message to the host
                if (AmongUsClient.Instance.AmHost)
                {
                    if (versionMismatch)
                    {
                        __instance.GameStartText.text = message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                        __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                        __instance.GameStartTextParent.SetActive(true);
                    }
                    else
                    {
                        __instance.GameStartText.transform.localPosition = Vector3.zero;
                        __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                        if (!__instance.GameStartText.text.StartsWith("Starting"))
                        {
                            __instance.GameStartText.text = String.Empty;
                            __instance.GameStartTextParent.SetActive(false);
                        }
                    }

                    // Make starting info available to clients:
                    if (StartingTimer <= 0 && __instance.startState == GameStartManager.StartingStates.Countdown)
                    {
                        RPCProcedure.Send(CustomRPC.SetGameStarting);
                        RPCProcedure.SetGameStarting();
                    }
                }

                // Client update with handshake infos
                else
                {
                    if (!PlayerVersions.ContainsKey(AmongUsClient.Instance.HostId) || StellarRolesPlugin.Version.CompareTo(PlayerVersions[AmongUsClient.Instance.HostId].version) != 0)
                    {
                        KickingTimer += Time.deltaTime;
                        if (KickingTimer > 10)
                        {
                            KickingTimer = 0;
                            AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                            SceneChanger.ChangeScene("MainMenu");
                        }

                        __instance.GameStartText.text = $"<color=#FF0000FF>The host has no or a different version of StellarRoles\nYou will be kicked in {Math.Round(10 - KickingTimer)}s</color>";
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                        __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                        __instance.GameStartTextParent.SetActive(true);
                    }
                    else if (versionMismatch)
                    {
                        __instance.GameStartText.text = $"<color=#FF0000FF>Players With Different Versions:\n</color>" + message;
                        __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                        __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                        __instance.GameStartTextParent.SetActive(true);
                    }
                    else
                    {
                        __instance.GameStartText.transform.localPosition = Vector3.zero;
                        __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                        if (!__instance.GameStartText.text.StartsWith("Starting"))
                        {
                            __instance.GameStartText.text = String.Empty;
                            __instance.GameStartTextParent.SetActive(false);
                        }
                    }
                }
                // Start Timer
                if (StartingTimer > 0)
                {
                    StartingTimer -= Time.deltaTime;
                }

                // Lobby timer
                if (!GameData.Instance || !__instance.PlayerCounter) return; // No instance
                var player = GameData.Instance.PlayerCount;

                string currentText = Helpers.ColorString(player >= 4 ? Palette.AcceptedGreen : Palette.ImpostorRed, $"{player}/{CustomOptionHolder.LobbySize.GetInt()}");

                Timer = Mathf.Max(0f, Timer -= Time.deltaTime);
                int minutes = (int)Timer / 60;
                int seconds = (int)Timer % 60;
                string suffix = $"\n({minutes:00}:{seconds:00})";
                int spectators = Spectator.ToBecomeSpectator.Count;
                string spectatorCount = spectators > 0 ? $"\nSpectators: {spectators}" : "";

                __instance.PlayerCounter.text = currentText + suffix + spectatorCount;
                //__instance.PlayerCounter.alignment = TMPro.TextAlignmentOptions.Center;
                //__instance.PlayerCounter.autoSizeTextContainer = true;
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        public class GameStartManagerBeginGame
        {
            public static bool Prefix()
            {
                // Block game start if not everyone has the same mod version
                if (AmongUsClient.Instance.AmHost)
                {
                    foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.GetFastEnumerator())
                    {
                        if (client.Character == null) continue;
                        DummyBehaviour dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                        if (dummyComponent != null && dummyComponent.enabled)
                            continue;

                        if (!PlayerVersions.ContainsKey(client.Id))
                            return false;

                        PlayerVersion PV = PlayerVersions[client.Id];
                        int diff = StellarRolesPlugin.Version.CompareTo(PV.version);
                        if (diff != 0 || !PV.GuidMatches())
                            return false;
                    }



                    if (CustomOptionHolder.DynamicMap.GetBool())
                    {
                        byte chosenMapId = 0;
                        List<Map> maps = new()
                        {
                            Map.Skeld,
                            Map.Mira,
                            Map.Polus,
                            Map.Airship,
                            Map.Fungal
                        };

                        if (SubmergedCompatibility.Loaded)
                            maps.Add(Map.Submerged);

                        List<Map> ensuredMapIds = new();
                        List<Map> chanceMapIds = new();

                        foreach (Map m in maps)
                            if (getSelectionForMapId(m) >= 10)
                                ensuredMapIds.AddRange(Enumerable.Repeat(m, getSelectionForMapId(m, true) / 10));
                            else
                                chanceMapIds.AddRange(Enumerable.Repeat(m, getSelectionForMapId(m, true)));

                        if (ensuredMapIds.Count <= 0 && chanceMapIds.Count <= 0)
                            return true;

                        if (ensuredMapIds.Count > 0)
                        {
                            ensuredMapIds = ensuredMapIds.OrderBy(a => StellarRoles.rnd.Next()).ToList();
                            chosenMapId = (byte)ensuredMapIds.ElementAt(StellarRoles.rnd.Next(ensuredMapIds.Count));
                        }
                        else
                        {
                            chanceMapIds = chanceMapIds.OrderBy(a => StellarRoles.rnd.Next()).ToList();
                            chosenMapId = (byte)chanceMapIds.ElementAt(StellarRoles.rnd.Next(chanceMapIds.Count));
                        }
                        RPCProcedure.Send(CustomRPC.DynamicMapOption, chosenMapId);
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId = chosenMapId;
                    }
                }

                return true;
            }

            public static int getSelectionForMapId(Map map, bool multiplyQuantity = false)
            {
                int selection = 0;
                switch (map)
                {
                    case Map.Skeld:
                        selection = CustomOptionHolder.DynamicMapEnableSkeld.GetSelection();
                        if (multiplyQuantity) selection *= CustomOptionHolder.DynamicMapEnableSkeld.GetQuantity();
                        break;

                    case Map.Mira:
                        selection = CustomOptionHolder.DynamicMapEnableMira.GetSelection();
                        if (multiplyQuantity) selection *= CustomOptionHolder.DynamicMapEnableMira.GetQuantity();
                        break;

                    case Map.Polus:
                        selection = CustomOptionHolder.DynamicMapEnablePolus.GetSelection();
                        if (multiplyQuantity) selection *= CustomOptionHolder.DynamicMapEnablePolus.GetQuantity();
                        break;

                    case Map.Airship:
                        selection = CustomOptionHolder.DynamicMapEnableAirShip.GetSelection();
                        if (multiplyQuantity) selection *= CustomOptionHolder.DynamicMapEnableAirShip.GetQuantity();
                        break;

                    case Map.Fungal:
                        selection = CustomOptionHolder.DynamicMapEnableFungal.GetSelection();
                        if (multiplyQuantity) selection *= CustomOptionHolder.DynamicMapEnableFungal.GetQuantity();
                        break;

                    case Map.Submerged:
                        selection = CustomOptionHolder.DynamicMapEnableSubmerged.GetSelection();
                        if (multiplyQuantity) selection *= CustomOptionHolder.DynamicMapEnableSubmerged.GetQuantity();
                        break;
                }

                return selection;
            }
        }

        public class PlayerVersion
        {
            public readonly Version version;
            public readonly Guid guid;

            public PlayerVersion(Version version, Guid guid)
            {
                this.version = version;
                this.guid = guid;
            }

            public bool GuidMatches()
            {
                return Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.Equals(guid);
            }
        }

        [HarmonyPatch(typeof(LobbyInfoPane), nameof(LobbyInfoPane.Update))]
        public class LobbyInfoPanePatch
        {
            public static void Postfix(LobbyInfoPane __instance)
            {
                var Preset0 = CustomOption.Options[0];
                var Preset = Preset0.Selections[Preset0.Selection].ToString();

                var GameModeText = GameObject.Find("GameModeText");
                if (GameModeText == null)  return;

                var text = GameModeText.GetComponent<TextMeshPro>().text;
                if (text != Preset)
                {
                    GameModeText.GetComponent<TextMeshPro>().text = Preset;
                }
            }
        }
    }
}
