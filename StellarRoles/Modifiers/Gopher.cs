namespace StellarRoles
{
    public static class Gopher
    {
        public static readonly PlayerList Players = new();

        public static void GetDescription()
        {
            RoleInfo.Gopher.SettingsDescription = Helpers.WrapText($"The {nameof(Gopher)} modifier allows a player's cooldowns to continue to count while they are in a vent, rather than pause like they normally would.");
        }

        public static void ClearAndReload()
        {
            Players.Clear();
        }
    }
}
