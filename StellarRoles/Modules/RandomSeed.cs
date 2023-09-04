using System.Linq;
using Random = System.Random;

namespace StellarRoles.Modules
{
    public static class RandomSeed
    {
        public static Random _random = new();

        public static void GenerateSeed()
        {
            if (!AmongUsClient.Instance.AmHost) return;
            ShareRandomSeed(new Random().Next());
        }

        public static void ShareRandomSeed(int seed)
        {
            RPCProcedure.Send(CustomRPC.ShareRandomSeed, seed);
            Rpc_ShareRandomSeed(seed);
        }

        public static void Rpc_ShareRandomSeed(int seed)
        {
            _random = new Random(seed);
        }

        public static void RandomizeMeetingPlayers(MeetingHud __instance)
        {
            System.Collections.Generic.List<PlayerVoteArea> alivePlayers = __instance.playerStates.Where(area => !area.AmDead).ToList();
            alivePlayers.Sort(SortListByNames);

            System.Collections.Generic.List<UnityEngine.Vector3> playerPositions = alivePlayers.Select(area => area.transform.localPosition).ToList();
            System.Collections.Generic.List<PlayerVoteArea> playersList = alivePlayers.OrderBy(_ => _random.Next()).ToList();

            for (int i = 0; i < playersList.Count; i++)
            {
                playersList[i].transform.localPosition = playerPositions[i];
            }
        }
        private static int SortListByNames(PlayerVoteArea a, PlayerVoteArea b)
        {
            return string.CompareOrdinal(a.NameText.text, b.NameText.text);
        }
    }
}
