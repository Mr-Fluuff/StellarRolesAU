using HarmonyLib;
using StellarRoles.Modules;
using System;

namespace StellarRoles.Patches
{
    [Harmony]
    public class LobbySizeUpdate
    {
        public static int LobbySize => CustomOptionHolder.LobbySize.GetInt();

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public static class LobbySizePatch
        {
            static void Postfix(GameStartManager __instance)
            {
                if (AmongUsClient.Instance.AmHost && __instance != null)
                {
                    try
                    {
                        if (DynamicLobbies.LobbyLimit != LobbySize)
                        {
                            DynamicLobbies.LobbyLimit = LobbySize;
                            GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = DynamicLobbies.LobbyLimit;
                            __instance.LastPlayerCount = DynamicLobbies.LobbyLimit;
                            GameManager.Instance.LogicOptions.SyncOptions();
                            //PlayerControl.LocalPlayer.RpcSyncSettings(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions, false));
                        }
                    }
                    catch (Exception e)
                    {
                        Helpers.Log("LobbySizePatchError " + e);
                    }
                }
            }
        }
    }
}