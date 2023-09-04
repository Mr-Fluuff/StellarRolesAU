using System.Collections.Generic;
using UnityEngine;


namespace StellarRoles
{
    public class Nightmare
    {
        public static readonly Color Color = new Color32(90, 20, 20, byte.MaxValue);

        public static readonly Dictionary<byte, Nightmare> PlayerToNightmare = new();
        public static float ParalyzeCooldown => CustomOptionHolder.NightmareParalyzeCooldown.GetFloat();
        public static float ParalyzeRootTime => CustomOptionHolder.NightmareParalyzeDuration.GetFloat();
        public static float BlindRadius => CustomOptionHolder.NightmareBlindRadius.GetFloat() * 6;
        public static float BlindTimer { get; set; } = 0f;
        public static float BlindDuration => CustomOptionHolder.NightmareBlindDuration.GetFloat();
        public static float BlindCooldown => CustomOptionHolder.NightmareBlindCooldown.GetFloat();

        private static Sprite _NightmareBlindSprite;
        private static Sprite _NightmareParalyzeSprite;

        public PlayerControl Player;
        public PlayerControl AbilityCurrentTarget;
        public float LightsOutTimer;
        public readonly PlayerList BlindedPlayers = new();
        public PlayerControl ParalyzedPlayer;

        public static void GetDescription()
        {
            RoleInfo.Nightmare.SettingsDescription = Helpers.WrapText(
               $"The {nameof(Nightmare)} has a Paralyze ability that freezes a target in place, rendering them unable use any of their buttons and pausing their ability cooldown progress.\n" +
               $"This ability lasts {Helpers.ColorString(Color.yellow, ParalyzeRootTime.ToString())} seconds and has a {Helpers.ColorString(Color.yellow, ParalyzeCooldown.ToString())} second cooldown.\n" +
               $"This ability is usable from vents.\n\n" +
               $"The {nameof(Nightmare)} has a blind ability that reduces the vision of all nearby players to that of a crewmate's vision during lights out.\n" +
               $"This ability works on players with {nameof(Jester)} vision and {Helpers.ColorString(Palette.ImpostorRed, "Impostor")} vision.\n" +
               $"The radius of this effect is {Helpers.ColorString(Color.yellow, CustomOptionHolder.NightmareBlindRadius.GetFloat().ToString())} and lasts for {Helpers.ColorString(Color.yellow, BlindDuration.ToString())} seconds.\n" +
               $"This ability is usable from vents.");
        }

        public Nightmare(PlayerControl player)
        {
            Player = player;
            PlayerToNightmare.Add(player.PlayerId, this);
        }

        public static Sprite GetNightmareBlindSprite()
        {
            return _NightmareBlindSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Nightmare_Blind.png", 115f);
        }

        public static Sprite GetNightmareParalyzeSprite()
        {
            return _NightmareParalyzeSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Nightmare_Paralyze.png", 115f);
        }

        public static bool IsNightmare(byte playerId, out Nightmare nightmare)
        {
            return PlayerToNightmare.TryGetValue(playerId, out nightmare);
        }

        public static bool isNightmare(PlayerControl playerConrol)
        {
            return Nightmare.PlayerToNightmare.ContainsKey(playerConrol.PlayerId);
        }
        public static void ClearAndReload()
        {
            PlayerToNightmare.Clear();
        }
    }

    public static class NightmareExtensions
    {
        public static bool IsNightmare(this PlayerControl player, out Nightmare nightmare) => Nightmare.IsNightmare(player.PlayerId, out nightmare);
    }
}
