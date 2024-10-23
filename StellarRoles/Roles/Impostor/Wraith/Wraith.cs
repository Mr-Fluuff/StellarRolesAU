using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    public static class Wraith
    {
        public const float SpeedMultiplier = 1.75f;

        public static PlayerControl Player { get; set; } = null;
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        public static bool HasLantern => CustomOptionHolder.WraithLantern.GetBool();
        public static float LanternCooldown => CustomOptionHolder.WraithLanternCooldown.GetFloat();
        public static float LanternDuration => CustomOptionHolder.WraithLanternDuration.GetFloat();
        public static float InvisibleDuration => CustomOptionHolder.WraithInvisibleDuration.GetFloat();

        public static float PhaseCooldown => CustomOptionHolder.WraithPhaseCooldown.GetFloat();
        public static float PhaseDuration => CustomOptionHolder.WraithPhaseDuration.GetFloat();
        public static bool IsNeutralKiller => CustomOptionHolder.WraithIsNeutral.GetBool();


        public static bool PhaseOn { get; set; } = false;
        public static float LanternTimer { get; set; } = 0;
        public static float InvisibleTimer { get; set; } = 0f;
        public static bool IsInvisible { get; set; } = false;

        private static Sprite _LanternButtonSprite;
        private static Sprite _PhaseButtonSprite;
        private static Sprite _ReturnButtonSprite;

        public static void GetDescription()
        {
            string settingsDescription =
                $"The {nameof(Wraith)} is a role focused on fast movement across the map.\n\n" +
                $"It has a Dash ability that lasts {Helpers.ColorString(Color.yellow, PhaseDuration.ToString())} seconds and has a {Helpers.ColorString(Color.yellow, PhaseCooldown.ToString())} second cooldown. " +
                $"It increases the movement speed of the {nameof(Wraith)} by an additional 75%.";

            if (HasLantern)
                settingsDescription +=
                    $"\n\nThe {nameof(Wraith)} can place a lantern on the ground that it can use to teleport back to. " +
                    $"No other players can see this lantern. To teleport back to the lantern, press the button again. " +
                    $"The lantern will break after {Helpers.ColorString(Color.yellow, LanternDuration.ToString())} seconds of being placed if left unused. " +
                    $"A broken lantern is visible to all players.";

            if (InvisibleDuration > 0)
                settingsDescription += $"\n\nTeleporting to the lantern will grant the Wraith {Helpers.ColorString(Color.yellow, InvisibleDuration.ToString())} seconds of invisibility.";

            RoleInfo.Wraith.SettingsDescription = RoleInfo.WraithNeutralKiller.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static float CalculateWraithReturn()
        {
            float result = LanternDuration;
            if (Ascended.IsAscended(Player))
            {
                result = result * 1.5f;
            }
            return result;
        }

        public static float CalculateDashDuration()
        {
            float result = PhaseDuration;
            if (Ascended.IsAscended(Player)) { result = result * 1.5f; }
            return result;
        }
        public static void ClearAndReload()
        {
            Player = null;
            IsInvisible = false;
            PhaseOn = false;
            LanternTimer = 0;
            InvisibleTimer = 0f;
            Lantern.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
        }

        public static Sprite GetLanternButtonSprite()
        {
            return _LanternButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.LanternButton.png", 115f);
        }

        public static Sprite GetPhaseButtonSprite()
        {
            return _PhaseButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Phase.png", 115f);
        }

        public static Sprite GetReturnButtonSprite()
        {
            return _ReturnButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Return.png", 115f);
        }
    }
}
