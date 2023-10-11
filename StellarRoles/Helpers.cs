using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using StellarRoles.Objects;
using StellarRoles.Patches;
using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace StellarRoles
{
    public enum LogLevel
    {
        Message,
        Error,
        Warning,
        Fatal,
        Info,
        Debug
    }
    public static class Helpers
    {
        private static readonly Dictionary<string, Sprite> _CachedSprites = new();

        public const int InfoLineLength = 56;

        public static void Log(LogLevel errorLevel, object @object)
        {
            BepInEx.Logging.ManualLogSource Logger = StellarRolesPlugin.Logger;
            string Message = @object as string;
            switch (errorLevel)
            {
                case LogLevel.Message:
                    Logger.LogMessage(Message);
                    break;
                case LogLevel.Error:
                    Logger.LogError(Message);
                    break;
                case LogLevel.Warning:
                    Logger.LogWarning(Message);
                    break;
                case LogLevel.Fatal:
                    Logger.LogFatal(Message);
                    break;
                case LogLevel.Info:
                    Logger.LogInfo(Message);
                    break;
                case LogLevel.Debug:
#if DEBUG
                    Logger.LogDebug(Message);
#endif
                    break;
            }
        }

        public static void Log(object @object)
        {
            Log(LogLevel.Error, @object);
        }




        public static bool IsInvisible(this PlayerControl player)
        {
            return (Shade.Player == player && Shade.IsInvisble) || (Wraith.Player == player && Wraith.IsInvisible);
        }

        public static void CheckPlayersAlive()
        {
            MapOptions.PlayersAlive = PlayerControl.AllPlayerControls.GetFastEnumerator().Where(player => !player.Data.IsDead).Count();
        }

        public static bool IsAlive(this PlayerControl player)
        {
            return !player.Data.IsDead && !player.Data.Disconnected;
        }

        public static bool CanGuess(this PlayerControl player)
        {
            return
                Vigilante.Player == player ||
                Assassin.Players.Contains(player) ||
                (player.IsNeutralKiller() && Assassin.NeutralKillersHaveAssassin());
        }

        public static void GameStartKillCD()
        {
            float cooldown = CustomOptionHolder.GameStartKillCD.GetFloat();
            HudManagerStartPatch.ImpKillButton.Timer = cooldown;
            RogueButtons.RogueKillButton.Timer = cooldown;
            PyromaniacButtons.PyromaniacKillButton.Timer = cooldown;
            HeadHunterButtons.HeadHunterKillButton.Timer = cooldown;
            RomanticButtons.VengefulRomanticKillButton.Timer = cooldown;
        }


        public static int RemainingShots(PlayerControl player, bool shoot = false)
        {
            int remainingShots = 0;
            bool isOwner = player.AmOwner;
            if (Assassin.Players.Contains(player))
            {
                remainingShots = Assassin.RemainingShotsAssassin;
                if (shoot)
                    Assassin.RemainingShotsAssassin = Mathf.Max(0, Assassin.RemainingShotsAssassin - 1);
            }
            else if (isOwner && player.IsNeutralKiller() && Assassin.NeutralKillersHaveAssassin())
            {
                remainingShots = Assassin.RemainingShotsNK;
                if (shoot)
                    Assassin.RemainingShotsNK = Mathf.Max(0, Assassin.RemainingShotsNK - 1);
            }
            else if (isOwner && Vigilante.Player == player)
            {
                remainingShots = Vigilante.RemainingShotsVigilante;
                if (shoot)
                    Vigilante.RemainingShotsVigilante = Mathf.Max(0, Vigilante.RemainingShotsVigilante - 1);
            }

            return remainingShots;
        }

        public static void MoveTrash()
        {
            if (IsMap(Map.Polus))
            {
                Console panel = UnityEngine.Object.FindObjectsOfType<Console>().FirstOrDefault(x => x.gameObject.name.Contains("panel_garbage"));
                if (panel != null)
                    panel.transform.localPosition = new Vector3(0.28f, -0.48f, -0.0001f);
            }
        }

        public static bool GameStarted => AmongUsClient.Instance?.GameState == InnerNet.InnerNetClient.GameStates.Started;
        public static bool IsHideAndSeek => GameOptionsManager.Instance.currentGameMode == GameModes.HideNSeek;

        public static float GetKillDistance()
        {
            return GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
        }

        public static void AvengedLover()
        {
            RPCProcedure.Send(CustomRPC.AvengedLover);
            VengefulRomantic.AvengedLover = true;
        }

        public static void LoverPairDead()
        {
            RPCProcedure.Send(CustomRPC.LoverPairDead);
            Romantic.PairIsDead = true;
        }

        public static void AddGameInfo(byte playerId, params InfoType[] infoTypes)
        {
            foreach (InfoType infoType in infoTypes)
            {
                RPCProcedure.Send(CustomRPC.AddGameInfo, playerId, (byte)infoType);
                RPCProcedure.AddGameInfo(playerId, infoType);
            }
        }

        public static IEnumerable<PlayerControl> FindClosestPlayers(PlayerControl player, float radius)
        {
            return PlayerControl.AllPlayerControls.GetFastEnumerator()
                .Where(p => (Vector2.Distance(p.GetTruePosition(), player.GetTruePosition()) <= radius)
                && p != player
            );
        }

        public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, IEnumerable<PlayerControl> untargetablePlayers = null, bool canIncrease = false, bool ascended = false, PlayerControl targetOverride = null)
        {
            PlayerControl localPlayer = targetOverride ?? PlayerControl.LocalPlayer;
            if (!MapUtilities.CachedShipStatus || localPlayer.Data.IsDead)
                return localPlayer;
            canIncrease = canIncrease && (Sniper.Players.Contains(localPlayer) || Ascended.AscendedSheriff(localPlayer) || ascended);
            Vector2 truePosition = localPlayer.GetTruePosition();
            float num = GetKillDistance();
            PlayerControl result = null;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                GameData.PlayerInfo data = player.Data;
                if (data.IsDead || data.Disconnected || localPlayer == player || (onlyCrewmates && data.Role.IsImpostor))
                    continue;

                if (untargetablePlayers?.Any(x => x == player) == true)
                    continue;

                if (player.inVent && !targetPlayersInVents)
                    continue;

                Vector2 position = player.GetTruePosition();
                Vector2 vector = position - truePosition;
                float magnitude = vector.magnitude;
                if (PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShadowMask, false) || PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipAndObjectsMask, false))
                    continue;

                if (magnitude <= (canIncrease ? (num + 0.6f) : num))
                {
                    result = player;
                    num = magnitude;
                }
            }
            return result;
        }

        public static DeadBody SetBodyTarget()
        {
            PlayerControl LocalPlayer = PlayerControl.LocalPlayer;
            bool isDead = LocalPlayer.Data.IsDead;
            Vector2 truePosition = LocalPlayer.GetTruePosition();
            float maxDistance = GetKillDistance() * 0.75f;
            Il2CppReferenceArray<Collider2D> colliders = Physics2D.OverlapCircleAll(truePosition, maxDistance, Constants.PlayersOnlyMask);
            DeadBody closestBody = null;
            float closestDistance = float.MaxValue;
            if (isDead)
                return closestBody;

            foreach (Collider2D collider in colliders)
            {
                if (collider.tag != "DeadBody")
                    continue;

                DeadBody component = collider.GetComponent<DeadBody>();
                float distance = Vector2.Distance(truePosition, component.TruePosition);

                if (
                    PhysicsHelpers.AnythingBetween(truePosition, component.TruePosition, Constants.ShipAndObjectsMask, false) ||
                    distance > maxDistance ||
                    distance >= closestDistance
                )
                    continue;

                closestBody = component;
                closestDistance = distance;
            }
            return closestBody;
        }

        public static bool IsMap(Map mapid)
        {
            return GameOptionsManager.Instance.currentNormalGameOptions.MapId == (byte)mapid;
        }

        public static void SetStartOfRoundCooldowns()
        {
            DetectiveButtons.DetectiveButton.Timer = Detective.Cooldown * 0.5f;

            ParityCopButtons.ParityCopCompareButton.Timer = ParityCop.CalculateParityCopCooldown(PlayerControl.LocalPlayer) * 0.5f;
            ParityCopButtons.ParityCopFakeCompareButton.Timer = ParityCop.CompareCooldown * 0.5f;

            TrackerButtons.TrackerMarkButton.Timer = Tracker.TrackTargetCooldown * 0.5f;

            TrapperButtons.TrapperCoverButton.Timer = Trapper.CoverCooldown * 0.5f;
            TrapperButtons.TrapperSetTrapButton.Timer = Trapper.TrapCooldown * 0.5f;

            BomberButtons.BomberBombButton.Timer = Bomber.Cooldown * 0.5f;

            CamouflagerButtons.CamouflagerButton.Timer = Camouflager.Cooldown * 0.5f;

            ChangelingButtons.ChangelingButton.Timer = 5f;

            JanitorButtons.CleanButton.Timer = Janitor.CalculateJanitorCooldown() * 0.5f;

            MinerButtons.MineButton.Timer = Miner.Cooldown * 0.5f;

            MorphlingButtons.MorphlingButton.Timer = Morphling.Cooldown * 0.5f;

            ShadeButtons.VanishButton.Timer = Shade.ShadeCooldown * 0.5f;

            UndertakerButtons.UndertakerDragButton.Timer = Undertaker.DraggingCooldown * 0.5f;

            VampireButtons.VampireBiteButton.Timer = Vampire.BiteCooldown * 0.5f;

            WarlockButtons.CurseButton.Timer = Warlock.Cooldown * 0.5f;

            WraithButtons.WraithPhaseButton.Timer = Wraith.PhaseCooldown * 0.5f;
            WraithButtons.WraithLanternPlaceButton.Timer = Wraith.LanternCooldown * 0.5f;

            ArsonistButtons.ArsonistButton.Timer = Arsonist.Cooldown * 0.5f;

            RomanticButtons.RomanticProtectButton.Timer = Romantic.Cooldown * 0.5f;

            ScavengerButtons.ScavengerEatButton.Timer = Scavenger.Cooldown * 0.5f;

            HeadHunterButtons.HeadHunterTrackerButton.Timer = HeadHunter.Cooldown * 0.5f;

            NightmareButtons.NightmareBlindButton.Timer = Nightmare.BlindCooldown * 0.5f;
            NightmareButtons.NightmareParalyzeButton.Timer = Nightmare.ParalyzeCooldown * 0.5f;

            PyromaniacButtons.PyromaniacDouseButton.Timer = Pyromaniac.DouseCooldown * 0.5f;
        }

        public static bool IsMorphlingTargetAndMorphed(this PlayerControl target)
        {
            return Morphling.MorphTimer > 0 && Morphling.Player != null && Morphling.MorphTarget == target;
        }

        public static bool IsMorphed(this PlayerControl player)
        {
            return player == Morphling.Player && Morphling.MorphTimer > 0 && Morphling.MorphTarget != null;
        }
        public static string ReplaceWithColors(string text)
        {
            return text
                .Replace("Administrator", ColorString(Administrator.Color, "Administrator"))
                .Replace("Detective", ColorString(Detective.Color, "Detective"))
                .Replace("Engineer", ColorString(Engineer.Color, "Engineer"))
                .Replace("Guardian", ColorString(Guardian.Color, "Guardian"))
                .Replace("Investigator", ColorString(Investigator.Color, "Investigator"))
                .Replace("Jailor", ColorString(Jailor.Color, "Jailor"))
                .Replace("Mayor", ColorString(Mayor.Color, "Mayor"))
                .Replace("Medic", ColorString(Medic.Color, "Medic"))
                .Replace("Parity Cop", ColorString(ParityCop.Color, "Parity Cop"))
                .Replace("Sheriff", ColorString(Sheriff.Color, "Sheriff"))
                .Replace("Spy", ColorString(Spy.Color, "Spy"))
                .Replace("Tracker", ColorString(Tracker.Color, "Tracker"))
                .Replace("Trapper", ColorString(Trapper.Color, "Trapper"))
                .Replace("Watcher", ColorString(Watcher.Color, "Watcher"))
                .Replace("Bomber", ColorString(Bomber.Color, "Bomber"))
                .Replace("Bounty Hunter", ColorString(BountyHunter.Color, "Bounty Hunter"))
                .Replace("Camouflager", ColorString(Camouflager.Color, "Camouflager"))
                .Replace("Changeling", ColorString(Changeling.Color, "Changeling"))
                .Replace("Cultist", ColorString(Cultist.Color, "Cultist"))
                .Replace("Hacker", ColorString(Hacker.Color, "Hacker"))
                .Replace("Follower", ColorString(Follower.Color, "Follower"))
                .Replace("Janitor", ColorString(Janitor.Color, "Janitor"))
                .Replace("Miner", ColorString(Miner.Color, "Miner"))
                .Replace("Morphling", ColorString(Morphling.Color, "Morphling"))
                .Replace("Shade", ColorString(Shade.Color, "Shade"))
                .Replace("Undertaker", ColorString(Undertaker.Color, "Undertaker"))
                .Replace("Vampire", ColorString(Vampire.Color, "Vampire"))
                .Replace("Warlock", ColorString(Warlock.Color, "Warlock"))
                .Replace("Wraith", ColorString(Wraith.Color, "Wraith"))
                .Replace("Arsonist", ColorString(Arsonist.Color, "Arsonist"))
                .Replace("Scavenger", ColorString(Scavenger.Color, "Scavenger"))
                .Replace("Executioner", ColorString(Executioner.Color, "Executioner"))
                .Replace("Jester", ColorString(Jester.Color, "Jester"))
                .Replace("Refugee", ColorString(Refugee.Color, "Refugee"))
                .Replace("Romantic", ColorString(Romantic.Color, "Romantic"))
                .Replace("Vengeful " + ColorString(Romantic.Color, "Romantic"), ColorString(VengefulRomantic.Color, "Vengeful Romantic"))
                .Replace("Ruthless " + ColorString(Romantic.Color, "Romantic"), ColorString(RuthlessRomantic.Color, "Ruthless Romantic"))
                .Replace("Headhunter", ColorString(HeadHunter.Color, "Headhunter"))
                .Replace("Nightmare", ColorString(Nightmare.Color, "Nightmare"))
                .Replace("Pyromaniac", ColorString(Pyromaniac.Color, "Pyromaniac"))
                .Replace("Assassin", ColorString(Palette.ImpostorRed, "Assassin"))
                .Replace("Vigilante", ColorString(Vigilante.Color, "Vigilante"))
                .Replace("Impostors", ColorString(Palette.ImpostorRed, "Impostors"))
                .Replace("Neutrals", ColorString(Color.gray, "Neutrals"))
                .Replace("Neutral Killers", ColorString(NeutralKiller.Color, "Neutral Killers"))
                .Replace("Psychic", ColorString(Psychic.Color, "Psychic"))
                .Replace("Refugee", ColorString(Refugee.Color, "Refugee"));
        }

        private static readonly Regex MatchLengthRegex = new("<color=#[A-Z0-9]{8}>(.+)<\\/color>");

        public static string WrapText(string text, int width = InfoLineLength)
        {
            List<string> newLines = new();
            string[] lines = text.Split("\n");

            foreach (string line in lines)
            {
                if (line.Length <= width)
                {
                    newLines.Add(line);
                    continue;
                }

                List<List<string>> splitLines = new()
                {
                    new List<string>()
                };
                int length = 0;
                foreach (string word in line.Split(" "))
                {
                    Match matches = MatchLengthRegex.Match(word);
                    int trueLength = matches.Groups.Count >= 2 ? matches.Groups[1].Value.Length : word.Length;
                    if (length + trueLength > width)
                    {
                        splitLines.Add(new List<string>());
                        length = 0;
                    }
                    splitLines[^1].Add(word);
                    length += trueLength;
                }

                foreach (List<string> splitLine in splitLines)
                    newLines.Add(string.Join(' ', splitLine));
            }

            return string.Join('\n', newLines);
        }

        public static void TrackTarget(PlayerControl target, Arrow arrow, Color color)
        {
            if (target == null) return;

            bool isDead = target.Data.IsDead;
            Vector3 position = target.transform.position;
            DeadBody body = null;
            if (isDead)
            { // Check for dead body
                body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
            }
            if (body != null)
                position = body.transform.position;

            arrow.Update(position, color);
            arrow.Object.SetActive(!isDead || body != null);
        }

        public static void SetBodySize()
        {
            Il2CppArrayBase<DeadBody> bodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            foreach (DeadBody body in bodies)
                if (body.IsGiantBody())
                {
                    if (body.transform.localScale != new Vector3(1f, 1f, 1f))
                        body.transform.localScale = new Vector3(1f, 1f, 1f);
                }
                else if (body.IsMiniBody())
                {
                    if (body.transform.localScale != new Vector3(0.5f, 0.5f, 1f))
                        body.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                }
        }

        public static void SetPlayerOutline(PlayerControl target, Color color)
        {
            // TODO: do we need to chain cosmeics?.currentBodySprite?.BodySprite?
            if (target?.cosmetics?.currentBodySprite?.BodySprite == null)
                return;

            target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
            target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
        }

        // TODO: just provide the text as a parameter?
        public static void ShowTargetNameOnButtonExplicit(PlayerControl target, CustomButton button, string defaultText)
        {
            button.ActionButton.OverrideText(target == null ? defaultText : target.Data.PlayerName);
            button.ShowButtonText = true;
        }

        public static void ResetVentBug()
        {
            RPCProcedure.Send(CustomRPC.ExitAllVents, PlayerControl.LocalPlayer.PlayerId);
            PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
        }

        public static Sprite LoadSpriteFromResources(string path, float pixelsPerUnit)
        {
            try
            {
                if (_CachedSprites.TryGetValue(path + pixelsPerUnit, out Sprite sprite))
                    return sprite;
                Texture2D texture = LoadTextureFromResources(path);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                return _CachedSprites[path + pixelsPerUnit] = sprite;
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Error loading sprite from path: {path} - {ex.StackTrace}");
            }
            return null;
        }

        public static unsafe Texture2D LoadTextureFromResources(string path)
        {
            try
            {
                Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
                long length = stream.Length;
                Il2CppStructArray<byte> byteTexture = new(length);
                stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Error loading texture from resources: {path} - {ex.StackTrace}");
            }
            return null;
        }

        public static Texture2D LoadTextureFromDisk(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                    ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false);
                    return texture;
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Error loading texture from disk: {path} - {ex.StackTrace}");
            }
            return null;
        }

        public static AudioClip LoadAudioClipFromResources(string path, string clipName = "UNNAMED_TOR_AUDIO_CLIP")
        {
            // must be "raw (headerless) 2-channel signed 32 bit pcm (le)" (can e.g. use Audacity� to export)
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
                byte[] byteAudio = new byte[stream.Length];
                stream.Read(byteAudio, 0, (int)stream.Length);
                float[] samples = new float[byteAudio.Length / 4]; // 4 bytes per sample
                int offset;
                for (int i = 0; i < samples.Length; i++)
                {
                    offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / int.MaxValue;
                }
                AudioClip audioClip = AudioClip.Create(clipName, samples.Length, 2, 48000, false);
                audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Error loading AudioClip from resources: {path} - {ex.StackTrace}");
            }
            return null;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (GameData.PlayerInfo player in GameData.Instance.AllPlayers.GetFastEnumerator())
                if (player.PlayerId == id)
                    return player.Object;

            return null;
        }

        public static bool IsNeutral(this PlayerControl player)
        {
            return
                player == Executioner.Player ||
                player == Arsonist.Player ||
                player == Scavenger.Player ||
                player == Romantic.Player ||
                player.IsNeutralKiller() ||
                player.IsJester(out _) ||
                player.IsNightmare(out _);

        }

        public static bool IsTeamCultist(this PlayerControl player) => (player == Cultist.Player || player == Follower.Player);
        public static bool SpecialFollower(this PlayerControl player) => (player == Follower.Player && Cultist.FollowerSpecialRoleAssigned);
        public static bool isFollower(this PlayerControl player) => (player == Follower.Player);


        public static bool IsCrew(this PlayerControl player)
        {
            if (player.Data.Role.IsImpostor || player.IsRefugee(out _))
                return false;
            else if (player == Romantic.Player)
                return Romantic.IsCrewmate;
            else if (player == VengefulRomantic.Player)
                return VengefulRomantic.IsCrewmate;
            else
                return !IsNeutral(player);
        }

        public static bool IsKiller(this PlayerControl player)
        {
            return
                player.Data.Role.IsImpostor ||
                player == Sheriff.Player ||
                (!VengefulRomantic.AvengedLover && player == VengefulRomantic.Player) ||
                player.IsNeutralKiller();

        }

        public static void SetKillerCooldown()
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            CustomButton killbutton = null;
            if (NeutralKiller.Players.Contains(player))
            {
                killbutton = RogueButtons.RogueKillButton;
            }
            else if (player.Data.Role.IsImpostor)
            {
                killbutton = HudManagerStartPatch.ImpKillButton;
            }
            else if (player.IsPyromaniac())
            {
                killbutton = PyromaniacButtons.PyromaniacKillButton;
            }
            else if (HeadHunter.Player == player)
            {
                killbutton = HeadHunterButtons.HeadHunterKillButton;
            }
            else if (Sheriff.Player == player)
            {
                killbutton = SheriffButtons.SheriffKillButton;
            }
            if (killbutton != null)
                killbutton.Timer = killbutton.MaxTimer;
        }

        public static bool IsRogueImpostor(byte playerId)
        {
            return NeutralKiller.RogueImps.Contains(playerId);
        }

        public static bool CantGuess(RoleInfo roleInfo)
        {
            RoleId roleId = roleInfo.RoleId;
            Faction faction = roleInfo.FactionId;

            RoleId guesserRole = (Vigilante.Player != null && PlayerControl.LocalPlayer.PlayerId == Vigilante.Player.PlayerId) ? RoleId.Vigilante : (PlayerControl.LocalPlayer.IsNeutralKiller() ? RoleId.NKAssassin : RoleId.Assassin);
            RoleManagerSelectRolesPatch.RoleAssignmentData roleData = RoleManagerSelectRolesPatch.GetRoleAssignmentData();

            if (roleId == RoleId.Crewmate)
                return
                    (!Assassin.AssassinCanGuessCrew && guesserRole == RoleId.Assassin) ||
                    (!Assassin.NeutralKillerAssassinCanGuessCrew && guesserRole == RoleId.NKAssassin);
            else if (roleId == RoleId.RogueImpostor || roleId == RoleId.Follower)
                return true;
            else if (roleId == RoleId.Impostor)
                return guesserRole != RoleId.Vigilante;
            else if (roleId == RoleId.Refugee)
            {
                if (roleData.MaxNeutralRoles != 0)
                {
                    if (
                        (CustomOptionHolder.ExecutionerSpawnRate.GetSelection() > 0 && Executioner.PromotesTo == ExePromotes.Refugee) ||
                        (CustomOptionHolder.ScavengerSpawnRate.GetSelection() > 0 && roleData.NeutralSettings.ContainsKey(RoleId.Scavenger)) ||
                        (CustomOptionHolder.RomanticSpawnRate.GetSelection() > 0 && roleData.NeutralSettings.ContainsKey(RoleId.Romantic))
                    )
                        return false;
                }
                return true;
            }
            else if (roleId == RoleId.Spy)
                return
                    roleData.Impostors.Count <= 1 ||
                    !roleData.CrewSettings.TryGetValue(RoleId.Spy, out int spySetting) || spySetting == 0 ||
                    (!Assassin.AssassinGuesserCanGuessSpy && guesserRole == RoleId.Assassin);
            else if (roleId == RoleId.VengefulRomantic)
                return roleData.MaxNeutralRoles == 0 || CustomOptionHolder.RomanticSpawnRate.GetSelection() == 0;
            else if (roleId == RoleId.RuthlessRomantic)
                return roleData.MaxNeutralRoles == 0 || (guesserRole != RoleId.Vigilante) || CustomOptionHolder.RomanticSpawnRate.GetSelection() == 0;
            else if (faction == Faction.Neutral)
            {
                if (roleData.MaxNeutralRoles == 0)
                    return true;

                if (roleId == RoleId.Jester)
                {
                    bool jesterCanSpawn = CustomOptionHolder.JesterSpawnRate.GetSelection() > 0;
                    bool executionerPromotesToJester = CustomOptionHolder.ExecutionerSpawnRate.GetSelection() > 0 && Executioner.PromotesTo == ExePromotes.Jester;

                    return !jesterCanSpawn && !executionerPromotesToJester;
                }

                return roleData.NeutralSettings.TryGetValue(roleId, out int neutralSetting) && neutralSetting == 0;
            }
            else if (faction == Faction.NK)
                return
                    roleData.MaxNeutralKillerRoles == 0 ||
                    guesserRole != RoleId.Vigilante ||
                    !roleData.NeutralKillerSettings.TryGetValue(roleId, out int neutralSetting) || neutralSetting == 0;
            else if (faction == Faction.Impostor)
                return
                    guesserRole != RoleId.Vigilante ||
                    !roleData.ImpSettings.TryGetValue(roleId, out int impSetting) || impSetting == 0;
            else if (faction == Faction.Crewmate)
                return !roleData.CrewSettings.TryGetValue(roleInfo.RoleId, out int crewSetting) || crewSetting == 0;
            else
                return false;
        }

        public static bool GetNotActiveRoles(RoleInfo roleInfo)
        {
            RoleId role = roleInfo.RoleId;
            Faction faction = roleInfo.FactionId;

            RoleManagerSelectRolesPatch.RoleAssignmentData roleData = RoleManagerSelectRolesPatch.GetRoleAssignmentData();

            if (role == RoleId.Crewmate || role == RoleId.Impostor)
                return false;
            if (role == RoleId.RogueImpostor)
                return true;

            if (faction == Faction.Crewmate)
            {
                if (role == RoleId.Refugee)
                {
                    if (roleData.MaxNeutralRoles != 0)
                    {
                        if (
                            (CustomOptionHolder.ExecutionerSpawnRate.GetSelection() > 0 && Executioner.PromotesTo == ExePromotes.Refugee && roleData.NeutralSettings.ContainsKey(RoleId.Executioner)) ||
                            (CustomOptionHolder.ScavengerSpawnRate.GetSelection() > 0 && roleData.NeutralSettings.ContainsKey(RoleId.Scavenger)) ||
                            (CustomOptionHolder.RomanticSpawnRate.GetSelection() > 0 && roleData.NeutralSettings.ContainsKey(RoleId.Romantic))
                        )
                            return false;
                    }
                    else
                        return true;
                }
                else if (role == RoleId.Spy && roleData.Impostors.Count <= 1)
                    return true;
                else if (!roleData.CrewSettings.TryGetValue(role, out int crewSetting) || crewSetting == 0)
                    return true;
            }
            else if (faction == Faction.Impostor)
            {
                if (role == RoleId.Follower)
                    return !roleData.ImpSettings.TryGetValue(RoleId.Cultist, out int cultistSetting) || cultistSetting == 0;

                else if (!roleData.ImpSettings.TryGetValue(role, out int impSetting) || impSetting == 0)
                    return true;
            }
            else if (faction == Faction.NK)
            {
                if (role == RoleId.RuthlessRomantic)
                {
                    if (roleData.MaxNeutralRoles == 0)
                        return true;

                    return !roleData.NeutralSettings.TryGetValue(RoleId.Romantic, out int romanticSetting) || romanticSetting == 0;
                }
                else if (!roleData.NeutralKillerSettings.TryGetValue(role, out int nkSetting) || nkSetting == 0)
                    return true;
            }
            else if (faction == Faction.Neutral)
            {
                if (roleData.MaxNeutralRoles == 0)
                    return true;
                else if (role == RoleId.Jester)
                {
                    bool jesterCanSpawn = CustomOptionHolder.JesterSpawnRate.GetSelection() > 0 && roleData.NeutralSettings.ContainsKey(RoleId.Jester);
                    bool executionerPromotesToJester = CustomOptionHolder.ExecutionerSpawnRate.GetSelection() > 0 && Executioner.PromotesTo == ExePromotes.Jester && roleData.NeutralSettings.ContainsKey(RoleId.Executioner);

                    return !jesterCanSpawn && !executionerPromotesToJester;
                }
                else if (role == RoleId.VengefulRomantic)
                    return !roleData.NeutralSettings.TryGetValue(RoleId.Romantic, out int romanticValue) || romanticValue == 0;
                else if (!roleData.NeutralSettings.TryGetValue(role, out int neutralValue) || neutralValue == 0)
                    return true;
            }

            return faction == Faction.Modifier && RoleManagerSelectRolesPatch.GetSelectionForRoleId(roleInfo.RoleId) <= 0;
        }

        public static void TurnToCrewmate(PlayerControl player)
        {
            RPCProcedure.Send(CustomRPC.TurnToCrewmate, player.PlayerId);
            RPCProcedure.TurnToCrewmate(player);
        }

        public static PlayerControl RomanticWinConditionNeutral(PlayerControl player)
        {
            if (Romantic.Player != null && Romantic.HasLover && Romantic.Lover == player)
                return Romantic.Player;
            else if (VengefulRomantic.Player != null && VengefulRomantic.Lover == player)
                return VengefulRomantic.Player;
            else if (Beloved.Player != null && Beloved.Romantic == player)
                return Beloved.Player;
            else
                return null;
        }

        public static void TurnToImpostor(PlayerControl player)
        {
            player.RpcSetRole(RoleTypes.Impostor);
            player.Data.Role.TeamType = RoleTeamTypes.Impostor;
            RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            player.SetKillTimer(Helpers.KillCooldown());

            foreach (PlayerControl otherPlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if (otherPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                    player.cosmetics.nameText.color = Palette.ImpostorRed;
        }

        public static bool IsSaboActive()
        {
            return PlayerControl.LocalPlayer.myTasks.GetFastEnumerator().Any(x =>
            x.TaskType == TaskTypes.FixLights ||
            x.TaskType == TaskTypes.RestoreOxy ||
            x.TaskType == TaskTypes.ResetReactor ||
            x.TaskType == TaskTypes.ResetSeismic ||
            x.TaskType == TaskTypes.StopCharles ||
            x.TaskType == TaskTypes.FixLights ||
            SubmergedCompatibility.IsSubmerged && x.TaskType == SubmergedCompatibility.RetrieveOxygenMask);
        }

        public static SabatageTypes GetActiveSabo()
        {
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
            {
                var tasktype = task.TaskType;

                if (tasktype == TaskTypes.FixLights)
                    return SabatageTypes.Lights;
                else if (tasktype == TaskTypes.RestoreOxy)
                    return SabatageTypes.O2;
                else if (tasktype == TaskTypes.ResetReactor)
                    return SabatageTypes.Reactor;
                else if (tasktype == TaskTypes.ResetSeismic)
                    return SabatageTypes.Seismic;
                else if (tasktype == TaskTypes.StopCharles)
                    return SabatageTypes.Charles;
                else if (tasktype == TaskTypes.FixComms)
                    return SabatageTypes.Comms;
                else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                {
                    return SabatageTypes.OxyMask;
                }
            }
            return SabatageTypes.None;
        }

        public static bool IsLightsActive()
        {
            return GetActiveSabo() == SabatageTypes.Lights;
        }

        public static bool IsCommsActive()
        {
            return GetActiveSabo() == SabatageTypes.Comms;
        }

        public static bool CanShowButtons =>
            !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
            !MeetingHud.Instance &&
            !ExileController.Instance;

        public static bool RoleCanSabotage(this PlayerControl player)
        {
            return player.Data?.Role.IsImpostor == true;
        }

        public static void CheckImpsAlive()
        {
            bool isLocalImp = PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            int count = PlayerControl.AllPlayerControls.GetFastEnumerator().Count(player => (player.Data.Role.IsImpostor || isLocalImp && Spy.Player == player) && !player.Data.IsDead);

            if (count == 0 && !MapOptions.Allimpsdead)
                MapOptions.Allimpsdead = true;
            else if (count == 1 && !MapOptions.LastImp)
                MapOptions.LastImp = true;
        }

        public static bool NeutralKillerCanSabo(this PlayerControl player)
        {
            return player.IsNeutralKiller() && (NeutralKiller.GainsSabo == GainSabo.ImpWipe && MapOptions.Allimpsdead) || NeutralKiller.GainsSabo == GainSabo.GameStart;
        }


        /// <summary>
        /// Player is a Rogue Impostor, Pyromaniac or HeadHunter
        /// </summary>
        public static bool IsNeutralKiller(this PlayerControl player)
        {
            return HeadHunter.Player == player || player.IsPyromaniac(out _) || NeutralKiller.Players.Contains(player);
        }

        public static bool IsCrewKillingRole(this PlayerControl player)
        {
            return player == Sheriff.Player || player == Vigilante.Player;
        }

        public static PlayerControl GetRomancePartner(this PlayerControl player)
        {
            if (Romantic.Lover == player)
                return Romantic.Player;
            else if (Romantic.Player == player)
                return Romantic.Lover;
            else
                return null;
        }

        public static PlayerControl GetCultistPartner(this PlayerControl player)
        {
            if (Cultist.Player == player && Follower.Player != null)
                return Follower.Player;
            else if (Follower.Player == player && Cultist.Player != null)
                return Cultist.Player;
            else
                return null;
        }

        public static float KillCooldown()
        {
            return GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
        }

        public static bool IsMiniBody(this DeadBody body)
        {
            return body.ParentId == Mini.Player?.PlayerId;
        }

        public static bool IsGiantBody(this DeadBody body)
        {
            return body.ParentId == Giant.Player?.PlayerId;
        }

        public static void HandleVampireBiteOnBodyReport()
        {
            // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
            CheckMurderAttemptAndKill(Vampire.Player, Vampire.Bitten, false, true);
            RPCProcedure.Send(CustomRPC.VampireResetBitten);
            Vampire.Bitten = null;
        }

        public static void HandleBomberExplodeOnBodyReport()
        {
            foreach (Bombed bombed in Bombed.BombedDictionary.Values)
            {
                if (bombed.BombActive)
                {
                    // Murder the bombed player if bomb active (regardless whether the kill was successful or not)
                    CheckBombedAttemptAndKill(bombed.Bomber, bombed.Player, true, false);
                    AddGameInfo(bombed.Bomber.PlayerId, InfoType.AddAbilityKill, InfoType.AddKill);
                }
                RPCProcedure.Send(CustomRPC.GiveBomb, bombed.Player.PlayerId, bombed.Bomber.PlayerId, true);
                RPCProcedure.GiveBomb(bombed.Player, bombed.Bomber, true);
            }
        }

        internal static string GetRoleString(RoleInfo roleInfo)
        {
            return ColorString(roleInfo.Color, $"{roleInfo.Name}: {roleInfo.ShortDescription}");
        }

        public static bool HasFakeTasks(this PlayerControl player)
        {
            return IsNeutral(player) || VengefulRomantic.Player == player || Beloved.Player == player || Spectator.IsSpectator(player.PlayerId) || Refugee.IsRefugee(player.PlayerId, out _);
        }

        public static void ClearAllTasks(this PlayerControl player)
        {
            foreach (PlayerTask playerTask in player.myTasks.GetFastEnumerator())
            {
                playerTask.OnRemove();
                UnityEngine.Object.Destroy(playerTask.gameObject);
            }
            player.myTasks.Clear();

            // TODO: does this need to use optional chaining
            player.Data?.Tasks?.Clear();
        }

        public static void RemoveSabo()
        {
            InfectedOverlay infect = PlayerControl.LocalPlayer.GetComponent<InfectedOverlay>();
            foreach (MapRoom sabo in infect.rooms)
                if (sabo.name.Contains("Reactor"))
                    sabo.gameObject.SetActive(false);
            foreach (ButtonBehavior sbo in infect.allButtons)
                if (sbo.gameObject.name.Contains("Reactor"))
                    sbo.gameObject.SetActive(false);
        }

        public static void SetSemiTransparent(this PoolablePlayer player, bool shouldBeTransparrent)
        {
            float alpha = shouldBeTransparrent ? 0.25f : 1f;
            foreach (SpriteRenderer renderer in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, alpha);
        }

        public static string ColorString(Color c, string s)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }

        private static byte ToByte(float f)
        {
            return (byte)(Mathf.Clamp01(f) * 255);
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            KeyValuePair<byte, int> result = new(byte.MaxValue, int.MinValue);
            foreach (KeyValuePair<byte, int> keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                    tie = true;
            }
            return result;
        }

        public static bool PlayerIsClose(PlayerControl player, float multiplier)
        {
            if (player.Data.Disconnected || player.Data.IsDead)
                return false;

            Vector2 targetPosition = player.GetTruePosition();
            if (player.AmOwner)
                return true;

            PlayerControl localPlayer = PlayerControl.LocalPlayer;

            return new Vector2(localPlayer.GetTruePosition().x - targetPosition.x, localPlayer.GetTruePosition().y - targetPosition.y).magnitude <= multiplier;
        }

        public static PlayerControl PlayerByName(string name)
        {
            string lowercaseName = name.ToLower();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.Data.PlayerName.ToLower() == lowercaseName)
                    return player;
            return null;
        }

        public static bool ShouldHidePlayerName(PlayerControl target)
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer.Data.IsDead)
                return false;
            if (target.Data.Disconnected)
                return true;

            return
                (Camouflager.CamouflageTimer > 0f && !(localPlayer.Data.Role.IsImpostor && (target.Data.Role.IsImpostor || target == Spy.Player)))
                || IsInvisible(target)
                || (MapOptions.HideOutOfSightNametags && PhysicsHelpers.AnythingBetween(localPlayer.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false)
                );
        }

        public static void SetDefaultLook(this PlayerControl target)
        {
            target.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        // TODO: this can be reworked to use player.Data rather than passing everything from it
        public static void SetLook(this PlayerControl target, string playerName, int colorId, string hatId, string visorId, string skinId, string petId)
        {
            target.RawSetColor(colorId);
            target.RawSetVisor(visorId, colorId);
            target.RawSetHat(hatId, colorId);
            target.RawSetName(ShouldHidePlayerName(target) ? "" : playerName);
            target.RawSetPet(petId, colorId);
            target.SetPlayerSize();

            SkinViewData nextSkin;
            try
            { nextSkin = ShipStatus.Instance.CosmeticsCache.GetSkin(skinId); }
            catch { return; };

            PlayerPhysics playerPhysics = target.MyPhysics;
            PowerTools.SpriteAnim spriteAnim = playerPhysics.myPlayer.cosmetics.skin.animator;
            AnimationClip currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();

            AnimationClip clip;
            if (currentPhysicsAnim == playerPhysics.Animations.group.RunAnim)
                clip = nextSkin.RunAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.SpawnAnim)
                clip = nextSkin.SpawnAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.EnterVentAnim)
                clip = nextSkin.EnterVentAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.ExitVentAnim)
                clip = nextSkin.ExitVentAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.IdleAnim)
                clip = nextSkin.IdleAnim;
            else
                clip = nextSkin.IdleAnim;
            float progress = playerPhysics.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            playerPhysics.myPlayer.cosmetics.skin.skin = nextSkin;
            playerPhysics.myPlayer.cosmetics.skin.UpdateMaterial();

            spriteAnim.Play(clip, 1f);
            spriteAnim.m_animator.Play("a", 0, progress % 1);
            spriteAnim.m_animator.Update(0f);
        }

        public static void ShowFlash(Color color, float duration = 1f)
        {
            HudManager hudManager = HudManager.Instance;
            if (hudManager.FullScreen == null)
                return;
            hudManager.FullScreen.gameObject.SetActive(true);
            hudManager.FullScreen.enabled = true;
            hudManager.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                SpriteRenderer renderer = hudManager.FullScreen;
                if (renderer == null)
                    return;

                renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(
                    (p < 0.5 ? (p * 2) : ((1 - p) * 2)) * 0.75f
                ));

                if (p == 1f)
                    renderer.enabled = false;
            })));
        }

        public static bool RoleCanUseVents(this PlayerControl player)
        {
            if (player.inVent)
                return true;

            bool commsActive = IsCommsActive();

            if (Engineer.Player == player && Engineer.CanVent)
                return !EngineerAbilities.IsRoleBlocked();
            else if ((Jester.CanEnterVents && player.IsJester(out _)) || (Scavenger.CanUseVents && Scavenger.Player == player))
                return !(MapOptions.NeutralRoleBlock && commsActive);
            else if (player == Undertaker.Player && Undertaker.DeadBodyDragged != null && !Undertaker.CanDragAndVent)
                return false;

            return
                player.Data?.Role.CanVent == true ||
                player.IsNeutralKiller() ||
                Spy.Player == player;
        }

        public static float SpitefulMultiplier(PlayerControl player)
        {
            return Spiteful.SpitefulRoles.Any(x => x.VotedBy.Any(p => p.PlayerId == player.PlayerId)) ? Spiteful.Punishment : 1f;
        }

        public static float ClutchMultiplier(PlayerControl player)
        {
            return Clutch.Players.Contains(player) && MapOptions.LastImp ? Clutch.Bonus : 1f;
        }

        public static bool IsFirstKilled(PlayerControl player) => MapOptions.FirstKillPlayer == player || MapOptions.FirstKillName == player.Data.PlayerName;

        public static MurderAttemptResult CheckBombedAttempt(PlayerControl killer, PlayerControl target, bool isMeetingStart = false)
        {
            if (!isMeetingStart)
            {
                // Handle first kill attempt
                if (MapOptions.ShieldFirstKill && IsFirstKilled(target))
                    return MurderAttemptResult.SuppressKill;

                // Block impostor shielded kill
                if (Guardian.Shielded == target && !GuardianAbilities.IsRoleBlocked())
                {
                    SoundEffectsManager.Play(Sounds.Fail);
                    return MurderAttemptResult.SuppressKill;
                }

                if (target.IsRefugeeAndVestActive() || target.IsRuthlessRomanticAndVestActive())
                {
                    SoundEffectsManager.Play(Sounds.Fail);
                    return MurderAttemptResult.SuppressKill;
                }

                if (Romantic.IsVestActive && Romantic.Lover == target)
                {
                    SoundEffectsManager.Play(Sounds.Fail);
                    return MurderAttemptResult.SuppressKill;
                }
            }

            if (target.Data.IsDead)
                return MurderAttemptResult.SuppressKill;

            return MurderAttemptResult.PerformKill;
        }

        public static MurderAttemptResult CheckMuderAttempt(PlayerControl killer, PlayerControl target, bool ignoreIfKillerIsDead = false)
        {
            // Modified vanilla checks
            // Allow non Impostor kills compared to vanilla code
            // Allow killing players in vents compared to vanilla code
            if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
            if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
            if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code

            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return MurderAttemptResult.PerformKill;

            // Handle first kill attempt
            if (MapOptions.ShieldFirstKill && IsFirstKilled(target))
                return MurderAttemptResult.SuppressKill;

            // Block impostor shielded kill
            if (Guardian.Shielded == target && !GuardianAbilities.IsRoleBlocked())
            {
                SoundEffectsManager.Play(Sounds.Fail);
                return MurderAttemptResult.SuppressKill;
            }

            if (target.IsRefugeeAndVestActive() || target.IsRuthlessRomanticAndVestActive())
            {
                SoundEffectsManager.Play(Sounds.Fail);
                return MurderAttemptResult.SuppressKill;
            }

            if (Romantic.IsVestActive && Romantic.Lover == target)
            {
                SoundEffectsManager.Play(Sounds.Fail);
                return MurderAttemptResult.SuppressKill;
            }

            return MurderAttemptResult.PerformKill;
        }

        public static void UncheckedMurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation = true)
        {
            RPCProcedure.Send(CustomRPC.UncheckedMurderPlayer, killer.PlayerId, target.PlayerId, showAnimation);
            RPCProcedure.UncheckedMurderPlayer(killer, target, showAnimation);
        }

        public static void ExilePlayer(PlayerControl player)
        {
            RPCProcedure.Send(CustomRPC.ExilePlayer, player.PlayerId);
            RPCProcedure.ExilePlayer(player);
        }

        public static void PlayerKilledByAbility(PlayerControl player)
        {
            RPCProcedure.Send(CustomRPC.PlayerKilledByAbility, player.PlayerId);
            Detective.PlayersKilledByAbility.Add(player);
        }

        public static MurderAttemptResult CheckBombedAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true)
        {
            // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
            // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible

            MurderAttemptResult murder = CheckBombedAttempt(killer, target, isMeetingStart);
            if (murder == MurderAttemptResult.PerformKill)
                UncheckedMurderPlayer(killer, target, showAnimation);
            return murder;
        }

        public static MurderAttemptResult CheckMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool showAnimation = true, bool ignoreIfKillerIsDead = false)
        {
            // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
            // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible

            MurderAttemptResult murder = CheckMuderAttempt(killer, target, ignoreIfKillerIsDead);
            if (murder == MurderAttemptResult.PerformKill)
            {
                UncheckedMurderPlayer(killer, target, showAnimation);

                if (Vampire.Player == killer && Vampire.Bitten == target)
                    PlayerKilledByAbility(target);
            }

            return murder;
        }

        public static void ShareGameVersion()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, SendOption.Reliable, -1);
            writer.Write((byte)StellarRolesPlugin.Version.Major);
            writer.Write((byte)StellarRolesPlugin.Version.Minor);
            writer.Write((byte)StellarRolesPlugin.Version.Build);
            writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.Timer : -1f);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(StellarRolesPlugin.Version.Revision < 0 ? 0xFF : StellarRolesPlugin.Version.Revision));
            Guid moduleVersionId = Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId;
            writer.Write(moduleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.VersionHandshake(
                StellarRolesPlugin.Version.Major,
                StellarRolesPlugin.Version.Minor,
                StellarRolesPlugin.Version.Build,
                StellarRolesPlugin.Version.Revision,
                moduleVersionId,
                AmongUsClient.Instance.ClientId
            );
        }

        public static void ResetZoom()
        {
            Camera.main.orthographicSize = 3.0f;
            HudManager hudManager = HudManager.Instance;
            hudManager.UICamera.orthographicSize = 3.0f;
            hudManager.transform.localScale = Vector3.one;
            hudManager.Chat.transform.localScale = Vector3.one;
            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }

        public static int TasksLeft(this PlayerControl player)
        {
            (int playerCompleted, int playerTotal) = TasksHandler.TaskInfo(player.Data);
            return playerTotal - playerCompleted;
        }

        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }

        public static void SetMovement(bool canMove)
        {
            PlayerControl player = PlayerControl.LocalPlayer;
            if (!player.inVent)
                player.moveable = canMove;

            if (!canMove)
                player.NetTransform.Halt();
        }
    }
}
