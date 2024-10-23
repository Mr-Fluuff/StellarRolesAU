using UnityEngine;

namespace StellarRoles
{
    public static class Spectator
    {
        public static readonly PlayerList Players = new();
        public static readonly PlayerList ToBecomeSpectator = new();
        public static readonly Color Color = Color.gray;
        public static PlayerControl Target { get; set; } = null;

        private static Sprite _SpectatorOverlaySprite;

        public static void ClearAndReload()
        {
            Players.Clear();
        }

        public static bool IsSpectator(byte playerId)
        {
            return Players.Contains(playerId);
        }

        public static Sprite GetSpectatorOverlay()
        {
            return _SpectatorOverlaySprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.SpectatorOverlay.png", 120f);
        }
    }
}