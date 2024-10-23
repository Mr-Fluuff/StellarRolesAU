using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch]
    public static class Shade
    {
        public static PlayerControl Player { get; set; } = null;

        public static readonly PlayerList BlindedPlayers = new();
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        public static float ShadeCooldown => CustomOptionHolder.ShadeCooldown.GetFloat();
        public static float ShadeDuration => CustomOptionHolder.ShadeDuration.GetFloat();
        public static float EvidenceDuration => CustomOptionHolder.ShadeEvidence.GetFloat();
        public static int KillsToGainBlind => CustomOptionHolder.ShadeKillsToGainBlind.GetInt();
        public static int Killed { get; set; } = 0;
        public static float BlindCooldown => CustomOptionHolder.ShadeBlindCooldown.GetFloat();
        public static float BlindDuration => CustomOptionHolder.ShadeBlindDuration.GetFloat();
        public static bool IsNeutralKiller => CustomOptionHolder.ShadeIsNeutral.GetBool();
        public static float LightsOutTimer { get; set; } = 0f;
        public static float InvisibleTimer { get; set; } = 0f;
        public static bool IsInvisble { get; set; } = false;
        public static int BlindRadius => CustomOptionHolder.ShadeBlindRange.GetSelection();
        private static Sprite _ShadeButtonSprite;
        private static Sprite _BlindSprite;
        private static Sprite _ShadeEvidenceSprite;

        public static void GetDescription()
        {
            RoleInfo.ShadeNeutralKiller.SettingsDescription = RoleInfo.Shade.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Shade)} can turn invisible using its Vanish ability. " +
                $"Vanish has a {Helpers.ColorString(Color.yellow, ShadeDuration.ToString())} second duration, but may be cancelled prematurely by pressing the button again. " +
                $"This ability has a {Helpers.ColorString(Color.yellow, ShadeCooldown.ToString())} second cooldown.\n\n" +
                $"The {nameof(Shade)} leaves evidence in the form of a dark, angry puddle both when it vanishes and when it reveals itself. " +
                $"The puddle lasts for {Helpers.ColorString(Color.yellow, EvidenceDuration.ToString())} seconds.\n\n" +
                $"After killing {Helpers.ColorString(Color.yellow, KillsToGainBlind.ToString())} players, the {nameof(Shade)} gets a new blind ability. " +
                $"Blind reduces the vision of all crewmate-vision players as if lights were called, without using a sabotage. " +
                $"This ability lasts {Helpers.ColorString(Color.yellow, BlindDuration.ToString())} seconds and has a {Helpers.ColorString(Color.yellow, BlindCooldown.ToString())} second cooldown.");
        }

        public static float CalculateShadeDuration()
        {
            float result = ShadeDuration;
            if (Ascended.IsAscended(Player))
            {
                result = result * 1.25f;
            }
            return result;
        }

        public static float CalculateShadeEvidence()
        {
            float result = EvidenceDuration;
            if (Ascended.IsAscended(Player))
            {
                result = result * .75f;
            }
            return result;
        }
        public static Sprite GetShadeButtonSprite()
        {
            return _ShadeButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ShadeVanish.png", 115f);
        }

        public static Sprite GetBlindButtonSprite()
        {
            return _BlindSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ShadeBlind.png", 115f);
        }

        public static Sprite GetShadeEvidenceSprite()
        {
            return _ShadeEvidenceSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ShadeEvidence.png", 158f);
        }

        public static float GetShadeRadius()
        {
            return ((BlindRadius * .25f) + .75f) * 4;
        }


        public static void ClearAndReload()
        {
            Player = null;
            BlindedPlayers.Clear();
            Killed = 0;
            LightsOutTimer = 0f;
            InvisibleTimer = 0f;
            IsInvisble = false;
        }
    }
}
