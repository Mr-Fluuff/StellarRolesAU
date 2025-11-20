using System;
using System.IO;
using HarmonyLib;
using InnerNet;

namespace StellarRoles
{
    [HarmonyPatch]
    public class AUMAntiCheat
    {
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.StartRpc))]
        public static class CheatStartRPCPatch
        {
            public static bool Prefix([HarmonyArgument(1)] byte callId)
            {
                return CheckAntiCheat(callId);
            }
        }

        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.StartRpcImmediately))]

        public static class CheatStartRPCImmediatelyPatch
        {
            public static bool Prefix([HarmonyArgument(1)] byte callId)
            {
                return CheckAntiCheat(callId);
            }
        }

        public static bool CheckAntiCheat(byte callId)
        {
            // id 85 is AUM
            // id 101 is AUMChat
            if (callId is 85 or 101)
            {
                if (HudManager.Instance != null)
                {
                    PlayerControl.LocalPlayer.RpcSendChat("[Cheating] I am using AUM Cheat. Please Ban Me");
                }
                return false;
            }
            return true;
        }
    }
}
