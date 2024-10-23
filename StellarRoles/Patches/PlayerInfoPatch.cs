using HarmonyLib;
using StellarRoles.Utilities;
using System.Linq;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class PlayerInfoPatch
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || Helpers.IsHideAndSeek) return;

            try
            {
                UpdatePlayerInfo();
            }
            catch { }
        }

        private static Vector3 colorBlindTextMeetingInitialLocalPos = new(0.3384f, -0.16666f, -0.01f);
        private static Vector3 colorBlindTextMeetingInitialLocalScale = new(0.9f, 1f, 1f);

        public static void UpdatePlayerInfo()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            for (int i = 0; i < GameData.Instance?.AllPlayers.Count; i++)
            {
                string meetingInfoText = "";
                string playerInfoText = "";

                var data = GameData.Instance.AllPlayers[i];
                PlayerControl player = null;
                if (data && data.Object != null)
                {
                    player = data.Object;
                }

                if (player != null)
                {
                    (int tasksCompleted, int tasksTotal) = TasksHandler.TaskInfo(data);
                    string roleText = RoleInfo.GetRolesString(player, true, false);
                    string deadRoleText = RoleInfo.GetRolesString(player, true, MapOptions.GhostsSeeModifier);
                    string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";
                    string taskPanelInfo = tasksTotal > 0 ? "Tasks " + taskInfo : Helpers.ColorString(Palette.ImpostorRed, "Fake Tasks");

                    TMPro.TextMeshPro playerInfo = player.cosmetics.nameText.transform.parent.FindChild("Info")?.GetComponent<TMPro.TextMeshPro>();
                    if (playerInfo == null)
                    {
                        playerInfo = Object.Instantiate(player.cosmetics.nameText, player.cosmetics.nameText.transform.parent);
                        playerInfo.transform.localPosition += Vector3.up * 0.225f;
                        playerInfo.fontSize *= 0.85f;
                        playerInfo.gameObject.name = "Info";
                    }


                    // Colorblind Text During the round
                    if (player.cosmetics.colorBlindText != null && player.cosmetics.showColorBlindText && player.cosmetics.colorBlindText.gameObject.active)
                    {
                        player.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1.2f, 0f);
                    }

                    player.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);  // This moves both the name AND the colorblindtext behind objects (if the player is behind the object), like the rock on polus

                    bool qualifiedRomantic = localPlayer == Romantic.Player && player == Romantic.Lover && (Romantic.KnowsRoleInfoImmediately || !Romantic.Lover.Data.IsDead) && !Romantic.Player.Data.IsDead;
                    bool qualifiedVengefulRomantic = localPlayer == VengefulRomantic.Player && player == VengefulRomantic.Lover && !VengefulRomantic.Lover.Data.IsDead;
                    bool qualifiedExecutioner = localPlayer == Executioner.Player && player == Executioner.Target && Ascended.IsAscended(localPlayer);
                    //bool qualifiedCultist = localPlayer == Cultist.Player && player == Follower.Player && Cultist.FollowerSpecialRoleAssigned && !Follower.Player.Data.IsDead;

                    if ((MapOptions.ShowRoles && MapOptions.ToggleRoles) || !MapOptions.ToggleRoles)
                    {
                        if (player.AmOwner)
                        {
                            roleText = data.IsDead ? deadRoleText : roleText;
                            if (player == Scavenger.Player)
                                playerInfoText = roleText + Helpers.ColorString(Scavenger.Color, $" ({Scavenger.BodiesRemainingToWin()})");
                            else if (player.IsJailor(out Jailor jailor))
                                playerInfoText = roleText + Helpers.ColorString(Jailor.Color, $" ({jailor.Charges})");
                            else
                                playerInfoText = roleText;
                            if (HudManager.Instance.TaskPanel != null)
                            {
                                TMPro.TextMeshPro tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                                tabText.SetText(taskPanelInfo);
                            }
                            meetingInfoText = $"{roleText} {taskInfo}".Trim();
                        }
                        else if (qualifiedRomantic || qualifiedVengefulRomantic)
                        {
                            if (Romantic.RomanticKnowsRole)
                                playerInfoText = meetingInfoText = roleText;
                            else if (Helpers.IsNeutral(player))
                                playerInfoText = meetingInfoText = "<color=#333333>Neutral</color>";
                            else if (player.Data.Role.IsImpostor)
                                playerInfoText = meetingInfoText = "<color=#d5000b>Imposter</color>";
                            else
                                playerInfoText = meetingInfoText = "<color=#3b98c7>Crewmate</color>";
                        }
                        else if (qualifiedExecutioner /*|| qualifiedCultist*/)
                        {
                            playerInfoText = meetingInfoText = roleText;

                        }
                        else if (localPlayer.Data.IsDead)
                        {
                            if (MapOptions.GhostsSeeRoles && MapOptions.GhostsSeeTasks)
                            {
                                if (player == Scavenger.Player)
                                    meetingInfoText = playerInfoText = roleText + Helpers.ColorString(Scavenger.Color, $" ({Scavenger.BodiesRemainingToWin()})");
                                else
                                    meetingInfoText = playerInfoText = $"{deadRoleText} {taskInfo}".Trim();
                            }
                            else if (MapOptions.GhostsSeeTasks)
                                meetingInfoText = playerInfoText = taskInfo.Trim();
                            else if (MapOptions.GhostsSeeRoles)
                            {
                                if (player == Scavenger.Player)
                                    meetingInfoText = playerInfoText = roleText + Helpers.ColorString(Scavenger.Color, $" ({Scavenger.BodiesRemainingToWin()})");
                                else
                                    meetingInfoText = playerInfoText = deadRoleText;
                            }
                        }

                        if (playerInfo != null)
                        {
                            playerInfo.text = playerInfoText;
                            playerInfo.gameObject.SetActive(player != null && !Helpers.ShouldHidePlayerName(player));
                        }
                    }
                }

                var playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == data.PlayerId);

                if (playerVoteArea != null)
                {
                    var meetingInfo = playerVoteArea.NameText.transform.parent.FindChild("Info")?.GetComponent<TMPro.TextMeshPro>();
                    if (meetingInfo == null)
                    {
                        meetingInfo = Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                        meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                        meetingInfo.fontSize *= 0.8f;
                        meetingInfo.gameObject.name = "Info";
                    }

                    if (playerVoteArea.ColorBlindName.gameObject.active)
                    {
                        // Colorblind Text in Meeting
                        playerVoteArea.ColorBlindName.alignment = TMPro.TextAlignmentOptions.Left;
                        playerVoteArea.ColorBlindName.transform.localPosition = colorBlindTextMeetingInitialLocalPos + new Vector3(-.41f, -.1f, 0f);
                        playerVoteArea.ColorBlindName.transform.localScale = colorBlindTextMeetingInitialLocalScale * 1.2f;
                    }
                    meetingInfo.text = "";

                    if (meetingInfo != null)
                    {
                        meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
                        // Set player name higher to align in middle
                        if (playerVoteArea != null && meetingInfoText != "")
                        {
                            playerVoteArea.NameText.transform.localPosition = new Vector3(0.3384f, 0.08f, -0.1f);
                        }
                    }
                }
            }
        }
    }
}
