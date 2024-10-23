using UnityEngine;

namespace StellarRoles
{
    public static class Warlock
    {
        public static PlayerControl Player { get; set; } = null;
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;
        public static PlayerControl CurrentCurseTarget { get; set; } = null;
        public static PlayerControl CurseVictim { get; set; } = null;
        public static PlayerControl CurseVictimTarget { get; set; } = null;
        public static float Cooldown => CustomOptionHolder.WarlockCooldown.GetFloat();
        public static float RootTime => CustomOptionHolder.WarlockRootTime.GetFloat();
        public static bool IsNeutralKiller => CustomOptionHolder.WarlockIsNeutral.GetBool();

        private static Sprite _CurseButtonSprite;
        private static Sprite _CurseKillButtonSprite;


        public static void GetDescription()
        {
            RoleInfo.Warlock.SettingsDescription = RoleInfo.WarlockNeutralKiller.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Warlock)} is able to use its Curse ability on players.\n\n" +
                $"The {nameof(Warlock)} can activate this curse using their Curse-Kill ability when a player is near the cursed target.\n\n" +
                $"Curse has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown. " +
                $"Curse-Kill will kill the nearest player to the cursed target and will freeze the {nameof(Warlock)} in place for {Helpers.ColorString(Color.yellow, RootTime.ToString())} seconds.\n\n" +
                $"Be careful! Curse-Kill can target anyone! This includes your partner!");
        }

        public static Sprite GetCurseButtonSprite()
        {
            return _CurseButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.CurseButton.png", 115f);
        }

        public static Sprite GetCurseKillButtonSprite()
        {
            return _CurseKillButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.CurseKillButton.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            CurrentCurseTarget = null;
            CurseVictim = null;
            CurseVictimTarget = null;
        }

        public static void ResetCurse()
        {
            WarlockButtons.CurseButton.Timer = WarlockButtons.CurseButton.MaxTimer;
            WarlockButtons.CurseButton.Sprite = GetCurseButtonSprite();
            WarlockButtons.CurseButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            CurrentCurseTarget = null;
            CurseVictim = null;
            CurseVictimTarget = null;
        }
    }
}
