using HarmonyLib;
using UnityEngine;

namespace StellarRoles.Modules
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ZoomHudUpdate
    {
        public static bool HasAlerted = false;
        public static TMPro.TextMeshPro ZoomText;
        public static void Postfix(HudManager __instance)
        {
            try
            {
                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                if (!localPlayer.Data.IsDead || localPlayer.Data.Role.IsImpostor || localPlayer.TasksLeft() != 0 || !Helpers.GameStarted) return;
                if (!HasAlerted)
                {
                    HasAlerted = true;

                    if (ZoomText == null)
                    {
                        ZoomText = Object.Instantiate(__instance.TaskPanel.taskText, __instance.transform);
                        ZoomText.text = "Scroll To Zoom";
                        ZoomText.color = Color.red;
                        ZoomText.transform.localScale = Vector3.one * 2;
                        ZoomText.transform.localPosition = new Vector3(1f, -4.5f);
                    }
                }

                ZoomText.gameObject.SetActive(!MeetingHud.Instance);

                if (MeetingHud.Instance) return;

                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    if (Camera.main.orthographicSize > 2f)
                    {
                        Camera.main.orthographicSize /= 1.25f;
                        __instance.transform.localScale /= 1.25f;
                        __instance.UICamera.orthographicSize /= 1.25f;
                    }
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    if (Camera.main.orthographicSize < 15.0f)
                    {
                        Camera.main.orthographicSize *= 1.25f;
                        __instance.transform.localScale *= 1.25f;
                        __instance.UICamera.orthographicSize *= 1.25f;
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
            Helpers.ResetZoom();
        }
    }
}
