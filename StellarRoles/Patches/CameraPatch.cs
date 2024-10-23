using HarmonyLib;
using Il2CppSystem;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Patches
{

    [Harmony]
    public class CameraPatch
    {
        static float cameraTimer = 0f;

        public static void ResetData()
        {
            cameraTimer = 0f;
            SurveillanceMinigamePatch.ResetData();
            PlanetSurveillanceMinigamePatch.ResetData();
        }

        static void UseCameraTime()
        {
            cameraTimer = 0f;
        }

        [HarmonyPatch]
        class SurveillanceMinigamePatch
        {
            private static int page = 0;
            private static float timer = 0f;
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

            [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
            class SurveillanceMinigameBeginPatch
            {
                public static void Prefix()
                {
                    cameraTimer = 0f;
                }

                public static void Postfix(SurveillanceMinigame __instance)
                {
                    // Add securityGuard cameras
                    page = 0;
                    timer = 0;
                    if (ShipStatus.Instance.AllCameras.Length > 4 && __instance.FilteredRooms.Length > 0)
                    {
                        __instance.textures = __instance.textures.ToList().Concat(new RenderTexture[ShipStatus.Instance.AllCameras.Length - 4]).ToArray();
                        for (int i = 4; i < ShipStatus.Instance.AllCameras.Length; i++)
                        {
                            SurvCamera surv = ShipStatus.Instance.AllCameras[i];
                            Camera camera = UnityEngine.Object.Instantiate(__instance.CameraPrefab);
                            camera.transform.SetParent(__instance.transform);
                            camera.transform.position = new Vector3(surv.transform.position.x, surv.transform.position.y, 8f);
                            camera.orthographicSize = 2.35f;
                            RenderTexture temporary = RenderTexture.GetTemporary(256, 256, 16, (RenderTextureFormat)0);
                            __instance.textures[i] = temporary;
                            camera.targetTexture = temporary;
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
            class SurveillanceMinigameUpdatePatch
            {
                public static bool Prefix(SurveillanceMinigame __instance)
                {
                    if (Helpers.TutorialActive) {return true;};
                    cameraTimer += Time.deltaTime;
                    if (cameraTimer > 0.1f)
                        UseCameraTime();

                    if (!MapOptions.CanUseCameras())
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
                            TimeRemaining.transform.localPosition = new Vector3(4.5f, -.2f, -60f);
                            TimeRemaining.transform.localScale *= 1.8f;
                            TimeRemaining.color = Palette.White;
                        }

                        //string s = Watcher.batteryTime > 1f ? "s" : "";
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

                    // Update normal and securityGuard cameras
                    timer += Time.deltaTime;
                    int numberOfPages = Mathf.CeilToInt(ShipStatus.Instance.AllCameras.Length / 4f);

                    bool update = false;

                    if (timer > 3f || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    {
                        update = true;
                        timer = 0f;
                        page = (page + 1) % numberOfPages;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                    {
                        page = (page + numberOfPages - 1) % numberOfPages;
                        update = true;
                        timer = 0f;
                    }
                    bool lockedOut = Hacker.LockedOut;

                    if ((__instance.isStatic || update) && !PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer) && !lockedOut)
                    {
                        __instance.isStatic = false;
                        for (int i = 0; i < __instance.ViewPorts.Length; i++)
                        {
                            __instance.ViewPorts[i].sharedMaterial = __instance.DefaultMaterial;
                            __instance.SabText[i].gameObject.SetActive(false);
                            if (page * 4 + i < __instance.textures.Length)
                                __instance.ViewPorts[i].material.SetTexture("_MainTex", __instance.textures[page * 4 + i]);
                            else
                                __instance.ViewPorts[i].sharedMaterial = __instance.StaticMaterial;
                        }
                    }
                    else if ((!__instance.isStatic && PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(PlayerControl.LocalPlayer)) || lockedOut)
                    {
                        __instance.isStatic = true;
                        for (int j = 0; j < __instance.ViewPorts.Length; j++)
                        {
                            __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                            __instance.SabText[j].gameObject.SetActive(true);
                        }
                    }
                    return false;
                }
            }

            [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Close))]
            class SurveillanceMinigameClosePatch
            {
                static void Prefix()
                {
                    UseCameraTime();
                }
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame))]
        class PlanetSurveillanceMinigamePatch
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

            [HarmonyPrefix()]
            [HarmonyPatch(nameof(PlanetSurveillanceMinigame.Begin))]
            public static void BeginPrefix()
            {
                cameraTimer = 0f;
            }

            [HarmonyPrefix()]
            [HarmonyPatch(nameof(PlanetSurveillanceMinigame.Update))]
            public static bool UpdatePrefix(PlanetSurveillanceMinigame __instance)
            {
                if (Helpers.TutorialActive) { return true; };

                cameraTimer += Time.deltaTime;
                if (cameraTimer > 0.1f)
                    UseCameraTime();

                if (!MapOptions.CanUseCameras())
                {
                    __instance.Close();
                    return false;
                }

                bool lockedOut = Hacker.LockedOut;

                if (lockedOut)
                {
                    __instance.ViewPort.sharedMaterial = __instance.StaticMaterial;
                    __instance.SabText.gameObject.SetActive(true);
                    return false;
                }

                if (PlayerControl.LocalPlayer == Watcher.Player && Watcher.IsActive)
                {
                    if (TimeRemaining == null)
                    {
                        TimeRemaining = UnityEngine.Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                        TimeRemaining.alignment = TMPro.TextAlignmentOptions.Center;
                        TimeRemaining.transform.position = Vector3.zero;
                        TimeRemaining.transform.localPosition = new Vector3(3.5f, 2.5f);
                        TimeRemaining.transform.localScale *= 1.8f;
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

                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    __instance.NextCamera(1);
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                    __instance.NextCamera(-1);

                return true;
            }

            [HarmonyPrefix()]
            [HarmonyPatch(nameof(PlanetSurveillanceMinigame.Close))]
            public static void ClosePrefix()
            {
                UseCameraTime();
            }

            [HarmonyPrefix()]
            [HarmonyPatch(nameof(PlanetSurveillanceMinigame.NextCamera))]
            public static bool NextPrefix(PlanetSurveillanceMinigame __instance, [HarmonyArgument(0)] int direction)
            {
                // c&p of function from game code. Its a bit of a hack but its the easiest way to fix the game breaking the hacker
                if (Hacker.LockedOut)
                {
                    if (direction != 0 && Constants.ShouldPlaySfx())
                    {
                        SoundManager.Instance.PlaySound(__instance.ChangeSound, false, 1f);
                    }
                    __instance.Dots[__instance.currentCamera].sprite = __instance.DotDisabled;
                    __instance.currentCamera = (__instance.currentCamera + direction).Wrap(__instance.survCameras.Length);
                    __instance.Dots[__instance.currentCamera].sprite = __instance.DotEnabled;
                    SurvCamera survCamera = __instance.survCameras[__instance.currentCamera];
                    __instance.Camera.transform.position = survCamera.transform.position + __instance.survCameras[__instance.currentCamera].Offset;
                    __instance.LocationName.text = (survCamera.NewName > StringNames.ExitButton) ? DestroyableSingleton<TranslationController>.Instance.GetString(survCamera.NewName, Array.Empty<UnityEngine.Object>()) : survCamera.CamName;
                    return false;
                }

                return true;
            }
        }
    }
}