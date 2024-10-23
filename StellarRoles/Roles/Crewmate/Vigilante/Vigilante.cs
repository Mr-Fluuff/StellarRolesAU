using UnityEngine;


namespace StellarRoles
{
    public static class Vigilante
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(255, 255, 0, byte.MaxValue);

        public static int RemainingShotsVigilante { get; set; } = 2;
        public static bool LimitVigiOneShotPerMeeting => CustomOptionHolder.VigilanteHasMultipleShotsPerMeeting.GetBool();

        public static void GetDescription()
        {
            RoleInfo.Vigilante.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Vigilante)} can attempt to guess a player's role in meetings to kill them. " +
                $"If they guess incorrectly, they will die instead. " +
                $"They have a total of {Helpers.ColorString(Color.yellow, CustomOptionHolder.VigilanteNumberOfShots.GetSelection().ToString())} shot(s) per game!\n\n" +
                $"The {nameof(Vigilante)} cannot guess jailed players and may only guess the {nameof(Jailor)} while currently in jail.\n\n" +
                $"This is the only role capable of killing a {nameof(Romantic)} in meetings.\n\n" +
                $"The {nameof(Vigilante)} cannot take a shot if there is less than ten seconds left in a meeting.");
        }

        public static void ClearAndReload()
        {
            Player = null;
            RemainingShotsVigilante = Mathf.RoundToInt(CustomOptionHolder.VigilanteNumberOfShots.GetFloat());
        }
    }
}

