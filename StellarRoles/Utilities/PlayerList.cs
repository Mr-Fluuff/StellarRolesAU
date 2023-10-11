using HarmonyLib;
using StellarRoles.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StellarRoles
{
    public class TargetMethodClass
    {
        [HarmonyPatch]
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            var type = typeof(PlayerControl).GetNestedTypes(AccessTools.all).FirstOrDefault(t => t.Name.Contains("Start"));
            return AccessTools.Method(type, nameof(IEnumerator.MoveNext));
        }
    }
    public class PlayerList : List<byte>
    {
        public IEnumerable<PlayerControl> GetPlayerEnumerator()
        {
            foreach (GameData.PlayerInfo player in GameData.Instance.AllPlayers.GetFastEnumerator())
                if (Contains(player.PlayerId))
                    yield return player.Object;
        }

        public IEnumerable<PlayerControl> Where(Func<PlayerControl, bool> predicate)
        {
            foreach (GameData.PlayerInfo player in GameData.Instance.AllPlayers.GetFastEnumerator())
                if (Contains(player.PlayerId) && predicate(player.Object)) yield return player.Object;
        }

        public PlayerControl GetPlayerAt(int index)
        {
            byte playerId = this[index];
            foreach (GameData.PlayerInfo player in GameData.Instance.AllPlayers.GetFastEnumerator())
                if (player.PlayerId == playerId)
                    return player.Object;

            throw new System.Exception("Unknown player ID stored in PlayerList collection.");
        }

        public bool Contains(PlayerControl player) => Contains(player.PlayerId);
        public void Add(PlayerControl player) => Add(player.PlayerId);
        public bool Remove(PlayerControl player) => Remove(player.PlayerId);
    }
}
