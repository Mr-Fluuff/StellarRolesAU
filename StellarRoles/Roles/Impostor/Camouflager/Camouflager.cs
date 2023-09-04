using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    public static class Camouflager
    {
        public static PlayerControl Player { get; set; }
        public static PlayerControl CurrentTarget { get; set; }
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        public static int ChargesRemaining { get; set; } = 0;
        public static int ChargePerKill => CustomOptionHolder.CamouflagerChargesPerKill.GetInt();
        public static float Cooldown => CustomOptionHolder.CamouflagerCooldown.GetFloat();
        public static float Duration => CustomOptionHolder.CamouflagerDuration.GetFloat();
        public static float CamouflageTimer { get; set; } = 0f;
        public static bool IsNeutralKiller => CustomOptionHolder.CamouflagerIsNeutral.GetBool();

        private static Sprite _ButtonSprite;

        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.CamoButton.png", 115f);
        }

        public static void ResetCamouflage()
        {
            CamouflageTimer = 0f;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                p.SetDefaultLook();
        }

        public static void ClearAndReload()
        {
            ResetCamouflage();
            Player = null;
            CamouflageTimer = 0f;
            ChargesRemaining = 0;
        }

        public static void GetDescription()
        {
            RoleInfo.CamouflagerNeutralKiller.SettingsDescription = RoleInfo.Camouflager.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Camouflager)} is able to disguise all players for {Helpers.ColorString(Color.yellow, Duration.ToString())} seconds." +
                "\n\n" +
                $"This ability has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown." +
                "\n\n" +
                $"The Camouflager earns {Helpers.ColorString(Color.yellow, ChargePerKill.ToString())} charge(s) of their Camouflage ability for every player it kills.");
        }
    }
}
