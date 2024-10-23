using HarmonyLib;
using StellarRoles.Utilities;
using System;
using System.Linq;

namespace StellarRoles.Modules
{
    [HarmonyPatch]
    public static class ChatCommands
    {

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        private static class SendChatPatch
        {
            static bool Prefix(ChatController __instance)
            {
                string text = __instance.freeChatField.Text;
                bool handled = false;
                bool meeting = MeetingHud.Instance != null || LobbyBehaviour.Instance != null;

                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                    if (text.ToLower().StartsWith("/kick "))
                    {
                        string playerName = text[6..];
                        PlayerControl target = Helpers.PlayerByName(playerName);
                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                        {
                            InnerNet.ClientData client = AmongUsClient.Instance.GetClient(target.OwnerId);
                            if (client != null)
                            {
                                AmongUsClient.Instance.KickPlayer(client.Id, false);
                                handled = true;
                            }
                        }
                    }
                    else if (text.ToLower().StartsWith("/ban "))
                    {
                        string playerName = text[6..];
                        PlayerControl target = Helpers.PlayerByName(playerName);
                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                        {
                            InnerNet.ClientData client = AmongUsClient.Instance.GetClient(target.OwnerId);
                            if (client != null)
                            {
                                AmongUsClient.Instance.KickPlayer(client.Id, true);
                                handled = true;
                            }
                        }
                    }
                }

                if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                {
                    if (text.ToLower().Equals("/murder"))
                    {
                        PlayerControl.LocalPlayer.Exiled();
                        HudManager.Instance.KillOverlay.ShowKillAnimation(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.Data);
                        handled = true;
                    }
                    else if (text.ToLower().StartsWith("/color "))
                    {
                        handled = true;
                        if (!int.TryParse(text[7..], out int col))
                        {
                            __instance.AddChat(PlayerControl.LocalPlayer, "Unable to parse color id\nUsage: /color {id}");
                        }
                        col = Math.Clamp(col, 0, Palette.PlayerColors.Length - 1);
                        PlayerControl.LocalPlayer.SetColor(col);
                        __instance.AddChat(PlayerControl.LocalPlayer, "Changed color succesfully"); ;
                    }
                }

                if (text.ToLower().StartsWith("/tp ") && PlayerControl.LocalPlayer.Data.IsDead)
                {
                    string playerName = text[4..].ToLower();
                    PlayerControl target = Helpers.PlayerByName(playerName);
                    if (target != null)
                    {
                        PlayerControl.LocalPlayer.transform.position = target.transform.position;
                        handled = true;
                    }
                }

                if (text.ToLower().StartsWith("/duel"))
                {
                    string message = text[6..].ToLower();

                    if (message.Length != 0)
                    {
                        Helpers.Log(LogLevel.Debug, "Starting Duel Command");
                        // TODO: convert into one RPC call
                        RPCProcedure.Send(CustomRPC.Duel, PlayerControl.LocalPlayer, message);
                        RPCProcedure.Duel(PlayerControl.LocalPlayer, message);

                        RPCProcedure.Send(CustomRPC.SendResultOfDuel);
                        RPCProcedure.DuelResults();

                        RPCProcedure.Send(CustomRPC.ClearDuel, PlayerControl.LocalPlayer);
                        RPCProcedure.RemoveCompletedDuels(PlayerControl.LocalPlayer);
                        handled = true;
                        Helpers.Log(LogLevel.Debug, "Ending Duel Command");
                    }
                }

                if (!meeting && PlayerControl.LocalPlayer.SpecialFollower())
                {
                    handled = true;
                }

                if (text.Any() && handled)
                {
                    __instance.freeChatField.Clear();
                    __instance.quickChatMenu.Clear();
                }

                return !handled;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        private static class EnableChat
        {
            private static void Postfix(HudManager __instance)
            {
                try
                {
                    if (
                        !__instance.Chat.isActiveAndEnabled && (
                            PlayerControl.LocalPlayer == Detective.Player ||
                            AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay
                        )
                    )
                        __instance.Chat.SetVisible(true);
                }
                catch
                {

                }
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        public static class SetBubbleName
        {
            public static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
            {
                if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && Spy.Player != null && Spy.Player?.Data.PlayerName == playerName)
                    __instance.NameText.color = Palette.ImpostorRed;
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, ref string chatText)
            {
                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                bool meeting = MeetingHud.Instance != null || LobbyBehaviour.Instance != null;
                bool localSource = sourcePlayer.AmOwner;

                bool player = meeting || localPlayer.Data.IsDead || localSource;

                if (__instance != HudManager.Instance.Chat) return true;

                if (chatText.StartsWith("[ImpChat]") && (localPlayer.Data.Role.IsImpostor || localPlayer.Data.IsDead)) return true;

                if (localPlayer == Detective.Player) return player;

                return player;
            }
        }
    }
}
