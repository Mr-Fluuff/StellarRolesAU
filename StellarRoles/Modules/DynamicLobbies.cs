using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using InnerNet;

namespace StellarRoles.Modules
{
    [HarmonyPatch]
    public static class DynamicLobbies
    {
        public static int LobbyLimit = 15;
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HostGame))]
        public static class InnerNetClientHostPatch
        {
            public static void Prefix([HarmonyArgument(0)] IGameOptions settings)
            {
                int maxPlayers;
                try
                {
                    maxPlayers = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                }
                catch
                {
                    maxPlayers = 15;
                }
                CustomOptionHolder.LobbySize.UpdateSelection(maxPlayers - 4);
                LobbyLimit = maxPlayers;
                // settings.MaxPlayers = 15; // Force 15 Player Lobby on Server
                DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
            }
            public static void Postfix([HarmonyArgument(0)] IGameOptions settings)
            {
                // settings.MaxPlayers = LobbyLimit;
            }
        }
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
        public static class InnerNetClientJoinPatch
        {
            public static void Prefix()
            {
                DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
            }
        }
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public static class AmongUsClientOnPlayerJoined
        {
            public static bool Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
            {
                if (LobbyLimit < __instance.allClients.Count)
                { // TODO: Fix this canceling start
                    DisconnectPlayer(__instance, client.Id);
                    return false;
                }
                return true;
            }

            private static void DisconnectPlayer(InnerNetClient _this, int clientId)
            {
                if (!_this.AmHost)
                {
                    return;
                }
                MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
                messageWriter.StartMessage(4);
                messageWriter.Write(_this.GameId);
                messageWriter.WritePacked(clientId);
                messageWriter.Write((byte)DisconnectReasons.GameFull);
                messageWriter.EndMessage();
                _this.SendOrDisconnect(messageWriter);
                messageWriter.Recycle();
            }
        }
    }
}
