using UnityEngine;

namespace StellarRoles
{
    public static class Bomber
    {
        public static PlayerControl Player { get; set; } = null;
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        public static bool IsNeutralKiller => CustomOptionHolder.BomberIsNeutral.GetBool();
        public static bool SeeBombTarget => CustomOptionHolder.BomberImpsSeeBombed.GetBool();
        public static bool HotPotatoMode => CustomOptionHolder.BomberHotPotatoMode.GetBool();

        public static float Cooldown => CustomOptionHolder.BomberBombCooldown.GetFloat();
        public static float BombDelay => CustomOptionHolder.BomberDelay.GetFloat();
        public static float BombTimer => CustomOptionHolder.BomberTimer.GetFloat();
        public static bool CanReport => CustomOptionHolder.BomberCanReport.GetBool();
        public static PlayerControl AbilityCurrentTarget { get; set; } = null;

        private static Sprite _ButtonSprite;
        private static Sprite _BombSprite;

        public static void GetDescription()
        {
            string settingsDescription;

            if (HotPotatoMode) settingsDescription = GetHotPotatoDescription();
            else settingsDescription = GetDefaultDescription();
            RoleInfo.BomberNeutralKiller.SettingsDescription = RoleInfo.Bomber.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static string GetDefaultDescription()
        {
            string settingsDescription =
                $"The {nameof(Bomber)} has the ability to put a bomb on another player. " +
                $"After {Helpers.ColorString(Color.yellow, BombDelay.ToString())} seconds, the player will be notified that they have a bomb.\n\n" +
                $"Bombed players will have {Helpers.ColorString(Color.yellow, BombTimer.ToString())} seconds to kill another player. " +
                $"If the time expires or a meeting is called while they are notified of the bomb, the bombed player will die.\n\n" +
                $"Bomb and Kill trigger each other's cooldowns when used. Bomb Ability has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown.";

            if (!CanReport)
                settingsDescription += "\n\nBombed player's are unable to report the bodies of players that they kill.";
            if (!SeeBombTarget)
                settingsDescription += $"\n\nYour impostor partner is unable to see who you bomb! Be careful who you use it on!";

            return Helpers.WrapText(settingsDescription);
        }

        public static string GetHotPotatoDescription()
        {
            string settingsDescription =
                $"Hot Potato Mode is Active!\n\n" +
                $"The {nameof(Bomber)} has the ability to put a bomb on another player. " +
                $"In Hot Potato Mode, the first person to get a bomb will be notified they have one after {Helpers.ColorString(Color.yellow, BombDelay.ToString())} seconds\n\n" +
                $"That Bombed player will start with {Helpers.ColorString(Color.yellow, BombTimer.ToString())} seconds to pass to another player. " +
                $"Each person to get a bomb that is active will have the remainder of the last persons timer to pass it off. " +
                $"If the time on a bomb expires or a meeting is called while they are notified of the bomb, the bombed player will die.\n\n" +
                $"If you try to button while having a bomb you will explode and a meeting will not be called\n\n" +
                $"Bomb and Kill trigger each other's cooldowns when used. Bomb Ability has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown.\n\n" +
                $"Remember... NO PASS BACKSIES. This means you cannot pass the bomb back to the person that just gave you the bomb. " +
                $"You also cannot pass the bomb to someone that also has a different bomb";

            if (!SeeBombTarget)
                settingsDescription += $"\n\nYour impostor partner is unable to see who you bomb! Be careful who you use it on!";

            return Helpers.WrapText(settingsDescription);
        }

        public static float CalculateBombTimer()
        {
            float result = BombTimer;
            if (Ascended.IsAscended(Player))
            {
                result *= .7f;
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
