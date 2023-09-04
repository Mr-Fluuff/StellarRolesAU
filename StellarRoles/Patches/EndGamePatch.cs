
using HarmonyLib;
using StellarRoles.Utilities;
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
        ArsonistWin = 11,
        ScavengerWin = 12,
        ExecutionerWin = 14,
        HeadHunterWin = 15,
        RogueImpostorWin = 16,
        RefugeeOnlyWin = 17,
        PyromaniacWin = 18,
        PyroAndArsoWin = 19,
        NightmareWin = 20,
        RuthlessRomanticWin = 21,
        ImpostorWin = 22,
        NobodyWins = 23,
        CrewmateWin = 24,
        OxygenWin = 25,
        ReactorWin = 26,
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
        AdditionalRomanticWin
    }

    public static class AdditionalTempData
    {
        // Should be implemented using a proper GameOverReason in the future
        public static WinCondition WinCondition { get; set; } = WinCondition.Default;
        public static readonly List<WinCondition> AdditionalWinConditions = new();
        public static readonly List<PlayerRoleInfo> PlayerRoles = new();

        public static void Clear()
        {
            PlayerRoles.Clear();
            AdditionalWinConditions.Clear();
            WinCondition = WinCondition.Default;
        }

        public class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public List<RoleInfo> Roles { get; set; }
            public List<RoleInfo> Modifiers { get; set; }
            public bool Loved { get; set; }
            public int TasksCompleted { get; set; }
            public int TasksTotal { get; set; }
            public int Kills { get; set; }
            public int Misfires { get; set; }
            public int CorrectShots { get; set; }
            public int CorrectGuesses { get; set; }
            public int IncorrectGuesses { get; set; }
            public int AbilityKills { get; set; }
            public int Eats { get; set; }

        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch
    {
        private static CustomGameOverReason _GameOverReason;
        public static void Prefix([HarmonyArgument(0)] ref EndGameResult endGameResult)
        {
            _GameOverReason = (CustomGameOverReason)endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10)
                endGameResult.GameOverReason = GameOverReason.ImpostorByKill;

            // Reset zoomed out ghosts
            Helpers.ResetZoom();
        }

        public static void Postfix()
        {
            AdditionalTempData.Clear();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                List<RoleInfo> roles = PlayerGameInfo.GetRoles(player);
                (int tasksCompleted, int tasksTotal) = TasksHandler.TaskInfo(player.Data);

                AdditionalTempData.PlayerRoles.Add(new AdditionalTempData.PlayerRoleInfo()
                {
                    PlayerName = player.Data.PlayerName,
                    Loved =
                        roles.Any(x => x.RoleId == RoleId.Beloved) ||
                        VengefulRomantic.Lover == player ||
                        Romantic.Lover == player ||
                        RuthlessRomantic.IsLover(player),
                    Roles = roles,
                    Modifiers = PlayerGameInfo.GetModifiers(player.PlayerId),
                    TasksTotal = tasksTotal,
                    TasksCompleted = tasksCompleted,
                    Kills = PlayerGameInfo.TotalKills(player.PlayerId),
                    Misfires = PlayerGameInfo.TotalMisfires(player.PlayerId),
                    CorrectShots = PlayerGameInfo.TotalCorrectShots(player.PlayerId),
                    CorrectGuesses = PlayerGameInfo.TotalCorrectGuesses(player.PlayerId),
                    IncorrectGuesses = PlayerGameInfo.TotalIncorrectGuesses(player.PlayerId),
                    AbilityKills = PlayerGameInfo.TotalAbilityKills(player.PlayerId),
                    Eats = roles.Any(x => x.RoleId == RoleId.Scavenger) ? PlayerGameInfo.TotalEaten(player.PlayerId) : 0
                });
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

            List<WinningPlayerData> winnersToRemove = new();

            foreach (WinningPlayerData winner in TempData.winners.GetFastEnumerator())
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName))
                    winnersToRemove.Add(winner);

            foreach (WinningPlayerData winner in winnersToRemove)
                TempData.winners.Remove(winner);

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
            bool crewmateWins = _GameOverReason == CustomGameOverReason.CrewmateWin;
            bool oxygenWin = _GameOverReason == CustomGameOverReason.OxygenWin;
            bool reactorWin = _GameOverReason == CustomGameOverReason.ReactorWin;

            bool isRefugeeLose = !crewmateWins && !refugeeOnlyWin;

            // Executioner win
            if (executionerWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new(Executioner.Player.Data);
                TempData.winners.Add(wpd);
                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Executioner.Player);
                if (additionalWinner != null)
                    TempData.winners.Add(new WinningPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.ExecutionerWin;
            }

            else if (impostorWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (player.Data.Role.IsImpostor)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.ImpostorWin;
            }

            else if (oxygenWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (player.Data.Role.IsImpostor)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.OxygenWin;
            }

            else if (reactorWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (player.Data.Role.IsImpostor)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.ReactorWin;
            }

            // Arsonist win
            else if (arsonistWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new(Arsonist.Player.Data);
                TempData.winners.Add(wpd);
                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Arsonist.Player);
                if (additionalWinner != null)
                    TempData.winners.Add(new WinningPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.ArsonistWin;
            }

            // Scavenger win
            else if (scavengerWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                WinningPlayerData wpd = new(Scavenger.Player.Data);
                TempData.winners.Add(wpd);
                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Scavenger.Player);
                if (additionalWinner != null)
                    TempData.winners.Add(new WinningPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.ScavengerWin;
            }

            else if (headHunterWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                TempData.winners.Add(new WinningPlayerData(HeadHunter.Player.Data));
                AdditionalTempData.WinCondition = WinCondition.HeadHunterWin;
            }

            else if (nightmareWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (player.IsNightmare(out _) && !player.Data.IsDead)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.NightmareWin;
            }

            else if (ruthlessRomanticWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (player.IsRuthlessRomantic(out _) && !player.Data.IsDead)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.RuthlessRomanticWin;
            }

            else if (pyroAndArsoWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if ((player.IsPyromaniac(out _) || (Arsonist.Player == player)) && !player.Data.IsDead)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.PyroAndArsoWin;
            }

            else if (pyromaniacWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (Pyromaniac.PyromaniacDictionary.ContainsKey(player.PlayerId) && !player.Data.IsDead)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.PyromaniacWin;
            }

            else if (crewmateWins)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (player.IsCrew() && !player.IsRefugee(out _))
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.CrewmateWin;
            }


            else if (rogueImpWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    //There should only ever be one winning Neutral Killer;
                    if (NeutralKiller.RogueImps.Contains(player) && !player.Data.IsDead)
                        TempData.winners.Add(new WinningPlayerData(player.Data));
                AdditionalTempData.WinCondition = WinCondition.RogueImpWin;
            }

            // Jester win
            else if (jesterWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                TempData.winners.Add(new WinningPlayerData(Jester.WinningJesterPlayer.Data));

                PlayerControl additionalWinner = Helpers.RomanticWinConditionNeutral(Jester.WinningJesterPlayer);
                if (additionalWinner != null)
                    TempData.winners.Add(new WinningPlayerData(additionalWinner.Data));
                AdditionalTempData.WinCondition = WinCondition.JesterWin;
            }

            else if (refugeeOnlyWin)
            {
                TempData.winners = new Il2CppSystem.Collections.Generic.List<WinningPlayerData>();
                foreach (Refugee refugee in Refugee.PlayerToRefugee.Values)
                    if (refugee.Player != null && !refugee.Player.Data.IsDead && !refugee.NotAckedExiled)
                        TempData.winners.Add(new WinningPlayerData(refugee.Player.Data));
                AdditionalTempData.WinCondition = WinCondition.RefugeeOnlyWin;
            }

            // Possible Additional winner: Romantic
            if ((Romantic.Player != null && Romantic.Lover != null) || Romantic.IsCrewmate || Romantic.IsImpostor)
            {
                WinningPlayerData romantic = null;

                if (Romantic.Lover != null)
                    foreach (WinningPlayerData winner in TempData.winners.GetFastEnumerator())
                    {
                        if (winner.PlayerName == Romantic.Lover.Data.PlayerName)
                            romantic = winner;
                    }
                else
                    foreach (WinningPlayerData winner in TempData.winners.GetFastEnumerator())
                        if ((Romantic.IsCrewmate && !winner.IsImpostor) || (Romantic.IsImpostor && winner.IsImpostor))
                            romantic = winner;

                if (romantic != null)
                {
                    if (!TempData.winners.ToArray().Any(winner => winner.PlayerName == Romantic.Player.Data.PlayerName))
                        TempData.winners.Add(new WinningPlayerData(Romantic.Player.Data));
                    AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalRomanticWin);
                }
            }

            // Possible Additional winner: Vengeful Romantic
            if ((VengefulRomantic.Player != null && VengefulRomantic.Lover != null) || VengefulRomantic.IsDisconnected)
            {
                WinningPlayerData vengefulRomantic = null;
                foreach (WinningPlayerData winner in TempData.winners.GetFastEnumerator())
                    if (winner.PlayerName == VengefulRomantic.Lover.Data.PlayerName)
                        vengefulRomantic = winner;

                if (VengefulRomantic.IsDisconnected)
                    foreach (WinningPlayerData winner in TempData.winners.GetFastEnumerator())
                        if ((VengefulRomantic.IsCrewmate && !winner.IsImpostor) || (VengefulRomantic.IsImpostor && winner.IsImpostor))
                            vengefulRomantic = winner;

                if (vengefulRomantic != null)
                {
                    if (!TempData.winners.ToArray().Any(winner => winner.PlayerName == VengefulRomantic.Player.Data.PlayerName))
                        TempData.winners.Add(new WinningPlayerData(VengefulRomantic.Player.Data));
                    AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalRomanticWin);
                }
            }

            // Possible Additional winner: Beloved
            if (Beloved.Player != null && Beloved.Romantic != null)
            {
                WinningPlayerData winningClient = null;
                foreach (WinningPlayerData winner in TempData.winners.GetFastEnumerator())
                    if (winner.PlayerName == Beloved.Romantic.Data.PlayerName)
                        winningClient = winner;
                if (winningClient != null)
                {
                    if (!TempData.winners.ToArray().Any(winner => winner.PlayerName == Beloved.Player.Data.PlayerName))
                        TempData.winners.Add(new WinningPlayerData(Beloved.Player.Data));
                    AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalRomanticWin);
                }
            }

            if (Refugee.PlayerToRefugee?.Values.Count > 0 && !refugeeOnlyWin && !isRefugeeLose && !TempData.winners.ToArray().Any(x => x.IsImpostor))
            {
                foreach (Refugee refugee in Refugee.PlayerToRefugee.Values)
                {
                    if (refugee.Player != null && !refugee.Player.Data.IsDead && !refugee.NotAckedExiled)
                    {
                        if (!TempData.winners.ToArray().Any(winner => winner.PlayerName == refugee.Player.Data.PlayerName))
                            TempData.winners.Add(new WinningPlayerData(refugee.Player.Data));
                        if (!AdditionalTempData.AdditionalWinConditions.Contains(WinCondition.AdditionalAliveRefugeeWin))
                            AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalAliveRefugeeWin);
                    }
                }
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
            // Delete and readd PoolablePlayers always showing the name and role of the player
            foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>())
                Object.Destroy(pb.gameObject);

            int num = Mathf.CeilToInt(7.5f);
            List<WinningPlayerData> list = TempData.winners.ToArray().OrderBy(b => b.IsYou ? 0 : -1).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                WinningPlayerData winner = list[i];
                int num2 = (i % 2 == 0) ? -1 : 1;
                int num3 = (i + 1) / 2;
                float num4 = (float)num3 / (float)num;
                float num5 = Mathf.Lerp(1f, 0.75f, num4);
                float num6 = (float)((i == 0) ? -8 : -1);
                PoolablePlayer poolablePlayer = Object.Instantiate(__instance.PlayerPrefab, __instance.transform);
                poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;

                float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
                Vector3 vector = new(num7, num7, 1f);
                poolablePlayer.transform.localScale = vector;
                if (winner.IsDead)
                {
                    poolablePlayer.SetBodyAsGhost();
                    poolablePlayer.SetDeadFlipX(i % 2 == 0);
                }
                else
                {
                    poolablePlayer.SetFlipX(i % 2 == 0);
                }
                poolablePlayer.UpdateFromPlayerOutfit(winner, PlayerMaterial.MaskType.ComplexUI, winner.IsDead, true);
                poolablePlayer.cosmetics.nameText.color = Color.white;
                poolablePlayer.cosmetics.nameText.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
                poolablePlayer.cosmetics.nameText.transform.localPosition = new Vector3(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y - 1.2f, -15f);
                poolablePlayer.cosmetics.nameText.text = winner.PlayerName;

                AdditionalTempData.PlayerRoleInfo roleInfo = AdditionalTempData.PlayerRoles.FirstOrDefault(data => data.PlayerName == winner.PlayerName);

                if (roleInfo != null && roleInfo.Roles.Count > 0)
                    poolablePlayer.cosmetics.nameText.text += $"\n{roleInfo.Roles.Select(x => Helpers.ColorString(x.Color, x.Name)).Last()}";
            }

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
                case WinCondition.PyroAndArsoWin:
                    Color burn = new(200, 68, 0);
                    textRenderer.text = $"</size=100%>The {Helpers.ColorString(Pyromaniac.Color, "Pyromaniac")} and {Helpers.ColorString(Arsonist.Color, "Arsonist")}\n Watch the World {Helpers.ColorString(burn, "BURN!")}</size>";
                    break;
                case WinCondition.RogueImpWin:
                    {
                        WinningPlayerData winner = list[0];
                        WinningPlayerData winner2 = list.Count > 1 ? list[1] : null;
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

                    roleSummaryText.AppendLine($"{data.PlayerName}{lover} - {modifiers}{roles}{taskInfo}{killInfo}{misfireInfo}{correctShotsInfo}{correctGuessInfo}{incorrectGuessInfo}{scavengerEaten}");
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
            _ = CheckAndEndGameForRefugeeOnly(statistics) ||
                CheckAndEndGameForJesterWin() ||
                CheckAndEndGameForExecutionerWin() ||
                CheckAndEndGameForArsonistWin() ||
                CheckAndEndGameForScavengerWin() ||
                CheckAndEndGameForSabotageWin() ||
                CheckAndEndGameForTaskWin(statistics) ||
                CheckAndEndGameForCrewmateWin(statistics) ||
                CheckAndEndGameForImpostorWin(statistics);

            CheckAndEndGameForHeadHunterWin(statistics);
            CheckAndEndGameForPyroAndArsoWin(statistics);
            CheckAndEndGameForPyromaniacWin(statistics);
            CheckAndEndGameForRogueImpsWin(statistics);
            CheckAndEndGameForNightmareWin(statistics);
            CheckAndEndGameForRuthlessRomanticWin(statistics);

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
            if (MapUtilities.Systems == null)
                return false;

            Il2CppSystem.Object systemType = MapUtilities.Systems.ContainsKey(SystemTypes.LifeSupp) ? MapUtilities.Systems[SystemTypes.LifeSupp] : null;
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
                MapUtilities.Systems.TryGetValue(SystemTypes.Reactor, out Il2CppSystem.Object systemType2) ||
                MapUtilities.Systems.TryGetValue(SystemTypes.Laboratory, out systemType2);
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
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
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
                (!MapOptions.JoustingPreventNK || (MapOptions.JoustingPreventNK && statistics.PowerCrewAlive == 0))
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
                (!MapOptions.JoustingPreventNK || (MapOptions.JoustingPreventNK && statistics.PowerCrewAlive == 0))
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
                (!MapOptions.JoustingPreventNK || (MapOptions.JoustingPreventNK && statistics.PowerCrewAlive == 0))
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
                (!MapOptions.JoustingPreventNK || (MapOptions.JoustingPreventNK && statistics.PowerCrewAlive == 0))
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
                (!MapOptions.JoustingPreventImp || (MapOptions.JoustingPreventImp && statistics.PowerCrewAlive == 0))
            )
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ImpostorWin, false);
                return true;
            }

            return false;
        }

        private static bool CheckAndEndGameForRefugeeOnly(PlayerStatistics statistics)
        {
            if (statistics.RefugeesAlive == statistics.TotalAlive)
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RefugeeOnlyWin, false);
                return true;
            }

            return false;

        }

        private static bool CheckAndEndGameForCrewmateWin(PlayerStatistics statistics)
        {
            if (
                statistics.TeamImpostorsAlive == 0 &&
                statistics.HeadHunterAlive == 0 &&
                statistics.RogueImpsAlive == 0 &&
                statistics.PyromaniacAlive == 0 &&
                statistics.NightmareAlive == 0 &&
                statistics.RuthlessRomanticAlive == 0
            )
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

            foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (playerInfo.Disconnected || playerInfo.IsDead)
                    continue;
                PlayerControl player = Helpers.PlayerById(playerInfo.PlayerId);

                TotalAlive++;

                if (playerInfo.Role.IsImpostor)
                    TeamImpostorsAlive++;
                if (player.IsCrew())
                    TotalCrewAlive++;

                if (HeadHunter.Player != null && HeadHunter.Player == player)
                    HeadHunterAlive++;
                else if (Helpers.IsRogueImpostor(player.PlayerId))
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

            if (TeamImpostorsAlive == 0 && RogueImpsAlive == 0 && HeadHunterAlive == 0 && PyromaniacAlive == 0 && NightmareAlive == 0 && RuthlessRomanticAlive == 0 && !Executioner.ConvertsImmediately)
                Executioner.ExecutionerCheckPromotion();
        }
    }
}
