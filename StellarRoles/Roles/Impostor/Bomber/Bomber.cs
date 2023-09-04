using UnityEngine;

namespace StellarRoles
{
    public static class Bomber
    {
        public static PlayerControl Player { get; set; }
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        public static bool IsNeutralKiller => CustomOptionHolder.BomberIsNeutral.GetBool();
        public static bool SeeBombTarget => CustomOptionHolder.BomberImpsSeeBombed.GetBool();
        public static bool HotPotatoMode => CustomOptionHolder.BomberHotPotatoMode.GetBool();

        public static float Cooldown => CustomOptionHolder.BomberBombCooldown.GetFloat();
        public static float BombDelay => CustomOptionHolder.BomberDelay.GetFloat();
        public static float BombTimer => CustomOptionHolder.BomberTimer.GetFloat();
        public static bool CanReport => CustomOptionHolder.BomberCanReport.GetBool();
        public static PlayerControl AbilityCurrentTarget { get; set; }

        private static Sprite _ButtonSprite;
        private static Sprite _BombSprite;
        public static void GetDescription()
        {
            string settingsDescription =
                $"The {nameof(Bomber)} has the ability to put a bomb on another player. " +
                $"After {Helpers.ColorString(Color.yellow, BombDelay.ToString())} seconds, the player will be notified that they have a bomb.\n\n" +
                $"Bombed players will have {Helpers.ColorString(Color.yellow, BombTimer.ToString())} seconds to kill another player. " +
                $"If the time expires or a meeting is called while they are notified of the bomb, the bombed player will die.\n\n" +
                $"Bomb and Kill trigger each other's cooldowns when used. Bomb has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown.";

            if (!CanReport)
                settingsDescription += "\n\nBombed player's are unable to report the bodies of players that they kill.";
            if (!SeeBombTarget)
                settingsDescription += $"\n\nYour impostor partner is unable to see who you bomb! Be careful who you use it on!";

            RoleInfo.BomberNeutralKiller.SettingsDescription = RoleInfo.Bomber.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static float CalculateBombTimer()
        {
            float result = BombTimer;
            if (Ascended.IsAscended(Bomber.Player))
            {
                result = result * .7f;
            }
            return result;
        }
        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Bomber.png", 115f);
        }
        public static Sprite GetBombSprite()
        {
            return _BombSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Bomber.png", 75f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            AbilityCurrentTarget = null;
        }
    }
}
