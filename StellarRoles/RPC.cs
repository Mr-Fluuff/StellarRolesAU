using AmongUs.Data;
using AmongUs.GameOptions;
using Cpp2IL.Core.Extensions;
using HarmonyLib;
using Hazel;
using StellarRoles.Modules;
using StellarRoles.Objects;
using StellarRoles.Patches;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StellarRoles.GameHistory;
using static StellarRoles.HudManagerStartPatch;
using static StellarRoles.MapOptions;
using static StellarRoles.Patches.FunglePatches;
using static StellarRoles.StellarRoles;

namespace StellarRoles
{
    public static class RPCProcedure
    {
        public static void Send(CustomRPC method, params object[] parameters)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                254,
                SendOption.Reliable
            );

            writer.Write((byte)method);

            foreach (var item in parameters)
            {
                writer.Write(item);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void Write(this MessageWriter writer, object parameter)
        {
            if (parameter is byte @byte)
                writer.Write(@byte);
            else if (parameter is bool @bool)
                writer.Write(@bool);
            else if (parameter is int @int)
                writer.Write(@int);
            else if (parameter is float @float)
                writer.Write(@float);
            else if (parameter is string @string)
                writer.Write(@string);
            else if (parameter is PlayerControl player)
                writer.Write(player.PlayerId);
            else if (parameter is DeadBody body)
                writer.Write(body.ParentId);
            else if (parameter is PlayerVoteArea area)
                writer.Write(area.TargetPlayerId);
            else if (parameter is Vent vent)
                writer.Write(vent.Id);
            else if (parameter is RoleId roleId)
                writer.Write((byte)roleId);
        }

        // Main Controls
        public static void ResetVariables()
        {
            Lantern.ClearLanterns();
            ShadeTrace.ClearTraces();
            MinerVent.ClearMinerVents();
            ClearAndReloadMapOptions();
            ClearAndReloadRoles();
            ClearGameHistory();
            SetCustomButtonCooldowns();
            SetOtherButtonCooldowns();
            Goopy.ClearGoopy();
            Helpers.ResetZoom();
            AdminPatch.ResetData();
            CameraPatch.ResetData();
            DoorLogPatch.ResetData();
            GameStartManagerPatch.GameStartManagerUpdatePatch.StartingTimer = 0;
            GameTimer.TriggerTimesUpEndGame = false;
        }

        public static void HandleShareOptions(byte numberOfOptions, MessageReader reader)
        {
            try
            {
                for (int i = 0; i < numberOfOptions; i++)
                {
                    uint optionId = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    CustomOption option = CustomOption.Options.First(option => option.Id == (int)optionId);
                    option.UpdateSelection((int)selection, i == numberOfOptions - 1);
                }
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Error while deserializing options: " + ex.StackTrace);
            }
        }

        public static void ForceEnd()
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if (!player.Data.Role.IsImpostor)
                {
                    GameData.Instance.GetPlayerById(player.PlayerId); // player.RemoveInfected(); (was removed in 2022.12.08, no idea if we ever need that part again, replaced by these 2 lines.) 

                    player.CoSetRole(RoleTypes.Crewmate, true);
                    player.MurderPlayer(player, MurderResultFlags.Succeeded);
                    player.Data.IsDead = true;
                }
        }

        public static void ResetKillButton(PlayerControl player)
        {
            Helpers.DeleteExtraBodies();

            if (PlayerControl.LocalPlayer != player) return;
            RogueButtons.RogueKillButton.Timer = 0;
            ImpKillButton.Timer = 0;
            HeadHunterButtons.HeadHunterKillButton.Timer = 0;
            PyromaniacButtons.PyromaniacKillButton.Timer = 0;
            SheriffButtons.SheriffKillButton.Timer = 0;
        }

        public static void AddDeadPlayer(PlayerControl player, PlayerControl Killer, DeathReason reason)
        {
            var deadPlayer = new DeadPlayer(player, DateTime.UtcNow, reason, Killer);
            Detective.FreshDeadBodies.Add(deadPlayer);
        }

        public static void SetRole(RoleId roleId, PlayerControl player)
        {
            if (player == null) return;

            switch (roleId)
            {
                case RoleId.Spectator:
                    Spectator.Players.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Spectator);
                    break;

                case RoleId.Jester:
                    _ = new Jester(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Jester);
                    break;

                case RoleId.Medic:
                    Medic.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Medic);
                    break;

                case RoleId.Executioner:
                    Executioner.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Executioner);
                    break;

                case RoleId.Mayor:
                    Mayor.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Mayor);
                    break;

                case RoleId.Engineer:
                    Engineer.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Engineer);
                    break;

                case RoleId.Sheriff:
                    Sheriff.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Sheriff);
                    break;

                case RoleId.Undertaker:
                    Undertaker.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Undertaker);
                    break;

                case RoleId.Investigator:
                    Investigator.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Investigator);
                    break;

                case RoleId.Guardian:
                    Guardian.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Guardian);
                    break;

                case RoleId.Administrator:
                    Administrator.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Administrator);
                    break;

                case RoleId.Watcher:
                    Watcher.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Watcher);
                    break;

                case RoleId.Trapper:
                    Trapper.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Trapper);
                    break;

                case RoleId.Morphling:
                    Morphling.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Morphling);
                    break;

                case RoleId.Camouflager:
                    Camouflager.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Camouflager);
                    break;

                case RoleId.Charlatan:
                    Charlatan.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Charlatan);
                    break;

                case RoleId.Tracker:
                    Tracker.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Tracker);
                    break;

                case RoleId.Vampire:
                    Vampire.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Vampire);
                    break;

                case RoleId.Spy:
                    Spy.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Spy);
                    break;

                case RoleId.Janitor:
                    Janitor.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Janitor);
                    break;

                case RoleId.Warlock:
                    Warlock.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Warlock);
                    break;

                case RoleId.Arsonist:
                    Arsonist.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Arsonist);
                    break;

                case RoleId.Jailor:
                    _ = new Jailor(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Jailor);
                    break;

                case RoleId.ParityCop:
                    _ = new ParityCop(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.ParityCop);
                    break;

                case RoleId.Psychic:
                    Psychic.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Psychic);
                    break;

                case RoleId.Vigilante:
                    Vigilante.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Vigilante);
                    break;

                case RoleId.Scavenger:
                    Scavenger.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Scavenger);
                    break;

                case RoleId.Detective:
                    Detective.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Detective);
                    break;

                case RoleId.Refugee:
                    _ = new Refugee(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Refugee);
                    break;

                case RoleId.Miner:
                    Miner.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Miner);
                    break;

                case RoleId.Follower:
                    Follower.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Follower);
                    break;

                case RoleId.Wraith:
                    Wraith.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Wraith);
                    break;

                case RoleId.Hacker:
                    Hacker.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Hacker);
                    break;

                case RoleId.Shade:
                    Shade.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Shade);
                    break;

                case RoleId.Parasite:
                    Parasite.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Parasite);
                    break;

                case RoleId.Romantic:
                    Romantic.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Romantic);
                    break;

                case RoleId.Beloved:
                    Beloved.Player = player;
                    break;

                case RoleId.Changeling:
                    Changeling.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Changeling);
                    break;

                case RoleId.Bomber:
                    Bomber.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Bomber);
                    break;

                case RoleId.Cultist:
                    Cultist.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Cultist);
                    break;

                // Neutral Killers
                case RoleId.RuthlessRomantic:
                    _ = new RuthlessRomantic(player);
                    NeutralKiller.Players.Add(player);
                    Romantic.PairIsDead = true;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.RuthlessRomantic);
                    break;

                case RoleId.HeadHunter:
                    HeadHunter.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.HeadHunter);
                    break;

                case RoleId.Nightmare:
                    _ = new Nightmare(player);
                    NeutralKiller.Players.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Nightmare);
                    break;

                case RoleId.VengefulRomantic:
                    VengefulRomantic.Player = player;
                    Romantic.PairIsDead = true;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.VengefulRomantic);
                    break;

                case RoleId.MorphlingNK:
                    Morphling.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.MorphlingNeutralKiller);
                    break;

                case RoleId.BomberNK:
                    Bomber.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.BomberNeutralKiller);
                    break;

                case RoleId.CamouflagerNK:
                    Camouflager.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.CamouflagerNeutralKiller);
                    break;

                case RoleId.CharlatanNK:
                    Charlatan.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.CharlatanNeutralKiller);
                    break;

                case RoleId.MinerNK:
                    Miner.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.MinerNeutralKiller);
                    break;

                case RoleId.JanitorNK:
                    Janitor.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.JanitorNeutralKiller);
                    break;

                case RoleId.ShadeNK:
                    Shade.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.ShadeNeutralKiller);
                    break;

                case RoleId.ParasiteNK:
                    Parasite.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.ParasiteNeutralKiller);
                    break;

                case RoleId.UndertakerNK:
                    Undertaker.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.UndertakerNeutralKiller);
                    break;

                case RoleId.VampireNK:
                    Vampire.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.VampireNeutralKiller);
                    break;

                case RoleId.WarlockNK:
                    Warlock.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.WarlockNeutralKiller);
                    break;

                case RoleId.Pyromaniac:
                    _ = new Pyromaniac(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.Pyromaniac);
                    break;

                case RoleId.WraithNK:
                    Wraith.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.WraithNeutralKiller);
                    break;
            }
        }

        public static void SetModifier(RoleId modifierId, PlayerControl player)
        {
            if (player == null) return;

            switch (modifierId)
            {
                case RoleId.Sleepwalker:
                    Sleepwalker.Players.Add(player);
                    PlayerGameInfo.AddModifier(player.PlayerId, RoleInfo.Sleepwalker);
                    break;
                case RoleId.Mini:
                    Mini.Player = player;
                    break;
                case RoleId.Giant:
                    Giant.Player = player;
                    break;
                case RoleId.Assassin:
                    Assassin.Players.Add(player);
                    break;
                case RoleId.Spiteful:
                    _ = new Spiteful(player);
                    PlayerGameInfo.AddModifier(player.PlayerId, RoleInfo.Spiteful);
                    break;
                case RoleId.Gopher:
                    Gopher.Players.Add(player);
                    PlayerGameInfo.AddModifier(player.PlayerId, RoleInfo.Gopher);
                    break;
                case RoleId.Sniper:
                    Sniper.Players.Add(player);
                    PlayerGameInfo.AddModifier(player.PlayerId, RoleInfo.Sniper);
                    break;
                case RoleId.Clutch:
                    Clutch.Players.Add(player);
                    PlayerGameInfo.AddModifier(player.PlayerId, RoleInfo.Clutch);
                    break;
                case RoleId.Ascended:
                    Ascended.Players.Add(player);
                    PlayerGameInfo.AddModifier(player.PlayerId, RoleInfo.Ascended);
                    break;
            }
        }

        public static void SendMessage(PlayerControl player, string message)
        {
            HudManager.Instance.Chat.AddChat(player, message);
        }

        public static void SetRandomId(string message)
        {
            AdditionalTempData.UniqueGameID = message;
        }

        public static void Duel(PlayerControl sender, string message)
        {
            if (message.Length == 0)
                return;

            string[] messageParts = message.ToLower().Split(" ");
            if (messageParts.Length < 1)
                return;

            RPS entry = RockPaperScissorsGame.ExtractEntry(messageParts[^1]);
            if (entry == RPS.None)
                return;

            string name = string.Join(" ", messageParts[..(messageParts.Length - 1)]);
            PlayerControl target = Helpers.PlayerByName(name);
            if (target != null && RockPaperScissorsGame.FetchPartner(target) != null)
            {
                if (sender.AmOwner)
                    SendMessage(sender, "Player Allready In Duel");
                return;
            }
            PlayerControl player = RockPaperScissorsGame.FetchPartner(sender) ?? target;
            if (player == null || player == sender)
            {
                if (sender.AmOwner)
                {
                    if (player == sender)
                        SendMessage(PlayerControl.LocalPlayer, "You Cannot Duel Yourself");
                    else
                        SendMessage(PlayerControl.LocalPlayer, "This Is Not The Duel Your Looking For");
                }
                return;
            }

            RockPaperScissorsGame duel = RockPaperScissorsGame.FetchDuel(sender, player);
            if (duel == null)
            {
                if (player.AmOwner)
                {
                    SendMessage(sender, "It's Time to Duel!");
                    SendMessage(sender, "Please respond with /duel {rock, paper, scissors}");
                }
                _ = new RockPaperScissorsGame
                {
                    PlayerOne = sender,
                    PlayerTwo = player,
                    PlayerOneRPS = entry
                };
                return;
            }
            else
                duel.PlayerTwoRPS = entry;

            duel.Winner = duel.Duel();
        }

        public static void DuelResults()
        {
            RockPaperScissorsGame rockPaperScissorsGame = RockPaperScissorsGame.FetchDuel(PlayerControl.LocalPlayer);
            if (rockPaperScissorsGame == null || !rockPaperScissorsGame.IsComplete) return;

            if (!rockPaperScissorsGame.PlayerOne.AmOwner || !rockPaperScissorsGame.PlayerTwo.AmOwner) return;

            string entries = $"DUEL: {rockPaperScissorsGame.PlayerOneRPS} VS {rockPaperScissorsGame.PlayerTwoRPS} RESULT: ";
            if (rockPaperScissorsGame.IsDraw)
                entries += "Draw";
            if (rockPaperScissorsGame.Winner != null)
                entries += rockPaperScissorsGame.Winner.name + " Wins";

            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, entries);
        }

        public static void RemoveCompletedDuels(PlayerControl player)
        {
            if (player == null) return;
            RockPaperScissorsGame game = RockPaperScissorsGame.FetchDuel(player);
            if (game?.IsComplete == true)
                RockPaperScissorsGame.Games.Remove(game);
        }

        public static void ConcealBody(byte bodyId)
        {
            Charlatan.ConcealedBodies.Add(bodyId);
            foreach (var dp in GameHistory.DeadPlayers)
            {
                if (dp.Data.PlayerId == bodyId)
                {
                    dp.Tampered = true;
                }
            }
        }

        public static void UpdatePlayerCount(byte team, int count)
        {
            switch (team)
            {
                case (byte)PlayerCount.AllPlayers:
                    MapOptions.PlayersAlive = count;
                    break;
                case (byte)PlayerCount.ImpsAlive:
                    MapOptions.ImpsAlive = count;
                    break;
                case (byte)PlayerCount.CrewAlive:
                    MapOptions.CrewAlive = count;
                    break;

            }
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            System.Version ver;
            if (revision < 0)
                ver = new System.Version(major, minor, build);
            else
                ver = new System.Version(major, minor, build, revision);
            GameStartManagerPatch.PlayerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static void AddSpectator(PlayerControl player)
        {
            if (player == null) return;
            Spectator.ToBecomeSpectator.Add(player.PlayerId);
            player.cosmetics.nameText.color = Color.gray;
        }

        public static void RemoveSpectator(PlayerControl player)
        {
            if (player == null) return;
            Spectator.ToBecomeSpectator.Remove(player);
            player.cosmetics.nameText.color = Color.white;
        }

        public static void AddGameInfo(PlayerControl player, InfoType info)
        {
            if (!PlayerGameInfo.Mapping.TryGetValue(player.PlayerId, out PlayerGameInfo gameInfo))
                PlayerGameInfo.Mapping.Add(player.PlayerId, gameInfo = new());

            switch (info)
            {
                case InfoType.AddKill:
                    gameInfo.Kills++;
                    break;
                case InfoType.AddCorrectShot:
                    gameInfo.CorrectShots++;
                    break;
                case InfoType.AddMisfire:
                    gameInfo.Misfires++;
                    break;
                case InfoType.AddCorrectGuess:
                    gameInfo.CorrectGuesses++;
                    break;
                case InfoType.AddIncorrectGuess:
                    gameInfo.IncorrectGuesses++;
                    break;
                case InfoType.AddAbilityKill:
                    gameInfo.AbilityKills++;
                    break;
                case InfoType.AddEat:
                    gameInfo.ScavengerEats++;
                    break;
                case InfoType.AddCorrectVote:
                    gameInfo.CorrectVotes++;
                    break;
                case InfoType.AddIncorrectVote:
                    gameInfo.IncorrectVotes++;
                    break;
                case InfoType.AddIncorrectEject:
                    gameInfo.IncorrectEjects++;
                    break;
                case InfoType.AddCorrectEject:
                    gameInfo.CorrectEjects++;
                    break;
                case InfoType.AddCrewmatesEjected:
                    gameInfo.CrewmatesEjected++;
                    break;
                case InfoType.PlayerDiedBeforeLastMeeting:
                    gameInfo.PlayerAlive = false;
                    break;
                case InfoType.FirstTwoPlayersDead:
                    gameInfo.firstTwoPlayersDead = true;
                    break;
                case InfoType.CriticalMeetingError:
                    gameInfo.criticalMeetingError = true;
                    break;
                case InfoType.CritcalMeetingErrorReverse:
                    gameInfo.criticalMeetingError = false;
                    break;
            }
        }

        public static void UpdateSurvivability(PlayerControl player)
        {
            Helpers.Log($"Surviveability {player.name} : {MapOptions.PlayersAlive}");
            if (!PlayerGameInfo.Mapping.TryGetValue(player.PlayerId, out PlayerGameInfo gameInfo))
                PlayerGameInfo.Mapping.Add(player.PlayerId, gameInfo = new());
            gameInfo.survivability = MapOptions.PlayersAlive;
        }

        public static void UpdateTasks(PlayerControl player)
        {
            Helpers.Log($"Surviveability {player.name} : {MapOptions.PlayersAlive}");
            if (!PlayerGameInfo.Mapping.TryGetValue(player.PlayerId, out PlayerGameInfo gameInfo))
                PlayerGameInfo.Mapping.Add(player.PlayerId, gameInfo = new());
            gameInfo.survivability = MapOptions.PlayersAlive;
        }

        public static void UncheckedMurderPlayer(PlayerControl source, PlayerControl target, bool showAnimation, bool bombkill)
        {
            if (!Helpers.GameStarted)
                return;

            if (!showAnimation)
                KillAnimationCoPerformKillPatch.HideNextAnimation = true;

            if (Romantic.Player != null && Romantic.HasLover && Romantic.Lover == target)
                VengefulRomantic.Target = source.IsBombed(out Bombed bombed) ? bombed.Bomber : source;

            if (Medic.Target == target && Medic.Player != null && !Medic.Player.Data.IsDead)
                MedicHeartMonitorFlash(source, bombkill);

            source.MurderPlayer(target, MurderResultFlags.Succeeded);
        }

        public static void ParalyzePlayer(Nightmare nightmare, PlayerControl target)
        {
            if (target.AmOwner)
            {
                Helpers.ResetVentBug();
                Helpers.SetMovement(false);
                Helpers.ShowFlash(Color.black, 1.5f);
                _ = new CustomMessage("The Nightmare Paralyzed You!", Nightmare.ParalyzeRootTime, true, Nightmare.Color);
            }

            nightmare.ParalyzedPlayer = target;

            Nightmare.ParalyzeRootTime.DelayedAction(() =>
            {
                if (target.AmOwner && target != Parasite.Controlled)
                    Helpers.SetMovement(true);
                nightmare.ParalyzedPlayer = null;
            });
        }

        public static void SetGameStarting()
        {
            GameStartManagerPatch.GameStartManagerUpdatePatch.StartingTimer = 5f;
        }

        public static void TurnToCrewmate(PlayerControl player)
        {
            ErasePlayerRoles(player, false, true);
            player.roleAssigned = false;
            DestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
            player.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            player.roleAssigned = true;
            foreach (PlayerControl otherPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if (!otherPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                    player.cosmetics.nameText.color = Palette.White;
        }

        public static void ChangelingChange(RoleId role)
        {
            PlayerControl player = Changeling.Player;
            bool wasChangeling = player.AmOwner;
            Changeling.ClearAndReload();
            SetRole(role, player);

            if (role == RoleId.Follower)
            {
                Cultist.FollowerSpecialRoleAssigned = false;
            }

            if (!wasChangeling) return;

            foreach (CustomButton button in CustomButton.Buttons)
                if (button == WarlockButtons.CurseButton)
                    button.Timer = 10f;
                else if (button != HelpButton && button != ImpKillButton)
                    button.Timer = 3f;
        }

        public static void EngineerFixLights()
        {
            var switchminigame = Minigame.Instance as SwitchMinigame;
            if (switchminigame != null) switchminigame.Close();

            SwitchSystem switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
        }

        public static void CleanBody(byte playerId)
        {
            DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(body => body.ParentId == playerId);
            if (body != null)
                CleanBody(body);
        }

        public static void CleanBody(DeadBody body)
        {
            UnityEngine.Object.Destroy(body.gameObject);
        }

        public static void DragBody(byte playerId)
        {
            DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(body => body.ParentId == playerId);
            if (body != null)
                Undertaker.DeadBodyDragged = body;
            foreach (var dp in GameHistory.DeadPlayers)
            {
                if (dp.Data.PlayerId == Undertaker.DeadBodyDragged.ParentId)
                {
                    dp.Tampered = true;
                }
            }
        }

        public static void CultistCreateImposter(PlayerControl player)
        {
            if (player == null) return;
            List<RoleInfo> roles = PlayerGameInfo.GetRoles(player);
            CreateImpostor(player);
            Cultist.NeedsFollower = false;

            RoleInfo roleInfo = roles.First();
            RoleId roleId = Cultist.CultistFollowerRole(roleInfo.RoleId);

            if (roleId != RoleId.Follower && Cultist.FollowerImpRolesEnabled)
            {
                Cultist.FollowerSpecialRoleAssigned = true;
                SetRole(roleId, player);
                Follower.Player = player;
            }
            else
            {
                SetRole(RoleId.Follower, player);
            }

            if (player == Executioner.Target)
                Executioner.Target = null;
            if (Follower.GetsAssassin)
                Assassin.Players.Add(player);
            if (player == Romantic.Lover)
            {
                Romantic.ResetAlignment();
                Romantic.IsImpostor = true;
            }
            if (player.AmOwner)
                ImpKillButton.Timer = 0;
        }

        public static void CreateImpostor(PlayerControl player)
        {
            if (player == null) return;
            ErasePlayerRoles(player, true);
            player.TurnToImpostor();
        }

        public static void ResetAnimation(PlayerControl player)
        {
            PlayerPhysics playerPhysics = player.MyPhysics;
            player.onLadder = false;
            playerPhysics.ResetAnimState();
        }

        public static void FallInLove(PlayerControl player)
        {
            Romantic.HasLover = true;
            Romantic.Lover = player;
            Arrow arrow = Romantic.Arrow = new Arrow(Romantic.Color);
            arrow.Object.SetActive(false);
            Romantic.NeutralSided = player.IsNeutral();

            // TODO: make all these getters ....
            Romantic.IsArsonist = player == Arsonist.Player;
            Romantic.IsScavenger = player == Scavenger.Player;
            Romantic.IsExecutioner = player == Executioner.Player;
            Romantic.IsHeadHunter = player == HeadHunter.Player;

            Romantic.IsJester = player.IsJester(out _);
            Romantic.IsPyromaniac = player.IsPyromaniac(out _);

            bool isImpostor = Romantic.IsImpostor = player.Data.Role.IsImpostor;
            Romantic.IsCrewmate = !isImpostor && !player.IsNeutral();
        }

        public static void GuardianSetShielded(PlayerControl shielded)
        {
            Guardian.UsedShield = true;
            Guardian.Shielded = shielded;
            Guardian.ShieldVisibilityTimer = 0f;
        }

        public static void GuardianResetShield()
        {
            Guardian.UsedShield = false;
            Guardian.Shielded = null;
        }

        public static void MedicSetHearMonitor(PlayerControl player)
        {
            Medic.Target = player;
            Medic.UsedHeartMonitor = true;
        }

        public static void MedicHeartMonitorFlash(PlayerControl killer, bool bombkill)
        {
            if (Medic.Player == null || Medic.Target == null || MedicAbilities.isRoleBlocked()) return;

            if (Medic.Player.AmOwner)
            {
                Helpers.ShowFlash(Medic.Color, 1.5f);
                _ = new CustomMessage("Your Target Died", 3f, true, Medic.Color);
            }

            if (killer.AmOwner && !killer.IsCrew() && Medic.NonCrewFlash && !bombkill)
            {
                Medic.NonCrewFlashDelay.DelayedAction(() =>
                {
                    Helpers.ShowFlash(Medic.Color, 1.5f);
                    _ = new CustomMessage($"You Killed The Medic Target {Medic.NonCrewFlashDelay} Seconds Ago", 3f, true, Medic.Color);
                });
            }
        }

        public static void JailorJail(Jailor jailor, PlayerControl target)
        {
            MeetingHud meetingHud = MeetingHud.Instance;
            if (!meetingHud)
                return;

            if (PlayerControl.LocalPlayer.CanGuess() && MeetingHudPatch.GuesserUI != null)
            {
                MeetingHudPatch.GuesserUIExitButton.OnClick.Invoke();
            }

            jailor.Target = target;
            jailor.HasJailed = true;

            foreach (PlayerVoteArea voteArea in meetingHud.playerStates)
            {
                Transform child = voteArea.transform.FindChild("JailIcon");
                if (child != null)
                    UnityEngine.Object.Destroy(child.gameObject);
            }
            AddJailedPlayerIcon(target.PlayerId, jailor);
        }

        public static void MorphlingMorph(PlayerControl target)
        {
            if (Morphling.Player == null) return;

            Morphling.MorphTimer = Morphling.Duration;
            Morphling.MorphTarget = target;
            if (Camouflager.CamouflageTimer <= 0f)
            {
                SetLook(Morphling.Player, target);
            }
        }

        public static void ControlPlayer(PlayerControl target)
        {
            Parasite.ControlTimer = Parasite.ControlDuration;
            ParasiteAbilites.timer = 0;
            Parasite.Position = target.transform.position;
            Parasite.Controlled = target;
            if (Parasite.Controlled.inVent)
            {
                Parasite.Controlled.MyPhysics.ExitAllVents();
            }
            if (Camouflager.CamouflageTimer <= 0 && !PlayerControl.LocalPlayer.IsMushroomMixupActive())
            {
                SetLook(Parasite.Controlled, Parasite.Player);
            }
            if (PlayerControl.LocalPlayer == Parasite.Controlled)
            {
                Helpers.SetMovement(false);
                new CustomMessage("You are being Controlled", 1f, true, Palette.ImpostorRed);
            }
        }

        public static void KillInfected(bool kill)
        {
            if (Parasite.Controlled == null || Parasite.Player == null) return;
            if (Parasite.Controlled.AmOwner)
            {
                Helpers.SetMovement(true);
            }
            if (kill)
            {
                var bodyid = Parasite.Controlled.PlayerId;
                UncheckedMurderPlayer(Parasite.Player, Parasite.Controlled, false, false);
                if (Parasite.Player.AmOwner)
                {
                    Helpers.RPCAddGameInfo(PlayerControl.LocalPlayer, InfoType.AddAbilityKill, InfoType.AddKill);
                    Helpers.PlayerKilledByAbility(Parasite.Controlled);
                }
                foreach (var dp in GameHistory.DeadPlayers)
                {
                    if (dp.Data.PlayerId == bodyid)
                    {
                        dp.Tampered = true;
                    }
                }
            }
            Parasite.Controlled.SetDefaultLook();
            if (Parasite.Controlled.IsMushroomMixupActive() && Helpers.IsMap(Map.Fungal))
                Parasite.Controlled.MixUpOutfit(MixUpOutfitPatch1.outfits[Parasite.Controlled.PlayerId]);

            Parasite.Controlled = null;

            if (Parasite.Player.AmOwner)
            {
                ParasiteButtons.InfestButton.Timer = Parasite.InfestCooldown * Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                Helpers.SetKillerCooldown();
                ParasiteAbilites.DestroyAssets();
            }
        }


        public static void SetLook(PlayerControl target, PlayerControl target2)
        {
            target.SetLook(target2.Data.PlayerName, target2.Data.DefaultOutfit.ColorId, target2.Data.DefaultOutfit.HatId, target2.Data.DefaultOutfit.VisorId, target2.Data.DefaultOutfit.SkinId, target2.Data.DefaultOutfit.PetId);
        }

        public static void CamouflagerCamouflage()
        {
            Camouflager.CamouflageTimer = Camouflager.Duration;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player.SetLook("", 6, "", "", "", "");
            }
        }

        public static void VampireSetBitten(byte targetId)
        {
            if (Vampire.Player == null) return;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if (player.PlayerId == targetId && !player.Data.IsDead)
                    Vampire.Bitten = player;
        }

        public static void RefugeeShield(Refugee refugee)
        {
            refugee.IsVestActive = true;
            Refugee.VestDuration.DelayedAction(() =>
            {
                refugee.IsVestActive = false;
            });
        }

        public static void RuthlessRomanticShield(RuthlessRomantic romantic)
        {
            romantic.IsVestActive = true;
            RuthlessRomantic.VestDuration.DelayedAction(() =>
            {
                romantic.IsVestActive = false;
            });
        }

        public static void RomanticShield()
        {
            Romantic.IsVestActive = true;
            Romantic.VestDuration.DelayedAction(() =>
            {
                Romantic.IsVestActive = false;
            });
        }

        public static void TrackerMarkPlayer(PlayerControl player)
        {
            Tracker.TrackedPlayers.Add(player);

            if (PlayerControl.LocalPlayer == Tracker.Player)
            {
                Tracker.UsedTracker = true;
                Arrow arrow = new(Tracker.AnonymousArrows ? Tracker.Color : player.Data.Color);
                arrow.Object.SetActive(false);
                Tracker.TrackedPlayerLocalArrows.Add(player.PlayerId, arrow);
                Tracker.NumberOfTracks--;
            }
        }

        public static void TrackerTrackWarning()
        {
            foreach (PlayerControl player in Tracker.TrackedPlayers.GetPlayerEnumerator())
            {
                if (player.AmOwner && !player.IsCrew())
                {
                    Tracker.TimeLeft = (int)Tracker.DelayDuration;
                    Helpers.ShowFlash(Tracker.Color, 1.5f);
                    HudManager.Instance.StartCoroutine(Effects.Lerp(Tracker.DelayDuration, new Action<float>((p) =>
                    {
                        int timeLeft = (int)(Tracker.DelayDuration - (Tracker.DelayDuration * p));
                        if (timeLeft <= Tracker.DelayDuration && Tracker.TimeLeft != timeLeft)
                        {
                            _ = new CustomMessage($"You Will Be Tracked In {timeLeft} Seconds", 1f, true, Tracker.Color);
                            Tracker.TimeLeft = timeLeft;
                        }
                        if (p == 1f)
                        {
                            TrackerBeginTrack();
                        }
                    })));
                }
            }
        }

        public static void TrackerBeginTrack()
        {
            if (!PlayerControl.LocalPlayer.AmOwner) return;

            Tracker.TimeLeft = (int)Tracker.CalculateTrackDuration();
            HudManager.Instance.StartCoroutine(Effects.Lerp(Tracker.CalculateTrackDuration(), new Action<float>((p) =>
            {
                int timeLeft = (int)(Tracker.CalculateTrackDuration() - (Tracker.CalculateTrackDuration() * p));
                if (timeLeft <= Tracker.CalculateTrackDuration() && Tracker.TimeLeft != timeLeft)
                {
                    _ = new CustomMessage($"Tracking you for {timeLeft} Seconds Remaining", 1f, true, Tracker.Color);
                    Tracker.TimeLeft = timeLeft;
                }
            })));
        }

        public static void ArsonistDouse(PlayerControl player)
        {
            Arsonist.DousedPlayers.Add(player);
        }

        public static void ScavengerEat(byte targetId)
        {
            CleanBody(targetId);
            Scavenger.EatenBodies++;
        }

        public static void ErasePlayerRoles(PlayerControl player, bool ignoreModifier = true, bool eraseHistory = false)
        {
            // Crewmate roles
            if (player == Mayor.Player)
                Mayor.ClearAndReload();
            else if (player == Engineer.Player)
                Engineer.ClearAndReload();
            else if (player == Sheriff.Player)
                Sheriff.ClearAndReload();
            else if (player == Investigator.Player)
                Investigator.ClearAndReload();
            else if (player == Guardian.Player)
                Guardian.ClearAndReload();
            else if (player == Tracker.Player)
                Tracker.ClearAndReload();
            else if (player == Spy.Player)
                Spy.ClearAndReload();
            else if (player == Administrator.Player)
                Administrator.ClearAndReload();
            else if (player == Trapper.Player)
                Trapper.ClearAndReload();
            else if (player == Detective.Player)
                Detective.ClearAndReload();
            else if (player == Medic.Player)
                Medic.ClearAndReload();
            else if (player == Watcher.Player)
                Watcher.ClearAndReload();
            else if (player == Vigilante.Player)
                Vigilante.ClearAndReload();
            else if (player.IsParityCop(out _))
                ParityCop.ParityCopDictionary.Remove(player.PlayerId);
            else if (player == Psychic.Player)
                Psychic.ClearAndReload();

            // Impostor roles
            else if (player == Bomber.Player)
                Bomber.ClearAndReload();
            else if (player == Camouflager.Player)
                Camouflager.ClearAndReload();
            else if (player == Changeling.Player)
                Changeling.ClearAndReload();
            else if (player == Cultist.Player)
                Cultist.ClearAndReload();
            else if (player == Follower.Player)
                Follower.ClearAndReload();
            else if (player == Janitor.Player)
                Janitor.ClearAndReload();
            else if (player == Miner.Player)
                Miner.ClearAndReload();
            else if (player == Morphling.Player)
                Morphling.ClearAndReload();
            else if (player == Shade.Player)
                Shade.ClearAndReload();
            else if (player == Parasite.Player)
                Parasite.ClearAndReload();
            else if (player == Undertaker.Player)
                Undertaker.ClearAndReload();
            else if (player == Vampire.Player)
                Vampire.ClearAndReload();
            else if (player == Warlock.Player)
                Warlock.ClearAndReload();
            else if (player == Wraith.Player)
                Wraith.ClearAndReload();
            else if (player == Hacker.Player)
                Hacker.ClearAndReload();

            // Other roles
            else if (player.IsJester(out _))
                Jester.RemoveJester(player.PlayerId);
            else if (player == Executioner.Player)
                Executioner.ClearAndReload();
            else if (player == Arsonist.Player)
                Arsonist.ClearAndReload();
            else if (player == Scavenger.Player)
                Scavenger.ClearAndReload();
            else if (player == Romantic.Player)
                Romantic.ClearAndReload();
            else if (player == VengefulRomantic.Player)
                VengefulRomantic.ClearAndReload();
            else if (player == Beloved.Player)
                Beloved.ClearAndReload();
            else if (player == HeadHunter.Player)
                HeadHunter.ClearAndReload();
            else if (player.IsRefugee(out _))
                Refugee.PlayerToRefugee.Remove(player.PlayerId);
            else if (player.IsRuthlessRomantic(out _))
                RuthlessRomantic.PlayerToRuthlessRomantic.Remove(player.PlayerId);
            else if (player.IsJailor(out _))
                Jailor.PlayerIdToJailor.Remove(player.PlayerId);
            else if (player.IsNightmare(out _))
                Nightmare.PlayerToNightmare.Remove(player.PlayerId);
            else if (player.IsPyromaniac(out _))
                Pyromaniac.PyromaniacDictionary.Remove(player.PlayerId);

            if (player.IsNeutralKiller())
                NeutralKiller.Players.Remove(player);


            // Modifier
            if (!ignoreModifier)
            {
                Sleepwalker.Players.RemoveAll(x => x == player.PlayerId);
                Assassin.Players.RemoveAll(x => x == player.PlayerId);
            }

            if (eraseHistory)
            {
                PlayerGameInfo.EraseHistory(player);
            }
        }

        public static void GiveBomb(PlayerControl player, PlayerControl bomber, bool reset)
        {
            if (reset)
            {
                Bombed.BombedDictionary.Remove(player.PlayerId);
                return;
            }

            Bombed bombed = new(player, bomber);
            Bombed.BombDelay.DelayedAction(() =>
            {
                if (!MeetingHud.Instance)
                {
                    bombed.BombActive = true;
                    if (bombed.Player.AmOwner)
                    {
                        bombed.AlertBombed(Bomber.CalculateBombTimer());
                        Helpers.ShowFlash(Bombed.AlertColor, 1f);
                    }
                }

            });
        }

        public static void PassBomb(byte bombedPlayerId, byte targetId, int timeLeft)
        {
            if (!Bombed.IsBombed(bombedPlayerId, out Bombed oldBombed)) return;

            oldBombed.PassedBomb = true;
            PlayerControl player = oldBombed.Player;
            PlayerControl bomber = oldBombed.Bomber;
            PlayerControl target = Helpers.PlayerById(targetId);
            Bombed.BombedDictionary.Remove(bombedPlayerId);

            Bombed bombed = new(target, bomber)
            {
                TimeLeft = timeLeft,
                BombActive = true,
                LastBombed = player
            };

            if (target.AmOwner)
            {
                bombed.AlertBombed(timeLeft);
                Helpers.ShowFlash(Bombed.AlertColor, 1f);
            }
        }

        public static void PlaceShadeTrace(byte[] buffer)
        {
            _ = new ShadeTrace(ConvertPosition(buffer), Shade.EvidenceDuration);
        }

        public static void SetInvisible(PlayerControl target, bool reset, bool fungle = false)
        {
            if (reset)
            {
                target.SetPlayerAlpha(1);
                if (Camouflager.CamouflageTimer <= 0)
                {
                    if (target == Parasite.Controlled && Parasite.Player != null)
                    {
                        SetLook(target, Parasite.Player);
                    }
                    target.SetDefaultLook();
                }
                if (target == Wraith.Player)
                {
                    Wraith.InvisibleTimer = 0f;
                    Wraith.IsInvisible = false;
                }
                else if (target == Shade.Player)
                {
                    Shade.InvisibleTimer = 0f;
                    Shade.IsInvisble = false;
                }

                if (target.IsMushroomMixupActive() && Helpers.IsMap(Map.Fungal))
                    target.MixUpOutfit(MixUpOutfitPatch1.outfits[target.PlayerId]);
                return;
            }

            //target.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, "", "", target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);

            float alpha = 0f;
            if (PlayerControl.LocalPlayer.Data.IsDead || target.AmOwner) alpha = 0.5f;
            target.SetPlayerAlpha(alpha);
            if (target == Wraith.Player)
            {
                if (!fungle)
                {
                    Wraith.InvisibleTimer = Wraith.InvisibleDuration;
                }
                Wraith.IsInvisible = true;
            }
            else if (target == Shade.Player)
            {
                if (!fungle)
                {
                    Shade.InvisibleTimer = Shade.CalculateShadeDuration();
                }
                Shade.IsInvisble = true;
            }
        }

        public static void WraithReturn()
        {
            if (Wraith.Player.AmOwner)
            {
                Vector3 pos = Lantern.CurrentLantern.LanternGameObject.transform.position;
                if (SubmergedCompatibility.IsSubmerged)
                    SubmergedCompatibility.ChangeFloor(pos.y > -7);
                Wraith.Player.NetTransform.RpcSnapTo(pos);
            }
            Lantern.CurrentLantern.LanternGameObject.SetActive(false);
            UnityEngine.Object.Destroy(Lantern.CurrentLantern.LanternGameObject);
            Lantern.CurrentLantern = null;
        }

        public static void MeetingStart()
        {
            if (Shade.Player != null && Shade.IsInvisble)
                SetInvisible(Shade.Player, true);
            if (Wraith.Player != null && Wraith.IsInvisible)
                SetInvisible(Wraith.Player, true);

            10f.DelayedAction(() =>
            {
                foreach (DeadBody deadBody in UnityEngine.Object.FindObjectsOfType<DeadBody>())
                {
                    UnityEngine.Object.Destroy(deadBody.gameObject);
                }

            });
        }

        public static void Mine(int ventId, byte[] buffer, float zAxis)
        {
            Vector3 position = ConvertPosition(buffer);

            Vent ventPrefab = UnityEngine.Object.FindObjectOfType<Vent>();
            Vent vent = UnityEngine.Object.Instantiate(ventPrefab, ventPrefab.transform.parent);
            vent.Id = ventId;
            vent.transform.position = new Vector3(position.x, position.y, zAxis);

            if (Miner.Vents.Count > 0)
            {
                Vent leftVent = Miner.Vents[^1];
                vent.Left = leftVent;
                leftVent.Right = vent;
            }
            else
            {
                vent.Left = null;
            }
            vent.Right = null;
            vent.Center = null;
            List<Vent> allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();
            Miner.Vents.Add(vent);
            if (SubmergedCompatibility.IsSubmerged)
            {
                vent.gameObject.layer = 12;
                vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover); // just in case elevator vent is not blocked
                Transform transform = vent.gameObject.transform;
                if (transform.position.y > -7)
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0.03f);
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0.0009f);
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.003f);
                }
            }
        }

        private static Vector3 ConvertPosition(byte[] buffer)
        {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buffer, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buffer, 1 * sizeof(float));
            return position;
        }

        public static void PlaceMinerVent(byte[] buffer)
        {
            _ = new MinerVent(ConvertPosition(buffer));
        }

        public static void PlaceLantern(byte[] buffer)
        {
            _ = new Lantern(ConvertPosition(buffer));
        }

        public static void PlaceSensor(byte[] buffer)
        {
            Sensor sensor = new(ConvertPosition(buffer));
            if (!Sensor.Sensors.ContainsKey(sensor.Id))
                Sensor.Sensors.Add(sensor.Id, sensor);
        }

        public static void TripSensor(PlayerControl player)
        {
            Watcher.TrackedPlayers.Add(player);

            if (Watcher.Player == PlayerControl.LocalPlayer)
            {
                Helpers.ShowFlash(Watcher.Color, 1.5f);
                _ = new CustomMessage("Sensor Tripped", 3f, true, Watcher.Color);
            }

            if (player.AmOwner && !player.IsCrew() && Watcher.NonCrewFlash)
            {
                Watcher.NonCrewFlashDelay.DelayedAction(() =>
                {
                    Helpers.ShowFlash(Watcher.Color, 1.5f);
                    _ = new CustomMessage($"You Tripped A Watcher Sensor {Watcher.NonCrewFlashDelay} Seconds Ago", 3f, true, Watcher.Color);

                });
            }
        }

        public static void ShadeGlobalBlind()
        {
            bool shadeIsImp = Shade.Player.Data.Role.IsImpostor;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player == Shade.Player || (shadeIsImp && player.Data.Role.IsImpostor)) continue;
                Shade.BlindedPlayers.Add(player);
            }
            Shade.LightsOutTimer = Shade.BlindDuration;
            // If the local player is impostor indicate lights out
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && shadeIsImp)
                _ = new CustomMessage("Lights are out", Shade.BlindDuration, true, Color.red);
        }

        public static void HackerJamToggle()
        {
            Hacker.LockedOut = true;
            Hacker.JamDuration.DelayedAction(() =>
            {
                Hacker.LockedOut = false;
            });
        }

        public static void ShadeNearBlind(PlayerControl player)
        {
            Shade.BlindedPlayers.Add(player);
            Shade.LightsOutTimer = Shade.BlindDuration;
        }

        public static void NightMareBlind(PlayerControl player, PlayerControl target)
        {
            player.IsNightmare(out Nightmare nightmare);
            nightmare.LightsOutTimer = Nightmare.BlindDuration;
            nightmare.BlindedPlayers.Add(target.PlayerId);
        }

        public static void SealVent(int ventid)
        {
            Vent vent = ShipStatus.Instance.AllVents.FirstOrDefault((x) => x != null && x.Id == ventid);
            if (vent == null) return;

            if (PlayerControl.LocalPlayer == Trapper.Player)
            {
                PowerTools.SpriteAnim animator = vent.myAnim;
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                vent.myRend.sprite = animator == null ? Trapper.GetStaticVentSealedSprite() : Trapper.GetAnimatedVentSealedSprite();
                animator?.Stop();
                if (Helpers.IsMap(Map.Fungal))
                {
                    vent.myRend.sprite = Trapper.GetFungalSealedSprite();
                    vent.myRend.transform.localPosition = new Vector3(0, -.01f);
                }
                if (vent.gameObject.name.StartsWith("MinerVent_")) vent.myRend.sprite = Trapper.GetStaticVentSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = Trapper.GetSubmergedCentralUpperSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = Trapper.GetSubmergedCentralLowerSealedSprite();
                vent.myRend.color = new Color(1f, 1f, 1f, 0.5f);
                vent.name = "FutureSealedVent_" + vent.name;
            }

            VentsToSeal.Add(vent);
        }


        public static void SetVentTrap(int ventid)
        {
            Vent vent = ShipStatus.Instance.AllVents.FirstOrDefault((x) => x != null && x.Id == ventid);
            if (vent == null) return;

            if (VentTrap.VentTrapMap.ContainsKey(ventid)) return;

            VentTrap venttrap = new(vent);

            if (Trapper.Player.AmOwner)
                venttrap.DecorateVent();
        }

        public static void TriggerVentTrap(PlayerControl player, int ventid)
        {
            Vent vent = ShipStatus.Instance.AllVents.FirstOrDefault((x) => x != null && x.Id == ventid);
            if (vent == null) return;

            VentTrap.VentTrapMap[ventid].useVentTrap();

            if (Trapper.Player.AmOwner)
            {
                Helpers.ShowFlash(Trapper.Color, 1.5f);
                _ = new CustomMessage("Your Trap was Activated!", 3f, true, Trapper.Color);
            }

            if (player.AmOwner)
            {
                Helpers.ShowFlash(Trapper.Color, 1.5f);
                _ = new CustomMessage("The Trapper caught you!", 3f, true, Trapper.Color);
            }
        }

        public static void ArsonistWin()
        {
            Arsonist.TriggerArsonistWin = true;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if (player != Arsonist.Player) player.Exiled();
        }

        public static void BecomeVengefulLover()
        {
            PlayerControl romantic = Romantic.Player;
            if (romantic.Data.IsDead) return;

            PlayerControl lover = Romantic.Lover;
            bool isImpostor = Romantic.IsImpostor;
            bool isNeutral = Romantic.NeutralSided;
            Romantic.ClearAndReload();
            SetRole(RoleId.VengefulRomantic, romantic);
            VengefulRomantic.Lover = lover;
            VengefulRomantic.IsImpostor = isImpostor;
            VengefulRomantic.IsCrewmate = !isImpostor && !isNeutral;
        }

        public static void BecomeLoversRole()
        {
            PlayerControl romantic = Romantic.Player;
            if (romantic.Data.IsDead || !Romantic.NeutralSided) return;

            PlayerControl lover = Romantic.Lover;
            bool isArsonist = Romantic.IsArsonist;
            bool isJester = Romantic.IsJester;
            bool isScavenger = Romantic.IsScavenger;
            bool IsExecutioner = Romantic.IsExecutioner;
            bool IsHeadHunter = Romantic.IsHeadHunter;
            bool IsPyromaniac = Romantic.IsPyromaniac;
            Romantic.ClearAndReload();

            SetRole(RoleId.Beloved, lover);
            Beloved.Romantic = romantic;
            Romantic.SetNameFirstMeeting = true;


            if (isJester)
            {
                Jester.JesterDictionary.Remove(lover.PlayerId);
                SetRole(RoleId.Jester, romantic);
                Beloved.WasJester = true;
            }
            else if (isArsonist)
            {
                List<byte> dousedPlayers = Arsonist.DousedPlayers.Clone();
                Beloved.WasArsonist = true;
                SetRole(RoleId.Arsonist, romantic);
                Arsonist.DousedPlayers.Clear();
                Arsonist.DousedPlayers.AddRange(dousedPlayers);
                Arsonist.DousedPlayers.Remove(romantic);

                Vector3 bottomLeft = IntroCutsceneOnDestroyPatch.BottomLeft;

                int playerCounter = 0;
                bool isLocalBeloved = PlayerControl.LocalPlayer == Beloved.Player;
                bool isLocalArsonist = PlayerControl.LocalPlayer == Arsonist.Player;
                if (isLocalArsonist || isLocalBeloved)
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (!PlayerIcons.TryGetValue(player.PlayerId, out PoolablePlayer icon))
                            continue;

                        if (
                            isLocalArsonist && player != Arsonist.Player && !player.Data.IsDead)
                        {
                            icon.transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) + Vector3.right * playerCounter * 0.35f;
                            icon.transform.localScale = Vector3.one * 0.2f;
                            icon.SetSemiTransparent(!Arsonist.DousedPlayers.Contains(player));
                            icon.gameObject.SetActive(true);
                            playerCounter++;
                        }
                        else if (isLocalBeloved)
                        {
                            icon.gameObject.SetActive(false);
                            playerCounter++;
                        }
                    }
            }
            else if (isScavenger)
            {
                int eatenBodies = Scavenger.EatenBodies;
                SetRole(RoleId.Scavenger, romantic);
                Scavenger.EatenBodies = eatenBodies;
                Beloved.WasScavenger = true;
            }
            else if (IsExecutioner)
            {
                SetRole(RoleId.Executioner, romantic);
                Beloved.EasExecutioner = true;
            }
            else if (Refugee.PlayerToRefugee.Remove(lover.PlayerId))
            {
                SetRole(RoleId.Refugee, romantic);
                Beloved.WasRefugee = true;

            }
            else if (NeutralKiller.Players.Contains(lover) || IsHeadHunter || IsPyromaniac)
            {
                SetRole(RoleId.RuthlessRomantic, romantic);
                romantic.IsRuthlessRomantic(out RuthlessRomantic ruthlessRomantic);
                ruthlessRomantic.DeadLover = lover;
                Beloved.WasNK = true;
            }
        }

        public static void ScavengerTurnToRefugee()
        {
            PlayerControl scavenger = Scavenger.Player;
            Scavenger.ClearAndReload();
            SetRole(RoleId.Refugee, scavenger);
        }

        public static void RomanticToRefugee()
        {
            PlayerControl romantic = Romantic.Player;
            if (romantic.Data.IsDead) return;

            Romantic.ClearAndReload();
            SetRole(RoleId.Refugee, romantic);
        }

        public static void VengefulRomanticToRefugee()
        {
            PlayerControl refugee = VengefulRomantic.Player;
            VengefulRomantic.ClearAndReload();
            SetRole(RoleId.Refugee, refugee);
        }

        public static void ExilePlayer(PlayerControl player)
        {
            player.Data.IsDead = true;
            player.Exiled();
        }

        public static void AddJailedPlayerIcon(byte targetId, Jailor jailor)
        {
            AddJailorIcon(targetId, jailor);
            jailor.TargetJailedIcons = true;
        }

        public static void AddJailorIcon(byte targetId, Jailor jailor)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(state => state.TargetPlayerId == targetId);

            GameObject template = voteArea.Buttons.transform.Find("CancelButton").gameObject;
            SpriteRenderer checkbox = new GameObject("JailorIcon").AddComponent<SpriteRenderer>();

            checkbox.name = "JailIcon";
            checkbox.transform.SetParent(voteArea.transform);
            checkbox.transform.position = template.transform.position;
            checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
            checkbox.gameObject.layer = voteArea.Megaphone.gameObject.layer;
            checkbox.sprite = Jailor.GetCheckSprite();
            checkbox.transform.localScale *= .8f;
            checkbox.color = Color.gray;
            checkbox.enabled = true;

            jailor.Bars.Add(checkbox.gameObject);
        }

        public static void ExecutionerChangeRole()
        {
            if (Executioner.TriggerExecutionerWin)
                return;

            PlayerControl executioner = Executioner.Player;
            Executioner.ClearAndReload();

            if (Executioner.PromotesTo == ExePromotes.Jester)
            {
                SetRole(RoleId.Jester, executioner);
                executioner.IsJester(out Jester jester);
                jester.WasExecutioner = true;
                if (Romantic.IsExecutioner)
                    Romantic.IsJester = true;
            }
            else
            {
                SetRole(RoleId.Refugee, executioner);
                executioner.IsRefugee(out Refugee refugee);
                refugee.WasExecutioner = true;
            }

            if (Romantic.IsExecutioner)
                Romantic.IsExecutioner = false;
        }

        public static void GuesserShoot(PlayerControl guesser, PlayerControl dyingTarget, PlayerControl guessedTarget, RoleId guessedRoleId)
        {
            if (guesser == null || dyingTarget == null || guessedTarget == null) return;

            var meetingHud = MeetingHud.Instance;

            if (guesser.AmOwner)
            {
                PlayerControl.LocalPlayer.RPCAddGameInfo(dyingTarget == guesser ? InfoType.AddIncorrectGuess : InfoType.AddCorrectGuess);
            }

            if (dyingTarget == Romantic.Lover && !Romantic.NeutralSided)
            {
                BecomeVengefulLover();
                VengefulRomantic.Target = guesser;
            }

            guesser.RemainingShots(true);
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);

            dyingTarget.Exiled();

            if (dyingTarget.AmOwner)
            {
                var KillOverlay = HudManager.Instance.KillOverlay;
                KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
            }

            foreach (PlayerVoteArea voteArea in meetingHud.playerStates)
            {
                Transform shootbutton = voteArea.transform.FindChild("ShootButton");
                if (shootbutton != null && (PlayerControl.LocalPlayer.Data.IsDead || voteArea.TargetPlayerId == dyingTarget.PlayerId))
                    UnityEngine.Object.Destroy(shootbutton.gameObject);
            }

            if (MeetingHudPatch.GuesserUI != null)
                MeetingHudPatch.GuesserUIExitButton.OnClick.Invoke();

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                RoleInfo roleInfo = RoleInfo.AllRoleInfos.First(x => x.RoleId == guessedRoleId);
                string msg = $"{guesser.Data.PlayerName} guessed the role {roleInfo.Name} for {guessedTarget.Data.PlayerName}!";
                if (AmongUsClient.Instance.AmClient)
                    HudManager.Instance.Chat.AddChat(guesser, msg);

                // what is this??
                if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
                    Assets.CoreScripts.UnityTelemetry.Instance.SendWho();
            }

            if (dyingTarget == Executioner.Target)
                Executioner.ExecutionerCheckPromotion();

            // Check and see if dying target is a jailor, if they are, break free the prisioners
            if (dyingTarget.IsJailor(out Jailor jailor) && jailor.HasJailed)
            {
                JailBreak(jailor);
                ReaddButtons();
            }

            Helpers.DelayedAction(0.1f, () =>
            {
                foreach (PlayerVoteArea voteArea in meetingHud.playerStates)
                {
                    if (voteArea.TargetPlayerId == dyingTarget.PlayerId)
                    {
                        voteArea.SetDead(voteArea.DidReport, true);
                        voteArea.Overlay.gameObject.SetActive(true);
                    }

                    //Give players back their vote if target is shot dead
                    if (voteArea.VotedFor != dyingTarget.PlayerId) continue;
                    voteArea.UnsetVote();
                    if (voteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId)
                        meetingHud.ClearVote();
                }

                if (AmongUsClient.Instance.AmHost)
                    meetingHud.CheckForEndVoting();
            });
        }

        public static void JailBreak(Jailor jailor)
        {
            if (jailor.Target == null) return;

            jailor.Target = null;
            jailor.HasJailed = false;
            jailor.TargetJailedIcons = false;
            foreach (GameObject bar in jailor.Bars)
                UnityEngine.Object.Destroy(bar);
            jailor.Bars.Clear();
        }

        public static void DropBody()
        {
            foreach (var body in GameHistory.DeadPlayers)
            {
                if (body.Data.PlayerId == Undertaker.DeadBodyDragged.ParentId)
                {
                    body.CurrentBodyPos = Undertaker.DeadBodyDragged.TruePosition;
                    body.Tampered = true;
                }
            }
            Undertaker.DeadBodyDragged = null;
        }

        public static void ReaddButtons()
        {
            if (PlayerControl.LocalPlayer.CanGuess())
                MeetingHudPatch.AddGuesserButtons();
        }

        public static void SetChatNotificationOverlay(byte targetPlayerId, bool impchat)
        {
            if (!MeetingHud.Instance) return;
            try
            {
                if (PlayerControl.LocalPlayer.PlayerId == targetPlayerId) return;
                if (impchat && !PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
                PlayerVoteArea playerState = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == targetPlayerId);

                SpriteRenderer rend = new GameObject("ChatOverlayNotification").AddComponent<SpriteRenderer>();
                rend.transform.SetParent(playerState.transform);
                rend.gameObject.layer = playerState.Megaphone.gameObject.layer;
                rend.transform.localPosition = new Vector3(-0.5f, 0.2f, -1f);
                rend.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.ChatOverlay.png", 130f);
                if (impchat)
                {
                    rend.color = Palette.ImpostorRed;
                }
                rend.gameObject.SetActive(true);

                2f.DelayedAction(() =>
                {
                    rend.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(rend.gameObject);
                });
            }
            catch
            {
                // ??????
                System.Console.WriteLine("Chat Notification Overlay is Detected");
            }
            return;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class RPCHandlerPatch
    {
        static void Postfix([HarmonyArgument(0)] byte packetId, [HarmonyArgument(1)] MessageReader reader)
        {
            if (packetId != 254) return;

            switch ((CustomRPC)reader.ReadByte())
            {
                // Main Controls

                case CustomRPC.ResetVaribles:
                    RPCProcedure.ResetVariables();
                    break;
                case CustomRPC.ShareOptions:
                    RPCProcedure.HandleShareOptions(reader.ReadByte(), reader);
                    break;
                case CustomRPC.ForceEnd:
                    RPCProcedure.ForceEnd();
                    break;
                case CustomRPC.LoverPairDead:
                    Romantic.PairIsDead = true;
                    break;
                case CustomRPC.SetRole:
                    RPCProcedure.SetRole(reader.ReadRoleID(), reader.ReadPlayer());
                    break;
                case CustomRPC.SetModifier:
                    RPCProcedure.SetModifier(reader.ReadRoleID(), reader.ReadPlayer());
                    break;
                case CustomRPC.VersionHandshake:
                    byte major = reader.ReadByte();
                    byte minor = reader.ReadByte();
                    byte patch = reader.ReadByte();
                    float timer = reader.ReadSingle();
                    if (!AmongUsClient.Instance.AmHost && timer >= 0f) GameStartManagerPatch.Timer = timer;
                    int versionOwnerId = reader.ReadPackedInt32();
                    byte revision = 0xFF;
                    Guid guid;
                    if (reader.Length - reader.Position >= 17)
                    { // enough bytes left to read
                        revision = reader.ReadByte();
                        // GUID
                        byte[] gbytes = reader.ReadBytes(16);
                        guid = new Guid(gbytes);
                    }
                    else
                    {
                        guid = new Guid(new byte[16]);
                    }
                    RPCProcedure.VersionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                    break;
                case CustomRPC.AddGameInfo:
                    RPCProcedure.AddGameInfo(reader.ReadPlayer(), (InfoType)reader.ReadByte());
                    break;
                case CustomRPC.UncheckedMurderPlayer:
                    RPCProcedure.UncheckedMurderPlayer(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean(), reader.ReadBoolean());
                    break;
                case CustomRPC.ParalyzePlayer:
                    var nightmare = reader.ReadByte();
                    var paralyze = reader.ReadPlayer();
                    if (Nightmare.IsNightmare(nightmare, out Nightmare nightmareA))
                        RPCProcedure.ParalyzePlayer(nightmareA, paralyze);
                    break;
                case CustomRPC.DynamicMapOption:
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId = reader.ReadByte();
                    break;
                case CustomRPC.ShareRandomSeed:
                    int randomSeed = reader.ReadInt32();
                    RandomSeed.Rpc_ShareRandomSeed(randomSeed);
                    break;

                // Role functionality

                case CustomRPC.EngineerFixLights:
                    RPCProcedure.EngineerFixLights();
                    break;
                case CustomRPC.EngineerFixSubmergedOxygen:
                    SubmergedCompatibility.RepairOxygen();
                    break;
                case CustomRPC.CleanBody:
                    RPCProcedure.CleanBody(reader.ReadByte());
                    break;
                case CustomRPC.DragBody:
                    RPCProcedure.DragBody(reader.ReadByte());
                    break;
                case CustomRPC.DropBody:
                    RPCProcedure.DropBody();
                    break;
                case CustomRPC.GuardianSetShielded:
                    RPCProcedure.GuardianSetShielded(reader.ReadPlayer());
                    break;
                case CustomRPC.AddSpectator:
                    RPCProcedure.AddSpectator(reader.ReadPlayer());
                    break;
                case CustomRPC.RemoveSpectator:
                    RPCProcedure.RemoveSpectator(reader.ReadPlayer());
                    break;
                case CustomRPC.GuardianResetShielded:
                    RPCProcedure.GuardianResetShield();
                    break;
                case CustomRPC.MedicSetHearMonitor:
                    RPCProcedure.MedicSetHearMonitor(reader.ReadPlayer());
                    break;
                case CustomRPC.GiveBomb:
                    RPCProcedure.GiveBomb(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean());
                    break;
                case CustomRPC.PassBomb:
                    RPCProcedure.PassBomb(reader.ReadByte(), reader.ReadByte(), reader.ReadInt32());
                    break;
                case CustomRPC.JailorJail:
                    var jailor = reader.ReadByte();
                    var jailed = reader.ReadPlayer();
                    if (Jailor.IsJailor(jailor, out Jailor jailorA))
                        RPCProcedure.JailorJail(jailorA, jailed);
                    break;
                case CustomRPC.JailBreak:
                    var jailor2 = reader.ReadByte();
                    if (Jailor.IsJailor(jailor2, out Jailor jailorB))
                        RPCProcedure.JailBreak(jailorB);
                    break;
                case CustomRPC.MorphlingMorph:
                    RPCProcedure.MorphlingMorph(reader.ReadPlayer());
                    break;
                case CustomRPC.SetLook:
                    RPCProcedure.SetLook(reader.ReadPlayer(), reader.ReadPlayer());
                    break;
                case CustomRPC.CamouflagerCamouflage:
                    RPCProcedure.CamouflagerCamouflage();
                    break;
                case CustomRPC.VampireSetBitten:
                    RPCProcedure.VampireSetBitten(reader.ReadByte());
                    break;
                case CustomRPC.VampireResetBitten:
                    Vampire.Bitten = null;
                    break;
                case CustomRPC.TrackerMarkPlayer:
                    RPCProcedure.TrackerMarkPlayer(reader.ReadPlayer());
                    break;
                case CustomRPC.TrackerTrackWarning:
                    RPCProcedure.TrackerTrackWarning();
                    break;
                case CustomRPC.ParityCopCompareAddition:
                    var parity = reader.ReadByte();
                    var compare = reader.ReadByte();
                    if (ParityCop.IsParityCop(parity, out ParityCop parityCopB))
                        parityCopB.ComparedPlayers.Add(compare);
                    break;
                case CustomRPC.PlayerKilledByAbility:
                    Detective.PlayersKilledByAbility.Add(reader.ReadPlayer());
                    break;
                case CustomRPC.SetJesterWinner:
                    Jester.WinningJesterPlayer = reader.ReadPlayer();
                    Jester.TriggerJesterWin = true;
                    break;
                case CustomRPC.SetExecutionerWin:
                    Executioner.TriggerExecutionerWin = true;
                    break;
                case CustomRPC.ArsonistDouse:
                    RPCProcedure.ArsonistDouse(reader.ReadPlayer());
                    break;
                case CustomRPC.PyromaniacDouse:
                    var pyro = reader.ReadByte();
                    var douse = reader.ReadByte();
                    if (Pyromaniac.IsPyromaniac(pyro, out Pyromaniac pyromaniac))
                        pyromaniac.DousedPlayers.Add(douse);
                    break;
                case CustomRPC.ScavengerEat:
                    RPCProcedure.ScavengerEat(reader.ReadByte());
                    break;
                case CustomRPC.AvengedLover:
                    VengefulRomantic.AvengedLover = true;
                    break;
                case CustomRPC.PlaceShadeTrace:
                    RPCProcedure.PlaceShadeTrace(reader.ReadBytesAndSize());
                    break;
                case CustomRPC.AddPet:
                    PlayerPetsToHide.Add(reader.ReadPlayer());
                    break;
                case CustomRPC.WraithPhase:
                    Wraith.PhaseOn = reader.ReadBoolean();
                    break;
                case CustomRPC.WraithReturn:
                    RPCProcedure.WraithReturn();
                    break;
                case CustomRPC.ResetAnimation:
                    RPCProcedure.ResetAnimation(reader.ReadPlayer());
                    break;
                case CustomRPC.WraithLanternBreak:
                    Lantern.BreakLantern();
                    break;
                case CustomRPC.PlaceMinerVent:
                    RPCProcedure.PlaceMinerVent(reader.ReadBytesAndSize());
                    break;
                case CustomRPC.PlaceLantern:
                    RPCProcedure.PlaceLantern(reader.ReadBytesAndSize());
                    break;
                case CustomRPC.TripSensor:
                    RPCProcedure.TripSensor(reader.ReadPlayer());
                    break;
                case CustomRPC.ShadeGlobalBlind:
                    RPCProcedure.ShadeGlobalBlind();
                    break;
                case CustomRPC.HackerJamToggle:
                    RPCProcedure.HackerJamToggle();
                    break;
                case CustomRPC.ShadeNearBlind:
                    RPCProcedure.ShadeNearBlind(reader.ReadPlayer());
                    break;
                case CustomRPC.NightMareBlind:
                    RPCProcedure.NightMareBlind(reader.ReadPlayer(), reader.ReadPlayer());
                    break;
                case CustomRPC.NightMareClear:
                    var nightmare1 = reader.ReadByte();
                    if (Nightmare.IsNightmare(nightmare1, out Nightmare nightmareC))
                        nightmareC.BlindedPlayers.Clear();
                    break;
                case CustomRPC.SealVent:
                    RPCProcedure.SealVent(reader.ReadInt32());
                    break;
                case CustomRPC.SetTrap:
                    RPCProcedure.SetVentTrap(reader.ReadInt32());
                    break;
                case CustomRPC.TriggerVentTrap:
                    RPCProcedure.TriggerVentTrap(reader.ReadPlayer(), reader.ReadInt32());
                    break;
                case CustomRPC.ExitAllVents:
                    reader.ReadPlayer().MyPhysics.ExitAllVents();
                    break;
                case CustomRPC.ArsonistWin:
                    RPCProcedure.ArsonistWin();
                    break;
                case CustomRPC.GuesserShoot:
                    PlayerControl guesser = reader.ReadPlayer();
                    PlayerControl dyingplayer = reader.ReadPlayer();
                    PlayerControl guessedTarget = reader.ReadPlayer();
                    RoleId roleId = reader.ReadRoleID();
                    RPCProcedure.GuesserShoot(guesser, dyingplayer, guessedTarget, roleId);
                    break;
                case CustomRPC.ScavengerWin:
                    Scavenger.TriggerScavengerWin = true;
                    break;
                case CustomRPC.ExecutionerSetTarget:
                    Executioner.Target = reader.ReadPlayer();
                    break;
                case CustomRPC.ExecutionerChangeRole:
                    RPCProcedure.ExecutionerChangeRole();
                    break;
                case CustomRPC.SetFirstKill:
                    FirstKillPlayer = reader.ReadPlayer();
                    break;
                case CustomRPC.SetFirstKillPlayers:
                    FirstKillPlayers.Add(reader.ReadPlayer());
                    break;
                case CustomRPC.SetInvisible:
                    RPCProcedure.SetInvisible(reader.ReadPlayer(), reader.ReadBoolean());
                    break;
                case CustomRPC.CultistCreateImposter:
                    RPCProcedure.CultistCreateImposter(reader.ReadPlayer());
                    break;
                case CustomRPC.CreateImpostor:
                    RPCProcedure.CreateImpostor(reader.ReadPlayer());
                    break;
                case CustomRPC.FallInLove:
                    RPCProcedure.FallInLove(reader.ReadPlayer());
                    break;
                case CustomRPC.TurnToCrewmate:
                    RPCProcedure.TurnToCrewmate(reader.ReadPlayer());
                    break;
                case CustomRPC.ChangelingChange:
                    RPCProcedure.ChangelingChange(reader.ReadRoleID());
                    break;
                case CustomRPC.RefugeeShield:
                    var refu = reader.ReadByte();
                    if (Refugee.IsRefugee(refu, out Refugee refugee))
                        RPCProcedure.RefugeeShield(refugee);
                    break;
                case CustomRPC.RuthlessRomanticShield:
                    var Ruthless = reader.ReadByte();
                    if (RuthlessRomantic.IsRuthlessRomantic(Ruthless, out RuthlessRomantic ruthlessRomanticA))
                        RPCProcedure.RuthlessRomanticShield(ruthlessRomanticA);
                    break;
                case CustomRPC.FakeCompare:
                    var Parity1 = reader.ReadByte();
                    var fake = reader.ReadBoolean();
                    if (ParityCop.IsParityCop(Parity1, out ParityCop parityCopA))
                        parityCopA.PressedFakeCompare = fake;
                    break;
                case CustomRPC.RomanticShield:
                    RPCProcedure.RomanticShield();
                    break;
                case CustomRPC.BecomeVengefulLover:
                    RPCProcedure.BecomeVengefulLover();
                    break;
                case CustomRPC.BecomeLoversRole:
                    RPCProcedure.BecomeLoversRole();
                    break;
                case CustomRPC.ControlPlayer:
                    RPCProcedure.ControlPlayer(reader.ReadPlayer());
                    break;
                case CustomRPC.KillInfected:
                    RPCProcedure.KillInfected(reader.ReadBoolean());
                    break;

                case CustomRPC.ScavengerTurnToRefugee:
                    RPCProcedure.ScavengerTurnToRefugee();
                    break;
                case CustomRPC.RomanticTurnToRefugee:
                    RPCProcedure.RomanticToRefugee();
                    break;
                case CustomRPC.VengefulRomanticToRefugee:
                    RPCProcedure.VengefulRomanticToRefugee();
                    break;
                case CustomRPC.ExilePlayer:
                    RPCProcedure.ExilePlayer(reader.ReadPlayer());
                    break;
                case CustomRPC.ReadButtons:
                    RPCProcedure.ReaddButtons();
                    break;
                case CustomRPC.ShadeClearBlind:
                    Shade.BlindedPlayers.Clear();
                    break;
                case CustomRPC.SetGameStarting:
                    RPCProcedure.SetGameStarting();
                    break;
                case CustomRPC.ResetSensors:
                    Watcher.ResetSensors();
                    break;
                case CustomRPC.SetMeetingChatOverlay:
                    RPCProcedure.SetChatNotificationOverlay(reader.ReadByte(), reader.ReadBoolean());
                    break;
                case CustomRPC.SpitefulVote:
                    var spite = reader.ReadPlayer();
                    var vote = reader.ReadByte();
                    if (spite.IsSpiteful(out Spiteful spiteful))
                        spiteful.VotedBy.Add(vote);
                    break;
                case CustomRPC.Duel:
                    RPCProcedure.Duel(reader.ReadPlayer(), reader.ReadString());
                    break;
                case CustomRPC.SetRandomID:
                    RPCProcedure.SetRandomId(reader.ReadString());
                    break;
                case CustomRPC.SendResultOfDuel:
                    RPCProcedure.DuelResults();
                    break;
                case CustomRPC.ClearDuel:
                    RPCProcedure.RemoveCompletedDuels(reader.ReadPlayer());
                    break;
                case CustomRPC.MayorRetire:
                    Mayor.Retired = true;
                    break;
                case CustomRPC.PsychicAddCount:
                    Psychic.AbilitesUsed++;
                    break;
                case CustomRPC.SnapToRpc:
                    var player1 = reader.ReadPlayer();
                    var player2 = reader.ReadPlayer();
                    if (player1 != null && player2 != null)
                        player1.NetTransform.SnapTo(player2.transform.position);
                    break;
                case CustomRPC.UpdateSurvivability:
                    RPCProcedure.UpdateSurvivability(reader.ReadPlayer());
                    break;
                case CustomRPC.ResetKillButton:
                    RPCProcedure.ResetKillButton(reader.ReadPlayer());
                    break;
                case CustomRPC.AddDeadPlayer:
                    RPCProcedure.AddDeadPlayer(reader.ReadPlayer(), reader.ReadPlayer(), (DeathReason)reader.ReadByte());
                    break;
                case CustomRPC.ClearToBeSpectators:
                    Spectator.ToBecomeSpectator.Clear();
                    break;
                case CustomRPC.ConcealBody:
                    RPCProcedure.ConcealBody(reader.ReadByte());
                    break;
                case CustomRPC.UpdatePlayerCount:
                    var team = reader.ReadByte();
                    var players = reader.ReadInt32();
                    RPCProcedure.UpdatePlayerCount(team, players);
                    break;
                case CustomRPC.DraftModePickOrder:
                    RoleDraft.receivePickOrder(reader.ReadByte(), reader);
                    break;
                case CustomRPC.DraftModePick:
                    RoleDraft.receivePick(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean());
                    break;

                case CustomRPC.MoveControlledPlayer:
                    //byte moveId = reader.ReadByte();
                    Vector2 newVel = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    Vector3 newPos = new Vector3(reader.ReadSingle(), reader.ReadSingle());

                    if (Parasite.Controlled != null && Parasite.Controlled.AmOwner)
                    {
                        Parasite.Position = newPos;
                        Parasite.Controlled.transform.position = newPos;
                        Parasite.Controlled.MyPhysics.body.position = newPos;
                        Parasite.Controlled.MyPhysics.body.velocity = newVel;
                        //Parasite.Controlled.MyPhysics.SetNormalizedVelocity(newVel);
                    }
                    break;

            }
        }

        private static PlayerControl ReadPlayer(this MessageReader reader)
        {
            byte player = reader.ReadByte();
            return Helpers.PlayerById(player);
        }
        private static RoleId ReadRoleID(this MessageReader reader)
        {
            byte role = reader.ReadByte();
            return (RoleId)role;
        }
    }
}
