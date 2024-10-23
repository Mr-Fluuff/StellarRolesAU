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
        AddEat,
        AddCorrectVote,
        AddIncorrectVote,
        AddCorrectEject,
        AddIncorrectEject,
        AddCrewmatesEjected, //Overall Goal for imposter scoring
        PlayerDiedBeforeLastMeeting,
        UpdateSurvivability,
        FirstTwoPlayersDead,
        CriticalMeetingError,
        CritcalMeetingErrorReverse

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
        public int CorrectVotes = 0;
        public int IncorrectVotes = 0;
        public int CorrectEjects = 0;
        public int IncorrectEjects = 0;
        public int CrewmatesEjected = 0;
        public bool PlayerAlive = true;
        public int survivability = 0;
        public bool firstTwoPlayersDead = false;
        public bool criticalMeetingError = false;
        public readonly List<RoleInfo> Roles = new();
        public readonly List<RoleInfo> Modifiers = new();

        public static int TotalKills(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.Kills : 0;
        }

        public static int Survivability(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.survivability : 0;
        }

        public static bool PlayerAliveAtLastMeeting(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) && gameInfo.PlayerAlive;
        }

        public static bool FirstTwoPlayersDead(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) && gameInfo.firstTwoPlayersDead;
        }

        public static bool WrongCriticalVote(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) && gameInfo.criticalMeetingError;
        }

        public static int TotalCrewmatesEjected(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.CrewmatesEjected : 0;
        }

        public static int TotalCorrectVotes(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.CorrectVotes : 0;

        }

        public static int TotalIncorrectVotes(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.IncorrectVotes : 0;
        }
        public static int TotalCorrectEjects(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.CorrectEjects : 0;

        }

        public static int TotalIncorrectEjects(byte playerId)
        {
            return Mapping.TryGetValue(playerId, out PlayerGameInfo gameInfo) ? gameInfo.IncorrectEjects : 0;

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

        public static List<RoleInfo> GetRoles(PlayerControl player) => GetRoles(player.Data);
        public static List<RoleInfo> GetRoles(NetworkedPlayerInfo player)
        {
            if (Mapping.TryGetValue(player.PlayerId, out PlayerGameInfo roleInfo))
            {
                if (roleInfo.Roles?.Count != 0)
                    return roleInfo.Roles;
            }
            else
            {
                Mapping.Add(player.PlayerId, roleInfo = new PlayerGameInfo());
            }

            if (player.Role.IsImpostor)
                roleInfo.Roles.Add(RoleInfo.Impostor);
            else
                roleInfo.Roles.Add(RoleInfo.Crewmate);

            return roleInfo.Roles;
        }

        public static void EraseHistory(PlayerControl player)
        {
            if (Mapping.TryGetValue(player.PlayerId, out PlayerGameInfo roleInfo))
                roleInfo.Roles.Clear();
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
