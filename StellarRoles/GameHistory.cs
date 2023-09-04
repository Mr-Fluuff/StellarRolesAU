using System;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public class DeadPlayer
    {
        public PlayerControl Player;
        public DateTime TimeOfDeath;
        public DeathReason DeathReason;
        public PlayerControl KillerIfExisting;

        public DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting)
        {
            Player = player;
            TimeOfDeath = timeOfDeath;
            DeathReason = deathReason;
            KillerIfExisting = killerIfExisting;
        }
    }

    public static class GameHistory
    {
        public static readonly List<Tuple<Vector3, bool>> LocalPlayerPositions = new();
        public static readonly List<DeadPlayer> DeadPlayers = new();

        public static void ClearGameHistory()
        {
            LocalPlayerPositions.Clear();
            DeadPlayers.Clear();
        }
    }
}