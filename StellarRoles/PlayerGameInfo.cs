using System.Collections.Generic;
using System.Linq;

namespace StellarRoles
{
    public enum InfoType
    {
        AddKill,
        AddCorrectShot,
        AddMisfire,
        AddCorrectGuess,
        AddIncorrectGuess,
        AddAbilityKill,
        AddEat
    }
    public class PlayerGameInfo
    {
        public static readonly Dictionary<byte, PlayerGameInfo> Mapping = new();
        public int Kills = 0;
        public int CorrectGuesses = 0;
        public int IncorrectGuesses = 0;
        public int CorrectShots = 0;
        public int Misfires = 0;
        public int AbilityKills = 0;
        public int ScavengerEats = 0;
        public readonly List<RoleInfo> Roles = new();
        public readonly List<RoleInfo> Modifiers = new();

        public static int TotalKills(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.Kills : 0;
        }

        public static int TotalCorrectShots(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.CorrectShots : 0;
        }

        public static int TotalMisfires(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.Misfires : 0;
        }

        public static int TotalCorrectGuesses(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.CorrectGuesses : 0;
        }

        public static int TotalIncorrectGuesses(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.IncorrectGuesses : 0;
        }

        public static int TotalAbilityKills(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.AbilityKills : 0;
        }

        public static int TotalEaten(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.ScavengerEats : 0;
        }

        public static List<RoleInfo> GetRoles(PlayerControl player)
        {
            if (Mapping.TryGetValue(player.PlayerId, out PlayerGameInfo roleInfo))
                return roleInfo.Roles;

            Mapping.Add(player.PlayerId, roleInfo = new PlayerGameInfo());

            if (player.Data.Role.IsImpostor)
                roleInfo.Roles.Add(RoleInfo.Impostor);
            else
                roleInfo.Roles.Add(RoleInfo.Crewmate);

            return roleInfo.Roles;
        }

        public static List<RoleInfo> GetRoles(GameData.PlayerInfo player)
        {
            if (Mapping.TryGetValue(player.PlayerId, out PlayerGameInfo roleInfo))
                return roleInfo.Roles;

            Mapping.Add(player.PlayerId, roleInfo = new PlayerGameInfo());

            if (player.Role.IsImpostor)
                roleInfo.Roles.Add(RoleInfo.Impostor);
            else
                roleInfo.Roles.Add(RoleInfo.Crewmate);

            return roleInfo.Roles;
        }

        public static void AddRole(byte playerId, RoleInfo role)
        {
            if (!Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo))
                Mapping.Add(playerId, gameInfo = new PlayerGameInfo());

            if (!gameInfo.Roles.Any(r => r.RoleId == role.RoleId))
                gameInfo.Roles.Add(role);
        }

        public static List<RoleInfo> GetModifiers(byte playerId)
        {
            if (!Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo))
                Mapping.Add(playerId, gameInfo = new PlayerGameInfo());
            return gameInfo.Modifiers;
        }

        public static void AddModifier(byte playerId, RoleInfo role)
        {
            if (!Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo))
                Mapping.Add(playerId, gameInfo = new PlayerGameInfo());

            if (!gameInfo.Modifiers.Any(r => r.RoleId == role.RoleId))
                gameInfo.Modifiers.Add(role);
        }

        public static void ClearAndReload()
        {
            Mapping.Clear();
        }
    }
}
