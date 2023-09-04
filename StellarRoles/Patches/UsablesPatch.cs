using HarmonyLib;
using System;
using UnityEngine;
using static StellarRoles.MapOptions;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    public static class DeadBodyOnClickPatch
    {
        public static bool Prefix(DeadBody __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;

            if (__instance.Reported || !GameManager.Instance.CanReportBodies())
            {
                return false;
            }
            if (!Bombed.CanReport && Bombed.KillerDictionary.TryGetValue(localPlayer.PlayerId, out PlayerList players) && players.Contains(__instance.ParentId))
            {
                return false;
            }

            Vector2 localPosition = localPlayer.GetTruePosition();
            Vector2 bodyPosition = __instance.TruePosition;
            if (Vector2.Distance(bodyPosition, localPosition) <= localPlayer.MaxReportDistance && localPlayer.CanMove && !PhysicsHelpers.AnythingBetween(localPosition, bodyPosition, Constants.ShipAndObjectsMask, false))
            {
                __instance.Reported = true;
                GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(__instance.ParentId);
                localPlayer.CmdReportDeadBody(playerById);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    class EmergencyMinigameUpdatePatch
    {
        static void Postfix(EmergencyMinigame __instance)
        {
            bool roleCanCallEmergency = true;
            string statusText = "";

            // Potentially deactivate emergency button for Jester
            if (PlayerControl.LocalPlayer.IsJester(out _) && !Jester.CanCallEmergency)
            {
                roleCanCallEmergency = false;
                statusText = "The Jester can't start an emergency meeting";
            }

            if (!roleCanCallEmergency)
            {
                __instance.StatusText.text = statusText;
                __instance.NumberText.text = string.Empty;
                __instance.ClosedLid.gameObject.SetActive(true);
                __instance.OpenLid.gameObject.SetActive(false);
                __instance.ButtonActive = false;
                return;
            }

            // Handle max number of meetings
            if (__instance.state == 1)
            {
                int localRemaining = PlayerControl.LocalPlayer.RemainingEmergencies;
                int teamRemaining = Mathf.Max(0, MaxNumberOfMeetings - MeetingsCount);
                int remaining = Mathf.Min(localRemaining, (Mayor.Player == PlayerControl.LocalPlayer) ? 1 : teamRemaining);
                __instance.NumberText.text = $"{localRemaining} and the ship has {teamRemaining}";
                __instance.ButtonActive = remaining > 0;
                __instance.ClosedLid.gameObject.SetActive(!__instance.ButtonActive);
                __instance.OpenLid.gameObject.SetActive(__instance.ButtonActive);
                return;
            }
        }
    }


    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public static class ConsoleCanUsePatch
    {
        public static bool Prefix(ref float __result, Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
        {
            canUse = couldUse = false;
            if (__instance.AllowImpostor || !Helpers.HasFakeTasks(pc.Object)) return true;
            __result = float.MaxValue;
            return false;
        }
    }

    [HarmonyPatch]
    class VitalsMinigamePatch
    {
        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
        class VitalsMinigameStartPatch
        {
            static void Postfix(VitalsMinigame __instance)
            {

                //Fix Visor in Vitals
                foreach (VitalsPanel panel in __instance.vitals)
                {
                    if (panel.PlayerIcon != null && panel.PlayerIcon.cosmetics.skin != null)
                    {
                        panel.PlayerIcon.cosmetics.skin.transform.position = new Vector3(0, 0, 0f);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        class VitalsMinigameUpdatePatch
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

            public static bool Prefix(VitalsMinigame __instance)
            {
                if (Hacker.LockedOut)
                {
                    foreach (VitalsPanel vitals in __instance.vitals)
                    {
                        vitals.gameObject.SetActive(false);

                    }
                    __instance.SabText.gameObject.SetActive(true);
                    return false;
                }
                return true;
            }

            static void Postfix(VitalsMinigame __instance)
            {
                if (PlayerControl.LocalPlayer == Medic.Player && Medic.IsActive)
                {
                    if (TimeRemaining == null)
                    {
                        TimeRemaining = UnityEngine.Object.Instantiate(HudManager.Instance.TaskPanel.taskText, __instance.transform);
                        TimeRemaining.alignment = TMPro.TextAlignmentOptions.Center;
                        TimeRemaining.transform.position = Vector3.zero;
                        TimeRemaining.transform.localPosition = new Vector3(3.5f, 2.5f, -60f);
                        TimeRemaining.transform.localScale *= 1.8f;
                        TimeRemaining.color = Palette.White;
                    }

                    TimeRemaining.text = $"{(int)Medic.Battery}";
                    TimeRemaining.gameObject.SetActive(true);
                    TimeRemaining.color = Medic.Battery > 3f ? Palette.AcceptedGreen : Palette.ImpostorRed;

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

                    BatteryIcon.GetComponent<SpriteRenderer>().color = Medic.Battery > 3f ? Palette.AcceptedGreen : Palette.ImpostorRed;

                }

                foreach (var panel in __instance.vitals)
                {
                    if (Spectator.IsSpectator(panel.PlayerInfo.PlayerId))
                        panel.gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
    class MedScanMinigameFixedUpdatePatch
    {
        static void Prefix(MedScanMinigame __instance)
        {
            if (AllowParallelMedBayScans)
            {
                __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
                __instance.medscan.UsersList.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(MedScanMinigame._WalkToOffset_d__15), nameof(MedScanMinigame._WalkToPad_d__16.MoveNext))]
    class MedscanMiniGamePatchWTP
    {
        static bool Prefix(MedScanMinigame._WalkToPad_d__16 __instance)
        {
            if (DisableMedscanWalking)
            {
                int num = __instance.__1__state;
                MedScanMinigame medScanMinigame = __instance.__4__this;
                switch (num)
                {
                    case 0:
                        __instance.__1__state = -1;
                        medScanMinigame.state = MedScanMinigame.PositionState.WalkingToPad;
                        __instance.__1__state = 1;
                        return true;
                    case 1:
                        __instance.__1__state = -1;
                        __instance.__2__current = new WaitForSeconds(0.1f);
                        __instance.__1__state = 2;
                        return true;
                    case 2:
                        __instance.__1__state = -1;
                        medScanMinigame.walking = null;
                        return false;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(MedScanMinigame._WalkToOffset_d__15), nameof(MedScanMinigame._WalkToOffset_d__15.MoveNext))]
    class MedscanMiniGamePatchWTO
    {
        static bool Prefix(MedScanMinigame._WalkToOffset_d__15 __instance)
        {
            if (DisableMedscanWalking)
            {
                int num = __instance.__1__state;
                MedScanMinigame medScanMinigame = __instance.__4__this;
                switch (num)
                {
                    case 0:
                        __instance.__1__state = -1;
                        medScanMinigame.state = MedScanMinigame.PositionState.WalkingToOffset;
                        __instance.__1__state = 1;
                        return true;
                    case 1:
                        __instance.__1__state = -1;
                        __instance.__2__current = new WaitForSeconds(0.1f);
                        __instance.__1__state = 2;
                        return true;
                    case 2:
                        __instance.__1__state = -1;
                        medScanMinigame.walking = null;
                        return false;
                    default:
                        return false;
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    class OpenSaboMapPatch
    {
        static void Postfix(HudManager __instance)
        {
            __instance.MapButton.OnClick.RemoveAllListeners();
            __instance.MapButton.OnClick.AddListener((Action)(() =>
            {
                bool canSabo = (PlayerControl.LocalPlayer.Data.Role.IsImpostor || PlayerControl.LocalPlayer.NeutralKillerCanSabo()) && !MeetingHud.Instance;
                if (MapBehaviour.Instance && MapBehaviour.Instance.gameObject.activeSelf)
                {
                    MapBehaviour.Instance.Close();
                    return;
                }
                if (__instance.IsIntroDisplayed)
                {
                    return;
                }
                if (!ShipStatus.Instance)
                {
                    return;
                }
                __instance.InitMap();
                if (canSabo)
                    MapBehaviour.Instance.ShowSabotageMap();
                else
                    MapBehaviour.Instance.ShowNormalMap();
            }));
        }
    }
}