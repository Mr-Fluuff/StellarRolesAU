using System.Collections.Generic;

namespace StellarRoles
{
    public enum RPS
    {
        Scissors,
        Rock,
        Paper,
        None
    }

    public class RockPaperScissorsGame
    {
        public static readonly List<RockPaperScissorsGame> Games = new();

        public PlayerControl PlayerOne;
        public PlayerControl PlayerTwo;
        public RPS PlayerOneRPS;
        public RPS PlayerTwoRPS;
        public bool IsDraw = false;
        public PlayerControl Winner;
        public bool IsComplete;

        public RockPaperScissorsGame()
        {
            Games.Add(this);
        }

        public PlayerControl Duel()
        {
            if (PlayerOneRPS == RPS.None || PlayerTwoRPS == RPS.None || PlayerOne == null || PlayerTwo == null)
                return null;
            IsComplete = true;
            if (PlayerOneRPS == PlayerTwoRPS)
            {
                IsDraw = true;
                return null;
            }

            switch (PlayerOneRPS)
            {
                case RPS.Rock:
                    if (PlayerTwoRPS == RPS.Scissors)
                        return PlayerOne;
                    else if (PlayerTwoRPS == RPS.Paper)
                        return PlayerTwo;
                    break;
                case RPS.Scissors:
                    if (PlayerTwoRPS == RPS.Rock)
                        return PlayerTwo;
                    else if (PlayerTwoRPS == RPS.Paper)
                        return PlayerOne;
                    break;
                case RPS.Paper:
                    if (PlayerTwoRPS == RPS.Rock)
                        return PlayerOne;
                    else if (PlayerTwoRPS == RPS.Scissors)
                        return PlayerTwo;
                    break;
            }

            IsComplete = false;
            return null;
        }

        public static RockPaperScissorsGame FetchDuel(PlayerControl playerId1, PlayerControl playerId2)
        {
            foreach (RockPaperScissorsGame game in Games)
            {
                if (playerId1 == game.PlayerOne && playerId2 == game.PlayerTwo) return game;
                if (playerId1 == game.PlayerTwo && playerId2 == game.PlayerOne) return game;
            }
            return null;
        }

        public static PlayerControl FetchPartner(PlayerControl player)
        {
            foreach (RockPaperScissorsGame game in Games)
                if (game.PlayerTwo == player)
                    return game.PlayerOne;
            return null;
        }

        public static RockPaperScissorsGame FetchDuel(PlayerControl player)
        {
            foreach (RockPaperScissorsGame game in Games)
            {
                if (game.PlayerOne == player) return game;
                if (game.PlayerTwo == player) return game;
            }
            return null;
        }

        public static RPS ExtractEntry(string entryName)
        {
            return entryName switch
            {
                "rock" => RPS.Rock,
                "scissors" => RPS.Scissors,
                "paper" => RPS.Paper,
                _ => RPS.None
            };
        }

        public static void ClearAndReload()
        {
            Games.Clear();
        }
    }
}
