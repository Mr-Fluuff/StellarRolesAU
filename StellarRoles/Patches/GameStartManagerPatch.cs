using HarmonyLib;
using Reactor.Utilities.Extensions;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class GameStartManagerPatch
    {
        public static readonly Dictionary<int, PlayerVersion> PlayerVersions = new();
        public static float Timer = 600f;
        private static float KickingTimer = 0f;
        public static bool VersionSent = false;

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        [HarmonyPostfix]
        public static void GameStartManagerStartPostfix()
        {
            // Trigger version refresh
            VersionSent = false;
            // Reset lobby countdown timer
            Timer = 600f;
            // Reset kicking timer
            KickingTimer = 0f;

        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.ResetStartState))]
        [HarmonyPrefix]
        public static void ResetStartStatePrefix(GameStartManager __instance)
        {
            if (__instance.startState is GameStartManager.StartingStates.Countdown)
            {
                SoundManager.Instance.StopSound(__instance.gameStartSound);
            }

            if (AmongUsClient.Instance.AmHost)
            {
                CustomOption.ShareOptionSelections();
                GameManager.Instance.LogicOptions.SyncOptions();
            }
            Timer = 600f;
            VersionSent = false;
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class GameStartManagerUpdatePatch
        {
            public static float StartingTimer = 0;
            public static bool sendGamemode = true;
            private static GameObject StopCountdownButton = null;

            public static void Prefix(GameStartManager __instance)
            {
                if (GameData.Instance)
                {
                    __instance.MinPlayers = 1;
                }
            }

            public static void Postfix(GameStartManager __instance)
            {
                if (CustomOptionHolder.DynamicMap.GetBool())
                {
                    __instance.MapImage.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RandomMapIcon.png", 400f);
                    __instance.MapImage.flipX = false;
                }

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
                        int diff = StellarRolesPlugin.VersionDeclared.CompareTo(PV.version);
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

                        if (__instance.startState != GameStartManager.StartingStates.Countdown)
                            StopCountdownButton?.Destroy();
                        // Make starting info available to clients:
                        if (StartingTimer <= 0 && __instance.startState == GameStartManager.StartingStates.Countdown)
                        {
                            RPCProcedure.Send(CustomRPC.SetGameStarting);
                            RPCProcedure.SetGameStarting();

                            // Activate Stop-Button
                            StopCountdownButton = GameObject.Instantiate(__instance.StartButton.gameObject, __instance.StartButton.gameObject.transform.parent);
                            StopCountdownButton.transform.localPosition = __instance.StartButton.transform.localPosition;
                            StopCountdownButton.SetActive(true);
                            var startButtonText = StopCountdownButton.GetComponentInChildren<TMPro.TextMeshPro>();
                            startButtonText.text = "Click Again to Cancel";
                            startButtonText.fontSize *= 0.8f;
                            startButtonText.fontSizeMax = startButtonText.fontSize;
                            startButtonText.fontStyle = FontStyles.UpperCase;
                            startButtonText.gameObject.transform.localPosition = new Vector3(0, -0.5f, 0);
                            PassiveButton startButtonPassiveButton = StopCountdownButton.GetComponent<PassiveButton>();
                            void StopStartFunc()
                            {
                                __instance.ResetStartState();
                                StopCountdownButton.Destroy();
                                StartingTimer = 0;
                            }
                            void Red()
                            {
                                __instance.StartCoroutine(Effects.Lerp(.05f, new System.Action<float>((p) =>
                                {
                                    startButtonText.color = Color.red;
                                })));
                            }
                            startButtonPassiveButton.OnClick.AddListener((Action)(() => StopStartFunc()));
                            startButtonPassiveButton.OnMouseOver.AddListener((Action)(() => Red()));
                            startButtonPassiveButton.OnMouseOut.AddListener((Action)(() => Red()));
                            __instance.StartCoroutine(Effects.Lerp(.1f, new System.Action<float>((p) =>
                            {
                                startButtonText.text = "^Click Again to Cancel^";
                                startButtonText.fontStyle = FontStyles.UpperCase;
                                Red();
                            })));
                        }
                    }
                }

                // Client update with handshake infos
                else
                {
                    if (!PlayerVersions.ContainsKey(AmongUsClient.Instance.HostId) || StellarRolesPlugin.VersionDeclared.CompareTo(PlayerVersions[AmongUsClient.Instance.HostId].version) != 0)
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
                if (GameData.Instance == null || __instance.PlayerCounter == null) return; // No instance
                var player = GameData.Instance.PlayerCount;

                string currentText = Helpers.ColorString(player >= 4 ? Palette.AcceptedGreen : Palette.ImpostorRed, $"{player}/{GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers}");

                Timer = Mathf.Max(0f, Timer -= Time.deltaTime);
                int minutes = (int)Timer / 60;
                int seconds = (int)Timer % 60;
                string suffix = $"\n({minutes:00}:{seconds:00})";
                int spectators = Spectator.ToBecomeSpectator.Count;
                string spectatorCount = spectators > 0 ? $"\nSpectators: {spectators}" : "";

                __instance.PlayerCounter.text = currentText + suffix + spectatorCount;
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
                        int diff = StellarRolesPlugin.VersionDeclared.CompareTo(PV.version);
                        if (diff != 0 || !PV.GuidMatches())
                            return false;
                    }

                    if (CustomOptionHolder.DynamicMap.GetBool())
                    {
                        byte chosenMapId = GetSelectedMap();
                        RPCProcedure.Send(CustomRPC.DynamicMapOption, chosenMapId);
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId = chosenMapId;
                    }
                }

                return true;
            }

            private static byte GetSelectedMap()
            {
                var skeldChance = CustomOptionHolder.DynamicMapEnableSkeld.GetSelection();
                var miraChance = CustomOptionHolder.DynamicMapEnableSkeld.GetSelection();
                var polusChance = CustomOptionHolder.DynamicMapEnablePolus.GetSelection();
                var airshipChance = CustomOptionHolder.DynamicMapEnableAirShip.GetSelection();
                var fungleChance = CustomOptionHolder.DynamicMapEnableFungal.GetSelection();
                var submergedChance = CustomOptionHolder.DynamicMapEnableSubmerged.GetSelection();

                System.Random rnd = new();
                int TotalWeight = 0;

                TotalWeight += skeldChance;
                TotalWeight += miraChance;
                TotalWeight += polusChance;
                TotalWeight += airshipChance;
                TotalWeight += fungleChance;
                TotalWeight += SubmergedCompatibility.Loaded ? submergedChance : 0;

                if (TotalWeight == 0)
                {
                    return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                }

                int randomNumber = rnd.Next(0, TotalWeight);

                if (randomNumber < skeldChance) //Skeld
                {
                    return 0;
                }
                randomNumber -= skeldChance;

                if (randomNumber < miraChance) //Mira
                {
                    return 1;
                }
                randomNumber -= miraChance;

                if (randomNumber < polusChance) //Polus
                {
                    return 2;
                }
                randomNumber -= polusChance;

                if (randomNumber < airshipChance) //Airship
                {
                    return 4;
                }
                randomNumber -= airshipChance;

                if (randomNumber < fungleChance) //Fungle
                {
                    return 5;
                }
                randomNumber -= fungleChance;

                if (SubmergedCompatibility.Loaded && randomNumber < submergedChance) //Submerged
                {
                    return 6;
                }

                return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
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
        [HarmonyPostfix]
        public static void LobbyInfoUpdatePostfix(LobbyInfoPane __instance)
        {
            if (CustomOption.Options.Count <= 0) return; // No instance
            var Preset0 = CustomOption.Options[0];
            if (Preset0.Selections.Length <= 0) return; // No instance
            var Preset = Preset0.Selections[Preset0.Selection].ToString();

            var GameModeText = GameObject.Find("GameModeText");
            if (GameModeText == null) return;

            var text = GameModeText.GetComponent<TextMeshPro>().text;
            if (text != Preset)
            {
                GameModeText.GetComponent<TextMeshPro>().text = Preset;
            }
        }
    }
}
