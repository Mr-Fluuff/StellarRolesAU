using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using Hazel;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;
using UnityEngine.UI;
using Reactor.Utilities.Extensions;
using StellarRoles.Patches;
using Object = UnityEngine.Object;
using TMPro;

namespace StellarRoles.Modules
{
    [HarmonyPatch]
    public class RoleDraft
    {
        public static bool isEnabled => CustomOptionHolder.isDraftMode.GetBool() && !Helpers.IsHideAndSeek;
        public static bool isRunning = false;

        public static List<byte> pickOrder = new();
        private static bool picked = false;
        private static float timer = 0f;
        private static List<Transform> buttons = new List<Transform>();
        private static TMPro.TextMeshPro feedText;
        public static List<RoleId> alreadyPicked = new();
        public static IEnumerator CoSelectRoles(IntroCutscene __instance)
        {
            isRunning = true;
            SoundEffectsManager.Forceplay(Sounds.Draft, 1f, true, true);
            alreadyPicked.Clear();
            bool playedAlert = false;
            feedText = UnityEngine.Object.Instantiate(__instance.TeamTitle, __instance.transform);
            var aspectPosition = feedText.gameObject.AddComponent<AspectPosition>();
            aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftTop;
            aspectPosition.DistanceFromEdge = new Vector2(1.62f, 1.2f);
            aspectPosition.AdjustPosition();
            feedText.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            feedText.text = "<size=200%>Roles Picked:</size>\n\n";
            feedText.alignment = TMPro.TextAlignmentOptions.TopLeft;
            feedText.autoSizeTextContainer = true;
            feedText.fontSize = 3f;
            feedText.enableAutoSizing = false;
            __instance.TeamTitle.transform.localPosition = __instance.TeamTitle.transform.localPosition + new Vector3(1f, 0f);
            __instance.TeamTitle.text = "Currently Picking:";
            __instance.BackgroundBar.enabled = false;
            __instance.TeamTitle.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
            __instance.TeamTitle.autoSizeTextContainer = true;
            __instance.TeamTitle.enableAutoSizing = false;
            __instance.TeamTitle.fontSize = 5;
            __instance.TeamTitle.alignment = TMPro.TextAlignmentOptions.Top;
            __instance.ImpostorText.gameObject.SetActive(false);
            GameObject.Find("BackgroundLayer")?.SetActive(false);
            foreach (var player in UnityEngine.Object.FindObjectsOfType<PoolablePlayer>())
            {
                if (player.name.Contains("Dummy"))
                {
                    player.gameObject.SetActive(false);
                }
            }
            __instance.FrontMost.gameObject.SetActive(false);

            if (AmongUsClient.Instance.AmHost)
            {
                sendPickOrder();
            }

            while (pickOrder.Count == 0)
            {
                yield return null;
            }

            while (pickOrder.Count > 0) 
            {
                picked = false;
                timer = 0;
                float maxTimer = CustomOptionHolder.draftModeTimeToChoose.GetFloat();
                string playerText = "";
                while (timer < maxTimer || !picked) 
                {
                    if (pickOrder.Count == 0)
                        break;
                    // wait for pick
                    timer += Time.deltaTime;
                    if (PlayerControl.LocalPlayer.PlayerId == pickOrder[0]) 
                    {
                        if (!playedAlert) 
                        {
                            playedAlert = true;
                            SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                        }
                        // Animate beginning of choice, by changing background color
                        float min = 50 / 255f;
                        Color backGroundColor = new Color(min, min, min, 1);
                        if (timer < 1) 
                        {
                            float max = 230 / 255f;
                            if (timer < 0.5f) 
                            { // White flash
                                float p = timer / 0.5f;
                                float value = (float)Math.Pow(p, 1f) * max;
                                backGroundColor = new Color(value, value, value, 1);
                            } else 
                            {
                                float p = (1 - timer) / 0.5f;
                                float value = (float)Math.Pow(p, 2f) * max + (1 - (float)Math.Pow(p, 2f)) * min;
                                backGroundColor = new Color(value, value, value, 1);
                            }

                        }
                        HudManager.Instance.FullScreen.color = backGroundColor;
                        GameObject.Find("BackgroundLayer")?.SetActive(false);

                        // enable pick, wait for pick
                        Color youColor = timer - (int)timer > 0.5 ? Color.red : Color.yellow;
                        playerText = Helpers.ColorString(youColor, "You!");

                        // Available Roles:
                        List<RoleInfo> availableRoles = new();
                        foreach (RoleInfo roleInfo in RoleInfo.AllRoleInfos) 
                        {
                            int impostorCount = PlayerControl.AllPlayerControls.ToArray().ToList().Where(x => x.Data.Role.IsImpostor).Count();
                            if (roleInfo.FactionId == Faction.Modifier) continue;
                            // Remove Impostor Roles
                            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && roleInfo.FactionId != Faction.Impostor) continue;
                            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor && roleInfo.FactionId == Faction.Impostor) continue;
                            if (roleInfo.RoleId is RoleId.Cultist or RoleId.Follower) continue;

                            RoleManagerSelectRolesPatch.RoleAssignmentData roleData = RoleManagerSelectRolesPatch.GetRoleAssignmentData();
                            if (roleData.NeutralSettings.ContainsKey(roleInfo.RoleId) && roleData.NeutralSettings[roleInfo.RoleId] == 0) continue;
                            else if (roleData.NeutralKillerSettings.ContainsKey(roleInfo.RoleId) && roleData.NeutralKillerSettings[roleInfo.RoleId] == 0) continue;
                            else if (roleData.ImpSettings.ContainsKey(roleInfo.RoleId) && roleData.ImpSettings[roleInfo.RoleId] == 0) continue;
                            else if (roleData.CrewSettings.ContainsKey(roleInfo.RoleId) && roleData.CrewSettings[roleInfo.RoleId] == 0) continue;

                            if (roleInfo.FactionId == Faction.Crewmate && !roleData.CrewSettings.ContainsKey(roleInfo.RoleId)) continue;
                            if (roleInfo.FactionId == Faction.NK && !roleData.NeutralKillerSettings.ContainsKey(roleInfo.RoleId)) continue;
                            if (roleInfo.FactionId == Faction.Neutral && !roleData.NeutralSettings.ContainsKey(roleInfo.RoleId)) continue;

                            if (roleInfo.RoleId == RoleId.Spy && impostorCount < 2) continue;
                            if (alreadyPicked.Contains(roleInfo.RoleId)) continue;

                            int impsPicked = 0;
                            foreach (var picked in alreadyPicked)
                            {
                                if (RoleInfo.AllRoleInfos.Any(x => x.RoleId == picked && x.FactionId == Faction.Impostor))
                                    impsPicked++;
                            }

                            // Handle forcing of 100% roles for impostors
                            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor) 
                            {
                                int impsMax = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();
                                int impsMin = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
                                if (impsMin > impsMax) impsMin = impsMax;
                                int impsLeft = pickOrder.Where(x => Helpers.PlayerById(x).Data.Role.IsImpostor).Count();
                                int imps100 = roleData.ImpSettings.Where(x => x.Value == 10).Count();
                                if (imps100 > impsMax) imps100 = impsMax;
                                int imps100Picked = alreadyPicked.Where(x => roleData.ImpSettings.GetValueSafe(x) == 10).Count();
                                if (imps100 - imps100Picked >= impsLeft && !(roleData.ImpSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0)) continue;
                                if (impsMin - impsPicked >= impsLeft && roleInfo.RoleId == RoleId.Impostor) continue;
                                if (impsPicked >= impsMax && roleInfo.RoleId != RoleId.Impostor) continue;
                            }

                            // Player is no impostor! Handle forcing of 100% roles for crew and neutral
                            else 
                            {
                                // No more neutrals possible!
                                int neutralsPicked = 0;
                                int nKPicked = 0;
                                foreach (var picked in alreadyPicked)
                                {
                                    if (RoleInfo.AllRoleInfos.Any(x => x.RoleId == picked && x.FactionId == Faction.Neutral)) 
                                        neutralsPicked++;
                                    if (RoleInfo.AllRoleInfos.Any(x => x.RoleId == picked && x.FactionId == Faction.NK))
                                        nKPicked++;
                                }
                                int crewPicked = alreadyPicked.Count - impsPicked - neutralsPicked - nKPicked;
                                int neutralsMax = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
                                int neutralsMin = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
                                int neutrals100 = roleData.NeutralSettings.Where(x => x.Value == 10).Count();
                                if (neutrals100 > neutralsMin) neutralsMin = neutrals100;
                                if (neutralsMin > neutralsMax) neutralsMin = neutralsMax;

                                int nKMax = CustomOptionHolder.NeutralKillerRolesCountMax.GetSelection();
                                int nKMin = CustomOptionHolder.NeutralKillerRolesCountMin.GetSelection();
                                int nK100 = roleData.NeutralKillerSettings.Where(x => x.Value == 10).Count();
                                if (nK100 > nKMin) nKMin = nK100;
                                if (nKMin > nKMax) nKMin = nKMax;


                                // If crewmate fill disabled and crew picked the amount of allowed crewmates alreay: no more crewmate except vanilla crewmate allowed!
                                int crewLimit = PlayerControl.AllPlayerControls.Count - impostorCount - (neutralsMin > neutrals100 ? neutralsMin : neutrals100 > neutralsMax ? neutralsMax : neutrals100) - (nKMin > nK100 ? nKMin : nK100 > nKMax ? nKMax : nK100);
                                int maxCrew = crewLimit;
                                if (crewPicked >= crewLimit && roleInfo.FactionId != Faction.Neutral && roleInfo.RoleId != RoleId.Crewmate) continue;
                                bool allowAnyNeutral = false;

                                if (neutralsPicked >= neutralsMax && roleInfo.FactionId == Faction.Neutral) continue;
                                if (nKPicked >= nKMax && roleInfo.FactionId == Faction.NK) continue;

                                // More neutrals needed? Then no more crewmates! This takes precedence over crew roles set to 100%!
                                var crewmatesLeft = pickOrder.Count - pickOrder.Where(x => Helpers.PlayerById(x).Data.Role.IsImpostor).Count();

                                if ((crewmatesLeft <= neutralsMin - neutralsPicked && roleInfo.FactionId != Faction.Neutral) || (crewmatesLeft <= nKMin - nKPicked && roleInfo.FactionId != Faction.NK)) 
                                {
                                    continue;
                                } 
                                else if ((neutralsMin - neutrals100 > neutralsPicked) || (nKMin - nK100 > nKPicked))
                                    allowAnyNeutral = true;
                                // Handle 100% Roles PER Faction.

                                int neutrals100Picked = alreadyPicked.Where(x => roleData.NeutralSettings.GetValueSafe(x) == 10).Count();
                                int nK100Picked = alreadyPicked.Where(x => roleData.NeutralKillerSettings.GetValueSafe(x) == 10).Count();
                                int crew100 = roleData.CrewSettings.Where(x => x.Value == 10).Count();
                                int crew100Picked = alreadyPicked.Where(x => roleData.CrewSettings.GetValueSafe(x) == 10).Count();

                                if (neutrals100 > neutralsMax) neutrals100 = neutralsMax;
                                if (nK100 > nKMax) nK100 = nKMax;
                                if (crew100 > maxCrew) crew100 = maxCrew;

                                if ((neutrals100 - neutrals100Picked >= crewmatesLeft || roleInfo.FactionId == Faction.Neutral && neutrals100 - neutrals100Picked >= neutralsMax - neutralsPicked) && !(neutrals100Picked >= neutralsMax) && !(roleData.NeutralSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0)) continue;
                                if ((nK100 - nK100Picked >= crewmatesLeft || roleInfo.FactionId == Faction.NK && nK100 - nK100Picked >= nKMax - nKPicked) && !(nK100Picked >= nKMax) && !(roleData.NeutralKillerSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0)) continue;

                                if (!(allowAnyNeutral && (roleInfo.FactionId == Faction.Neutral || roleInfo.FactionId == Faction.NK)) && crew100 - crew100Picked >= crewmatesLeft && !(roleData.CrewSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0)) continue;

                                if (!(allowAnyNeutral && roleInfo.FactionId == Faction.Neutral) && neutrals100 + crew100 - neutrals100Picked - crew100Picked >= crewmatesLeft && !(roleData.CrewSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0 || roleData.NeutralSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0)) continue;

                                if (!(allowAnyNeutral && roleInfo.FactionId == Faction.NK) && nK100 + crew100 - nK100Picked - crew100Picked >= crewmatesLeft && !(roleData.CrewSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0 || roleData.NeutralSettings.Where(x => x.Value == 10 && x.Key == roleInfo.RoleId).Count() > 0)) continue;

                            }
                            // Handle role pairings that are blocked, e.g. Vampire Warlock, Cleaner Vulture etc.
                            bool blocked = false;
                            foreach (var blockedRoleId in CustomOptionHolder.BlockedRolePairings) 
                            {
                                if (alreadyPicked.Contains(blockedRoleId.Key) && blockedRoleId.Value.ToList().Contains(roleInfo.RoleId)) 
                                {
                                    blocked = true;
                                    break;
                                }
                            }
                            if (blocked) continue;

                            availableRoles.Add(roleInfo);
                        }

                        // Fallback for if all roles are somehow removed. (This is only the case if there is a bug, hence print a warning
                        if (availableRoles.Count == 0) 
                        {
                            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                                availableRoles.Add(RoleInfo.Impostor);
                            else
                                availableRoles.Add(RoleInfo.Crewmate);
                        }

                        List<RoleInfo> originalAvailable = new(availableRoles);

                        // remove some roles, so that you can't always get the same roles:
                        if (availableRoles.Count > CustomOptionHolder.draftModeAmountOfChoices.GetFloat()) 
                        {
                            int countToRemove = availableRoles.Count - (int)CustomOptionHolder.draftModeAmountOfChoices.GetFloat();
                            while (countToRemove-- > 0) {
                                var toRemove = availableRoles.OrderBy(_ => Guid.NewGuid()).First();
                                availableRoles.Remove(toRemove);
                            }
                        }

                        if (timer >= maxTimer) 
                        {
                            sendPick((byte)originalAvailable.OrderBy(_ => Guid.NewGuid()).First().RoleId, true);
                        }


                        if (GameObject.Find("RoleButton") == null) 
                        {
                            int i = 0;
                            int a = 0;
                            int buttonsPerRow = 3;
                            int lastRow = availableRoles.Count / buttonsPerRow;
                            int buttonsInLastRow = availableRoles.Count % buttonsPerRow;

                            var buttonTemplate = HudManager.Instance.SettingsButton.transform;
                            TextMeshPro textTemplate = HudManager.Instance.TaskPanel.taskText;

                            foreach (RoleInfo roleInfo in availableRoles) 
                            {
                                if (a == buttonsPerRow) a = 0;
                                float row = i / buttonsPerRow;
/*                                float col = i % buttonsPerRow;
                                if (buttonsInLastRow != 0 && row == lastRow) 
                                {
                                    col += (buttonsPerRow - buttonsInLastRow) / 2f;
                                }*/
                                // planned rows: maximum of 4, hence the following calculation for rows as well:
                                row += (4 - lastRow - 1) / 2f;

                                var RoleButton = Object.Instantiate(buttonTemplate, __instance.TeamTitle.transform);
                                RoleButton.GetComponent<AspectPosition>().Destroy();
                                RoleButton.transform.position = __instance.TeamTitle.transform.position;
                                RoleButton.name = "RoleButton";
                                RoleButton.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
                                RoleButton.transform.localScale = new Vector3(3f, 3f);
                                var ActiveSprite = RoleButton.FindChild("Active").GetComponent<SpriteRenderer>();
                                var InactiveSprite = RoleButton.FindChild("Inactive").GetComponent<SpriteRenderer>();
                                RoleButton.FindChild("Background").gameObject.active = false;
                                ActiveSprite.sprite = HelpMenu.GetPlateSprite();
                                InactiveSprite.sprite = HelpMenu.GetPlateSprite();
                                ActiveSprite.color = Color.green;

                                RoleButton.transform.localPosition = new Vector3(FloatRange.SpreadToEdges(-8, 8, a, 3), -7.5f - row * 2.25f);
                                a++;

                                TextMeshPro text = Object.Instantiate(textTemplate, RoleButton.transform);
                                text.text = Helpers.ColorString(roleInfo.Color, roleInfo.Name);
                                text.alignment = TextAlignmentOptions.Center;
                                text.transform.localPosition = new Vector3(0, 0, -1);
                                text.transform.localScale = new Vector3(1.8f, 1.8f, 1f);

                                PassiveButton button = RoleButton.GetComponent<PassiveButton>();
                                button.OnClick.RemoveAllListeners();
                                button.OnClick = new Button.ButtonClickedEvent();

                                button.OnClick.AddListener((Action)(() => 
                                {
                                    sendPick((byte)roleInfo.RoleId, false);
                                }));
                                buttons.Add(RoleButton);
                                i++;
                            }

                            var RandomRoleButton = Object.Instantiate(buttonTemplate, __instance.TeamTitle.transform);
                            RandomRoleButton.GetComponent<AspectPosition>().Destroy();
                            RandomRoleButton.transform.position = __instance.TeamTitle.transform.position;
                            RandomRoleButton.name = "RoleButton";
                            RandomRoleButton.GetComponent<BoxCollider2D>().size = new Vector2(2.5f, 0.55f);
                            RandomRoleButton.transform.localScale = new Vector3(3f, 3f);
                            var RActiveSprite = RandomRoleButton.FindChild("Active").GetComponent<SpriteRenderer>();
                            var RInactiveSprite = RandomRoleButton.FindChild("Inactive").GetComponent<SpriteRenderer>();
                            RandomRoleButton.FindChild("Background").gameObject.active = false;
                            RActiveSprite.sprite = HelpMenu.GetPlateSprite();
                            RInactiveSprite.sprite = HelpMenu.GetPlateSprite();
                            RActiveSprite.color = Color.green;

                            RandomRoleButton.transform.localPosition = new Vector3(0, -8f);
                            a++;

                            TextMeshPro Rtext = Object.Instantiate(textTemplate, RandomRoleButton.transform);
                            Rtext.text = Helpers.ColorString(Palette.AcceptedGreen, "Random");
                            Rtext.alignment = TextAlignmentOptions.Center;
                            Rtext.transform.localPosition = new Vector3(0, 0, -1);
                            Rtext.transform.localScale = new Vector3(1.8f, 1.8f, 1f);

                            PassiveButton Rbutton = RandomRoleButton.GetComponent<PassiveButton>();
                            Rbutton.OnClick.RemoveAllListeners();
                            Rbutton.OnClick = new Button.ButtonClickedEvent();

                            Rbutton.OnClick.AddListener((Action)(() =>
                            {
                                sendPick((byte)originalAvailable.OrderBy(_ => Guid.NewGuid()).First().RoleId, true);
                            }));
                            buttons.Add(RandomRoleButton);
                        }

                    } 
                    else 
                    {
                        int currentPick = PlayerControl.AllPlayerControls.Count - pickOrder.Count + 1;
                        playerText = $"Player {currentPick}";
                        HudManager.Instance.FullScreen.color = Color.black;
                    }
                    __instance.TeamTitle.text = $"{Helpers.ColorString(Color.red, "<size=280%>Draft Mode</size>")}\n\n\n<size=200%> Currently Picking:</size>\n\n\n<size=250%>{playerText}</size>";
                    int waitMore = pickOrder.IndexOf(PlayerControl.LocalPlayer.PlayerId);
                    string waitMoreText = "";
                    if (waitMore > 0) 
                    {
                        waitMoreText = $" (Your turn in {waitMore})";
                    }
                    __instance.TeamTitle.text += $"\n\n{waitMoreText}\nRandom Selection In... {(int)(maxTimer + 1 - timer)}";
                    yield return null;
                }
            }
            HudManager.Instance.FullScreen.color = Color.black;
            __instance.FrontMost.gameObject.SetActive(true);
            GameObject.Find("BackgroundLayer")?.SetActive(true);
            if (AmongUsClient.Instance.AmHost)
            {
                RoleManagerSelectRolesPatch.AssignRoleTargets(); // Assign targets
                RoleManagerSelectRolesPatch.AssignAssassins();
                RoleManagerSelectRolesPatch.AssignModifiers(); // Assign modifier
            }

            float myTimer = 0f;
            while (myTimer < 3f)
            {
                myTimer += Time.deltaTime;
                Color c = new Color(0, 0, 0, myTimer / 3.0f);
                __instance.FrontMost.color = c;
                yield return null;
            }

            SoundEffectsManager.Stop(Sounds.Draft);
            isRunning = false;
            yield break;
        }

        public static void receivePick(byte playerId, byte roleId, bool random)
        {
            if (!isEnabled) return;
            RPCProcedure.SetRole((RoleId)roleId, Helpers.PlayerById(playerId));
            alreadyPicked.Add((RoleId)roleId);
            try
            {
                pickOrder.Remove(playerId);
                timer = 0;
                picked = true;
                RoleInfo roleInfo = RoleInfo.AllRoleInfos.First(x => (byte)x.RoleId == roleId);
                string roleString = Helpers.ColorString(roleInfo.Color, roleInfo.Name);
                int roleLength = roleInfo.Name.Length;  // Not used for now, but stores the amount of charactes of the roleString.
                if (!CustomOptionHolder.draftModeShowRoles.GetBool())
                {
                    if (random)
                    {
                        roleString = Helpers.ColorString(Palette.AcceptedGreen, "Random");
                        roleLength = roleString.Length;
                    }
                    else
                    {
                        if (playerId != PlayerControl.LocalPlayer.PlayerId)
                        {
                            if (roleInfo.FactionId == Faction.Impostor)
                            {
                                roleString = Helpers.ColorString(Palette.ImpostorRed, "Impostor");
                                roleLength = roleString.Length;
                            }
                            else if (roleInfo.FactionId == Faction.Crewmate)
                            {
                                roleString = Helpers.ColorString(Palette.CrewmateBlue, "Crewmate");
                                roleLength = roleString.Length;
                            }
                            else
                            {
                                roleString = Helpers.ColorString(Color.gray, "Neutral");
                                roleLength = roleString.Length;
                            }
                        }
                    }
                }
                else if (CustomOptionHolder.draftModeHideImpRoles.GetBool() && roleInfo.FactionId == Faction.Impostor && !(playerId == PlayerControl.LocalPlayer.PlayerId))
                {
                    roleString = Helpers.ColorString(Palette.ImpostorRed, "Impostor Role");
                    roleLength = "Impostor".Length;
                }
                else if (CustomOptionHolder.draftModeHideNeutralRoles.GetBool() && roleInfo.FactionId == Faction.Neutral && !(playerId == PlayerControl.LocalPlayer.PlayerId))
                {
                    roleString = Helpers.ColorString(Color.gray, "Neutral Role");
                    roleLength = "Neutral".Length;
                }
                string line = $"{(playerId == PlayerControl.LocalPlayer.PlayerId ? "You" : alreadyPicked.Count)}:";
                line = line + string.Concat(Enumerable.Repeat(" ", 6 - line.Length)) + roleString;
                feedText.text += line + "\n";
                //SoundEffectsManager.Play("select");
            }

            catch (Exception e) { StellarRolesPlugin.Logger.LogError(e); }
        }

        public static void sendPick(byte RoleId, bool random)
        {
            //SoundEffectsManager.Stop("timeMasterShield");

            RPCProcedure.Send(CustomRPC.DraftModePick, PlayerControl.LocalPlayer.PlayerId, RoleId, random);
            receivePick(PlayerControl.LocalPlayer.PlayerId, RoleId, random);

            // destroy all the buttons:
            foreach (var button in buttons)
            {
                button?.gameObject?.Destroy();
            }
            buttons.Clear();
        }


        public static void sendPickOrder()
        {
            pickOrder = PlayerControl.AllPlayerControls.ToArray().Select(x => x.PlayerId).OrderBy(_ => Guid.NewGuid()).ToList().ToList();
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 254, SendOption.Reliable, -1);
            writer.Write((byte)CustomRPC.DraftModePickOrder);
            writer.Write((byte)pickOrder.Count);
            foreach (var item in pickOrder)
            {
                writer.Write(item);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }


        public static void receivePickOrder(int amount, MessageReader reader)
        {
            pickOrder.Clear();
            for (int i = 0; i < amount; i++)
            {
                pickOrder.Add(reader.ReadByte());
            }
        }


        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowTeam))]

        class ShowRolePatch
        {
            [HarmonyPostfix]
            public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
            {
                if (!isEnabled) return;
                var newEnumerator = new Helpers.PatchedEnumerator()
                {
                    enumerator = __result.WrapToManaged(),
                    Postfix = CoSelectRoles(__instance)
                };
                __result = newEnumerator.GetEnumerator().WrapToIl2Cpp();
            }

        }
    }
}
