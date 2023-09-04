using UnityEngine;

namespace StellarRoles
{
    public static class Follower
    {
        public static PlayerControl Player { get; set; }
        public static readonly Color Color = Palette.ImpostorRed;
        public static bool GetsAssassin => CustomOptionHolder.AssassinCount.GetSelection() >= 2;

        public static void GetDescription()
        {
            RoleInfo.Follower.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Follower)} is a player who was converted into an Impostor by a {nameof(Cultist)}.\n\n" +
                $"The {nameof(Follower)} cannot be killed in meeting by the {nameof(Vigilante)}.\n\n" +
                $"The {nameof(Cultist)} and {nameof(Follower)} have arrows pointing to one another and are alerted when the other kills with a red flash on their screen. " +
                $"The {nameof(Cultist)} and {nameof(Follower)} can mid round chat with one another.");
        }

        public static void ClearAndReload()
        {
            Player = null;
        }
    }
}
