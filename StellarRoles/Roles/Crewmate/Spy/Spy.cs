using UnityEngine;

namespace StellarRoles
{
    public static class Spy
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = Palette.ImpostorRed;
        public static bool ImpostorsCanKillAnyone => CustomOptionHolder.SpyImpostorsCanKillAnyone.GetBool();
        // TODO: does this game option need to be removed
        public static bool RoleBlock => CustomOptionHolder.SpyRoleBlock.GetBool();

        private static Sprite _VentButtonSprite;

        public static void GetDescription()
        {
            string description =
                $"The {nameof(Spy)} is a role designed to confuse the impostors.\n\n" +
                $"When a {nameof(Spy)} spawns in game, the Impostors will see them as part of their team. " +
                $"The {nameof(Spy)}'s goal is to confuse the impostors into turning on each other by acting like one themselves.\n\n";

            if (ImpostorsCanKillAnyone)
                description += "Impostor friendly fire is enabled!\n\n";

            description +=
                $"Be careful to not act too suspicious! The crew may end up voting for you!\n\n" +
                $"The {nameof(Spy)} cannot be shot by the {nameof(Vigilante)} or {nameof(Assassin)}.\n\n";

            if (Sheriff.SpyCanDieToSheriff)
                description += $"The {nameof(Spy)} can be shot by the {nameof(Sheriff)}.";


            RoleInfo.Spy.SettingsDescription = Helpers.WrapText(description);
        }

        public static Sprite GetVentButtonSprite()
        {
            return _VentButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.VentButtons.SpyVentButton.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
        }
    }
}
