using BepInEx.Unity.IL2CPP;
using Discord;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace StellarRoles.Patches.Misc
{
    // Patch taken from https://github.com/All-Of-Us-Mods/LaunchpadReloaded/blob/master/LaunchpadReloaded/Patches/Generic/DiscordManagerPatch.cs
    [HarmonyPatch]
    public static class DiscordStatus
    {
        private const long ClientId = 1453234835173806231;
        private const uint SteamAppId = 945360;
        private static string ModInfo = $"StellarRoles v{StellarRolesPlugin.UpdateString}";
        private static string _smallIcon = "???";

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DiscordManager), nameof(DiscordManager.Start))]
        public static bool DiscordManagerStartPrefix(DiscordManager __instance)
        {
            DiscordManager.ClientId = ClientId;
            if (Application.platform == RuntimePlatform.Android)
            {
                return true;
            }

            InitializeDiscord(__instance);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
        public static void ActivityManagerUpdateActivityPrefix(ActivityManager __instance, [HarmonyArgument(0)] Activity activity)
        {
            var modCount = $"{IL2CPPChainloader.Instance.Plugins.Count} Mods";
            activity.Details = (string.IsNullOrEmpty(activity.Details)) ? ModInfo : ModInfo + " | " + activity.Details;
            activity.State = (string.IsNullOrEmpty(activity.State)) ? modCount : $"{modCount} | {activity.State}";
            activity.Assets.LargeImage = "icon";
            activity.Assets.SmallImage = _smallIcon;
        }

        private static void InitializeDiscord(DiscordManager __instance)
        {
            __instance.presence = new Discord.Discord(ClientId, 1UL);
            var activityManager = __instance.presence.GetActivityManager();

            activityManager.RegisterSteam(SteamAppId);
            activityManager.add_OnActivityJoin((Action<string>)__instance.HandleJoinRequest);
            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
            {
                __instance.OnSceneChange(scene.name);
            }));
            __instance.SetInMenus();
        }
    }
}