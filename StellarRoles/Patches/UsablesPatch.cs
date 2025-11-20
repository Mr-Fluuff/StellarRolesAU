using Cpp2IL.Core.Extensions;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using StellarRoles.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StellarRoles.MapOptions;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public static class RearangeButtonsPatch
    {
        public static void Postfix(HudManager __instance)
        {
            var Chatbutton = GameObject.Find("ChatButton");
            var MenuButton = GameObject.Find("MenuButton");
            var MapButton = GameObject.Find("Main Camera/Hud/Buttons/TopRight/MapButton");
            var FriendsButton = GameObject.Find("Main Camera/Hud/Friends List Button");
            var LobbyInfoPane = GameObject.Find("Main Camera/Hud/LobbyInfoPane");
            if (Chatbutton != null)
            {
                Chatbutton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(0.86f, 0.56f, -400f);
                Chatbutton.transform.localScale = Vector3.one * 1.05f;
            }
            if (MenuButton != null)
            {
                MenuButton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(0.4f, 0.43f, -400f);
                MenuButton.transform.localScale = Vector3.one * 1.05f;
            }
            if (MapButton != null)
            {
                MapButton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(0.39f, 1.12f, -40f);
                MapButton.transform.localScale = Vector3.one * 1.05f;
            }
            if (FriendsButton != null)
            {
                FriendsButton.transform.localPosition = new Vector3(0, 0, 22);
                var Fbutton = FriendsButton.transform.FindChild("Friends List Button");
                Fbutton.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(1.63f, 0.43f, -450f);
                Fbutton.transform.localScale = Vector3.one * 0.2724f;
            }
            if (LobbyInfoPane != null)
            {
                LobbyInfoPane.transform.localScale = Vector3.one * 0.4098f;
                var viewsettings = LobbyInfoPane.transform.FindChild("AspectSize").FindChild("RulesPopOutWindow");
                viewsettings.localScale = Vector3.one * 1.5f;
                viewsettings.localPosition = new Vector3(-12.4675f, -4.5114f, -10);
            }
        }
    }
    [HarmonyPatch(typeof(ModManager), nameof(ModManager.LateUpdate))]
    public static class ModStampPatch
    {
        public static void Postfix(ModManager __instance)
        {
            if (__instance.ModStamp.enabled)
            {
                __instance.ModStamp.transform.position = AspectPosition.ComputeWorldPosition(__instance.localCamera, AspectPosition.EdgeAlignments.LeftTop, new Vector3(0.6f, 0.6f, __instance.localCamera.nearClipPlane + 0.1f));
            }
        }
    }

    [HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.ShowKillAnimation), new Type[] { typeof(OverlayKillAnimation), typeof(KillOverlayInitData) })]
    public static class KillOverlayMeetingPatch
    {
        public static bool Prefix(KillOverlay __instance, OverlayKillAnimation killAnimation, KillOverlayInitData initData)
        {
            if (!MeetingHud.Instance) return true;

            __instance.queue.Enqueue((Il2CppSystem.Func<Il2CppSystem.Collections.IEnumerator>)delegate
            {
                {
                    OverlayKillAnimation overlayKillAnimation = UnityEngine.Object.Instantiate<OverlayKillAnimation>(killAnimation, MeetingHud.Instance.transform);
                    overlayKillAnimation.transform.SetLocalZ(-20f);
                    overlayKillAnimation.Initialize(initData);
                    overlayKillAnimation.gameObject.SetActive(false);
                    return __instance.CoShowOne(overlayKillAnimation);
                }
            });
            if (__instance.showAll == null)
            {
                __instance.showAll = __instance.StartCoroutine(__instance.ShowAll());
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    public static class DeadBodyOnClickPatch
    {
        public static bool Prefix(DeadBody __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            Vector2 localPosition = localPlayer.GetTruePosition();

            if (__instance.Reported || !GameManager.Instance.CanReportBodies())
            {
                return false;
            }
            if (!Bombed.CanReport && Bombed.KillerDictionary.TryGetValue(localPlayer.PlayerId, out PlayerList players) && players.Contains(__instance.ParentId))
            {
                return false;
            }

            if (Minigame.Instance)
            {
                return false;
            }

            bool canReport = localPlayer.CanMove || localPlayer.inMovingPlat || localPlayer.onLadder;
            Vector2 offset = new Vector2(-0.2f, -0.25f);
            Vector2 bodyPosition = __instance.TruePosition + offset;
            float distance = Vector2.Distance(bodyPosition, localPosition);
            var dist = localPlayer.MaxReportDistance;
            bool Reportable = false;

            bool concealed = Charlatan.ConcealedBodies.Any(x => x == __instance.ParentId);
            float concealRange = localPlayer.MaxReportDistance * Charlatan.ConcealReportRange;
            bool blocked = PhysicsHelpers.AnythingBetween(localPosition, __instance.TruePosition, Constants.ShipAndObjectsMask, false);

            if (distance < 0.5f)
            {
                Reportable = true;
            }
            else if (!blocked)
            {
                if (concealed && distance < concealRange)
                {
                    Reportable = true;
                }

                if (!concealed && distance < localPlayer.MaxReportDistance)
                {
                    Reportable = true;
                }
            }

            if (Reportable)
            {
                __instance.Reported = true;
                NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(__instance.ParentId);
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

    [HarmonyPatch(typeof(EmptyGarbageMinigame), nameof(EmptyGarbageMinigame.Begin))]
    class EmptyGarbageMinigameBeginPatch
    {
        static List<Sprite> _sprites = new();
        static void Postfix(EmptyGarbageMinigame __instance)
        {
            if (_sprites.Count <= 0)
            {
                for (int a = 0; a < 6; a++)
                {
                    _sprites.Add(Helpers.LoadSpriteFromResources($"StellarRoles.Resources.Leaves{a + 1}.png", 115f));
                    //Helpers.Log($"Added Leaves{a + 1}");
                }
            }
            var garbage = __instance.Objects;
            int i = 0;

            List<int> choices = [0, 1, 2, 3, 4, 5];
            choices.Shuffle();
            List<int> list = [choices.RemoveAndReturn(0)];
            var chance50 = Helpers.TrueRandom(1, 100);
            var chance10 = Helpers.TrueRandom(1, 100);
            var chance1 = Helpers.TrueRandom(1, 100);

            //Helpers.Log($"{chance50}, {chance10}, {chance1}");


            if (chance1 == 72)
            {
                while (i < garbage.Count)
                {
                    garbage[i].sprite = _sprites.Random();
                    i++;
                }
            }
            else
            {
                int b = chance10 <= 10 ? 4 : chance50 > 50 ? 2 : 0;

                if (b > 0)
                {
                    for (int a = 0; a < b; a++)
                    {
                        list.Add(choices.RemoveAndReturn(0));
                    }
                }

                while (i < list.Count)
                {
                    garbage[i].sprite = _sprites[list[i]];
                    i++;
                }
            }
        }
    }



    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public static class ConsoleCanUsePatch
    {
        public static bool Prefix(ref float __result, Console __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
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

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.StartMeeting))]
    class ShipStatusStartMeetingPatch
    {
        static void Prefix()
        {
/*            var renderer = HudManager.Instance?.FullScreen;

            if (renderer == null) return;
            renderer.gameObject.SetActive(true);
            renderer.enabled = true;
            //renderer.color = Color.black;
            var color = Color.black;

            HudManager.Instance.StartCoroutine(Effects.Lerp(0.75f, new Action<float>((p) =>
            { // Delayed action
                var alpha = Mathf.Clamp01(p < 0.25f ? 1 : (1 - p));
                renderer.color = new Color(color.r, color.g, color.b, alpha);
                if (p == 1)
                {
                    renderer.enabled = false;
                }
            })));*/


            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p == PlayerControl.LocalPlayer) continue;

                p.SetPlayerAlpha(0);
            }
            Helpers.DelayedAction(3f, () => 
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p == PlayerControl.LocalPlayer) continue;

                    p.SetPlayerAlpha(1);
                }
            });
        }
    }

    [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), new Type[] { typeof(PlayerControl), typeof(bool) })]
    class FixHatColorZipline
    {
        public static void Postfix(ZiplineBehaviour __instance, PlayerControl player, bool fromTop)
        {
            __instance.StartCoroutine(Effects.Lerp(fromTop ? __instance.downTravelTime : __instance.upTravelTime, new System.Action<float>((p) =>
            {
                __instance.playerIdHands.TryGetValue(player.PlayerId, out HandZiplinePoolable hand);
                if (hand != null)
                {
                    if (Camouflager.CamouflageTimer <= 0 && !PlayerControl.LocalPlayer.IsMushroomMixupActive())
                    {
                        if (player == Parasite.Controlled)
                        {
                            hand.SetPlayerColor(Parasite.Player.CurrentOutfit, PlayerMaterial.MaskType.None, 1f);
                            // Also set hat color, cause the line destroys it...
                            player.RawSetHat(Parasite.Player.Data.DefaultOutfit.HatId, Parasite.Player.Data.DefaultOutfit.ColorId);
                        }
                        else if (player.IsMorphed())
                        {
                            hand.SetPlayerColor(Morphling.MorphTarget.CurrentOutfit, PlayerMaterial.MaskType.None, 1f);
                            // Also set hat color, cause the line destroys it...
                            player.RawSetHat(Morphling.MorphTarget.Data.DefaultOutfit.HatId, Morphling.MorphTarget.Data.DefaultOutfit.ColorId);
                        }
                        else if (player.IsInvisible())
                        {
                            hand.SetPlayerColor(player.CurrentOutfit, PlayerMaterial.MaskType.None, 0f);
                        }
                        else
                        {
                            hand.SetPlayerColor(player.CurrentOutfit, PlayerMaterial.MaskType.None, player.cosmetics.GetPhantomRoleAlpha());
                        }
                    }
                    else
                    {
                        PlayerMaterial.SetColors(6, hand.handRenderer);
                    }
                }
            })));
        }
    }
}