namespace StellarRoles
{
    public static class Ascended
    {
        public static readonly PlayerList Players = new();
        public static void GetDescription()
        {
            RoleInfo.Ascended.SettingsDescription = Helpers.WrapText(
               $"You role is improved! Check the in-game wiki to see how!");
        }

        public static bool IsAscended(PlayerControl playerControl)
        {
            if (playerControl == null)
            {
                return false;
            }
            return Players.Contains(playerControl);

        }

        public static bool AscendedSheriff(PlayerControl playerControl)
        {
            return playerControl == Sheriff.Player && IsAscended(playerControl);
        }

        public static bool AscendedNightmare(PlayerControl playerControl)
        {
            return Nightmare.isNightmare(playerControl) && IsAscended(playerControl);
        }
        public static bool AscendedMiner(PlayerControl playerControl)
        {
            return playerControl == Miner.Player && IsAscended(playerControl);
        }

        public static bool AscendedJester(PlayerControl playerControl)
        {
            return Jester.IsJester(playerControl) && IsAscended(playerControl);
        }
        public static void ClearAndReload()
        {
            Players.Clear();
        }
    }
}
