using Cpp2IL.Core.Extensions;
using StellarRoles.Objects;
using StellarRoles.Patches;
using StellarRoles.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class HeadHunter
    {
        public static readonly Color Color = new Color32(175, 70, 40, byte.MaxValue);
        public static readonly Color TargetColor = new Color32(45, 45, 45, byte.MaxValue);

        public const float HeadHunterBenefit = 5f;
        public const float HeadHunterPenalty = 10f;

        public static PlayerControl Player { get; set; }
        public static readonly PlayerList Bounties = new();
        public static PlayerControl CurrentTarget { get; set; }
        public static float HeadHunterCurrentCooldown { get; set; }
        public static float PursueCurrentTimer { get; set; }
        public static float PursueDuration => CustomOptionHolder.HeadHunterTrackerDuration.GetFloat();
        public static float Cooldown => CustomOptionHolder.HeadHunterTrackingCooldown.GetFloat();
        public static bool ShuffleButton { get; set; } = true;
        public static readonly Dictionary<byte, Arrow> PlayerLocalArrows = new();

        private static Sprite _HeadHunterPursueSprite;
        private static Sprite _HeadHunterShuffleSprite;

        public static void GetDescription()
        {
            string settingsDescription =
               $"The {nameof(HeadHunter)} is given 3 targets to kill. If any of these targets die, replacement targets will be assigned on the next round.\n\n" +
               $"Killing any of these targets will permanently decrease the {nameof(HeadHunter)}'s kill cooldown by 5 seconds. " +
               $"Killing any other player will permanently increase it by 10 seconds.\n\n" +
               $"The {nameof(HeadHunter)} can track its current targets with arrows using its pursue ability. " +
               $"This ability has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown and lasts for {Helpers.ColorString(Color.yellow, PursueDuration.ToString())} seconds.\n\n";

            if (ShuffleButton)
                settingsDescription +=
                    $"As long as the {nameof(HeadHunter)} has not killed, the {nameof(HeadHunter)} may re-shuffle its targets. " +
                    $"Doing so will clear all targets for 10 seconds and then apply 3 random new ones.";

            RoleInfo.HeadHunter.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static float CalculatePursueDuration()
        {
            float result = PursueDuration;
            if (Ascended.IsAscended(Player))
            {
                result *= 1.5f;
            }
            return result;
        }

        public static Sprite GetHeadHunterPursueSprite()
        {
            return _HeadHunterPursueSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.headhunterpursue.png", 115f);
        }

        public static Sprite GetHeadHunterShuffleSprite()
        {
            return _HeadHunterShuffleSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HeadHunterShuffle.png", 115f);
        }

        static List<PlayerControl> HeadHunterPossibleTargets()
        {
            List<PlayerControl> possibleTargets = new();
            List<PlayerControl> impostors = new();
            int playersRemaining = 0;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (
                    player == Player ||
                    Bounties.Contains(player) ||
                    player.Data.IsDead ||
                    player.Data.Disconnected ||
                    player == Player.GetRomancePartner() ||
                    (player.Data.Role.IsImpostor && MapOptions.PlayersAlive >= 8) ||
                    player == MapOptions.FirstKillPlayer
                ) continue;

                if (player.Data.Role.IsImpostor)
                    impostors.Add(player);
                else
                {
                    possibleTargets.Add(player);
                    playersRemaining++;
                }
            }

            if (impostors.Count > 1)
            {
                System.Random random = new();
                int index = random.Next(impostors.Count);
                possibleTargets.Add(impostors[index]);
            }
            else if (impostors.Count == 1 && playersRemaining < 4)
                possibleTargets.Add(impostors[0]);

            return possibleTargets;
        }

        public static void HeadHunterUpdate()
        {
            if (Player == null || !Player.AmOwner) return;

            //remove dead players
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                if ((player.Data.IsDead || player.Data.Disconnected) && Bounties.Remove(player))
                {
                    Arrow arrow = PlayerLocalArrows[player.PlayerId];
                    Object.Destroy(arrow.Object);
                    PlayerLocalArrows.Remove(player.PlayerId);
                }

            if (Bounties.Count < 3)
            {
                System.Random random = new();
                List<PlayerControl> possibleTargets = HeadHunterPossibleTargets();

                while (possibleTargets.Count != 0 && Bounties.Count < 3)
                {
                    PlayerControl targetPlayer = possibleTargets.RemoveAndReturn(random.Next(0, possibleTargets.Count));
                    Bounties.Add(targetPlayer);
                    Arrow arrow = new(Color);
                    arrow.Object.SetActive(false);
                    PlayerLocalArrows.Add(targetPlayer.PlayerId, arrow);
                }
            }

            if (Bounties.Count == 0) return;

            HudManager hudManager = HudManager.Instance;
            if (hudManager != null && hudManager.UseButton != null)
            {
                foreach (PoolablePlayer pp in MapOptions.PlayerIcons.Values) pp.gameObject.SetActive(false);

                Vector3 bottomLeft = IntroCutsceneOnDestroyPatch.BottomLeft;
                for (int i = 0; i < Bounties.Count; i++)
                {
                    if (MapOptions.PlayerIcons.TryGetValue(Bounties[i], out PoolablePlayer poolablePlayer) && poolablePlayer.gameObject != null)
                        poolablePlayer.gameObject.SetActive(true);

                    poolablePlayer.transform.localPosition = bottomLeft
                        + new Vector3(-0.25f, -0.25f, 0)
                        + Vector3.right * i * 0.35f;
                    poolablePlayer.transform.localScale = Vector3.one * 0.2f;
                }
            }
        }

        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            Bounties.Clear();
            HeadHunterCurrentCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;

            PursueCurrentTimer = 0f;
            foreach (PoolablePlayer p in MapOptions.PlayerIcons.Values)
#pragma warning disable IDE0031 // Use null propagation
                // suppressed due to Unity being weird and not liking optional chaining here
                if (p.gameObject != null) p.gameObject.SetActive(false);
#pragma warning restore IDE0031 // Use null propagation
            foreach (Arrow arrow in PlayerLocalArrows.Values)
                Object.Destroy(arrow.Object);
            PlayerLocalArrows.Clear();
            ShuffleButton = true;
        }
    }
}
