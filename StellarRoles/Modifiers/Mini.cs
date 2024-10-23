using UnityEngine;

namespace StellarRoles
{
    public static class Mini
    {
        public static PlayerControl Player { get; set; }
        public static float SpeedMultiplier => (CustomOptionHolder.ModifierMiniSpeed.GetSelection() * .1f) + 0.7f;

        public static void GetDescription()
        {
            RoleInfo.Mini.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Mini)} modifier can be applied to any player and reduces their size.\n\n" +
                $"The {nameof(Mini)} moves {(SpeedMultiplier != 1 ? $" {Helpers.ColorString(Color.yellow, ((int)(SpeedMultiplier / 1 * 100)).ToString("00"))}% the speed" : " the same speed")} of a normal player.") + "\n\n";
        }
        public static void ClearAndReload()
        {
            Player = null;
        }
    }
}
