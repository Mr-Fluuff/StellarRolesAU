using BepInEx.Unity.IL2CPP.Utils;
using StellarRoles.Objects;
using StellarRoles.Utilities;
using System;
using System.Collections;
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
            BombedDictionary.TryAdd(player.PlayerId, this);
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

        public void KillBombed()
        {
            if (PassedBomb || !BombActive || MeetingHud.Instance || Player.Data.IsDead) return;
            // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
            Helpers.CheckBombedAttemptAndKill(Bomber, Player, showAnimation: false);
            RPCProcedure.Send(CustomRPC.GiveBomb, Player, Bomber, true);
            RPCProcedure.GiveBomb(Player, Bomber, true);
            Bomber.RPCAddGameInfo(InfoType.AddAbilityKill, InfoType.AddKill);
        }

        public void AlertBombed(float time)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(time, new Action<float>((p) =>
            { // Delayed action
                if (!PassedBomb && BombActive && !MeetingHud.Instance && !Player.Data.IsDead)
                {
                    int timeLeft = (int)(time - (time * p));
                    if (timeLeft <= BombTimer && TimeLeft != timeLeft)
                    {
                        _ = new CustomMessage($"Your Bomb will explode in {timeLeft} seconds!", 1f, true, Color.red);
                        TimeLeft = timeLeft;
                    }
                    if (p == 1f)
                    {
                        HudManager.Instance.StartCoroutine(BombAction());
                    }
                }
            })));
        }

        private static readonly WaitForSeconds delay = new WaitForSeconds(0.25f);
        public IEnumerator BombAction()
        {
            while (Player.inMovingPlat || Player.onLadder)
            {
                yield return delay;
            }
            KillBombed();
        }

    }
    public static class BombedExtensions
    {
        public static bool IsBombed(this PlayerControl player, out Bombed bombed) => Bombed.IsBombed(player.PlayerId, out bombed);
        public static bool IsBombed(this PlayerControl player) => Bombed.IsBombed(player.PlayerId, out _);
        public static bool IsBombedAndActive(this PlayerControl player) => Bombed.IsBombedAndActive(player.PlayerId);
        public static void AlertBombed(this Bombed bombed) => bombed.AlertBombed();
    }
}
