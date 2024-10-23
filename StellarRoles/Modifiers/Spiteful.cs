using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    public class Spiteful
    {
        public static readonly List<Spiteful> SpitefulRoles = new();
        public static readonly Color Color = new Color32(156, 190, 228, byte.MaxValue);
        public static float Punishment => 1 + (.25f + (CustomOptionHolder.ModifierSpitefulImpact.GetSelection() * .25f));
        public static void GetDescription()
        {
            RoleInfo.Spiteful.SettingsDescription = Helpers.WrapText(
                $"When a {nameof(Spiteful)} player is voted out, all players who voted for them have their ability cooldowns increased by " +
                $"{Helpers.ColorString(Color.yellow, Punishment.ToString())}x.");
        }

        public PlayerControl Player;
        public bool IsExiled = false;
        public readonly PlayerList VotedBy = new();

        public Spiteful(PlayerControl player)
        {
            Player = player;
            IsExiled = false;
            SpitefulRoles.Add(this);
        }

        public static void ClearAndReload()
        {
            SpitefulRoles.Clear();
        }
    }

    public static class SpitefulExtensions
    {
        public static bool IsSpiteful(this PlayerControl player, out Spiteful spiteful)
        {
            spiteful = Spiteful.SpitefulRoles.FirstOrDefault(role => role.Player == player);
            return spiteful != null;
        }
        public static bool IsSpiteful(byte playerId, out Spiteful spiteful)
        {
            spiteful = Spiteful.SpitefulRoles.FirstOrDefault(role => role.Player.PlayerId == playerId);
            return spiteful != null;
        }
    }
}
