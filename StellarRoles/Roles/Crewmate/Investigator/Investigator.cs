using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    public static class Investigator
    {
        public static PlayerControl Player { get; set; }
        public static readonly Color Color = new Color32(76, 131, 76, byte.MaxValue);

        public static float FootprintIntervall => CustomOptionHolder.InvestigatorFootprintInterval.GetFloat() * 3;
        public static float FootprintDuration => CustomOptionHolder.InvestigatorFootprintDuration.GetFloat();
        public static bool AnonymousFootprints => CustomOptionHolder.InvestigatorAnonymousFootprints.GetBool();

        public static readonly PlayerList AllPlayers = new();
        public static float Timer { get; set; } = 6.2f;
        public static bool RoleBlock => CustomOptionHolder.InvestigatorRoleBlock.GetBool() && CustomOptionHolder.CrewRoleBlock.GetBool();

        private static Sprite _InvestigatorButton;

        public static void GetDescription()
        {
            RoleInfo.Investigator.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Investigator)} sees a lingering {(AnonymousFootprints ? "anonymous" : "colored")} footprint trail behind all players that lasts for {Helpers.ColorString(Color.yellow, FootprintDuration.ToString())} seconds.\n\n" +
                $"When reporting a body, the timing since the body was killed will appear in the {nameof(Investigator)}'s chat window.");
        }

        public static float CalculateFootprintInterval()
        {
            float result = FootprintIntervall;
            if (Ascended.IsAscended(Player))
            {
                result *= .5f;
            }
            return result;
        }

        public static float CalculateFootprintDuration()
        {
            float result = FootprintDuration;
            if (Ascended.IsAscended(Player))
            {
                result *= 1.5f;
            }
            return result;
        }
        public static void ClearAndReload()
        {
            Player = null;
            Timer = 6.2f;
            AllPlayers.Clear();
            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                AllPlayers.Add(player);
            }
        }

        public static Sprite GetDetectiveButton()
        {
            return _InvestigatorButton ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.DetectiveInspect.png", 115f);
        }
    }
}
