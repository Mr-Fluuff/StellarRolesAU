using UnityEngine;

namespace StellarRoles
{
    public static class Janitor
    {
        public static PlayerControl Player { get; set; }
        public static PlayerControl CurrentTarget { get; set; }
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;
        public static float Cooldown => CustomOptionHolder.JanitorCooldown.GetFloat();
        public static int ChargesPerKill => CustomOptionHolder.JanitorChargesPerKill.GetInt();
        public static bool IsNeutralKiller => CustomOptionHolder.JanitorIsNeutral.GetBool();
        public static int ChargesRemaining { get; set; }

        private static Sprite _ButtonSprite;

        public static void GetDescription()
        {
            RoleInfo.JanitorNeutralKiller.SettingsDescription = RoleInfo.Janitor.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Janitor)} is a role who can clean bodies, removing them from the map. " +
                $"Clean has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown, and this cooldown is shared with kill.\n\n" +
                $"A cleaned body will show as a disconnect rather than dead on vitals." +
                $"\n\nThe Janitor begins the game with {Helpers.ColorString(Color.yellow, CustomOptionHolder.JanitorInitialCharges.GetInt().ToString())} charge(s) of their clean ability, and earns {Helpers.ColorString(Color.yellow, ChargesPerKill.ToString())} more charge(s) for every player it kills.");
        }

        public static float CalculateJanitorCooldown()
        {
            float result = Cooldown;
            if (Ascended.IsAscended(Janitor.Player))
            {
                result *= .75f;
            }
            return result;
        }

        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.CleanButton.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            ChargesRemaining = CustomOptionHolder.JanitorInitialCharges.GetInt();
        }
    }
}
