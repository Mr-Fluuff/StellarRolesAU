using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Psychic
    {
        public static readonly Color Color = new Color32(134, 55, 86, byte.MaxValue);
        public static PlayerControl Player { get; set; } = null;
        public static int AbilitesUsed { get; set; }
        public static bool isRoleBlocked => CustomOptionHolder.PsychicRoleBlock.GetBool() && Helpers.IsCommsActive();
        public static float PlayerRange => CustomOptionHolder.PsychicPlayerRange.GetFloat() * 6;
        public static bool IncludeInVent => CustomOptionHolder.PsychicDetectInVent.GetBool();
        public static bool IncludeInvisible => CustomOptionHolder.PsychicDetectInvisible.GetBool();
        public static int InRange { get; set; }


        private static Sprite _AbilitiesButtonSprite;
        private static Sprite _PlayersButtonSprite;

        public static void GetDescription()
        {
            string InvisibleCounter = IncludeInvisible ? "This counter includes invisible players. " : "This counter does not include invisible players. ";
            string VentingCounter = IncludeInVent ? "This counter includes venting players." : "This counter does not include venting players.";

            string settingDescription = $"The {nameof(Psychic)} is a role that has two passive abilities to help them understand the actions of players around them. " +
                "The Psychic's ability buttons are used like counters to display information.\n\n";

            settingDescription += $"The 'Players' button tells the Psychic how many players are within {Helpers.ColorString(Color.yellow, CustomOptionHolder.PsychicPlayerRange.GetFloat().ToString())}x vision range of them. ";
            settingDescription += InvisibleCounter + VentingCounter + "\n\n";

            settingDescription += $"The 'Abilities' button displays a counter for every time a role ability is used during a round. " +
                $"The counter will reset when the round ends. Passive abilities are not included in the counter. \n";

            settingDescription += NonTrackedAbilities();
            RoleInfo.Psychic.SettingsDescription = Helpers.WrapText(settingDescription);
        }

        private static string NonTrackedAbilities()
        {
            List<string> abilites = new();
            if (CustomOptionHolder.BomberSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Successful Bomb Kill ({nameof(Bomber)} victim)");
            if (CustomOptionHolder.ChangelingSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Transform ({nameof(Changeling)})");
            if (CustomOptionHolder.ShadeSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Emerge ({nameof(Shade)})");
            if (CustomOptionHolder.PyromaniacSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Burn ({nameof(Pyromaniac)})");
            if (CustomOptionHolder.RomanticSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Successful Avenge (Vengeful Romantic)");
            if (CustomOptionHolder.GuardianSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Shield ({nameof(Guardian)})");
            if (CustomOptionHolder.ParityCopSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Fake Out (Parity Cop)");
            if (CustomOptionHolder.SheriffSpawnRate.GetSelection() > 0 || !Helpers.GameStarted)
                abilites.Add($"Successful Shoot ({nameof(Sheriff)})");

            string description = abilites.Count > 0
                ? $"The following abilities are NOT included in the abilities counter: {string.Join(", ", abilites)}."
                : "";
            return description;
        }

        public static Sprite GetPsychicAbilitiesButtonSprite()
        {
            return _AbilitiesButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.PsychicAbilities.png", 115f);
        }

        public static Sprite GetPsychicPlayersButtonSprite()
        {
            return _PlayersButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.PsychicPlayers.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            AbilitesUsed = 0;
            InRange = 0;
        }
    }
}
