using HarmonyLib;
using Rewired.Utils.Platforms.Windows;
using System;
using TMPro;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public class VersionChecker : MonoBehaviour
    {
        public static bool IsSupported { get; private set; } = true;
        public static TextMeshPro Text = null;
        public static Camera Camera;
        public static Camera PlayerCamera;

        //public static Vector3 TextOffset = new(0, 0f, -1000f);

        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        private static class VersionCheckerVersionShowerStartPatch
        {
            private static void Postfix(VersionShower __instance)
            {
                Check();
                if (!IsSupported)
                {
                    CreateText(__instance.text, false);
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
        [HarmonyPriority (Priority.Last)]
        private static class VersionCheckerMainMenuUpdate
        {
            private static void Postfix()
            {
                UpdateText(false);
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPriority(Priority.Last)]
        private static class VersionCheckerHudManagerUpdate
        {
            private static void Postfix(HudManager __instance)
            {
                UpdateText(true);
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        private static class VersionCheckerHudManagerStart
        {
            private static void Postfix(HudManager __instance)
            {
                if (!IsSupported)
                {
                    if (Text != null)
                    {
                        Text = null;
                    }

                    Helpers.DelayedAction(2f, () => { CreateText(HudManager.Instance.TaskPanel.taskText, true); });
                }
            }
        }

        public static void Check()
        {
            Version AmongUsVersion = Version.Parse(Application.version);

            Version SupportedVersion = Version.Parse(StellarRolesPlugin.SupportedAUVersion);

            IsSupported = AmongUsVersion == SupportedVersion;

        }

        public static void UpdateText(bool hud)
        {
            if (IsSupported) return;
            if (!Text.enabled) return;
            Vector3 TextOffset = new(0, 2f, -1000f);

            if (hud)
            {
                TextOffset = new(0, -2f, -1000f);
            }

            Text.transform.position = AspectPosition.ComputeWorldPosition(Camera.main, AspectPosition.EdgeAlignments.Center, TextOffset);
        }

        public static void CreateText(TextMeshPro baseText, bool hud)
        {
            
            Text = Instantiate(baseText);
            Text.enabled = true;
            Text.text = $"StellarRoles v{StellarRolesPlugin.UpdateString} is not compatible with this Among Us Version.\nPlease use Among Us v{StellarRolesPlugin.SupportedAUVersionNumber}";
            Text.color = Color.red;
            Text.outlineColor = Color.black;
            Text.alignment = TextAlignmentOptions.Top;
            Text.fontStyle = FontStyles.Bold;
            Text.outlineWidth = 0.25f;

            if (hud)
            {
                Text.transform.localScale = Vector3.one * 1.4f;
            }
            else
            {
                Text.transform.localScale = Vector3.one * 2.5f;
            }
        }
    }
}
