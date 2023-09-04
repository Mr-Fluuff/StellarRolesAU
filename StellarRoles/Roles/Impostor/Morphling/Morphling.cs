using UnityEngine;

namespace StellarRoles
{
    public static class Morphling
    {
        public static PlayerControl Player { get; set; }
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;
        public static float Cooldown => CustomOptionHolder.MorphlingCooldown.GetFloat();
        public static float Duration => CustomOptionHolder.MorphlingDuration.GetFloat();
        public static PlayerControl SampledTarget { get; set; }
        public static PlayerControl MorphTarget { get; set; }
        public static PlayerControl AbilityCurrentTarget { get; set; }
        public static float MorphTimer { get; set; } = 0f;

        private static Sprite _SampleSprite;
        private static Sprite _MorphSprite;

        public static bool IsNeutralKiller => CustomOptionHolder.MorphlingIsNeutral.GetBool();

        public static void GetDescription()
        {
            RoleInfo.Morphling.SettingsDescription = RoleInfo.MorphlingNeutralKiller.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Morphling)} can sample another player to take their appearance for {Helpers.ColorString(Color.yellow, Duration.ToString())} seconds. " +
                $"The Morph ability has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown. " +
                $"Sample's cooldown at the start of the round is half of Morph's cooldown.");
        }
        public static void ResetMorph()
        {
            MorphTarget = null;
            MorphTimer = 0f;
            SampledTarget = null;
            if (Player == null) return;
            Player.SetDefaultLook();
        }

        public static void ClearAndReload()
        {
            ResetMorph();
            Player = null;
            SampledTarget = null;
            MorphTarget = null;
            AbilityCurrentTarget = null;
            MorphTimer = 0f;
        }

        public static Sprite GetSampleSprite()
        {
            return _SampleSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.SampleButton.png", 115f);
        }

        public static Sprite GetMorphSprite()
        {
            return _MorphSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.MorphButton.png", 115f);
        }
    }
}
