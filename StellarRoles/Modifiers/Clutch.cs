using UnityEngine;

namespace StellarRoles
{
    public static class Clutch
    {
        public static readonly PlayerList Players = new();
        public static float Bonus => 1 - ((CustomOptionHolder.ModifierClutchImpact.GetSelection() / 10f) + .1f);

        public static void GetDescription()
        {
            RoleInfo.Clutch.SettingsDescription = Helpers.WrapText(
                $"A {nameof(Clutch)} Impostor's abilities have {Helpers.ColorString(Color.yellow, ((int)((CustomOptionHolder.ModifierClutchImpact.GetSelection() / 10f + .1f) * 100)).ToString("00"))}% reduced cooldowns after their partner is killed or voted out.\n\n" +
                $"Players with abilities that can kill another player cannot spawn as a {nameof(Clutch)} Impostor.");
        }

        public static void ClearAndReload()
        {
            Players.Clear();
        }
    }
}
