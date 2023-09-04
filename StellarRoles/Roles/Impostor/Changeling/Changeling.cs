using UnityEngine;

namespace StellarRoles
{
    public static class Changeling
    {
        public static PlayerControl Player { get; set; }
        public static readonly Color Color = Palette.ImpostorRed;

        private static Sprite _ButtonSprite;
        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ChanglingButton.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
        }

        public static void GetDescription()
        {
            RoleInfo.Changeling.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Player)} is able to choose another impostor role to play once per game. " +
                $"It is unable to choose the role of its impostor partner or any roles exclusive to its partner's role. " +
                $"The {nameof(Shade)}, {nameof(Warlock)}, and {nameof(Vampire)} cannot spawn with one another. " +
                $"If the partner is one of these roles, the {nameof(Player)} may not choose any of them.");
        }
    }
}
