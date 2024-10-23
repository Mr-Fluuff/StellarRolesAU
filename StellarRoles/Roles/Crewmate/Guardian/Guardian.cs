using UnityEngine;

namespace StellarRoles
{
    public static class Guardian
    {
        public static PlayerControl Player { get; set; } = null;
        public static PlayerControl Shielded { get; set; } = null;

        public static readonly Color Color = new Color32(119, 221, 119, byte.MaxValue);
        public static bool UsedShield { get; set; }

        public static bool ShieldFadesOnDeath => CustomOptionHolder.GuardianShieldFadesOnDeath.GetBool();
        public static float ShieldVisibilityDelay => CustomOptionHolder.GuardianShieldVisibilityDelay.GetFloat();
        public static float ShieldVisibilityTimer { get; set; } = 0f;
        public static float ShieldVisionRadius => CustomOptionHolder.GuardianVisionRangeOfShield.GetFloat() * 4.25f;
        public static bool LimitedVisionShield => CustomOptionHolder.GuardianShieldVisibilityDelay.GetBool();
        public static int SelfShieldCharges = 0;

        public static bool RoleBlock => CustomOptionHolder.GuardianRoleBlock.GetBool();
        public static readonly Color ShieldedColor = new Color32(188, 245, 28, byte.MaxValue);

        public static PlayerControl CurrentTarget { get; set; } = null;

        private static Sprite _ButtonSprite;

        public static void GetDescription()
        {
            string description =
                $"The {nameof(Guardian)}'s goal is to choose a player to protect. " +
                $"This protection resets every round and is visible after a delay of {Helpers.ColorString(Color.yellow, ShieldVisibilityDelay.ToString())} seconds. " +
                $"The Shield is visible from a distance of {Helpers.ColorString(Color.yellow, CustomOptionHolder.GuardianVisionRangeOfShield.GetFloat().ToString())}x (think vision range)."
                + "\n\n"
                + (ShieldFadesOnDeath
                    ? $"If the {nameof(Guardian)} is killed, the shield will be removed."
                    : $"The shield persists even if the {nameof(Guardian)} is killed.")
                    + $"\n\n";

            if (RoleBlock)
                description += "The comms sabotage will temporarily disable the shield.\n\n";

            if (CustomOptionHolder.GuardianSelfShield.GetBool())
                description += $"The {nameof(Guardian)} may use their shield on themselves once per game";

            RoleInfo.Guardian.SettingsDescription = Helpers.WrapText($"{description}{CustomOptionHolder.GuardianShieldIsVisibleTo.GetSelection() switch
            {
                0 => $"Only the protected player can see this shield.",
                1 => $"Everyone but the protected player can see the shield. ",
                2 => $"Only players capable of killing may see the shield.",
                3 => $"All Players can see the shield.",
                4 => $"Only the {nameof(Guardian)} can see the shield.",
                _ => throw new System.ArgumentOutOfRangeException(nameof(CustomOptionHolder.GuardianShieldIsVisibleTo))
            }}");
        }

        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.GuardianShield.png", 115f);
        }
        public static bool CanSeeShield(PlayerControl player)
        {
            // TODO: make this an enum
            return CustomOptionHolder.GuardianShieldIsVisibleTo.GetSelection() switch
            {
                0 => player == Shielded, // shielded
                1 => player != Shielded, // everyone but shielded
                2 => player.IsKiller(), // killers
                3 => true, // everyone
                4 => false, // guardian only
                _ => false, // unknown
            };
        }

        public static void ClearAndReload()
        {
            Player = null;
            Shielded = null;
            CurrentTarget = null;
            UsedShield = false;
            ShieldVisibilityTimer = ShieldVisibilityDelay;
            SelfShieldCharges = CustomOptionHolder.GuardianSelfShield.GetBool() ? 1 : 0;
        }
    }
}
