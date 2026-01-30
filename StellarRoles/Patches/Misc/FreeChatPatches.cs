using HarmonyLib;
using UnityEngine;

namespace StellarRoles.Patches.Misc
{
    [HarmonyPatch(typeof(FreeChatInputField))]
    public static class FreeChatPatches
    {
        //  Update character count text on Awake
        [HarmonyPostfix]
        [HarmonyPatch(nameof(FreeChatInputField.Awake))]
        public static void AwakePostfix(FreeChatInputField __instance)
        {
            // Bigger character limit
            __instance.textArea.characterLimit = 300;
            __instance.UpdateCharCount();

            if (__instance.charCountText != null && __instance.textArea != null)
            {
                int length = __instance.textArea.text.Length;
                int limit = __instance.textArea.characterLimit;
                __instance.charCountText.text = $"{length}/{limit}";
            }
        }

        //  Char count color + limit update
        [HarmonyPostfix]
        [HarmonyPatch(nameof(FreeChatInputField.UpdateCharCount))]
        public static void UpdateCharCountPostfix(FreeChatInputField __instance)
        {
            int length = __instance.textArea.text.Length;
            int limit = __instance.textArea.characterLimit;

            __instance.charCountText.text = $"{length}/{limit}";

            if (length < 200)
            {
                __instance.charCountText.color = Color.black;
                return;
            }

            if (length < 225)
            {
                __instance.charCountText.color = Color.yellow;
                return;
            }

            if (length < 250)
            {
                __instance.charCountText.color = new Color(1f, 0.5f, 0f, 1f); // Orange
                return;
            }

            __instance.charCountText.color = Color.red;
        }
    }
}
