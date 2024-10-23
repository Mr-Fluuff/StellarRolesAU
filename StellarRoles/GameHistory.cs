using Reactor.Utilities.Extensions;
using StellarRoles.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static StellarRoles.Patches.AdditionalTempData;
using Object = UnityEngine.Object;

namespace StellarRoles
{
    public class DeadPlayer
    {
        public PlayerControl Player;
        public NetworkedPlayerInfo Data;
        public string Name;
        public DateTime TimeOfDeath;
        public DeathReason DeathReason;
        public PlayerControl KillerIfExisting;

        public DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting)
        {
            Player = player;
            Data = player.Data;
            TimeOfDeath = timeOfDeath;
            DeathReason = deathReason;
            KillerIfExisting = killerIfExisting;
            Name = player.Data.PlayerName;
            GameHistory.DeadPlayers.Add(this);
        }
    }

    public static class GameHistory
    {
        public static readonly List<Tuple<Vector3, bool>> LocalPlayerPositions = new();
        public static readonly List<DeadPlayer> DeadPlayers = new();

        public static void ClearGameHistory()
        {
            LocalPlayerPositions.Clear();
            DeadPlayers.Clear();
        }
    }

    public class PreviousGameHistory
    {
        public static List<PreviousGameHistory> PreviousGameList = new();
        public static Sprite _HistoryPanel;
        public static Sprite _LeftArrow;
        public static Sprite _RightArrow;

        public PlayerEndGameStats PlayerEndGameStats;
        public PlayerRoleInfo PlayerRoleInfo;

        public PreviousGameHistory()
        {
            PreviousGameList.Add(this);
        }

        public static Sprite GetHistoryPanelSprite()
        {
            return _HistoryPanel ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.GameHistory.GameHistoryScreen.png", 110f);
        }
        public static Sprite GetLeftArrowSprite()
        {
            return _LeftArrow ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.GameHistory.ArrowLeft.png", 480f);
        }
        public static Sprite GetRightArrowSprite()
        {
            return _RightArrow ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.GameHistory.ArrowRight.png", 480f);
        }


        public static String GetRoleSummary()
        {
            StringBuilder roleSummaryText = new();
            foreach (var d in PreviousGameList)
            {
                var data = d.PlayerRoleInfo;
                string roles = data.Roles.Count > 0 ? string.Join(" -> ", data.Roles.Select(x => Helpers.ColorString(x.Color, x.Name))) : "";
                string modifiers = data.Modifiers.Count > 0 ? string.Join(" ", data.Modifiers.Select(x => Helpers.ColorString(x.Color, $"({x.Name}) "))) : "";

                string taskInfo = data.TasksTotal > 0 ? $" - <color=#FAD934FF>({data.TasksCompleted}/{data.TasksTotal})</color>" : "";
                string abilityKillsInfo = data.AbilityKills > 0 ? $" ({data.AbilityKills} Ability)" : "";
                string killInfo = data.Kills > 0 ? $" - {Helpers.ColorString(Impostor.color, $"Kills: {data.Kills}{abilityKillsInfo}")}" : "";
                string misfireInfo = data.Misfires > 0 ? $" - {Helpers.ColorString(Impostor.color, $"Misfires: {data.Misfires}")}" : "";
                string correctShotsInfo = data.CorrectShots > 0 ? $" - {Helpers.ColorString(Sheriff.Color, $"Correct Shots: {data.CorrectShots}")}" : "";
                string correctGuessInfo = data.CorrectGuesses > 0 ? $" - {Helpers.ColorString(Color.green, $"Correct Guesses: {data.CorrectGuesses}")}" : "";
                string incorrectGuessInfo = data.IncorrectGuesses > 0 ? $" - {Helpers.ColorString(Impostor.color, $"Incorrect Guesses: {data.IncorrectGuesses}")}" : "";
                string scavengerEaten = data.Eats > 0 ? $" - {Helpers.ColorString(Scavenger.Color, $"Bodies Eaten: {data.Eats}")}" : "";
                string lover = data.Loved ? $"{Helpers.ColorString(Romantic.Color, $" ♥")}" : "";

                roleSummaryText.AppendLine($"{data.PlayerName}{lover} - {modifiers}" +
                    $"{roles}{taskInfo}{killInfo}{misfireInfo}{correctShotsInfo}" +
                    $"{correctGuessInfo}{incorrectGuessInfo}{scavengerEaten}");
            }

            return roleSummaryText.ToString();
        }

        public static String GetTournamentSummary()
        {
            StringBuilder TournSummaryText = new();
            string win = PreviousGameList[0].PlayerEndGameStats.WinType;
            TournSummaryText.AppendLine($"WinType {win}");
            TournSummaryText.AppendLine();

            TournSummaryText.AppendLine(Helpers.ColorString(Palette.CrewmateBlue, "Crewmates"));
            foreach (var d in PreviousGameList)
            {
                var data = d.PlayerRoleInfo;
                if (data.WasImp) continue;

                var data2 = d.PlayerEndGameStats;
                string critical = data2.CriticalMeetingError ? Helpers.ColorString(Palette.ImpostorRed, $"True") : Helpers.ColorString(Color.green, $"False");
                string taskInfo = data.TasksTotal > 0 ? $"<color=#FAD934FF>({data.TasksCompleted}/{data.TasksTotal})</color>" + ", " : "";
                string IncorrectVotes = data.IncorrectVotes > 0 ? Helpers.ColorString(Impostor.color, $"Incorrect Votes: {data.IncorrectVotes}" + ", ") : "";
                string CorrectVotes = data.CorrectVotes > 0 ? Helpers.ColorString(Palette.CrewmateBlue, $"Correct Votes: {data.CorrectVotes}" + ", ") : "";
                string IncorrectEjects = data.IncorrectEjects > 0 ? Helpers.ColorString(Impostor.color, $"Incorrect Ejects: {data.IncorrectEjects}" + ", ") : "";
                string CorrectEjects = data.CorrectEjects > 0 ? Helpers.ColorString(Palette.CrewmateBlue, $"Correct Ejects: {data.CorrectEjects}" + ", ") : "";
                string MeetingError = Helpers.ColorString(Impostor.color, $"Critical Error") + ": " + critical + ", ";
                string Alive = data.AliveAtLastMeeting ? "Alive at Last Meeting" + ", " : "";
                string Survivability = Helpers.ColorString(Palette.CrewmateBlue, $"Survival: {data2.Survivability}");

                TournSummaryText.AppendLine($"{data.PlayerName} - {taskInfo}{IncorrectVotes}{CorrectVotes}" +
                    $"{IncorrectEjects}{CorrectEjects}\n{MeetingError}{Alive}{Survivability}");
            }

            TournSummaryText.AppendLine();
            TournSummaryText.AppendLine(Helpers.ColorString(Palette.ImpostorRed, "Impostors"));
            foreach (var d in PreviousGameList)
            {
                var data = d.PlayerRoleInfo;
                if (!data.WasImp) continue;

                var data2 = d.PlayerEndGameStats;
                string killInfo = Helpers.ColorString(Impostor.color, $"Kills: {data.Kills}");
                string CrewEjected = Helpers.ColorString(Impostor.color, $"Ejected Crew: {data2.NumberOfCrewmatesEjectedTotal}");

                TournSummaryText.AppendLine($"{data.PlayerName} - {killInfo}, {CrewEjected}");
            }

            return TournSummaryText.ToString();
        }

        public static GameObject HistoryUI = null;
        public static TextMeshPro UITitle = null;
        public static TextMeshPro UIInfo = null;

        public static void CreateHistoryScreen()
        {
            var buttonref = HudManager.Instance.MapButton.GetComponent<PassiveButton>();
            SpriteRenderer container = new GameObject("GameHistoryContainer").AddComponent<SpriteRenderer>();
            container.sprite = GetHistoryPanelSprite();
            container.transform.SetParent(HudManager.Instance.transform);
            //container.gameObject.transform.SetLocalZ(-200);
            container.transform.localPosition = new Vector3(0, 0, -30f);
            container.transform.localScale = new Vector3(.75f, .7f, 1f);
            container.gameObject.layer = 5;

            TextMeshPro textTemplate = HudManager.Instance.TaskPanel.taskText;

            UITitle = Object.Instantiate(textTemplate, container.transform);
            UITitle.text = "Role Info Menu";
            UITitle.color = Color.white;
            UITitle.outlineWidth = .25f;
            UITitle.alignment = TextAlignmentOptions.Top;
            UITitle.transform.localPosition = new Vector3(0f, 0f, -2f);
            UITitle.transform.localScale = Vector3.one * 2.3f;

            UIInfo = Object.Instantiate(textTemplate, container.transform);
            UIInfo.color = Color.white;
            UIInfo.text = "";
            UIInfo.enableWordWrapping = false;
            UIInfo.transform.localScale = Vector3.one * 0.75f;
            UIInfo.transform.localPosition = new Vector3(-3.9f, 1.1f, -2f);
            UIInfo.alignment = TextAlignmentOptions.TopLeft;
            UIInfo.fontStyle = FontStyles.Bold;

            SpriteRenderer LeftArrow = new GameObject("LeftArrow").AddComponent<SpriteRenderer>();
            LeftArrow.transform.SetParent(UITitle.transform);
            LeftArrow.gameObject.transform.SetLocalZ(-200);
            LeftArrow.gameObject.layer = 5;
            LeftArrow.sprite = GetLeftArrowSprite();
            LeftArrow.transform.localPosition = new Vector3(-1.7829f, 1.1334f, -3f);
            LeftArrow.color = Color.green;

            SpriteRenderer RightArrow = new GameObject("RightArrow").AddComponent<SpriteRenderer>();
            RightArrow.transform.SetParent(UITitle.transform);
            RightArrow.gameObject.transform.SetLocalZ(-200);
            RightArrow.gameObject.layer = 5;
            RightArrow.sprite = GetRightArrowSprite();
            RightArrow.transform.localPosition = new Vector3(1.7829f, 1.1334f, -3f);
            RightArrow.color = Color.green;

            var LeftButton = new CustomPassiveButton(LeftArrow.transform);
            LeftButton.button.OnClick.AddListener((Action)(() => 
            {
                LeftArrow.gameObject.SetActive(false);
                LeftArrow.color = Color.green;

                RightArrow.gameObject.SetActive(true);

                ShowRoleStats();
            }));
            LeftButton.button.OnMouseOver.AddListener((Action)(() =>
            {
                LeftArrow.color = Color.red;
            }));
            LeftButton.button.OnMouseOut.AddListener((Action)(() =>
            {
                LeftArrow.color = Color.green;
            }));

            var RightButton = new CustomPassiveButton(RightArrow.transform);
            RightButton.button.OnClick.AddListener((Action)(() =>
            {
                RightArrow.gameObject.SetActive(false);
                RightArrow.color = Color.green;

                LeftArrow.gameObject.SetActive(true);

                ShowTournamentStats();
            }));
            RightButton.button.OnMouseOver.AddListener((Action)(() =>
            {
                RightArrow.color = Color.red;
            }));
            RightButton.button.OnMouseOut.AddListener((Action)(() =>
            {
                RightArrow.color = Color.green;
            }));

            HistoryUI = container.gameObject;
            HistoryUI.SetActive(false);
        }

        public static void ToggleHistoryScreen()
        {
            if (HistoryUI == null)
            {
                CreateHistoryScreen();
            }
            if (HistoryUI.active)
            {
                HistoryUI.SetActive(false);
                return;
            }
            UITitle.text = "Last Game Summary";
            UITitle.transform.FindChild("RightArrow").gameObject.SetActive(true);
            UITitle.transform.FindChild("LeftArrow").gameObject.SetActive(false);
            if (PreviousGameList.Count == 0) 
            {
                UIInfo.text = PlaceHolder();
            }
            else
            {
                UIInfo.text = GetRoleSummary();
            }
            HistoryUI.SetActive(true);
        }

        public static void ShowTournamentStats()
        {
            if (PreviousGameList.Count == 0)
            {
                UIInfo.text = PlaceHolder();
            }
            else
            {
                UIInfo.text = GetTournamentSummary();
            }
            UITitle.text = "Last Game Tournament Summary";
        }

        public static void ShowRoleStats()
        {
            if (PreviousGameList.Count == 0)
            {
                UIInfo.text = PlaceHolder();
            }
            else
            {
                UIInfo.text = GetRoleSummary();
            }
            UITitle.text = "Last Game Summary";
        }

        public static String PlaceHolder()
        {
            UITitle.text = "PlaceHolderText";
            StringBuilder TournSummaryText = new();
            TournSummaryText.AppendLine("This is a Test WinType");
            TournSummaryText.AppendLine("");
            TournSummaryText.AppendLine("Crewmates");
            TournSummaryText.AppendLine("Crew 1 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 2 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 3 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 4 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 5 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 6 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 7 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 8 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 9 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 10 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 11 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Crew 12 Info..................................................................................................................... ");
            TournSummaryText.AppendLine();
            TournSummaryText.AppendLine("Impostors");
            TournSummaryText.AppendLine("Imp 1 Info.....................................................................................................................");
            TournSummaryText.AppendLine("Imp 2Info.....................................................................................................................");

            return TournSummaryText.ToString();
        }
    }
}