using Cpp2IL.Core.Extensions;
using HarmonyLib;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;
namespace StellarRoles.Patches
{
    [HarmonyPatch]
    class MeetingHudPatch
    {
        private static readonly Dictionary<byte, bool> Selections = new();
        private static readonly Dictionary<byte, SpriteRenderer> Renderers = new();
        private static readonly Dictionary<byte, PassiveButton> Buttons = new();

        private static TextMeshPro JailorChargesText;
        private static TextMeshPro JailorConfirmButtonLabel;

        private static TextMeshPro MayorRetireButtonLabel;
        private static bool MayorClicked = false;


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
        class MeetingCalculateVotesPatch
        {
            private static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
            {
                Dictionary<byte, int> dictionary = new();
                for (int i = 0; i < __instance.playerStates.Length; i++)
                {

                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (playerVoteArea.VotedFor != 252 && playerVoteArea.VotedFor != 255 && playerVoteArea.VotedFor != 254)
                    {

                        PlayerControl player = Helpers.PlayerById(playerVoteArea.TargetPlayerId);
                        PlayerControl voted = Helpers.PlayerById(playerVoteArea.VotedFor);
                        if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                            continue;


                        bool notjailed = !Jailor.IsJailorTarget(player) && !Jailor.IsJailorTarget(voted);
                        bool isMayor = Mayor.Player != null && Mayor.Player.PlayerId == playerVoteArea.TargetPlayerId && !Mayor.Retired;

                        int additionalVotes = (isMayor && notjailed) ? 2 : 1; // Mayor vote
                        if (dictionary.TryGetValue(playerVoteArea.VotedFor, out int currentVotes))
                            dictionary[playerVoteArea.VotedFor] = currentVotes + additionalVotes;
                        else
                            dictionary[playerVoteArea.VotedFor] = additionalVotes;
                    }
                }
                return dictionary;
            }


            static bool Prefix(MeetingHud __instance)
            {
                if (__instance.playerStates.All((PlayerVoteArea ps) => ps.AmDead || ps.DidVote))
                {
                    Dictionary<byte, int> self = CalculateVotes(__instance);
                    KeyValuePair<byte, int> max = self.MaxPair(out bool tie);
                    GameData.PlayerInfo exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == max.Key && !v.IsDead);

                    MeetingHud.VoterState[] array = new MeetingHud.VoterState[__instance.playerStates.Length];
                    for (int i = 0; i < __instance.playerStates.Length; i++)
                    {
                        PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                        array[i] = new MeetingHud.VoterState
                        {
                            VoterId = playerVoteArea.TargetPlayerId,
                            VotedForId = playerVoteArea.VotedFor
                        };
                    }

                    // RPCVotingComplete
                    __instance.RpcVotingComplete(array, exiled, tie);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
        class MeetingHudBloopAVoteIconPatch
        {
            public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] GameData.PlayerInfo voterPlayer, [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
            {
                SpriteRenderer spriteRenderer = Object.Instantiate(__instance.PlayerVotePrefab);
                int cId = voterPlayer.DefaultOutfit.ColorId;

                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                bool isJailed = Jailor.IsJailorTarget(localPlayer);
                bool isMayor = Mayor.Player != null && Mayor.Player.PlayerId == localPlayer.PlayerId;
                bool mayorCompletedEnoughTasks = TasksHandler.TaskInfo(localPlayer.Data).Item1 >= Mayor.TasksNeededForVoteColors() && Mayor.CanSeeVoteColorsInMeeting;
                bool verifiedMayor = isMayor && !isJailed && mayorCompletedEnoughTasks;
                bool anonymousVotes = GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes;
                bool deadPeopleSeeVotes = localPlayer.Data.IsDead && MapOptions.GhostsSeeVotes;

                if (anonymousVotes && !deadPeopleSeeVotes && !verifiedMayor)
                    voterPlayer.Object.SetColor(6);

                voterPlayer.Object.SetPlayerMaterialColors(spriteRenderer);
                spriteRenderer.transform.SetParent(parent);
                spriteRenderer.transform.localScale = Vector3.zero;

                __instance.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
                parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
                voterPlayer.Object.SetColor(cId);
                return false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
        class MeetingHudPopulateVotesPatch
        {
            static bool Prefix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
            {

                __instance.TitleText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                int num = 0;

                for (int i = 0; i < __instance.playerStates.Length; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    byte targetPlayerId = playerVoteArea.TargetPlayerId;
                    playerVoteArea.ClearForResults();
                    int num2 = 0;
                    bool mayorFirstVoteDisplayed = false;
                    for (int j = 0; j < states.Length; j++)
                    {
                        MeetingHud.VoterState voterState = states[j];
                        GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(voterState.VoterId);
                        if (playerById == null)
                        {
                            Debug.LogError(string.Format("Couldn't find player info for voter: {0}", voterState.VoterId));
                        }
                        else if (i == 0 && voterState.SkippedVote && !playerById.IsDead)
                        {
                            __instance.BloopAVoteIcon(playerById, num, __instance.SkippedVoting.transform);
                            num++;
                        }
                        else if (voterState.VotedForId == targetPlayerId && !playerById.IsDead)
                        {
                            if (SpitefulExtensions.IsSpiteful(targetPlayerId, out Spiteful spiteful))
                            {
                                RPCProcedure.Send(CustomRPC.SpitefulVote, targetPlayerId, voterState.VoterId);
                                // Is voterState.VoterId just the player whos vote area it is? need to investigate
                                spiteful.VotedBy.Add(Helpers.PlayerById(voterState.VoterId));
                            }
                            __instance.BloopAVoteIcon(playerById, num2, playerVoteArea.transform);
                            num2++;
                        }

                        PlayerControl targetPlayer = Helpers.PlayerById(targetPlayerId);

                        // Major vote, redo this iteration to place a second vote
                        bool isMayor = Mayor.Player != null && voterState.VoterId == (sbyte)Mayor.Player.PlayerId && !Mayor.Retired;
                        bool isTargetFree = !Jailor.IsJailorTarget(targetPlayer);
                        bool isMayorFree = !Jailor.IsJailorTarget(Mayor.Player);

                        if (isMayor && isTargetFree && isMayorFree && !mayorFirstVoteDisplayed)
                        {
                            mayorFirstVoteDisplayed = true;
                            j--;
                        }
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        class MeetingHudVotingCompletedPatch
        {
            static void Postfix([HarmonyArgument(1)] GameData.PlayerInfo exiled)
            {
                //Refugee save next to be exiled, because RPC of ending game comes before RPC of exiled
                foreach (Refugee refugee in Refugee.PlayerToRefugee.Values)
                {
                    refugee.NotAckedExiled = false;
                    if (exiled != null)
                        refugee.NotAckedExiled = refugee.Player != null && refugee.Player.PlayerId == exiled.PlayerId;
                }
            }
        }

        static void JailorOnClick(PlayerVoteArea voteArea, Jailor jailor)
        {
            MeetingHud meetingHud = MeetingHud.Instance;
            if (meetingHud.state == MeetingHud.VoteStates.Results || jailor.Charges <= 0 || voteArea.AmDead)
                return;

            int selectedCount = Selections.Count(pair => pair.Value);
            byte index = voteArea.TargetPlayerId;
            if (!Renderers.TryGetValue(index, out SpriteRenderer renderer))
            {
                Helpers.Log(LogLevel.Warning, $"(JailorOnClick): No SpriteRenderer for {index} ({Helpers.PlayerById(index).Data.PlayerName})");
                return;
            }
            if (!Selections.TryGetValue(index, out bool selected))
            {
                Helpers.Log(LogLevel.Warning, $"(JailorOnClick): No Selection for {index} ({Helpers.PlayerById(index).Data.PlayerName})");
                return;
            }
            //0 is hovering?
            switch (selectedCount)
            {
                case 0:
                    renderer.color = Color.yellow;
                    Selections[index] = true;
                    break;
                case 1:
                    if (selected)
                    {
                        renderer.color = Color.red;
                        Selections[index] = false;
                        JailorConfirmButtonLabel.text = Helpers.ColorString(Color.red, "Confirm Jailed");
                    }
                    break;
            }
        }

        static void JailorConfirm(Jailor jailor)
        {
            MeetingHud meetingHud = MeetingHud.Instance;
            if (
                meetingHud.state == MeetingHud.VoteStates.Results ||
                // TODO: this `selections` field seems.... weird
                Selections.Count(pair => pair.Value) != 1 ||
                jailor.Charges <= 0 || jailor.HasJailed
            )
                return;



            PlayerVoteArea firstPlayer = null;
            foreach ((byte index, bool selected) in Selections)
            {
                if (!Renderers.TryGetValue(index, out SpriteRenderer renderer))
                {
                    Helpers.Log(LogLevel.Warning, $"(JailorConfirm): No Selection for {index} ({Helpers.PlayerById(index).Data.PlayerName})");
                    continue;
                }
                if (!Buttons.TryGetValue(index, out PassiveButton button))
                {
                    Helpers.Log(LogLevel.Warning, $"(JailorConfirm): No Selection for {index} ({Helpers.PlayerById(index).Data.PlayerName})");
                    continue;
                }
                if (selected)
                {
                    firstPlayer ??= meetingHud.playerStates.First(voteArea => voteArea.TargetPlayerId == index);
                    renderer.color = new Color(1f, 1f, 1f);
                }
                else
                    renderer.color = new Color(1f, 1f, 1f, 0f);

                button.OnClick.RemoveAllListeners(); // Jailor buttons can't be clicked / changed anymore
            }
            if (firstPlayer != null)
            {
                jailor.Charges--;
                RPCProcedure.Send(CustomRPC.JailorJail, jailor.Player.PlayerId, firstPlayer.TargetPlayerId);
                RPCProcedure.JailorJail(jailor, Helpers.PlayerById(firstPlayer.TargetPlayerId));

                JailorConfirmButtonLabel.text = Helpers.ColorString(Jailor.Color, "Jailed!");
                JailorChargesText.text = $"Jails: {jailor.Charges}";
            }
        }

        static void MayorRetire()
        {
            if (Mayor.Retired) return;

            MeetingHud meetingHud = MeetingHud.Instance;
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            PlayerVoteArea playerVoteArea = meetingHud.playerStates.First(x => x.TargetPlayerId == localPlayer.PlayerId);
            if (playerVoteArea.DidVote) return;

            if (MayorClicked)
            {
                MayorRetireButtonLabel.text = Helpers.ColorString(Color.red, "Retired");
                RPCProcedure.Send(CustomRPC.MayorRetire);
                Mayor.Retired = true;
            }
            else
            {
                MayorRetireButtonLabel.text = Helpers.ColorString(Color.red, "Are You Sure?");
                MayorClicked = true;
            }
        }


        public static GameObject GuesserUI;
        public static PassiveButton GuesserUIExitButton;
        static void GuesserOnClick(PlayerControl target)
        {
            MeetingHud meetingHud = MeetingHud.Instance;
            if (GuesserUI != null || !(meetingHud.state == MeetingHud.VoteStates.Voted || meetingHud.state == MeetingHud.VoteStates.NotVoted))
                return;
            if (Jailor.IsJailorTarget(target) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            foreach (PlayerVoteArea playerVoteArea in meetingHud.playerStates)
                playerVoteArea.gameObject.SetActive(false);

            Transform phoneUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
            Transform container = Object.Instantiate(phoneUI, meetingHud.transform);
            container.transform.localPosition = new Vector3(0, 0, -5f);
            GuesserUI = container.gameObject;

            PlayerVoteArea templateVoteArea = meetingHud.playerStates[0];
            Transform buttonTemplate = templateVoteArea.transform.FindChild("votePlayerBase");
            Transform maskTemplate = templateVoteArea.transform.FindChild("MaskArea");
            Transform smallButtonTemplate = templateVoteArea.Buttons.transform.Find("CancelButton");
            TextMeshPro textTemplate = templateVoteArea.NameText;

            Transform exitButtonParent = new GameObject("ExitGuesserMenu").transform;
            exitButtonParent.SetParent(container);
            Transform exitButton = Object.Instantiate(smallButtonTemplate.transform, exitButtonParent);
            exitButton.GetChild(0).transform.localPosition = new(0f, 0f, 0.1f);
            Transform exitButtonMask = Object.Instantiate(maskTemplate, exitButtonParent);
            exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
            exitButtonParent.transform.localPosition = new Vector3(2.725f, 2.1f, -5);
            exitButton.transform.localPosition = new Vector3(0f, 0f, 0f);
            GuesserUIExitButton = exitButton.GetComponent<PassiveButton>();
            GuesserUIExitButton.OnClick.RemoveAllListeners();
            GuesserUIExitButton.OnMouseOver.RemoveAllListeners();
            GuesserUIExitButton.OnMouseOut.RemoveAllListeners();
            GuesserUIExitButton.OnClick.AddListener((Action)(() =>
            {
                Object.Destroy(container.gameObject);
                foreach (PlayerVoteArea playerVoteArea in meetingHud.playerStates)
                {
                    if (Spectator.IsSpectator(playerVoteArea.TargetPlayerId)) continue;

                    playerVoteArea.gameObject.SetActive(true);
                    Transform shootButton = playerVoteArea.transform.FindChild("ShootButton");
                    if (shootButton != null && PlayerControl.LocalPlayer.Data.IsDead)
                        Object.Destroy(shootButton.gameObject);
                }
                buttonTemplate.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            }));
            GuesserUIExitButton.OnMouseOver.AddListener((Action)(() =>
                exitButton.GetChild(0).GetComponent<SpriteRenderer>().enabled = true
            ));
            GuesserUIExitButton.OnMouseOut.AddListener((Action)(() =>
                exitButton.GetChild(0).GetComponent<SpriteRenderer>().enabled = false
            ));

            List<Transform> buttons = new();
            Transform selectedButton = null;

            RoleId guesserRole = PlayerControl.LocalPlayer == Vigilante.Player
                ? RoleId.Vigilante
                : (PlayerControl.LocalPlayer.IsNeutralKiller() ? RoleId.NKAssassin : RoleId.Assassin);

            int i = 0;
            void AddButton(RoleInfo roleInfo, int buttonCount)
            {
                Transform buttonParent = new GameObject($"GuessRole{roleInfo.Name}").transform;
                buttonParent.SetParent(container);
                Transform buttonTransform = Object.Instantiate(buttonTemplate, buttonParent);
                Transform buttonMask = Object.Instantiate(maskTemplate, buttonParent);
                TextMeshPro label = Object.Instantiate(textTemplate, buttonTransform);
                buttonTransform.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
                buttons.Add(buttonTransform);
                int row = buttonCount / 5, col = buttonCount % 5;
                buttonParent.localPosition = new Vector3(-3.47f + 1.75f * col, 1.5f - 0.45f * row, -5);
                buttonParent.localScale = new Vector3(0.6f, 0.6f, 1f);
                label.text = Helpers.ColorString(roleInfo.Color, roleInfo.Name);
                label.alignment = TextAlignmentOptions.Center;
                label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                label.transform.localScale *= 1.55f;

                PassiveButton button = buttonTransform.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                if (PlayerControl.LocalPlayer.Data.IsDead)
                    return;
                button.OnClick.AddListener((Action)(() =>
                {
                    if (selectedButton != buttonTransform)
                    {
                        selectedButton = buttonTransform;
                        buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                    }
                    else
                    {
                        if (!(meetingHud.state == MeetingHud.VoteStates.Voted || meetingHud.state == MeetingHud.VoteStates.NotVoted) || target == null || Helpers.RemainingShots(PlayerControl.LocalPlayer) <= 0)
                            return;

                        RoleInfo mainRoleInfo = RoleInfo.GetRoleInfoForPlayer(target, false).FirstOrDefault();
                        if (mainRoleInfo == null)
                            return;

                        PlayerControl dyingTarget = mainRoleInfo == roleInfo ? target : PlayerControl.LocalPlayer;

                        // Reset the GUI
                        meetingHud.playerStates.ToList().ForEach(x =>
                        {
                            if (!Spectator.IsSpectator(x.TargetPlayerId))
                                x.gameObject.SetActive(true);
                        });
                        Object.Destroy(container.gameObject);


                        if (Helpers.RemainingShots(PlayerControl.LocalPlayer) > 1 && dyingTarget != PlayerControl.LocalPlayer && (
                            (guesserRole == RoleId.Vigilante && !Vigilante.LimitVigiOneShotPerMeeting) ||
                            (guesserRole == RoleId.Assassin && !Assassin.AssassinLimitOneShotPerMeeting) ||
                            (guesserRole == RoleId.NKAssassin && !Assassin.NeutralKillerAssassinLimitOneShotPerMeeting)
                        ))
                        {
                            PlayerVoteArea state = meetingHud.playerStates.First(x => x.TargetPlayerId == dyingTarget.PlayerId);
                            Transform shootButton = state.transform.FindChild("ShootButton");
                            if (state.TargetPlayerId == dyingTarget.PlayerId && shootButton != null)
                                Object.Destroy(shootButton.gameObject);
                        }
                        else
                            foreach (PlayerVoteArea voteArea in meetingHud.playerStates)
                            {
                                Transform shootButton = voteArea.transform.FindChild("ShootButton");
                                if (shootButton != null)
                                    Object.Destroy(shootButton.gameObject);
                            }

                        // Shoot player and send chat info if activated
                        RPCProcedure.Send(CustomRPC.GuesserShoot, PlayerControl.LocalPlayer.PlayerId, dyingTarget.PlayerId, target.PlayerId, (byte)roleInfo.RoleId);
                        RPCProcedure.GuesserShoot(PlayerControl.LocalPlayer, dyingTarget, target, roleInfo.RoleId);

                        if (dyingTarget.IsJailor(out Jailor jailor) && jailor.HasJailed)
                        {
                            RPCProcedure.Send(CustomRPC.ReadButtons);
                            RPCProcedure.ReadButtons();
                        }
                    }
                }));
                button.OnMouseOver.AddListener((Action)(() =>
                    buttonTransform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true
                ));
                button.OnMouseOut.AddListener((Action)(() =>
                    buttonTransform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false
                ));
            }

            byte playerId = PlayerControl.LocalPlayer.PlayerId;

            List<RoleInfo> impRoles = new();
            List<RoleInfo> crewRoles = new();
            List<RoleInfo> neutralRoles = new();
            List<RoleInfo> neutralKRoles = new();

            if (Jailor.IsJailorTarget(PlayerControl.LocalPlayer))
                while (i < 45)
                    AddButton(RoleInfo.Jailor, i++);
            else
            {
                List<RoleInfo> roleInfos = RoleInfo.AllRoleInfos.Clone();
                roleInfos.Sort((i1, i2) => string.Compare(i1.Name, i2.Name));

                foreach (RoleInfo roleInfo in RoleInfo.AllRoleInfos)
                {
                    // Remove roles that cannot spawn, the guesser role, and Romantic if assassin
                    if (Helpers.CantGuess(roleInfo) || roleInfo.RoleId == guesserRole || (
                        guesserRole == RoleId.Assassin && roleInfo.RoleId == RoleId.Romantic
                     ))
                        continue;

                    switch (roleInfo.FactionId)
                    {
                        case Faction.Crewmate:
                            crewRoles.Add(roleInfo);
                            break;
                        case Faction.Neutral:
                            neutralRoles.Add(roleInfo);
                            break;
                        case Faction.NK:
                            neutralKRoles.Add(roleInfo);
                            break;
                        case Faction.Impostor:
                            impRoles.Add(roleInfo);
                            break;
                    }
                }
            }

            foreach (RoleInfo roleInfo in impRoles.Concat(crewRoles).Concat(neutralRoles).Concat(neutralKRoles))
                AddButton(roleInfo, i++);

            container.transform.localScale *= 0.75f;
        }

        public static void ResetMeetingHud()
        {
            MeetingHud meetingHud = MeetingHud.Instance;
            if (meetingHud != null && meetingHud.isActiveAndEnabled)
                foreach (PlayerVoteArea state in meetingHud.playerStates)
                {
                    if (Spectator.IsSpectator(state.TargetPlayerId)) continue;
                    state.gameObject.SetActive(true);
                }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
        class PlayerVoteAreaSelectPatch
        {
            static bool Prefix()
            {
                return !((PlayerControl.LocalPlayer.CanGuess() && GuesserUI != null) || HelpMenu.RolesUI != null);
            }
        }

        static void AddJailorButton()
        {
            PlayerControl.LocalPlayer.IsJailor(out Jailor jailor);
            if (jailor != null && !jailor.Player.Data.IsDead && jailor.Charges > 0)
            {
                MeetingHud meetingHud = MeetingHud.Instance;
                Selections.Clear();
                Renderers.Clear();
                Buttons.Clear();

                foreach (PlayerVoteArea voteArea in meetingHud.playerStates)
                {
                    if (voteArea.AmDead || (!Jailor.CanJailSelf && jailor.Player.PlayerId == voteArea.TargetPlayerId))
                        continue;

                    GameObject template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject checkbox = Object.Instantiate(template);
                    checkbox.name = "JailIcon";
                    checkbox.transform.SetParent(voteArea.transform);
                    checkbox.transform.position = template.transform.position;
                    checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                    SpriteRenderer renderer = checkbox.GetComponent<SpriteRenderer>();
                    renderer.sprite = Jailor.GetCheckSprite();
                    renderer.color = new Color(1f, 1f, 1f, .5f);

                    if (jailor.Charges <= 0)
                        renderer.color = Color.gray;

                    byte index = voteArea.TargetPlayerId;
                    PassiveButton button = checkbox.GetComponent<PassiveButton>();
                    Buttons.Add(index, button);
                    button.OnClick.RemoveAllListeners();
                    button.OnClick.AddListener((Action)(() => JailorOnClick(voteArea, jailor)));

                    Selections.Add(index, false);
                    Renderers.Add(index, renderer);
                }

                // Add the "Confirm Jail" button and "Jails: X" text next to it
                Transform meetingUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
                PlayerVoteArea templateVoteArea = meetingHud.playerStates[0];
                Transform buttonTemplate = templateVoteArea.transform.FindChild("votePlayerBase");
                Transform maskTemplate = templateVoteArea.transform.FindChild("MaskArea");
                TextMeshPro textTemplate = templateVoteArea.NameText;
                Transform confirmJailButtonParent = new GameObject("ConfirmJailButton").transform;
                confirmJailButtonParent.SetParent(meetingUI);
                Transform confirmJailButton = Object.Instantiate(buttonTemplate, confirmJailButtonParent);

                Transform infoTransform = templateVoteArea.NameText.transform.parent.FindChild("Info");
                TextMeshPro meetingInfo = infoTransform?.GetComponent<TextMeshPro>();
                JailorChargesText = Object.Instantiate(templateVoteArea.NameText, confirmJailButtonParent);
                JailorChargesText.text = $"Jails: {jailor.Charges}";
                JailorChargesText.enableWordWrapping = false;
                JailorChargesText.transform.localScale = Vector3.one * 1.7f;
                JailorChargesText.transform.localPosition = new Vector3(-2.5f, 0f, 0f);

                Transform confirmJailButtonMask = Object.Instantiate(maskTemplate, confirmJailButtonParent);
                JailorConfirmButtonLabel = Object.Instantiate(textTemplate, confirmJailButton);
                confirmJailButton.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
                confirmJailButtonParent.localPosition = new Vector3(0, -2.225f, -5);
                confirmJailButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
                JailorConfirmButtonLabel.text = Helpers.ColorString(Color.red, "Confirm Jail");
                JailorConfirmButtonLabel.alignment = TextAlignmentOptions.Center;
                JailorConfirmButtonLabel.transform.localPosition = new Vector3(0, 0, JailorConfirmButtonLabel.transform.localPosition.z);
                JailorConfirmButtonLabel.transform.localScale *= 1.7f;

                PassiveButton passiveButton = confirmJailButton.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnMouseOut = new UnityEvent();
                passiveButton.OnMouseOver = new UnityEvent();
                if (!PlayerControl.LocalPlayer.Data.IsDead)
                    passiveButton.OnClick.AddListener((Action)(() => JailorConfirm(jailor)));
                confirmJailButton.parent.gameObject.SetActive(false);
                meetingHud.StartCoroutine(Effects.Lerp(4.27f, new Action<float>((p) =>
                { // Button appears delayed, so that its visible in the voting screen only!
                    if (p == 1f)
                    {
                        confirmJailButton.parent.gameObject.SetActive(true);
                    }
                })));
                passiveButton.OnMouseOver.AddListener((Action)(() =>
                    confirmJailButton.GetChild(0).GetComponent<SpriteRenderer>().enabled = true
                ));
                passiveButton.OnMouseOut.AddListener((Action)(() =>
                    confirmJailButton.GetChild(0).GetComponent<SpriteRenderer>().enabled = false
                ));
            }
        }

        static void AddMayorButton()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer != Mayor.Player || localPlayer.Data.IsDead || Mayor.Retired || !Mayor.CanRetire || localPlayer.TasksLeft() != 0) return;
            MayorClicked = false;

            MeetingHud meetingHud = MeetingHud.Instance;

            //References
            Transform meetingUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
            PlayerVoteArea templateVoteArea = meetingHud.playerStates[0];
            Transform maskTemplate = templateVoteArea.transform.FindChild("MaskArea");
            Transform buttonTemplate = templateVoteArea.transform.FindChild("votePlayerBase");
            TextMeshPro textTemplate = templateVoteArea.NameText;

            //Button
            Transform mayoreRetireButtonParent = new GameObject("MayorRetireButton").transform;
            mayoreRetireButtonParent.SetParent(meetingUI);
            Transform mayorRetireButton = Object.Instantiate(buttonTemplate, mayoreRetireButtonParent);
            mayorRetireButton.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
            mayoreRetireButtonParent.localPosition = new Vector3(0, -2.225f, -5);
            mayoreRetireButtonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
            Transform confirmJailButtonMask = Object.Instantiate(maskTemplate, mayoreRetireButtonParent);

            //ButtonText
            MayorRetireButtonLabel = Object.Instantiate(textTemplate, mayorRetireButton);
            MayorRetireButtonLabel.text = "Retire?";
            MayorRetireButtonLabel.color = Mayor.Color;
            MayorRetireButtonLabel.alignment = TextAlignmentOptions.Center;
            MayorRetireButtonLabel.transform.localPosition = new Vector3(0, 0, MayorRetireButtonLabel.transform.localPosition.z);
            MayorRetireButtonLabel.transform.localScale *= 1.7f;

            //ToolTip
            GameObject toolTip = Object.Instantiate(new GameObject("MayorRetireButtonToolTip"), mayorRetireButton);
            SpriteRenderer tips = toolTip.gameObject.AddComponent<SpriteRenderer>();
            toolTip.transform.localPosition = new Vector3(0.5f, 0.75f, -1f);
            toolTip.layer = templateVoteArea.Megaphone.gameObject.layer;
            tips.sprite = Mayor.ToolTipSprite();
            toolTip.SetActive(false);

            //Action
            PassiveButton passiveButton = mayorRetireButton.GetComponent<PassiveButton>();
            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOver = new UnityEvent();
            passiveButton.OnClick.AddListener((Action)(() => MayorRetire()));
            passiveButton.OnMouseOver.AddListener((Action)(() =>
            {
                mayorRetireButton.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                toolTip.SetActive(true);
            }));
            passiveButton.OnMouseOut.AddListener((Action)(() =>
            {
                mayorRetireButton.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                toolTip.SetActive(false);
            }));

            //Button appears delayed, so that its visible in the voting screen only!
            mayorRetireButton.gameObject.SetActive(false);
            meetingHud.StartCoroutine(Effects.Lerp(4.27f, new Action<float>((p) =>
            {
                if (p == 1f)
                {
                    mayorRetireButton.gameObject.SetActive(true);
                }
            })));
        }

        static void AddParityCopOverlay()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            bool comparedCrewmate = !localPlayer.Data.IsDead && localPlayer.IsCrew();
            MeetingHud meetingHud = MeetingHud.Instance;

            foreach (ParityCop parityCop in ParityCop.ParityCopDictionary.Values)
            {
                //Should always be either one or two values or null
                List<PlayerControl> playersToCompare = parityCop.FindPlayersToCompare();
                if (playersToCompare == null)
                    continue;

                if (parityCop.PressedFakeCompare)
                {
                    playersToCompare.Add(parityCop.Player);
                    parityCop.FakeCompareCharges--;
                }

                bool showComparisons = playersToCompare.Any(p => p.PlayerId == localPlayer.PlayerId);
                bool isParityCop = parityCop.Player.AmOwner;

                Sprite compareSprite = parityCop.RetrieveCompareSprite();
                Sprite compareToolTipSprite = parityCop.RetrieveCompareToolTipSprite();

                foreach (PlayerVoteArea voteArea in meetingHud.playerStates)
                {
                    if (!isParityCop && !showComparisons && !PlayerControl.LocalPlayer.Data.IsDead)
                        continue;
                    if (!playersToCompare.Any(player => player.PlayerId == voteArea.TargetPlayerId) || (comparedCrewmate && !isParityCop))
                        continue;

                    GameObject template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject exitButton = Object.Instantiate(template, voteArea.transform);
                    SpriteRenderer rend = exitButton.GetComponent<SpriteRenderer>();
                    exitButton.layer = voteArea.Megaphone.gameObject.layer;
                    exitButton.transform.localPosition = new Vector3(1.00f, 0.03f, -1.3f);
                    exitButton.transform.localScale = Vector3.oneVector * .8f;
                    rend.sprite = compareSprite;

                    GameObject toolTip = new();
                    SpriteRenderer tips = toolTip.AddComponent<SpriteRenderer>();
                    toolTip.transform.SetParent(rend.transform);
                    toolTip.gameObject.layer = voteArea.Megaphone.gameObject.layer;
                    toolTip.transform.localPosition = new Vector3(0.5f, -0.5f, -1f);
                    tips.sprite = compareToolTipSprite;
                    toolTip.SetActive(false);

                    PassiveButton button = exitButton.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    button.OnMouseOut.RemoveAllListeners();
                    button.OnMouseOver.RemoveAllListeners();
                    button.OnMouseOver = new UnityEvent();
                    button.OnMouseOut = new UnityEvent();

                    button.OnMouseOver.AddListener((Action)(() => toolTip.SetActive(true)));
                    button.OnMouseOut.AddListener((Action)(() => toolTip.SetActive(false)));
                }
            }
        }

        static void AddPyromaniacOverlay()
        {
            if (Pyromaniac.PyromaniacDictionary.Values.Count != 0)
            {
                MeetingHud meetingHud = MeetingHud.Instance;
                foreach (Pyromaniac pyromaniac in Pyromaniac.PyromaniacDictionary.Values)
                    for (int i = 0; i < meetingHud.playerStates.Length; i++)
                    {
                        PlayerVoteArea voteArea = meetingHud.playerStates[i];
                        if (!pyromaniac.DousedPlayers.Any(playerId => playerId == voteArea.TargetPlayerId))
                            continue;

                        GameObject template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
                        GameObject cheesit = Object.Instantiate(template);
                        cheesit.name = "PyroIcon";
                        cheesit.transform.SetParent(voteArea.transform);
                        cheesit.transform.position = template.transform.position;
                        cheesit.transform.localPosition = new Vector3(-0.5f, 0f, -1.3f);
                        cheesit.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
                        SpriteRenderer renderer = cheesit.GetComponent<SpriteRenderer>();
                        renderer.sprite = Pyromaniac.GetPyroOverlaySprite();
                        renderer.color = Pyromaniac.Color;

                        GameObject tooltip = new();
                        SpriteRenderer tips = tooltip.AddComponent<SpriteRenderer>();
                        tips.sprite = Pyromaniac.GetPyroTooltipSprite();
                        tooltip.transform.SetParent(cheesit.transform);
                        tooltip.layer = voteArea.Megaphone.gameObject.layer;
                        tooltip.transform.localPosition = new Vector3(1.3f, -1.8f, -1.5f);
                        tooltip.SetActive(false);

                        PassiveButton button = cheesit.GetComponent<PassiveButton>();
                        button.OnClick.RemoveAllListeners();
                        button.OnMouseOver.RemoveAllListeners();
                        button.OnMouseOut.RemoveAllListeners();
                        button.OnMouseOut = new UnityEvent();
                        button.OnMouseOver = new UnityEvent();

                        button.OnMouseOver.AddListener((Action)(() => tooltip.SetActive(true)));
                        button.OnMouseOut.AddListener((Action)(() => tooltip.SetActive(false)));
                    }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.SortButtons))]
        class SortButtonsPatch
        {
            static void Postfix()
            {
                RemoveSpectatorPlate();
            }
            static void RemoveSpectatorPlate()
            {
                if (Spectator.Players.Count <= 0) return;
                SortMeetingPlayers();

                MeetingHud.Instance.playerStates.ToList().ForEach(state =>
                {
                    if (Spectator.Players.Contains(state.TargetPlayerId))
                        state.gameObject.SetActive(false);
                });
            }
            public static void SortMeetingPlayers()
            {
                var allPlayers = MeetingHud.Instance.playerStates;
                List<Vector3> playerPositions = allPlayers.Select(area => area.transform.localPosition).ToList();

                _ = allPlayers.OrderBy((pva) =>
                {
                    if (!pva.AmDead)
                    {
                        return 0;
                    }
                    else if (!Spectator.IsSpectator(pva.TargetPlayerId))
                    {
                        return 5;
                    }
                    return 50;
                });

                for (int i = 0; i < allPlayers.Length; i++)
                {
                    allPlayers[i].transform.localPosition = playerPositions[i];
                }
            }
        }

        static void AddWatcherOverlay()
        {
            if (Watcher.Player == null || !Watcher.TrackedPlayers.Contains(PlayerControl.LocalPlayer))
                return;

            bool isRomanticCrew = PlayerControl.LocalPlayer == Romantic.Player && Romantic.IsCrewmate;
            bool isevil = Helpers.IsNeutral(PlayerControl.LocalPlayer) || PlayerControl.LocalPlayer.Data.Role.IsImpostor;

            if (isevil && !isRomanticCrew)
            {
                PlayerVoteArea pva = MeetingHud.Instance.playerStates[PlayerControl.LocalPlayer.PlayerId];
                Transform meetingUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

                SpriteRenderer watcherOverlay = new GameObject("WatcherOverlay").AddComponent<SpriteRenderer>();
                watcherOverlay.sprite = Watcher.GetTrippedWireOverlay();
                watcherOverlay.transform.SetParent(meetingUI);
                watcherOverlay.gameObject.layer = pva.Megaphone.gameObject.layer;
                watcherOverlay.transform.localPosition = new Vector3(-3.5f, 2.28f, -1f);


                GameObject template = pva.Buttons.transform.Find("CancelButton").gameObject;
                GameObject watched = Object.Instantiate(template, watcherOverlay.transform);
                SpriteRenderer rend = watched.GetComponent<SpriteRenderer>();
                rend.sprite = null;
                watched.transform.position = watcherOverlay.transform.position;

                GameObject tooltip = new("WatcherOverlayToolTip");
                SpriteRenderer tips = tooltip.AddComponent<SpriteRenderer>();
                tips.sprite = Watcher.GetTrippedWireToolTip();
                tooltip.transform.SetParent(meetingUI);
                tooltip.layer = pva.Megaphone.gameObject.layer;
                tooltip.transform.localPosition = new Vector3(-3f, 1.7f, -1.5f);
                tooltip.SetActive(false);

                PassiveButton button = watched.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                button.OnMouseOut = new UnityEvent();
                button.OnMouseOver = new UnityEvent();

                button.OnMouseOver.AddListener((Action)(() => tooltip.SetActive(true)));
                button.OnMouseOut.AddListener((Action)(() => tooltip.SetActive(false)));
            }
        }



        public static void AddGuesserButtons()
        {
            MeetingHud meetingHud = MeetingHud.Instance;
            if (!PlayerControl.LocalPlayer.CanGuess() || PlayerControl.LocalPlayer.Data.IsDead || Helpers.RemainingShots(PlayerControl.LocalPlayer) <= 0)
                return;

            for (int i = 0; i < meetingHud.playerStates.Length; i++)
            {
                PlayerVoteArea voteArea = meetingHud.playerStates[i];
                if (voteArea.AmDead || voteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId)
                    continue;

                GameObject template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
                GameObject targetBox = Object.Instantiate(template, voteArea.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = Assassin.GetTargetSprite();
                PassiveButton button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener((Action)(() => GuesserOnClick(Helpers.PlayerById(voteArea.TargetPlayerId))));
            }
        }

        public static void AddRomanticOverlay()
        {
            if (!Romantic.HasLover || Romantic.PairIsDead)
                return;

            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates[0];
            Transform meetingUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            SpriteRenderer romance = new GameObject("RomanticOverlay").AddComponent<SpriteRenderer>();
            romance.transform.SetParent(meetingUI);
            romance.gameObject.layer = voteArea.Megaphone.gameObject.layer;
            romance.transform.localPosition = new Vector3(-2.5f, 2.3f, -1f);

            GameObject template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
            GameObject love = Object.Instantiate(template, romance.transform);
            SpriteRenderer rend = love.GetComponent<SpriteRenderer>();
            rend.sprite = null;
            love.transform.position = romance.transform.position;

            SpriteRenderer tooltip = new GameObject("RomanticTooltip").AddComponent<SpriteRenderer>();
            tooltip.transform.SetParent(meetingUI);
            tooltip.gameObject.layer = voteArea.Megaphone.gameObject.layer;
            tooltip.transform.localPosition = new Vector3(-2f, 1.7f, -1.5f);
            tooltip.gameObject.SetActive(false);

            PassiveButton button = love.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnMouseOut = new UnityEvent();
            button.OnMouseOver = new UnityEvent();

            button.OnMouseOver.AddListener((Action)(() => tooltip.gameObject.SetActive(true)));
            button.OnMouseOut.AddListener((Action)(() => tooltip.gameObject.SetActive(false)));

            romance.sprite = Romantic.GetRomanticAliveMeetingOverlay();
            tooltip.sprite = Romantic.GetAliveToolTip();
        }

        public static void AddBrokenRomanticHeart()
        {
            if (!Romantic.PairIsDead) return;

            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates[0];
            Transform meetingUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");

            SpriteRenderer romance = new GameObject("RomanticOverlay").AddComponent<SpriteRenderer>();
            romance.transform.SetParent(meetingUI);
            romance.gameObject.layer = voteArea.Megaphone.gameObject.layer;
            romance.transform.localPosition = new Vector3(-2.5f, 2.3f, -1f);

            GameObject template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
            GameObject love = Object.Instantiate(template, romance.transform);
            SpriteRenderer rend = love.GetComponent<SpriteRenderer>();
            rend.sprite = null;
            love.transform.position = romance.transform.position;

            SpriteRenderer tooltip = new GameObject("RomanticTooltip").AddComponent<SpriteRenderer>();
            tooltip.transform.SetParent(meetingUI);
            tooltip.gameObject.layer = voteArea.Megaphone.gameObject.layer;
            tooltip.transform.localPosition = new Vector3(-2f, 1.7f, -1.5f);
            tooltip.gameObject.SetActive(false);

            PassiveButton button = love.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnMouseOut = new UnityEvent();
            button.OnMouseOver = new UnityEvent();

            button.OnMouseOver.AddListener((Action)(() => tooltip.gameObject.SetActive(true)));
            button.OnMouseOut.AddListener((Action)(() => tooltip.gameObject.SetActive(false)));

            romance.sprite = Romantic.GetRomanticDeadMeetingOverlay();
            tooltip.sprite = Romantic.GetDeadToolTip();
        }

        static void PopulateButtonsPostfix()
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(3, new Action<float>((p) =>
            { // Delayed action
                if (p == 1f)
                {
                    //Romantic Overlay
                    AddRomanticOverlay();
                    //Vengful Romantic Overlay
                    AddBrokenRomanticHeart();
                    //Add Overlay for Compared Players
                    AddParityCopOverlay();
                    //Add Overlay for Watched Players
                    AddWatcherOverlay();
                    // Add Guesser Buttons
                    AddGuesserButtons();
                    //Add Jailor Button
                    AddJailorButton();
                    //Add Pyromanic Cheesit
                    AddPyromaniacOverlay();
                    //Add Mayore Retire Button
                    AddMayorButton();
                }
            })));
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
        class MeetingServerStartPatch
        {
            static void Postfix()
            {
                PopulateButtonsPostfix();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
        class MeetingDeserializePatch
        {
            static void Postfix([HarmonyArgument(1)] bool initialState)
            {
                if (initialState)
                    PopulateButtonsPostfix();
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch
        {
            public static void Prefix([HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
            {
                // Save Sleepwalker position, if the player is able to move (i.e. not on a ladder or a gap thingy)
                if (PlayerControl.LocalPlayer.MyPhysics.enabled && PlayerControl.LocalPlayer.moveable || PlayerControl.LocalPlayer.inVent)
                    Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;

                if (Romantic.Lover != null && Romantic.PartnerSeesLoveAfterMeeting)
                    Romantic.SetNameFirstMeeting = true;


                // Medium meeting start time
                Detective.MeetingStartTime = DateTime.UtcNow;
                // Reset vampire bitten
                Vampire.Bitten = null;
                // Count meetings
                if (meetingTarget == null)
                    MapOptions.MeetingsCount++;
                // Save the meeting target
                // Meeting start time
                MapOptions.Meetingtime = GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime + GameOptionsManager.Instance.currentNormalGameOptions.VotingTime + 7f;

                RPCProcedure.MeetingStart();

                Scavenger.ScavengerToRefugeeCheck();

                if (!Executioner.ConvertsImmediately)
                    Executioner.ExecutionerCheckPromotion();

                // Stop all playing sounds
                SoundEffectsManager.StopAll();

                if (HelpMenu.RolesUI != null) Object.Destroy(HelpMenu.RolesUI);

            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch
        {
            static void Postfix(MeetingHud __instance)
            {
                if (__instance.state >= MeetingHud.VoteStates.Discussion)
                {
                    // Remove first kill shield
                    MapOptions.FirstKillPlayer = null;
                    // No Longer First Round
                    MapOptions.IsFirstRound = false;
                    if (!Executioner.ConvertsImmediately)
                        Executioner.ExecutionerCheckPromotion();

                    Scavenger.ScavengerToRefugeeCheck();

                    GuesserUpdate();
                }
            }

            static void GuesserUpdate()
            {
                if (MapOptions.Meetingtime > 10f || GameOptionsManager.Instance.currentNormalGameOptions.VotingTime == 0f)
                    return;

                MeetingHud.Instance.playerStates.ToList().ForEach(state =>
                {
                    Transform child = state.transform.FindChild("ShootButton");
                    if (child != null)
                        Object.Destroy(child.gameObject);
                });

                if (GuesserUI == null)
                    return;

                Object.Destroy(GuesserUI);
                GuesserUI = null;
                MeetingHud.Instance.playerStates.ToList().ForEach(state => state.gameObject.SetActive(true));
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        class MeetingChatNotification
        {
            static void Postfix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, ref string chatText)
            {
                if (__instance == null || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || !MeetingHud.Instance)
                    return;
                PlayerControl localPlayer = PlayerControl.LocalPlayer;

                if (sourcePlayer.AmOwner && !localPlayer.Data.IsDead && !chatText.ToLower().StartsWith("/cultist "))
                {
                    RPCProcedure.Send(CustomRPC.SetMeetingChatOverlay, localPlayer.PlayerId);
                    RPCProcedure.SetChatNotificationOverlay(localPlayer.PlayerId);
                }
            }
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        class FastChatPatch
        {
            static void Postfix(ChatController __instance)
            {
                if (__instance == null || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                    return;
                int time = (int)__instance.timeSinceLastMessage;

                if (time == 0)
                {
                    __instance.timeSinceLastMessage = 3;
                }
            }
        }
    }
}
