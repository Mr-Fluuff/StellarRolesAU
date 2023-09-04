using HarmonyLib;
using StellarRoles.Modules;
using StellarRoles.Utilities;

namespace StellarRoles.Patches
{
    [Harmony]
    public class LobbySizeUpdate
    {
        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        public static class LobbySize
        {
            static void Postfix(LobbyBehaviour __instance)
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    if (DynamicLobbies.LobbyLimit != CustomOptionHolder.LobbySize.GetSelection() + 4)
                    {
                        DynamicLobbies.LobbyLimit = CustomOptionHolder.LobbySize.GetSelection() + 4;
                        GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = DynamicLobbies.LobbyLimit;
                        FastDestroyableSingleton<GameStartManager>.Instance.LastPlayerCount = DynamicLobbies.LobbyLimit;
                        PlayerControl.LocalPlayer.RpcSyncSettings(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions));
                    }
                }
            }
        }
    }
}