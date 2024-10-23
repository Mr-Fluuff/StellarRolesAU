using UnityEngine;

namespace StellarRoles
{
    public static class Vampire
    {
        public static PlayerControl Player { get; set; } = null;
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;
        public static float Delay => CustomOptionHolder.VampireKillDelay.GetFloat();
        public static float BiteCooldown => CustomOptionHolder.VampireCooldown.GetFloat();
        public static bool HasKillButton => CustomOptionHolder.VampireKillButton.GetBool();
        public static bool IsNeutralKiller => CustomOptionHolder.VampireIsNeutral.GetBool();
        public static PlayerControl Bitten { get; set; } = null;
        public static PlayerControl AbilityCurrentTarget { get; set; } = null;

        private static Sprite _ButtonSprite;

        public static float CalculateBiteDelay()
        {
            float result = Delay;
            if (Ascended.IsAscended(Vampire.Player))
            {
                result *= 1.25f;
            }
            return result;
        }
        public static void GetDescription()
        {
            string settingsDescription =
                $"The {nameof(Vampire)} kills players in a delayed fashion with its bite ability. " +
                $"This ability takes {Helpers.ColorString(Color.yellow, Delay.ToString())} seconds to kill the player and has a {Helpers.ColorString(Color.yellow, BiteCooldown.ToString())} second cooldown.\n\n";

            if (HasKillButton)
                settingsDescription += $"The {nameof(Vampire)} can kill normally as well, but kill and bite share cooldowns.";
            else
                settingsDescription += $"The {nameof(Vampire)} cannot kill normally and can only kill in this delayed fashion.";

            RoleInfo.Vampire.SettingsDescription = RoleInfo.VampireNeutralKiller.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.VampireButton.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            Bitten = null;
            AbilityCurrentTarget = null;
        }
    }
}
