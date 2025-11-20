using HarmonyLib;
using UnityEngine;

namespace StellarRoles.Modules
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ZoomHudUpdate
    {
        public static TMPro.TextMeshPro ZoomText;
        public static void Postfix(HudManager __instance)
        {
            try
            {
                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                if (!localPlayer.Data.IsDead || localPlayer.Data.Role.IsImpostor || localPlayer.TasksLeft() != 0 || !Helpers.GameStarted)
                {
                    ZoomText?.gameObject.SetActive(false);
                    return;
                }

                if (ZoomText == null)
                {
                    ZoomText = Object.Instantiate(__instance.TaskPanel.taskText, __instance.transform);
                    ZoomText.text = "Scroll To Zoom";
                    ZoomText.color = Color.red;
                    ZoomText.transform.localScale = Vector3.one * 2;
                    ZoomText.transform.localPosition = new Vector3(1f, -4.5f);
                }

                ZoomText.gameObject.SetActive(!MeetingHud.Instance);

                if (__instance.Chat.IsOpenOrOpening && Camera.main.orthographicSize != 3f)
                {
                    Helpers.ResetZoom();
                    __instance.Chat.Toggle();
                }

                if (MeetingHud.Instance) return;

                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    if (Camera.main.orthographicSize > 3f)
                    {
                        Camera.main.orthographicSize -= 1f;
                        __instance.UICamera.orthographicSize -= 1f;

                        var percent = Camera.main.orthographicSize / 3;
                        __instance.transform.localScale = Vector3.one * percent;
                    }
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    if (Camera.main.orthographicSize < 14.0f)
                    {
                        Camera.main.orthographicSize += 1f;
                        __instance.UICamera.orthographicSize += 1f;

                        var percent = Camera.main.orthographicSize / 3;
                        __instance.transform.localScale = Vector3.one * percent;
                    }
                }
                __instance.TaskPanel.gameObject.SetActive(Camera.main.orthographicSize == 3.0f);
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public static class ZoomMeetingStart
    {
        public static void Postfix()
        {
            Helpers.DelayedAction(0.25f, Helpers.ResetZoom);
        }
    }
}
