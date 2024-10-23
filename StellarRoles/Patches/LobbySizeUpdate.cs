using HarmonyLib;
using StellarRoles.Modules;
using StellarRoles.Utilities;

namespace StellarRoles.Patches
{
    [Harmony]
    public class LobbySizeUpdate
    {
        public static int LobbySize => CustomOptionHolder.LobbySize.GetInt();

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        public static class LobbySizePatch
        {
            static void Postfix(LobbyBehaviour __instance)
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    if (DynamicLobbies.LobbyLimit != LobbySize)
                    {
                        DynamicLobbies.LobbyLimit = LobbySize;
                        GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = DynamicLobbies.LobbyLimit;
                        GameStartManager.Instance.LastPlayerCount = DynamicLobbies.LobbyLimit;
                        PlayerControl.LocalPlayer.RpcSyncSettings(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions, false));
                    }
                }
            }
        }
    }
}