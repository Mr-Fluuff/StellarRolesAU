using UnityEngine;

namespace StellarRoles
{
    public static class Giant
    {
        public const float DefaultColliderRadius = 0.2233912f;
        public const float DefaultColliderOffset = 0.3636057f;

        public static PlayerControl Player { get; set; }
        public static float SpeedMultiplier => (CustomOptionHolder.ModifierGiantSpeed.GetSelection() * .1f) + 0.7f;

        public static void GetDescription()
        {
            RoleInfo.Giant.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Giant)} modifier can be applied to any player and increases their size.\n\n" +
                $"The {nameof(Giant)} player moves {(SpeedMultiplier != 1f ? $"{Helpers.ColorString(Color.yellow, (SpeedMultiplier / 1 * 100).ToString("00"))}% the speed " : "the same speed ")}of a normal player.") + "\n\n";
        }
        public static void ClearAndReload()
        {
            Player = null;
        }
    }
}
