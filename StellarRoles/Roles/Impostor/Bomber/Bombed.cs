using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public class Bombed
    {
        public static readonly Dictionary<byte, Bombed> BombedDictionary = new();
        public static readonly Dictionary<byte, PlayerList> KillerDictionary = new();

        public readonly PlayerControl Player;
        public readonly PlayerControl Bomber;
        public PlayerControl LastBombed = null;
        public bool BombActive = false;
        public bool HasAlerted = false;
        public bool PassedBomb = false;
        public int TimeLeft = 0;
        public PlayerControl CurrentTarget = null;

        public static readonly Color AlertColor = Palette.ImpostorRed;
        public static float BombDelay => CustomOptionHolder.BomberDelay.GetFloat();
        public static float BombTimer => CustomOptionHolder.BomberTimer.GetFloat();
        public static bool CanReport => CustomOptionHolder.BomberCanReport.GetBool();

        public Bombed(PlayerControl player, PlayerControl bomber)
        {
            Player = player;
            Bomber = bomber;
            BombActive = false;
            HasAlerted = false;
            TimeLeft = (int)BombTimer;
            BombedDictionary.Add(player.PlayerId, this);
        }

        public static void ClearAndReload()
        {
            BombedDictionary.Clear();
            KillerDictionary.Clear();
        }

        public static bool IsBombed(byte playerId, out Bombed bombed)
        {
            return BombedDictionary.TryGetValue(playerId, out bombed);
        }

        public static bool IsBombedAndActive(byte playerId)
        {
            return IsBombed(playerId, out Bombed bombed) && bombed.BombActive;
        }
    }

    public static class BombedExtensions
    {
        public static bool IsBombed(this PlayerControl player, out Bombed bombed) => Bombed.IsBombed(player.PlayerId, out bombed);
        public static bool IsBombedAndActive(this PlayerControl player) => Bombed.IsBombedAndActive(player.PlayerId);
    }
}
