using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace StellarRoles
{
    public class RuthlessRomantic
    {
        public static readonly Color Color = new Color32(210, 30, 70, byte.MaxValue);

        public static readonly Dictionary<byte, RuthlessRomantic> PlayerToRuthlessRomantic = new();
        public static float VestDuration => CustomOptionHolder.RomanticProtectDuration.GetFloat();
        public static float VestCooldown => CustomOptionHolder.RomanticProtectCooldown.GetFloat();
        private static Sprite _RuthlessRomanticButton;

        public readonly PlayerControl Player;
        public bool IsVestActive = false;
        public PlayerControl DeadLover = null;

        public static void GetDescription()
        {
            RoleInfo.RuthlessRomantic.SettingsDescription = Helpers.WrapText(
                $"The Ruthless {nameof(Romantic)} was once a {nameof(Romantic)} to a Neutral Killer. " +
                $"Now they are a Neutral Killer. Its goal is to finish its fallen love's objective and kill all players!\n\n" +
                $"The Ruthless {nameof(Romantic)} has a Shield ability that protects it from attacks for {Helpers.ColorString(Color.yellow, VestDuration.ToString())} seconds on a {Helpers.ColorString(Color.yellow, VestCooldown.ToString())} second cooldown.");
        }

        public RuthlessRomantic(PlayerControl player)
        {
            Player = player;
            PlayerToRuthlessRomantic.Add(player.PlayerId, this);
        }

        public static Sprite GetRuthlessRomanticSprite()
        {
            return _RuthlessRomanticButton ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Romantic.RuthlessRomanticButton.png", 115f);
        }

        public static bool IsRuthlessRomantic(byte playerId, out RuthlessRomantic ruthlessRomantic)
        {
            return PlayerToRuthlessRomantic.TryGetValue(playerId, out ruthlessRomantic);
        }

        public static bool IsRuthlessRomanticAndVestActive(byte playerId)
        {
            return IsRuthlessRomantic(playerId, out RuthlessRomantic ruthlessRomantic) && ruthlessRomantic.IsVestActive;
        }

        public static bool IsLover(PlayerControl p)
        {
            return PlayerToRuthlessRomantic.Values.Any(x => x.DeadLover == p);
        }

        public static void ClearAndReload()
        {
            PlayerToRuthlessRomantic.Clear();
        }
    }

    public static class RuthlessRomanticExtensions
    {
        public static bool IsRuthlessRomantic(this PlayerControl player, out RuthlessRomantic ruthlessRomantic) => RuthlessRomantic.IsRuthlessRomantic(player.PlayerId, out ruthlessRomantic);
        public static bool IsRuthlessRomanticAndVestActive(this PlayerControl player) => RuthlessRomantic.IsRuthlessRomanticAndVestActive(player.PlayerId);
    }
}
