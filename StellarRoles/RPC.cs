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
using static StellarRoles.StellarRoles;

namespace StellarRoles
{
    public static class RPCProcedure
    {
        public static void Send(CustomRPC method, params object[] parameters)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte)method,
                SendOption.Reliable,
                -1
            );

            if (parameters != null)
                foreach (object parameter in parameters)
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

            AmongUsClient.Instance.FinishRpcImmediately(writer);
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
            GameStartManagerPatch.GameStartManagerUpdatePatch.StartingTimer = 0;
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
                    option.UpdateSelection((int)selection);
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
                    player.SetRole(RoleTypes.Crewmate);
                    player.MurderPlayer(player);
                    player.Data.IsDead = true;
                }
        }

        public static void SetRole(RoleId roleId, PlayerControl player)
        {
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

                case RoleId.BountyHunter:
                    BountyHunter.Player = player;
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.BountyHunter);
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

                case RoleId.BountyHunterNK:
                    BountyHunter.Player = player;
                    NeutralKiller.Players.Add(player);
                    NeutralKiller.RogueImps.Add(player);
                    PlayerGameInfo.AddRole(player.PlayerId, RoleInfo.BountyHunterNeutralKiller);
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
            RockPaperScissorsGame game = RockPaperScissorsGame.FetchDuel(player);
            if (game?.IsComplete == true)
                RockPaperScissorsGame.Games.Remove(game);
        }


        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            GameStartManagerPatch.PlayerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(
                revision < 0 ? new Version(major, minor, build) : new Version(major, minor, build, revision),
                guid
            );
        }

        public static void AddSpectator(PlayerControl player)
        {
            Spectator.ToBecomeSpectator.Add(player.PlayerId);
            player.cosmetics.nameText.color = Color.gray;
        }

        public static void RemoveSpectator(byte playerId)
        {
            PlayerControl player = Helpers.PlayerById(playerId);
            if (player != null)
            {
                RemoveSpectator(player);
                return;
            }

            Spectator.ToBecomeSpectator.Remove(playerId);
            Spectator.Players.Remove(playerId);
        }
        public static void RemoveSpectator(PlayerControl player)
        {
            Spectator.ToBecomeSpectator.Remove(player.PlayerId);
            if (!player.Data.Disconnected)
                player.cosmetics.nameText.color = Color.white;
        }

        public static void AddGameInfo(byte playerId, InfoType info)
        {
            if (!PlayerGameInfo.Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo))
                PlayerGameInfo.Mapping.Add(playerId, gameInfo = new());

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
            }
        }

        public static void UncheckedMurderPlayer(PlayerControl source, PlayerControl target, bool showAnimation)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                return;

            if (!showAnimation)
                KillAnimationCoPerformKillPatch.HideNextAnimation = true;

            if (Romantic.Player != null && Romantic.HasLover && Romantic.Lover == target)
                VengefulRomantic.Target = source.IsBombed(out Bombed bombed) ? bombed.Bomber : source;

            if (Medic.Target == target && Medic.Player != null && !Medic.Player.Data.IsDead)
                MedicHeartMonitorFlash(source);

            source.MurderPlayer(target);
        }

        public static void ParalyzePlayer(Nightmare nightmare, PlayerControl target)
        {
            if (target.AmOwner)
            {
                Helpers.SetMovement(false);
                Helpers.ShowFlash(Color.black, 1.5f);
                _ = new CustomMessage("The Nightmare Paralyzed You!", Nightmare.ParalyzeRootTime, true, Nightmare.Color);
            }

            nightmare.ParalyzedPlayer = target;

            HudManager.Instance.StartCoroutine(Effects.Lerp(Nightmare.ParalyzeRootTime, new Action<float>((p) =>
            { // Delayed action
                if (p == 1f)
                {
                    if (target.AmOwner)
                        Helpers.SetMovement(true);
                    nightmare.ParalyzedPlayer = null;
                }
            })));
        }

        public static void SetGameStarting()
        {
            GameStartManagerPatch.GameStartManagerUpdatePatch.StartingTimer = 5f;
        }

        public static void TurnToCrewmate(PlayerControl player)
        {
            ErasePlayerRoles(player, false);
            player.RpcSetRole(RoleTypes.Crewmate);
            player.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
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

            if (role == RoleId.BountyHunter)
            {
                Vector3 bottomLeft = IntroCutsceneOnDestroyPatch.BottomLeft + new Vector3(-0.25f, 0.25f, 0f);
                BountyHunter.BountyUpdateTimer = 0f;
                BountyHunter.CooldownText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                BountyHunter.CooldownText.alignment = TMPro.TextAlignmentOptions.Center;
                BountyHunter.CooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -0.35f, -62f);
                BountyHunter.CooldownText.gameObject.SetActive(true);

                foreach (PlayerControl cachedPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    if (PlayerIcons.TryGetValue(cachedPlayer.PlayerId, out PoolablePlayer icon))
                    {
                        icon.SetSemiTransparent(false);
                        icon.transform.localPosition = bottomLeft;
                        icon.transform.localScale = Vector3.one * 0.4f;
                        icon.gameObject.SetActive(false);
                    }
            }

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

            SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
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
        }
        public static void CultistCreateImposter(PlayerControl player)
        {
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
            ErasePlayerRoles(player, true);
            Helpers.TurnToImpostor(player);
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

            if (Romantic.Player.AmOwner)
                Romantic.Lover.cosmetics.nameText.text += Helpers.ColorString(Romantic.Color, " heart");
            else if (Romantic.Lover.AmOwner)
                Romantic.Player.cosmetics.nameText.text += Helpers.ColorString(Romantic.Color, " heart");
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

        public static void MedicHeartMonitorFlash(PlayerControl killer)
        {
            if (Medic.Player == null || Medic.Target == null || MedicAbilities.isRoleBlocked()) return;

            if (Medic.Player.AmOwner)
            {
                Helpers.ShowFlash(Medic.Color, 1.5f);
                _ = new CustomMessage("Your Target Died", 3f, true, Medic.Color);
            }

            if (killer.AmOwner && !killer.IsCrew() && Medic.NonCrewFlash)
            {
                HudManager.Instance.StartCoroutine(Effects.Lerp(Medic.NonCrewFlashDelay, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        Helpers.ShowFlash(Medic.Color, 1.5f);
                        _ = new CustomMessage($"You Killed The Medic Target {Medic.NonCrewFlashDelay} Seconds Ago", 3f, true, Medic.Color);
                    }
                })));
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
                Morphling.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
            }
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
            HudManager.Instance.StartCoroutine(Effects.Lerp(Refugee.VestDuration, new Action<float>((p) =>
            {
                if (p == 1f) refugee.IsVestActive = false;
            })));

        }

        public static void RuthlessRomanticShield(RuthlessRomantic romantic)
        {
            romantic.IsVestActive = true;
            HudManager.Instance.StartCoroutine(Effects.Lerp(RuthlessRomantic.VestDuration, new Action<float>((p) =>
            {
                if (p == 1f)
                    romantic.IsVestActive = false;
            })));
        }

        public static void RomanticShield()
        {
            Romantic.IsVestActive = true;
            HudManager.Instance.StartCoroutine(Effects.Lerp(Romantic.VestDuration, new Action<float>((p) =>
            {
                if (p == 1f) Romantic.IsVestActive = false;
            })));

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

        public static void ErasePlayerRoles(PlayerControl player, bool ignoreModifier = true)
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
            else if (player == BountyHunter.Player)
                BountyHunter.ClearAndReload();
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
        }

        public static void GiveBomb(PlayerControl player, PlayerControl bomber, bool reset)
        {
            if (reset)
            {
                Bombed.BombedDictionary.Remove(player.PlayerId);
                return;
            }

            Bombed bombed = new(player, bomber);

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Bombed.BombDelay, new Action<float>((p) =>
            {
                if (p == 1f && !MeetingHud.Instance)
                {
                    bombed.BombActive = true;
                    if (bombed.Player.AmOwner)
                    {
                        AlertBombed(bombed, Bomber.CalculateBombTimer());
                        Helpers.ShowFlash(Bombed.AlertColor, 1f);
                    }
                }
            })));
        }

        public static void PassBomb(byte bombedPlayerId, byte targetId, int timeLeft)
        {
            Bombed.IsBombed(bombedPlayerId, out Bombed oldBombed);
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
                AlertBombed(bombed, timeLeft);
                Helpers.ShowFlash(Bombed.AlertColor, 1f);
            }
        }

        public static void AlertBombed(Bombed bombed, float time)
        {
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(time, new Action<float>((p) =>
            { // Delayed action
                if (!bombed.PassedBomb && bombed.BombActive && !MeetingHud.Instance)
                {
                    int timeLeft = (int)(time - (time * p));
                    if (timeLeft <= Bombed.BombTimer && bombed.TimeLeft != timeLeft)
                    {
                        _ = new CustomMessage($"Your Bomb will explode in {timeLeft} seconds!", 1f, true, Color.red);
                        bombed.TimeLeft = timeLeft;
                    }
                    if (p == 1f)
                    {
                        // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                        Helpers.CheckBombedAttemptAndKill(bombed.Bomber, bombed.Player, showAnimation: false);
                        Send(CustomRPC.GiveBomb, bombed.Player.PlayerId, bombed.Bomber.PlayerId, true);
                        GiveBomb(bombed.Player, bombed.Bomber, true);
                        Helpers.AddGameInfo(bombed.Bomber.PlayerId, InfoType.AddAbilityKill, InfoType.AddKill);
                    }
                }
            })));
        }

        public static void PlaceShadeTrace(byte[] buffer)
        {
            _ = new ShadeTrace(ConvertPosition(buffer), Shade.EvidenceDuration);
        }

        public static void SetInvisible(PlayerControl target, bool reset)
        {
            if (reset)
            {
                target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
                target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
                if (Camouflager.CamouflageTimer <= 0) target.SetDefaultLook();
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
                return;
            }

            target.SetLook("", 6, "", "", "", "");
            Color color = Color.clear;
            if (PlayerControl.LocalPlayer.Data.IsDead || target.AmOwner) color.a = 0.1f;
            target.cosmetics.currentBodySprite.BodySprite.color = color;
            target.cosmetics.colorBlindText.gameObject.SetActive(false);
            if (target == Wraith.Player)
            {
                Wraith.InvisibleTimer = Wraith.InvisibleDuration;
                Wraith.IsInvisible = true;
            }
            else if (target == Shade.Player)
            {
                Shade.InvisibleTimer = Shade.CalculateShadeDuration();
                Shade.IsInvisble = true;
            }
        }

        public static void WraithReturn()
        {
            Vector3 pos = Lantern.CurrentLantern.LanternGameObject.transform.position;
            if (SubmergedCompatibility.IsSubmerged)
                SubmergedCompatibility.ChangeFloor(pos.y > -7);
            Wraith.Player.NetTransform.RpcSnapTo(pos);
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

            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) =>
            {
                if (p == 1)
                    foreach (DeadBody deadBody in UnityEngine.Object.FindObjectsOfType<DeadBody>())
                        UnityEngine.Object.Destroy(deadBody.gameObject);
            })));
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
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Watcher.NonCrewFlashDelay, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        Helpers.ShowFlash(Watcher.Color, 1.5f);
                        _ = new CustomMessage($"You Tripped A Watcher Sensor {Watcher.NonCrewFlashDelay} Seconds Ago", 3f, true, Watcher.Color);
                    }
                })));
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
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Hacker.JamDuration, new Action<float>((p) =>
            { // Delayed action
                if (p == 1f)
                {
                    Hacker.LockedOut = false;
                }
            })));
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
            Vent vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault((x) => x != null && x.Id == ventid);
            if (vent == null) return;

            if (PlayerControl.LocalPlayer == Trapper.Player)
            {
                PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
                animator?.Stop();
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                vent.myRend.sprite = animator == null ? Trapper.GetStaticVentSealedSprite() : Trapper.GetAnimatedVentSealedSprite();
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
            Vent vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault((x) => x != null && x.Id == ventid);
            if (vent == null) return;

            if (VentTrap.VentTrapMap.ContainsKey(ventid)) return;

            VentTrap venttrap = new(vent);

            if (Trapper.Player.AmOwner)
                venttrap.DecorateVent();
        }

        public static void TriggerVentTrap(PlayerControl player, int ventid)
        {
            Vent vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault((x) => x != null && x.Id == ventid);
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
            if (guesser.AmOwner)
            {
                Helpers.AddGameInfo(guesser.PlayerId, dyingTarget == guesser ? InfoType.AddIncorrectGuess : InfoType.AddCorrectGuess);
            }

            if (dyingTarget == Romantic.Lover && !Romantic.NeutralSided)
            {
                BecomeVengefulLover();
                VengefulRomantic.Target = guesser;
            }

            // Check and see if dying target is a jailor, if they are, break free the prisioners
            if (dyingTarget.IsJailor(out Jailor jailor))
                JailBreak(jailor);

            dyingTarget.Exiled();

            Helpers.RemainingShots(guesser, true);
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);

            foreach (PlayerVoteArea voteArea in MeetingHud.Instance.playerStates)
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
                    MeetingHud.Instance.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost)
                MeetingHud.Instance.CheckForEndVoting();

            if (dyingTarget.AmOwner)
                HudManager.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);

            foreach (PlayerVoteArea voteArea in MeetingHud.Instance.playerStates)
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
                    FastDestroyableSingleton<Assets.CoreScripts.Telemetry>.Instance.SendWho();
            }

            if (dyingTarget == Executioner.Target)
                Executioner.ExecutionerCheckPromotion();
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

        public static void ReadButtons()
        {
            if (PlayerControl.LocalPlayer.CanGuess())
                MeetingHudPatch.AddGuesserButtons();
        }

        public static void SetChatNotificationOverlay(byte targetPlayerId)
        {
            if (!MeetingHud.Instance) return;
            try
            {
                if (PlayerControl.LocalPlayer.PlayerId == targetPlayerId) return;
                PlayerVoteArea playerState = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == targetPlayerId);

                SpriteRenderer rend = new GameObject("ChatOverlayNotification").AddComponent<SpriteRenderer>();
                rend.transform.SetParent(playerState.transform);
                rend.gameObject.layer = playerState.Megaphone.gameObject.layer;
                rend.transform.localPosition = new Vector3(-0.5f, 0.2f, -1f);
                rend.sprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.ChatOverlay.png", 130f);
                rend.gameObject.SetActive(true);

                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(2f, new Action<float>((p) =>
                { // Delayed action
                    if (p == 1f)
                    {
                        rend.gameObject.SetActive(false);
                        UnityEngine.Object.Destroy(rend.gameObject);
                    }
                })));
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
            switch ((CustomRPC)packetId)
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
                    RPCProcedure.SetRole((RoleId)reader.ReadByte(), reader.ReadPlayer());
                    break;
                case CustomRPC.SetModifier:
                    RPCProcedure.SetModifier((RoleId)reader.ReadByte(), reader.ReadPlayer());
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
                        guid = new Guid(new byte[16]);
                    RPCProcedure.VersionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                    break;
                case CustomRPC.AddGameInfo:
                    RPCProcedure.AddGameInfo(reader.ReadByte(), (InfoType)reader.ReadByte());
                    break;
                case CustomRPC.UncheckedMurderPlayer:
                    RPCProcedure.UncheckedMurderPlayer(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean());
                    break;
                case CustomRPC.ParalyzePlayer:
                    if (Nightmare.IsNightmare(reader.ReadByte(), out Nightmare nightmareA))
                        RPCProcedure.ParalyzePlayer(nightmareA, reader.ReadPlayer());
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
                    Undertaker.DeadBodyDragged = null;
                    break;
                case CustomRPC.GuardianSetShielded:
                    RPCProcedure.GuardianSetShielded(reader.ReadPlayer());
                    break;
                case CustomRPC.AddSpectator:
                    RPCProcedure.AddSpectator(reader.ReadPlayer());
                    break;
                case CustomRPC.RemoveSpectator:
                    RPCProcedure.RemoveSpectator(reader.ReadByte());
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
                    if (Jailor.IsJailor(reader.ReadByte(), out Jailor jailorA))
                        RPCProcedure.JailorJail(jailorA, reader.ReadPlayer());
                    break;
                case CustomRPC.JailBreak:
                    if (Jailor.IsJailor(reader.ReadByte(), out Jailor jailorB))
                        RPCProcedure.JailBreak(jailorB);
                    break;
                case CustomRPC.MorphlingMorph:
                    RPCProcedure.MorphlingMorph(reader.ReadPlayer());
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
                    if (ParityCop.IsParityCop(reader.ReadByte(), out ParityCop parityCopB))
                        parityCopB.ComparedPlayers.Add(reader.ReadByte());
                    break;
                case CustomRPC.PlayerKilledByAbility:
                    Detective.PlayersKilledByAbility.Add(reader.ReadPlayer());
                    break;
                case CustomRPC.SetJesterWinner:
                    Jester.WinningJesterPlayer = reader.ReadPlayer();
                    break;
                case CustomRPC.SetExecutionerWin:
                    Executioner.TriggerExecutionerWin = true;
                    break;
                case CustomRPC.ArsonistDouse:
                    RPCProcedure.ArsonistDouse(reader.ReadPlayer());
                    break;
                case CustomRPC.PyromaniacDouse:
                    if (Pyromaniac.IsPyromaniac(reader.ReadByte(), out Pyromaniac pyromaniac))
                        pyromaniac.DousedPlayers.Add(reader.ReadByte());
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
                    if (Nightmare.IsNightmare(reader.ReadByte(), out Nightmare nightmareC))
                        nightmareC.BlindedPlayers.Clear();
                    break;
                case CustomRPC.SealVent:
                    RPCProcedure.SealVent(reader.ReadPackedInt32());
                    break;
                case CustomRPC.SetTrap:
                    RPCProcedure.SetVentTrap(reader.ReadPackedInt32());
                    break;
                case CustomRPC.TriggerVentTrap:
                    RPCProcedure.TriggerVentTrap(reader.ReadPlayer(), reader.ReadPackedInt32());
                    break;
                case CustomRPC.ExitAllVents:
                    reader.ReadPlayer().MyPhysics.ExitAllVents();
                    break;
                case CustomRPC.ArsonistWin:
                    RPCProcedure.ArsonistWin();
                    break;
                case CustomRPC.GuesserShoot:
                    RPCProcedure.GuesserShoot(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadPlayer(), (RoleId)reader.ReadByte());
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
                    RPCProcedure.ChangelingChange((RoleId)reader.ReadByte());
                    break;
                case CustomRPC.RefugeeShield:
                    if (Refugee.IsRefugee(reader.ReadByte(), out Refugee refugee))
                        RPCProcedure.RefugeeShield(refugee);
                    break;
                case CustomRPC.RuthlessRomanticShield:
                    if (RuthlessRomantic.IsRuthlessRomantic(reader.ReadByte(), out RuthlessRomantic ruthlessRomanticA))
                        RPCProcedure.RuthlessRomanticShield(ruthlessRomanticA);
                    break;
                case CustomRPC.FakeCompare:
                    if (ParityCop.IsParityCop(reader.ReadByte(), out ParityCop parityCopA))
                        parityCopA.PressedFakeCompare = reader.ReadBoolean();
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
                    RPCProcedure.ReadButtons();
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
                    RPCProcedure.SetChatNotificationOverlay(reader.ReadByte());
                    break;
                case CustomRPC.SpitefulVote:
                    if (reader.ReadPlayer().IsSpiteful(out Spiteful spiteful))
                        spiteful.VotedBy.Add(reader.ReadPlayer());
                    break;
                case CustomRPC.Duel:
                    RPCProcedure.Duel(reader.ReadPlayer(), reader.ReadString());
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
                    reader.ReadPlayer().NetTransform.SnapTo(reader.ReadPlayer().transform.position);
                    break;

            }
        }

        private static PlayerControl ReadPlayer(this MessageReader reader)
        {
            return Helpers.PlayerById(reader.ReadByte());
        }
    }
}
