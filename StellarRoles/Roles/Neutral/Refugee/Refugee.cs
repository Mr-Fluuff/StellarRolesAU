using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public class Refugee
    {
        public static readonly Color Color = new Color32(180, 120, 60, byte.MaxValue);
        public static readonly Color RefugeeShield = new Color32(250, 170, 50, byte.MaxValue);

        public static readonly Dictionary<byte, Refugee> PlayerToRefugee = new();
        public static float VestDuration => CustomOptionHolder.VestDuration.GetFloat();
        public static float VestCooldown => CustomOptionHolder.VestCooldown.GetFloat();
        public static bool CanBeGuessed = true;

        public readonly PlayerControl Player;
        public bool NotAckedExiled = false;
        public bool WasExecutioner = false;
        public bool IsVestActive = false;
        public PlayerControl DeadLover = null;

        private static Sprite _RefugeeSprite;

        public static void GetDescription()
        {
            string settingsDescription =
                $"The {nameof(Refugee)} doesn't spawn normally but is the result of another neutral role losing their win condition in a manner that is out of their hands.\n\n" +
                $"The goal of the {nameof(Refugee)} is to stay alive until the end of the game, but it may not win with evil killing roles or neutral roles.\n\n" +
                $"The {nameof(Refugee)} has a Refuge ability it can use to protect itself from attacks. " +
                $"This ability lasts for {Helpers.ColorString(Color.yellow, VestDuration.ToString())} seconds and has a {Helpers.ColorString(Color.yellow, VestCooldown.ToString())} second cooldown.";

            if (!CanBeGuessed)
                settingsDescription += $"\n\nThe {nameof(Refugee)} cannot be killed by an {nameof(Assassin)} or {nameof(Vigilante)} in meetings.";

            RoleInfo.Refugee.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public Refugee(PlayerControl player)
        {
            Player = player;
            PlayerToRefugee.Add(player.PlayerId, this);
        }

        public static Sprite GetRefugeeSprite()
        {
            return _RefugeeSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.RefugeeButton.png", 115f);
        }

        public static bool IsRefugee(byte playerId, out Refugee refugee)
        {
            return PlayerToRefugee.TryGetValue(playerId, out refugee);
        }

        public static bool IsRefugeeAndVestActive(byte playerId)
        {
            return IsRefugee(playerId, out Refugee refugee) && refugee.IsVestActive;
        }

        public static void ClearAndReload()
        {
            PlayerToRefugee.Clear();
        }
    }

    public static class RefugeeExtensions
    {
        public static bool IsRefugee(this PlayerControl player, out Refugee refugee) => Refugee.IsRefugee(player.PlayerId, out refugee);
        public static bool IsRefugeeAndVestActive(this PlayerControl player) => Refugee.IsRefugeeAndVestActive(player.PlayerId);
    }
}
