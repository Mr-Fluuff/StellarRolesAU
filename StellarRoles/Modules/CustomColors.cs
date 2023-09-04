using AmongUs.Data.Legacy;
using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Modules
{
    public class CustomColors
    {
        protected static Dictionary<int, string> ColorStrings = new();
        public static uint pickableColors = (uint)Palette.ColorNames.Length;

        private static readonly List<int> ORDER = new()
        {
7,  33, 13, 5,  32, 22, 25,
14, 10, 24, 30, 19, 20, 2,
18, 21, 16, 4,  26, 27, 31,
28, 11, 3,  29, 0,  8,  1,
15, 17, 23, 9,  6,  12
        };
        public static void Load()
        {
            List<StringNames> longlist = Enumerable.ToList(Palette.ColorNames);
            List<Color32> colorlist = Enumerable.ToList(Palette.PlayerColors);
            List<Color32> shadowlist = Enumerable.ToList(Palette.ShadowColors);

            List<CustomColor> colors = new()
            {
                /* Custom Colors */
                new CustomColor
                {
                    LongName = "Salmon",
                    Color = new Color32(255, 164, 166, byte.MaxValue), // color = new Color32(0xD8, 0x82, 0x83, byte.MaxValue),
                    Shadow = new Color32(206, 134, 136, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Olive",
                    Color = new Color32(154, 140, 61, byte.MaxValue),
                    Shadow = new Color32(104, 95, 40, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Turquoise",
                    Color = new Color32(22, 132, 176, byte.MaxValue),
                    Shadow = new Color32(15, 89, 117, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Mint",
                    Color = new Color32(111, 192, 156, byte.MaxValue),
                    Shadow = new Color32(65, 148, 111, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Lavender",
                    Color = new Color32(173, 126, 201, byte.MaxValue),
                    Shadow = new Color32(131, 58, 203, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Nougat",
                    Color = new Color32(160, 101, 56, byte.MaxValue),
                    Shadow = new Color32(115, 15, 78, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Peach",
                    Color = new Color32(255, 164, 119, byte.MaxValue),
                    Shadow = new Color32(238, 128, 100, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Wasabi",
                    Color = new Color32(112, 143, 46, byte.MaxValue),
                    Shadow = new Color32(72, 92, 29, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Hot Pink",
                    Color = new Color32(255, 51, 102, byte.MaxValue),
                    Shadow = new Color32(232, 0, 58, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Petrol",
                    Color = new Color32(0, 99, 105, byte.MaxValue),
                    Shadow = new Color32(0, 61, 54, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Lemon",
                    Color = new Color32(0xDB, 0xFD, 0x2F, byte.MaxValue),
                    Shadow = new Color32(0x74, 0xE5, 0x10, byte.MaxValue)
                },
                new CustomColor
                {
                    LongName = "Signal",
                    Color = new Color32(0xF7, 0x44, 0x17, byte.MaxValue),
                    Shadow = new Color32(0x9B, 0x2E, 0x0F, byte.MaxValue),
                },
                new CustomColor
                {
                    LongName = "Teal",
                    Color = new Color32(0x25, 0xB8, 0xBF, byte.MaxValue),
                    Shadow = new Color32(0x12, 0x89, 0x86, byte.MaxValue),
                },
                new CustomColor
                {
                    LongName = "Blurple",
                    Color = new Color32(0x59, 0x3C, 0xD6, byte.MaxValue),
                    Shadow = new Color32(0x29, 0x17, 0x96, byte.MaxValue),
                },
                new CustomColor
                {
                    LongName = "Sunrise",
                    Color = new Color32(0xFF, 0xCA, 0x19, byte.MaxValue),
                    Shadow = new Color32(0xDB, 0x44, 0x42, byte.MaxValue),
                },
                new CustomColor
                {
                    LongName = "Ice",
                    Color = new Color32(0xA8, 0xDF, 0xFF, byte.MaxValue),
                    Shadow = new Color32(0x59, 0x9F, 0xC8, byte.MaxValue),
                },
            };

            pickableColors += (uint)colors.Count; // Colors to show in Tab
            /** Hidden Colors **/

            /** Add Colors **/
            int id = 500000;
            foreach (CustomColor customColor in colors)
            {
                longlist.Add((StringNames)id);
                ColorStrings[id++] = customColor.LongName;
                colorlist.Add(customColor.Color);
                shadowlist.Add(customColor.Shadow);
            }

            Palette.ColorNames = longlist.ToArray();
            Palette.PlayerColors = colorlist.ToArray();
            Palette.ShadowColors = shadowlist.ToArray();
        }

        protected internal struct CustomColor
        {
            public string LongName;
            public Color32 Color;
            public Color32 Shadow;
        }

        [HarmonyPatch]
        public static class CustomColorPatches
        {
            [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] {
                typeof(StringNames),
                typeof(Il2CppReferenceArray<Il2CppSystem.Object>)
            })]
            private class ColorStringPatch
            {
                [HarmonyPriority(Priority.Last)]
                public static bool Prefix(ref string __result, [HarmonyArgument(0)] StringNames name)
                {
                    if ((int)name >= 500000)
                    {
                        string text = ColorStrings[(int)name];
                        if (text != null)
                        {
                            __result = text;
                            return false;
                        }
                    }
                    return true;
                }
            }
            [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
            private static class PlayerTabEnablePatch
            {
                public static void Postfix(PlayerTab __instance)
                { // Replace instead
                    Il2CppArrayBase<ColorChip> chips = __instance.ColorChips.ToArray();

                    int cols = 7; // TODO: Design an algorithm to dynamically position chips to optimally fill space
                    for (int i = 0; i < ORDER.Count; i++)
                    {
                        int pos = ORDER[i];
                        if (pos < 0 || pos > chips.Length)
                            continue;
                        ColorChip chip = chips[pos];
                        int row = i / cols, col = i % cols; // Dynamically do the positioning
                        chip.transform.localPosition = new Vector3(-1.75f + (col * 0.485f), 1.75f - (row * 0.49f), chip.transform.localPosition.z);
                        chip.transform.localScale *= 0.78f;
                    }
                    for (int j = ORDER.Count; j < chips.Length; j++)
                    { // If number isn't in order, hide it
                        ColorChip chip = chips[j];
                        chip.transform.localScale *= 0f;
                        chip.enabled = false;
                        chip.Button.enabled = false;
                        chip.Button.OnClick.RemoveAllListeners();
                    }
                }
            }
            [HarmonyPatch(typeof(LegacySaveManager), nameof(LegacySaveManager.LoadPlayerPrefs))]
            private static class LoadPlayerPrefsPatch
            { // Fix Potential issues with broken colors
                private static bool needsPatch = false;
                public static void Prefix([HarmonyArgument(0)] bool overrideLoad)
                {
                    if (!LegacySaveManager.loaded || overrideLoad)
                        needsPatch = true;
                }
                public static void Postfix()
                {
                    if (!needsPatch) return;
                    LegacySaveManager.colorConfig %= pickableColors;
                    needsPatch = false;
                }
            }
            [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
            private static class PlayerControlCheckColorPatch
            {
                private static bool IsTaken(PlayerControl player, uint color)
                {
                    foreach (GameData.PlayerInfo p in GameData.Instance.AllPlayers.GetFastEnumerator())
                        if (!p.Disconnected && p.PlayerId != player.PlayerId && p.DefaultOutfit.ColorId == color)
                            return true;
                    return false;
                }
                public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor)
                { // Fix incorrect color assignment
                    uint color = bodyColor;
                    if (IsTaken(__instance, color) || color >= Palette.PlayerColors.Length)
                    {
                        int num = 0;
                        while (num++ < 50 && (color >= pickableColors || IsTaken(__instance, color)))
                        {
                            color = (color + 1) % pickableColors;
                        }
                    }
                    __instance.RpcSetColor((byte)color);
                    return false;
                }
            }
        }
    }
}