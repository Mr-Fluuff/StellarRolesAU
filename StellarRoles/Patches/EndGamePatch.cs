
using HarmonyLib;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StellarRoles.Patches
{
    public enum CustomGameOverReason
    {
        JesterWin = 10,
        ArsonistWin,
        ScavengerWin,
        ExecutionerWin,
        HeadHunterWin,
        RogueImpostorWin,
        RefugeeOnlyWin,
        PyromaniacWin,
        PyroAndArsoWin,
        NightmareWin,
        RuthlessRomanticWin,
        ImpostorWin,
        NobodyWins,
        CrewmateWin,
        OxygenWin,
        ReactorWin,
        TaskWin,
        TimesUp
    }

    public enum WinCondition
    {
        Default,
        ArsonistWin,
        ScavengerWin,
        ExecutionerWin,
        HeadHunterWin,
        PyromaniacWin,
        PyroAndArsoWin,
        JesterWin,
        RogueImpWin,
        RefugeeOnlyWin,
        NightmareWin,
        RuthlessRomanticWin,
        ImpostorWin,
        CrewmateWin,
        OxygenWin,
        ReactorWin,
        AdditionalAliveRefugeeWin,
        AdditionalRomanticWin,
        TaskWin,
        TimesUp
    }

    public static class AdditionalTempData
    {
        // Should be implemented using a proper GameOverReason in the future
        public static WinCondition WinCondition { get; set; } = WinCondition.Default;
        public static readonly List<WinCondition> AdditionalWinConditions = new();
        public static readonly List<PlayerRoleInfo> PlayerRoles = new();
        public static List<PlayerEndGameStats> PlayerEndGameStats = new();

        public static void Clear()
        {
            PlayerRoles.Clear();
            AdditionalWinConditions.Clear();
            WinCondition = WinCondition.Default;
            PlayerEndGameStats.Clear();
        }

        public class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public List<RoleInfo> Roles { get; set; }
            public List<RoleInfo> Modifiers { get; set; }
            public bool Loved { get; set; }
            public bool WasImp { get; set; }
            public int TasksCompleted { get; set; }
            public int TasksTotal { get; set; }
            public int Kills { get; set; }
            public int Misfires { get; set; }
            public int CorrectShots { get; set; }
            public int CorrectGuesses { get; set; }
            public int IncorrectGuesses { get; set; }
            public int AbilityKills { get; set; }
            public int Eats { get; set; }

            public int CorrectVotes { get; set; }
            public int IncorrectVotes { get; set; }
            public int CorrectEjects { get; set; }
            public int IncorrectEjects { get; set; }

            public bool AliveAtLastMeeting { get; set; }

        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch
    {
        private static CustomGameOverReason _GameOverReason;
        public static void Prefix([HarmonyArgument(0)] ref EndGameResult endGameResult)
        {
            ExtraStats.UpdateSurvivability();

            _GameOverReason = (CustomGameOverReason)endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10)
                endGameResult.GameOverReason = GameOverReason.ImpostorsByKill;

            // Reset zoomed out ghosts
            Helpers.ResetZoom();

            StellarRoles.NormalOptions.KillCooldown = ShipStatusPatch.OriginalKillCD;
        }

        public static void Postfix()
        {
            AdditionalTempData.Clear();
            PreviousGameHistory.PreviousGameList.Clear();

            foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (data == null) continue;
                var player = data.Object ?? null;
                List<RoleInfo> roles = PlayerGameInfo.GetRoles(data);
                
                (int tasksCompleted, int tasksTotal) = TasksHandler.TaskInfo(data);

                var playerRoles = new AdditionalTempData.PlayerRoleInfo()
                {
                    PlayerName = data.PlayerName,
                    Loved = roles.Any(x => x.RoleId == RoleId.Beloved) ||
                        VengefulRomantic.Lover == player ||
                        Romantic.Lover == player ||
                        RuthlessRomantic.IsLover(player),
                    Roles = roles,
                    WasImp = data.Role.IsImpostor,
                    Modifiers = PlayerGameInfo.GetModifiers(data.PlayerId),
                    TasksTotal = tasksTotal,
                    TasksCompleted = tasksCompleted,
                    Kills = PlayerGameInfo.TotalKills(data.PlayerId),
                    Misfires = PlayerGameInfo.TotalMisfires(data.PlayerId),
                    CorrectShots = PlayerGameInfo.TotalCorrectShots(data.PlayerId),
                    CorrectGuesses = PlayerGameInfo.TotalCorrectGuesses(data.PlayerId),
                    IncorrectGuesses = PlayerGameInfo.TotalIncorrectGuesses(data.PlayerId),
                    AbilityKills = PlayerGameInfo.TotalAbilityKills(data.PlayerId),
                    CorrectEjects = PlayerGameInfo.TotalCorrectEjects(data.PlayerId),
                    IncorrectEjects = PlayerGameInfo.TotalIncorrectEjects(data.PlayerId),
                    CorrectVotes = PlayerGameInfo.TotalCorrectVotes(data.PlayerId),
                    IncorrectVotes = PlayerGameInfo.TotalIncorrectVotes(data.PlayerId),
                    AliveAtLastMeeting = PlayerGameInfo.PlayerAliveAtLastMeeting(data.PlayerId),
                    Eats = roles.Any(x => x.RoleId == RoleId.Scavenger) ? PlayerGameInfo.TotalEaten(data.PlayerId) : 0

                };

                var endGameStats = new PlayerEndGameStats()
                {
                    Name = data.PlayerName,
                    Disconnected = data.Disconnected,
                    Role = data.Role.IsImpostor ? "IMPOSTER" : "CREWMATE",
                    CorrectEjects = PlayerGameInfo.TotalCorrectEjects(data.PlayerId),
                    IncorrectEjects = PlayerGameInfo.TotalIncorrectEjects(data.PlayerId),
                    CorrectVotes = PlayerGameInfo.TotalCorrectVotes(data.PlayerId),
                    IncorrectVotes = PlayerGameInfo.TotalIncorrectVotes(data.PlayerId),
                    AliveAtLastMeeting = PlayerGameInfo.PlayerAliveAtLastMeeting(data.PlayerId),
                    FirstTwoVictimsRound1 = PlayerGameInfo.FirstTwoPlayersDead(data.PlayerId),
                    TasksTotal = tasksTotal,
                    TasksCompleted = tasksCompleted,
                    Kills = PlayerGameInfo.TotalKills(data.PlayerId),
                    NumberOfCrewmatesEjectedTotal = PlayerGameInfo.TotalCrewmatesEjected(data.PlayerId),
                    CriticalMeetingError = PlayerGameInfo.WrongCriticalVote(data.PlayerId),
                    Survivability = PlayerGameInfo.Survivability(data.PlayerId),
                    ImposterDisconnectedWhileAlive = false,
                    WinType = Enum.GetName(_GameOverReason),
                };

                AdditionalTempData.PlayerRoles.Add(playerRoles);
                AdditionalTempData.PlayerEndGameStats.Add(endGameStats);

                _ = new PreviousGameHistory()
                {
                    PlayerEndGameStats = endGameStats,
                    PlayerRoleInfo = playerRoles
                };
            }

            List<PlayerControl> notWinners = new();
            if (Executioner.Player != null)
                notWinners.Add(Executioner.Player);
            if (Arsonist.Player != null)
                notWinners.Add(Arsonist.Player);
            if (Scavenger.Player != null)
                notWinners.Add(Scavenger.Player);
            if (Romantic.Player != null)
                notWinners.Add(Romantic.Player);
            if (VengefulRomantic.Player != null)
                notWinners.Add(VengefulRomantic.Player);
            if (Beloved.Player != null)
                notWinners.Add(Beloved.Player);
            if (HeadHunter.Player != null)
                notWinners.Add(HeadHunter.Player);

            notWinners.AddRange(PlayerControl.AllPlayerControls.GetFastEnumerator().Where(player =>
                player.IsRefugee(out _) || player.IsJester(out _) || player.IsPyromaniac(out _) || NeutralKiller.Players.Contains(player)
            ));

            List<CachedPlayerData> winnersToRemove = new();

            foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator())
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName))
                    winnersToRemove.Add(winner);

            foreach (CachedPlayerData winner in winnersToRemove)
                EndGameResult.CachedWinners.Remove(winner);

            bool jesterWin = _GameOverReason == CustomGameOverReason.JesterWin;
            bool arsonistWin = _GameOverReason == CustomGameOverReason.ArsonistWin;
            bool scavengerWin = _GameOverReason == CustomGameOverReason.ScavengerWin;
            bool executionerWin = _GameOverReason == CustomGameOverReason.ExecutionerWin;
            bool headHunterWin = _GameOverReason == CustomGameOverReason.HeadHunterWin;
            bool pyromaniacWin = _GameOverReason == CustomGameOverReason.PyromaniacWin;
            bool pyroAndArsoWin = _GameOverReason == CustomGameOverReason.PyroAndArsoWin;
            bool rogueImpWin = _GameOverReason == CustomGameOverReason.RogueImpostorWin;
            bool refugeeOnlyWin = _GameOverReason == CustomGameOverReason.RefugeeOnlyWin;
            bool ruthlessRomanticWin = _GameOverReason == CustomGameOverReason.RuthlessRomanticWin;
            bool nightmareWin = _GameOverReason == CustomGameOverReason.NightmareWin;
            bool impostorWin = _GameOverReason == CustomGameOverReason.ImpostorWin;
            bool crewmateWins = _GameOverReason is CustomGameOverReason.CrewmateWin or CustomGameOverReason.TaskWin;
            bool oxygenWin = _GameOverReason == CustomGameOverReason.OxygenWin;
            bool reactorWin = _GameOverReason == CustomGameOverReason.ReactorWin;
            bool timesUp = _GameOverReason == CustomGameOverReason.TimesUp;

            bool isRefugeeLose = !crewmateWins && !refugeeOnlyWin;

            // Executioner win
            if (executionerWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Executioner.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Executioner.Player);
                if (additionalWinner != null)
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.ExecutionerWin;
            }

            else if (impostorWin || timesUp)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (player.Data.Role.IsImpostor)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                if (impostorWin)
                {
                    AdditionalTempData.WinCondition = WinCondition.ImpostorWin;
                }
                else if (timesUp)
                {
                    AdditionalTempData.WinCondition = WinCondition.TimesUp;
                }
            }

            else if (oxygenWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (player.Data.Role.IsImpostor)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.OxygenWin;
            }

            else if (reactorWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (player.Data.Role.IsImpostor)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.ReactorWin;
            }

            // Arsonist win
            else if (arsonistWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Arsonist.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Arsonist.Player);
                if (additionalWinner != null)
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.ArsonistWin;
            }

            // Scavenger win
            else if (scavengerWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new(Scavenger.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Scavenger.Player);
                if (additionalWinner != null)
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.ScavengerWin;
            }

            else if (headHunterWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                EndGameResult.CachedWinners.Add(new CachedPlayerData(HeadHunter.Player.Data));
                AdditionalTempData.WinCondition = WinCondition.HeadHunterWin;
            }

            else if (nightmareWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (player.IsNightmare(out _) && !player.Data.IsDead)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.NightmareWin;
            }

            else if (ruthlessRomanticWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (player.IsRuthlessRomantic(out _) && !player.Data.IsDead)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.RuthlessRomanticWin;
            }

            else if (pyroAndArsoWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if ((player.IsPyromaniac(out _) || (Arsonist.Player == player)) && !player.Data.IsDead)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.PyroAndArsoWin;
            }

            else if (pyromaniacWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (Pyromaniac.PyromaniacDictionary.ContainsKey(player.PlayerId) && !player.Data.IsDead)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.PyromaniacWin;
            }

            else if (crewmateWins)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (player.IsCrew() && !player.IsRefugee(out _))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                if (_GameOverReason == CustomGameOverReason.TaskWin) 
                {
                    AdditionalTempData.WinCondition = WinCondition.TaskWin;
                }
                else AdditionalTempData.WinCondition = WinCondition.CrewmateWin;
            }


            else if (rogueImpWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (NeutralKiller.RogueImps.Contains(player) && !player.Data.IsDead)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.RogueImpWin;
            }

            // Jester win
            else if (jesterWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                EndGameResult.CachedWinners.Add(new CachedPlayerData(Jester.WinningJesterPlayer.Data));

                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Jester.WinningJesterPlayer);
                if (additionalWinner != null)
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.JesterWin;
            }

            else if (refugeeOnlyWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (Refugee refugee in Refugee.PlayerToRefugee.Values)
                    if (refugee.Player != null && !refugee.Player.Data.IsDead && !refugee.NotAckedExiled)
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(refugee.Player.Data));
                AdditionalTempData.WinCondition = WinCondition.RefugeeOnlyWin;
            }

            // Possible Additional winner: Romantic
            if ((Romantic.Player != null && Romantic.Lover != null) || Romantic.IsCrewmate || Romantic.IsImpostor)
            {
                CachedPlayerData romantic = null;

                if (Romantic.Lover != null)
                    foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator())
                    {
                        if (winner.PlayerName == Romantic.Lover.Data.PlayerName)
                            romantic = winner;
                    }
                else
                    foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator())
                        if ((Romantic.IsCrewmate && !winner.IsImpostor) || (Romantic.IsImpostor && winner.IsImpostor))
                            romantic = winner;

                if (romantic != null)
                {
                    if (!EndGameResult.CachedWinners.ToArray().Any(winner => winner.PlayerName == Romantic.Player.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(Romantic.Player.Data));
                    AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalRomanticWin);
                }
            }

            // Possible Additional winner: Vengeful Romantic
            if ((VengefulRomantic.Player != null && VengefulRomantic.Lover != null) || VengefulRomantic.IsDisconnected)
            {
                CachedPlayerData vengefulRomantic = null;
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator())
                    if (winner.PlayerName == VengefulRomantic.Lover.Data.PlayerName)
                        vengefulRomantic = winner;

                if (VengefulRomantic.IsDisconnected)
                    foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator())
                        if ((VengefulRomantic.IsCrewmate && !winner.IsImpostor) || (VengefulRomantic.IsImpostor && winner.IsImpostor))
                            vengefulRomantic = winner;

                if (vengefulRomantic != null)
                {
                    if (!EndGameResult.CachedWinners.ToArray().Any(winner => winner.PlayerName == VengefulRomantic.Player.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(VengefulRomantic.Player.Data));
                    AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalRomanticWin);
                }
            }

            // Possible Additional winner: Beloved
            if (Beloved.Player != null && Beloved.Romantic != null)
            {
                CachedPlayerData winningClient = null;
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator())
                    if (winner.PlayerName == Beloved.Romantic.Data.PlayerName)
                        winningClient = winner;
                if (winningClient != null)
                {
                    if (!EndGameResult.CachedWinners.ToArray().Any(winner => winner.PlayerName == Beloved.Player.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(Beloved.Player.Data));
                    AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalRomanticWin);
                }
            }

            if (Refugee.PlayerToRefugee?.Values.Count > 0 && !refugeeOnlyWin && !isRefugeeLose && !EndGameResult.CachedWinners.ToArray().Any(x => x.IsImpostor))
            {
                foreach (Refugee refugee in Refugee.PlayerToRefugee.Values)
                {
                    if (refugee.Player != null && !refugee.Player.Data.IsDead && !refugee.NotAckedExiled)
                    {
                        if (!EndGameResult.CachedWinners.ToArray().Any(winner => winner.PlayerName == refugee.Player.Data.PlayerName))
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(refugee.Player.Data));
                        if (!AdditionalTempData.AdditionalWinConditions.Contains(WinCondition.AdditionalAliveRefugeeWin))
                            AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalAliveRefugeeWin);
                    }
                }
            }

            if (MapOptions.TournamentLogs)
            {
                Helpers.writeStats(AdditionalTempData.PlayerEndGameStats);
            }

            // Reset Settings
            RPCProcedure.ResetVariables();
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch
    {
        public static void Postfix(EndGameManager __instance)
        {
            List<CachedPlayerData> list = EndGameResult.CachedWinners.ToArray().OrderBy(b => b.IsYou ? 0 : -1).ToList();

            // Additional code
            GameObject bonusText = Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.5f, __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMP_Text textRenderer = bonusText.GetComponent<TMP_Text>();
            textRenderer.text = "";

            switch (AdditionalTempData.WinCondition)
            {
                case WinCondition.JesterWin:
                    textRenderer.text = "Jester Wins";
                    textRenderer.color = Jester.Color;
                    break;
                case WinCondition.ExecutionerWin:
                    textRenderer.text = "Executioner Wins";
                    textRenderer.color = Executioner.Color;
                    break;
                case WinCondition.ArsonistWin:
                    textRenderer.text = "Arsonist Wins";
                    textRenderer.color = Arsonist.Color;
                    break;
                case WinCondition.ScavengerWin:
                    textRenderer.text = "Scavenger Wins";
                    textRenderer.color = Scavenger.Color;
                    break;
                case WinCondition.HeadHunterWin:
                    textRenderer.text = "HeadHunter Wins";
                    textRenderer.color = HeadHunter.Color;
                    break;
                case WinCondition.RuthlessRomanticWin:
                    textRenderer.text = "Ruthless Romantic Wins";
                    textRenderer.color = RuthlessRomantic.Color;
                    break;
                case WinCondition.NightmareWin:
                    textRenderer.text = "Nightmare Wins";
                    textRenderer.color = Nightmare.Color;
                    break;
                case WinCondition.PyromaniacWin:
                    textRenderer.text = "Pyromaniac Wins";
                    textRenderer.color = Pyromaniac.Color;
                    break;
                case WinCondition.CrewmateWin:
                    textRenderer.text = "Crewmates Win";
                    textRenderer.color = Palette.CrewmateBlue;
                    SoundManager.instance.StopAllSound();
                    SoundEffectsManager.Forceplay(Sounds.Victory);
                    break;
                case WinCondition.TaskWin:
                    textRenderer.text = "Crewmate Task Win";
                    textRenderer.color = Palette.CrewmateBlue;
                    SoundManager.instance.StopAllSound();
                    SoundEffectsManager.Forceplay(Sounds.Victory);
                    break;
                case WinCondition.PyroAndArsoWin:
                    Color burn = new(200, 68, 0);
                    textRenderer.text = $"</size=100%>The {Helpers.ColorString(Pyromaniac.Color, "Pyromaniac")} and {Helpers.ColorString(Arsonist.Color, "Arsonist")}\n Watch the World {Helpers.ColorString(burn, "BURN!")}</size>";
                    break;
                case WinCondition.RogueImpWin:
                    {
                        CachedPlayerData winner = list[0];
                        CachedPlayerData winner2 = list.Count > 1 ? list[1] : null;
                        string winnerRole = "";
                        string winnerRole2 = "";
                        foreach (AdditionalTempData.PlayerRoleInfo data in AdditionalTempData.PlayerRoles)
                        {
                            RoleInfo role = data.Roles.Where(x => x.FactionId != Faction.Modifier).FirstOrDefault();
                            if (data.PlayerName == winner.PlayerName)
                                winnerRole = role.Name;
                            if (winner2 != null && data.PlayerName == winner2.PlayerName)
                                winnerRole2 = role.Name;
                        }

                        if (winnerRole == "Romantic")
                            winnerRole = winnerRole2;
                        else if (winnerRole2 == "Romantic")
                            winnerRole2 = winnerRole;

                        textRenderer.text = $"{winnerRole} Wins";
                        textRenderer.color = NeutralKiller.Color;
                        break;
                    }
                case WinCondition.RefugeeOnlyWin:
                    {
                        int refugees = Refugee.PlayerToRefugee.Count(pair => !pair.Value.Player.Data.IsDead);
                        if (refugees > 1)
                            textRenderer.text = "The Refugees\nare the last remaining\nsurvivors";
                        else
                            textRenderer.text = "The Refugee\nis the last remaining\nsurvivor";
                        textRenderer.color = Refugee.Color;
                        break;
                    }
                case WinCondition.ImpostorWin:
                    textRenderer.text = "Impostors Win";
                    textRenderer.color = Palette.ImpostorRed;
                    break;
                case WinCondition.TimesUp:
                    textRenderer.text = "Times Up\nImpostors Win";
                    textRenderer.color = Palette.ImpostorRed;
                    break;
                case WinCondition.OxygenWin:
                    textRenderer.text = "Crewmates Forgot How To Breathe";
                    textRenderer.color = Palette.ImpostorRed;
                    break;
                case WinCondition.ReactorWin:
                    textRenderer.text = "Crewmates Were Consumed By Radiation";
                    textRenderer.color = Palette.ImpostorRed;
                    break;
            }
            foreach (WinCondition cond in AdditionalTempData.AdditionalWinConditions)
                if (cond == WinCondition.AdditionalAliveRefugeeWin)
                {
                    if (Refugee.PlayerToRefugee.Count(pair => !pair.Value.Player.Data.IsDead) > 1)
                        textRenderer.text += $"\n{Helpers.ColorString(Refugee.Color, "The Refugees survived")}";
                    else
                        textRenderer.text += $"\n{Helpers.ColorString(Refugee.Color, "The Refugee survived")}";
                }

            if (MapOptions.ShowRoleSummary)
            {
                Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -214f);
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                StringBuilder roleSummaryText = new();
                roleSummaryText.AppendLine("<u><size=120%>End of Game Stats</size></u>");
                roleSummaryText.AppendLine();
                foreach (AdditionalTempData.PlayerRoleInfo data in AdditionalTempData.PlayerRoles)
                {
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
                TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
                roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.fontSizeMin = 1.5f;
                roleSummaryTextMesh.fontSizeMax = 1.5f;
                roleSummaryTextMesh.fontSize = 1.5f;

                RectTransform roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
            }
            AdditionalTempData.Clear();
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static class CheckEndCriteriaPatch
    {
        public static bool Prefix()
        {
            if (!GameData.Instance)
                return false;
            if (DestroyableSingleton<TutorialManager>.InstanceExists) // InstanceExists | Don't check Custom Criteria when in Tutorial
                return true;
            PlayerStatistics statistics = new();
            // TODO: does it really matter if one win condition stops all the others being checked as well
            if (CheckAndEndGameForRefugeeOnly(statistics) ||
                CheckAndEndGameForJesterWin() ||
                CheckAndEndGameForExecutionerWin() ||
                CheckAndEndGameForArsonistWin() ||
                CheckAndEndGameForScavengerWin() ||
                CheckAndEndGameForSabotageWin() ||
                CheckAndEndGameForTaskWin(statistics) ||
                CheckAndEndGameForCrewmateWin(statistics) ||
                CheckAndEndGameForImpostorWin(statistics)) return false;

            CheckAndEndGameForHeadHunterWin(statistics);
            CheckAndEndGameForPyroAndArsoWin(statistics);
            CheckAndEndGameForPyromaniacWin(statistics);
            CheckAndEndGameForRogueImpsWin(statistics);
            CheckAndEndGameForNightmareWin(statistics);
            CheckAndEndGameForRuthlessRomanticWin(statistics);
            CheckAndEndGameForImpostorTimeWin(statistics);

            return false;
        }

        private static bool CheckAndEndGameForJesterWin()
        {
            if (Jester.TriggerJesterWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForExecutionerWin()
        {
            if (Executioner.TriggerExecutionerWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ExecutionerWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForArsonistWin()
        {
            if (Arsonist.TriggerArsonistWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForScavengerWin()
        {
            if (Scavenger.TriggerScavengerWin)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ScavengerWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForSabotageWin()
        {
            if (ShipStatus.Instance.Systems == null)
                return false;

            var systemType = ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp) ? ShipStatus.Instance.Systems[SystemTypes.LifeSupp] : null;
            if (systemType != null)
            {
                LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
                {
                    EndGameForOxygenSabotage();
                    lifeSuppSystemType.Countdown = 10000f;
                    return true;
                }
            }
            bool hasSystem =
                ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Reactor, out ISystemType systemType2) ||
                ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Laboratory, out systemType2) ||
                ShipStatus.Instance.Systems.TryGetValue(SystemTypes.HeliSabotage, out systemType2);
            if (hasSystem)
            {
                ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSystem != null && criticalSystem.Countdown < 0f)
                {
                    EndGameForReactorSabotage();
                    criticalSystem.ClearSabotage();
                    return true;
                }
            }
            return false;
        }

        private static bool CheckAndEndGameForTaskWin(PlayerStatistics statistics)
        {
            if (
                GameData.Instance.TotalTasks > 0 &&
                GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks &&
                (!MapOptions.DeadCrewPreventTaskWin || statistics.TotalCrewAlive > 0)
            )
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TaskWin, false);
                return true;
            }
            return false;
        }

        private static void CheckAndEndGameForHeadHunterWin(PlayerStatistics statistics)
        {
            if (
                statistics.HeadHunterAlive >= statistics.TotalAlive - statistics.HeadHunterAlive &&
                statistics.RogueImpsAlive == 0 &&
                statistics.PyromaniacAlive == 0 &&
                statistics.NightmareAlive == 0 &&
                statistics.RuthlessRomanticAlive == 0 &&
                statistics.TeamImpostorsAlive == 0 &&
                (!MapOptions.JoustingPreventNK || statistics.PowerCrewAlive == 0)
            )
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HeadHunterWin, false);
        }

        private static void CheckAndEndGameForNightmareWin(PlayerStatistics statistics)
        {
            if (
                statistics.NightmareAlive >= statistics.TotalAlive - statistics.NightmareAlive &&
                statistics.RogueImpsAlive == 0 &&
                statistics.PyromaniacAlive == 0 &&
                statistics.HeadHunterAlive == 0 &&
                statistics.RuthlessRomanticAlive == 0 &&
                statistics.TeamImpostorsAlive == 0 &&
                (!MapOptions.JoustingPreventNK || (MapOptions.JoustingPreventNK && statistics.PowerCrewAlive == 0))
            )
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.NightmareWin, false);
        }

        private static void CheckAndEndGameForRuthlessRomanticWin(PlayerStatistics statistics)
        {
            if (statistics.RuthlessRomanticAlive >= statistics.TotalAlive - statistics.RuthlessRomanticAlive &&
                statistics.RogueImpsAlive == 0 &&
                statistics.PyromaniacAlive == 0 &&
                statistics.HeadHunterAlive == 0 &&
                statistics.NightmareAlive == 0 &&
                statistics.TeamImpostorsAlive == 0 &&
                (!MapOptions.JoustingPreventNK || statistics.PowerCrewAlive == 0)
            )
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RuthlessRomanticWin, false);
        }

        private static void CheckAndEndGameForRogueImpsWin(PlayerStatistics statistics)
        {
            if (statistics.RogueImpsAlive == 1 && statistics.TotalAlive - statistics.RogueImpsAlive <= 1 &&
                statistics.HeadHunterAlive == 0 &&
                statistics.PyromaniacAlive == 0 &&
                statistics.NightmareAlive == 0 &&
                statistics.RuthlessRomanticAlive == 0 &&
                statistics.TeamImpostorsAlive == 0 &&
                (!MapOptions.JoustingPreventNK || statistics.PowerCrewAlive == 0)
            )
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RogueImpostorWin, false);
        }

        private static void CheckAndEndGameForPyromaniacWin(PlayerStatistics statistics)
        {
            if (statistics.PyromaniacAlive >= statistics.TotalAlive - statistics.PyromaniacAlive &&
                statistics.RogueImpsAlive == 0 &&
                statistics.NightmareAlive == 0 &&
                statistics.HeadHunterAlive == 0 &&
                statistics.RuthlessRomanticAlive == 0 &&
                statistics.TeamImpostorsAlive == 0 &&
                (!MapOptions.JoustingPreventNK || statistics.PowerCrewAlive == 0)
            )
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PyromaniacWin, false);
        }

        private static void CheckAndEndGameForPyroAndArsoWin(PlayerStatistics statistics)
        {
            if (statistics.PyromaniacAlive == 1 && statistics.ArsonistAlive == 1 && statistics.TotalAlive == 2)
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PyroAndArsoWin, false);
        }

        private static bool CheckAndEndGameForImpostorWin(PlayerStatistics statistics)
        {
            if (
                statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive &&
                statistics.RogueImpsAlive == 0 &&
                statistics.PyromaniacAlive == 0 &&
                statistics.HeadHunterAlive == 0 &&
                statistics.RuthlessRomanticAlive == 0 &&
                statistics.NightmareAlive == 0 &&
                (!MapOptions.JoustingPreventImp || statistics.PowerCrewAlive == 0)
            )
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ImpostorWin, false);
                return true;
            }

            return false;
        }

        private static bool CheckAndEndGameForImpostorTimeWin(PlayerStatistics statistics)
        {
            if (GameTimer.TriggerTimesUpEndGame)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TimesUp, false);
                return true;
            }

            return false;
        }

        private static bool CheckAndEndGameForRefugeeOnly(PlayerStatistics statistics)
        {
            if (statistics.RefugeesAlive > 0 && statistics.RefugeesAlive == statistics.TotalAlive)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RefugeeOnlyWin, false);
                return true;
            }

            return false;

        }

        private static bool CheckAndEndGameForCrewmateWin(PlayerStatistics statistics)
        {
            if (statistics.TotalEvilAlive == 0)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.CrewmateWin, false);
                return true;
            }
            return false;
        }

        private static void EndGameForOxygenSabotage()
        {
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.OxygenWin, false);
        }

        private static void EndGameForReactorSabotage()
        {
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ReactorWin, false);
        }
    }

    public class PlayerStatistics
    {
        public int TeamImpostorsAlive { get; set; }
        public int TotalAlive { get; set; }
        public int HeadHunterAlive { get; set; }
        public int PyromaniacAlive { get; set; }
        public int ArsonistAlive { get; set; }
        public int RogueImpsAlive { get; set; }
        public int NightmareAlive { get; set; }
        public int RuthlessRomanticAlive { get; set; }
        public int RefugeesAlive { get; set; }
        public int PowerCrewAlive { get; set; }
        public int TotalCrewAlive { get; set; }
        public int TotalEvilAlive { get; set; }


        public PlayerStatistics()
        {
            GetPlayerCounts();
        }

        private void GetPlayerCounts()
        {
            TeamImpostorsAlive = 0;
            TotalAlive = 0;
            HeadHunterAlive = 0;
            RogueImpsAlive = 0;
            RefugeesAlive = 0;
            PyromaniacAlive = 0;
            NightmareAlive = 0;
            RuthlessRomanticAlive = 0;
            ArsonistAlive = 0;
            PowerCrewAlive = 0;
            TotalCrewAlive = 0;

            foreach (NetworkedPlayerInfo playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (playerInfo == null || playerInfo.Disconnected || playerInfo.IsDead)
                    continue;
                PlayerControl player = playerInfo.Object;

                if (player == null) continue;
                TotalAlive++;

                if (playerInfo.Role.IsImpostor)
                    TeamImpostorsAlive++;
                if (player.IsCrew())
                    TotalCrewAlive++;

                if (HeadHunter.Player != null && HeadHunter.Player == player)
                    HeadHunterAlive++;
                else if (player.IsRogueImpostor())
                    RogueImpsAlive++;
                else if (player.IsRefugee(out _))
                    RefugeesAlive++;
                else if (player.IsNightmare(out _))
                    NightmareAlive++;
                else if (player.IsRuthlessRomantic(out _))
                    RuthlessRomanticAlive++;
                else if (player.IsPyromaniac(out _))
                    PyromaniacAlive++;
                else if (Arsonist.Player != null && Arsonist.Player == player)
                    ArsonistAlive++;
                else if (Mayor.Player != null && Mayor.Player == player)
                    PowerCrewAlive++;
                else if (Vigilante.Player != null && Vigilante.Player == player)
                    PowerCrewAlive++;
                else if (Sheriff.Player != null && Sheriff.Player == player)
                    PowerCrewAlive++;
                else if (VengefulRomantic.Player != null && VengefulRomantic.Player == player && VengefulRomantic.Target != null && !VengefulRomantic.Target.Data.IsDead)
                    PowerCrewAlive++;
            }
            MapOptions.CrewAlive = TotalCrewAlive;

            TotalEvilAlive = TeamImpostorsAlive + HeadHunterAlive + PyromaniacAlive + RuthlessRomanticAlive + NightmareAlive + RogueImpsAlive;

            if (TeamImpostorsAlive == 0 && RogueImpsAlive == 0 && HeadHunterAlive == 0 && PyromaniacAlive == 0 && NightmareAlive == 0 && RuthlessRomanticAlive == 0 && !Executioner.ConvertsImmediately)
                Executioner.ExecutionerCheckPromotion();
        }
    }
}
