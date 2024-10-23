using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StellarRoles.Patches
{
    [Harmony]
    public class AdminPatch
    {
        //private static readonly HashSet<SystemTypes> Players = new();
        private static float AdminTimer = 0f;
        private static TextMeshPro TimeRemaining;
        private static GameObject BatteryIcon;
        //static bool clearedIcons = false;

        public static void ResetData()
        {
            AdminTimer = 0f;
            if (TimeRemaining != null)
            {
                Object.Destroy(TimeRemaining);
                TimeRemaining = null;
            }

            if (BatteryIcon != null)
            {
                Object.Destroy(BatteryIcon);
                BatteryIcon = null;
            }
        }

        static void UseAdminTime()
        {
            AdminTimer = 0f;
        }

        [HarmonyPatch(typeof(MapConsole), nameof(MapConsole.Use))]
        public static class MapConsoleUsePatch
        {
            public static bool Prefix()
            {
                return MapOptions.CanUseAdmin();
            }
        }

        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.OnEnable))]
        class MapCountOverlayOnEnablePatch
        {
            static void Prefix(MapCountOverlay __instance)
            {
                __instance.SetOptions(false, true);

                AdminTimer = 0f;
            }
        }

        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.OnDisable))]
        class MapCountOverlayOnDisablePatch
        {
            static void Prefix()
            {
                UseAdminTime();
                if (BatteryIcon != null)
                {
                    Object.Destroy(BatteryIcon);
                    BatteryIcon = null;
                }

                if (TimeRemaining != null)
                {
                    Object.Destroy(TimeRemaining);
                    TimeRemaining = null;
                }
            }
        }

        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
        class MapCountOverlayUpdatePatch
        {

            static bool Prefix(MapCountOverlay __instance)
            {
                AdminTimer += Time.deltaTime;
                if (AdminTimer > 0.1f)
                    UseAdminTime();

                __instance.timer += Time.deltaTime;
                if (__instance.timer < 0.1f)
                {
                    return false;
                }
                __instance.timer = 0f;

                //Players.Clear();

                if (PlayerControl.LocalPlayer == Administrator.Player && Administrator.IsActive)
                {
                    if (TimeRemaining == null)
                    {
                        TimeRemaining = Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                        TimeRemaining.alignment = TextAlignmentOptions.Center;
                        TimeRemaining.transform.position = Vector3.zero;
                        TimeRemaining.transform.localPosition = new Vector3(5f, 3f);
                        TimeRemaining.transform.localScale *= 2f;
                        TimeRemaining.color = Palette.White;
                    }

                    TimeRemaining.text = $"{(int)Administrator.BatteryTime}";
                    TimeRemaining.color = Administrator.BatteryTime > 3f ? Palette.AcceptedGreen : Palette.ImpostorRed;
                    TimeRemaining.gameObject.SetActive(true);

                    if (BatteryIcon == null)
                    {
                        BatteryIcon = Object.Instantiate(new GameObject("BatteryIcon"), TimeRemaining.transform);
                        SpriteRenderer SpriteRenderer = BatteryIcon.AddComponent<SpriteRenderer>();
                        SpriteRenderer.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.BatteryIcon.png", 200f);
                        BatteryIcon.transform.localPosition = new Vector3(-.25f, 0f);
                        BatteryIcon.transform.localScale *= .5f;
                    }

                    BatteryIcon.GetComponent<SpriteRenderer>().color = Administrator.BatteryTime > 3f ? Palette.AcceptedGreen : Palette.ImpostorRed;
                }

                bool commsActive = Helpers.IsCommsActive();
                bool lockedOut = Hacker.LockedOut;

                if (!__instance.isSab && (commsActive || lockedOut))
                {
                    __instance.isSab = true;
                    __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                    __instance.SabotageText.gameObject.SetActive(true);
                    return false;
                }

                if (__instance.isSab && !commsActive && !lockedOut)
                {
                    __instance.isSab = false;
                    __instance.BackgroundColor.SetColor(Color.green);
                    __instance.SabotageText.gameObject.SetActive(false);
                }

                HashSet<int> hashSet = new();
                for (int i = 0; i < __instance.CountAreas.Length; i++)
                {
                    CounterArea counterArea = __instance.CountAreas[i];
                    //Players.Add(counterArea.RoomType);

                    if (!commsActive && !lockedOut)
                    {
                        if (ShipStatus.Instance.FastRooms.TryGetValue(counterArea.RoomType, out PlainShipRoom plainShipRoom) && plainShipRoom.roomArea)
                        {
                            int num = plainShipRoom.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                            int num2 = 0;
                            for (int j = 0; j < num; j++)
                            {
                                Collider2D collider2D = __instance.buffer[j];
                                if (collider2D.CompareTag("DeadBody") && __instance.includeDeadBodies)
                                {
                                    DeadBody component = collider2D.GetComponent<DeadBody>();
                                    if (component != null && hashSet.Add(component.ParentId))
                                    {
                                        num2++;
                                    }
                                }
                                else if (!collider2D.isTrigger)
                                {
                                    PlayerControl component2 = collider2D.GetComponent<PlayerControl>();
                                    if (component2?.Data != null && !component2.Data.Disconnected && !component2.Data.IsDead &&
                                        (__instance.showLivePlayerPosition || !component2.AmOwner) && hashSet.Add((int)component2.PlayerId))
                                    {
                                        num2++;
                                    }
                                }
                            }
                            counterArea.UpdateCount(num2);
                        }
                        else
                        {
                            Helpers.Log(LogLevel.Warning, "Couldn't find counter for: " + counterArea.RoomType);
                        }
                    }
                    else
                    {
                        counterArea.UpdateCount(0);
                    }
                }
                return false;
            }
        }
        /*[HarmonyPatch(typeof(CounterArea), nameof(CounterArea.UpdateCount))]
        class CounterAreaUpdateCountPatch
        {
            private static Material DefaultMat;
            private static Material NewMat;
            static void Postfix(CounterArea __instance)
            {
                if (Players.Contains(__instance.RoomType))
                {
                    int i = -1;
                    foreach (PoolableBehavior icon in __instance.myIcons.GetFastEnumerator())
                    {
                        i += 1;
                        SpriteRenderer renderer = icon.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            if (DefaultMat == null) DefaultMat = renderer.material;
                            if (NewMat == null) NewMat = Object.Instantiate(DefaultMat);
                            renderer.material = DefaultMat;
                        }
                    }
                }
            }
        }*/
    }
}