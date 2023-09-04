namespace StellarRoles
{
    public static class Sniper
    {
        public static readonly PlayerList Players = new();
        public static void GetDescription()
        {
            RoleInfo.Sniper.SettingsDescription = Helpers.WrapText($"The {nameof(Sniper)} modifier increases the range of a player's kill button by 60%.");
        }
        public static void ClearAndReload()
        {
            Players.Clear();
        }
    }
}
