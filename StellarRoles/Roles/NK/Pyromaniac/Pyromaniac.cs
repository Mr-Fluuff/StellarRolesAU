using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public class Pyromaniac
    {
        public static readonly Dictionary<byte, Pyromaniac> PyromaniacDictionary = new();
        public static readonly Color Color = new Color32(255, 164, 0, byte.MaxValue);

        public static float DouseCooldown => CustomOptionHolder.PyromaniacDouseCooldown.GetFloat();
        public static float DouseKillCooldown => CustomOptionHolder.PyromaniacDouseKillCooldown.GetFloat();
        public static float DouseDuration => CustomOptionHolder.PyromaniacDousedDuration.GetFloat();

        private static Sprite _PyroManiacDouseSprite;
        private static Sprite _IgniteSprite;
        private static Sprite _PyroOverlaySprite;
        private static Sprite _PyroTooltipSprite;

        public readonly PlayerList DousedPlayers = new();
        public readonly PlayerControl Player;
        public PlayerControl CurrentTarget;
        public PlayerControl AbilityCurrentTarget;
        public PlayerControl DouseTarget;

        public static void GetDescription()
        {
            string settingsDescription =
                $"The {nameof(Pyromaniac)} is able to douse and kill players. " +
                $"Killing a doused player will put kill on a {Helpers.ColorString(Color.yellow, DouseKillCooldown.ToString())} second cooldown.\n\n" +
                $"Douse has a {Helpers.ColorString(Color.yellow, DouseCooldown.ToString())} second cooldown. " +
                $"The {nameof(Pyromaniac)} must stand near a player for {Helpers.ColorString(Color.yellow, DouseDuration.ToString())} seconds to douse them. " +
                $"Killing or dousing will trigger half of the other button's cooldown.\n\n" +
                $"Players who have been doused by the {nameof(Pyromaniac)} will have an orange symbol by their name in meetings for the rest of the game.";

            if (IsArsonistEnabled() || !Helpers.GameStarted)
                settingsDescription += $"\n\nThey may also win as a final 2 team with the {nameof(Arsonist)}, but they are not given in-game information as to who they are, and can kill them.";


            RoleInfo.Pyromaniac.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public Pyromaniac(PlayerControl player)
        {
            Player = player;
            PyromaniacDictionary.Add(player.PlayerId, this);
        }

        public static Sprite GetPyroIgniteSprite()
        {
            return _IgniteSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.pyroIgnite.png", 115f);
        }

        public static Sprite GetPyroOverlaySprite()
        {
            return _PyroOverlaySprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.PyromaniacOverlay.png", 250f);
        }

        public static Sprite GetPyroTooltipSprite()
        {
            return _PyroTooltipSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.PyromaniacTooltip.png", 350f);
        }

        public static Sprite GetPyromaniacDouse()
        {
            return _PyroManiacDouseSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.PyroDouse.png", 115f);
        }


        public static bool IsPyromaniac(byte playerId, out Pyromaniac pyromaniac)
        {
            return PyromaniacDictionary.TryGetValue(playerId, out pyromaniac);
        }

        public static bool IsArsonistEnabled()
        {
            return CustomOptionHolder.ArsonistSpawnRate.GetSelection() > 0 && CustomOptionHolder.NeutralRolesCountMin.GetFloat() > 0;
        }

        public static void ClearAndReload()
        {
            PyromaniacDictionary.Clear();
        }
    }

    public static class PyromaniacExtensions
    {
        public static bool IsPyromaniac(this PlayerControl player, out Pyromaniac pyromaniac) => Pyromaniac.IsPyromaniac(player.PlayerId, out pyromaniac);
    }
}

