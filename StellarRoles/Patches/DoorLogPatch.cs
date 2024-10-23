using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class DoorLogPatch
    {
        static TMPro.TextMeshPro TimeRemaining;
        static GameObject BatteryIcon;
        public static void ResetData()
        {
            if (TimeRemaining != null)
            {
                UnityEngine.Object.Destroy(TimeRemaining);
                TimeRemaining = null;
            }
            if (BatteryIcon != null)
            {
                UnityEngine.Object.Destroy(BatteryIcon);
                BatteryIcon = null;
            }
        }

        [HarmonyPatch(typeof(SecurityLogGame), nameof(SecurityLogGame.Update))]
        class SecurityLogGameUpdatePatch
        {
            public static bool Prefix(SecurityLogGame __instance)
            {
                (int playerCompleted, int playerTotal) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
                if (playerCompleted < Math.Min(MapOptions.TasksTilLogsAccessOnMira, playerTotal))
                {
                    __instance.Close();
                    return false;
                }

                if (PlayerControl.LocalPlayer == Watcher.Player && Watcher.IsActive)
                {
                    if (TimeRemaining == null)
                    {
                        TimeRemaining = UnityEngine.Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                        TimeRemaining.alignment = TMPro.TextAlignmentOptions.Center;
                        TimeRemaining.transform.position = Vector3.zero;
                        TimeRemaining.transform.localPosition = new Vector3(4f, 2f);
                        TimeRemaining.transform.localScale *= 1.6f;
                        TimeRemaining.color = Palette.White;
                    }

                    TimeRemaining.text = $"{(int)Watcher.BatteryTime}";
                    TimeRemaining.gameObject.SetActive(true);
                    TimeRemaining.color = Watcher.BatteryTime > 3f ? Palette.AcceptedGreen : Palette.ImpostorRed;

                    if (BatteryIcon == null)
                    {
                        BatteryIcon = UnityEngine.Object.Instantiate(new GameObject("BatteryIcon"), TimeRemaining.transform);
                        SpriteRenderer SpriteRenderer = BatteryIcon.AddComponent<SpriteRenderer>();
                        SpriteRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.BatteryIcon.png", 200f);
                        BatteryIcon.transform.localPosition = new Vector3(-.25f, 0f);
                        BatteryIcon.transform.SetLocalZ(-80f);
                        BatteryIcon.transform.localScale *= .5f;
                        BatteryIcon.layer = __instance.gameObject.layer;
                    }

                    BatteryIcon.GetComponent<SpriteRenderer>().color = Watcher.BatteryTime > 3f ? Palette.AcceptedGreen : Palette.ImpostorRed;
                }

                if (Hacker.LockedOut)
                {
                    __instance.SabText.gameObject.SetActive(true);
                    return false;
                }


                return true;
            }
        }
    }
}
