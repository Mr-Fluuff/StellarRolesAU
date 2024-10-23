using HarmonyLib;
using StellarRoles.Utilities;

namespace StellarRoles
{
    [HarmonyPatch]
    public static class TasksHandler
    {
        public static (int, int) TaskInfo(NetworkedPlayerInfo playerInfo)
        {
            int total = 0;
            int completed = 0;
            if (!playerInfo.Disconnected && playerInfo.Tasks != null &&
                playerInfo.Object &&
                playerInfo.Role && playerInfo.Role.TasksCountTowardProgress &&
                !playerInfo.Object.HasFakeTasks() && !playerInfo.Role.IsImpostor
                )
            {
                foreach (var playerInfoTask in playerInfo.Tasks.GetFastEnumerator())
                {
                    if (playerInfoTask.Complete) completed++;
                    total++;
                }
            }
            return (completed, total);
        }

        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private static class GameDataRecomputeTaskCountsPatch
        {
            private static bool Prefix(GameData __instance)
            {
                int totalTasks = 0;
                int completedTasks = 0;

                foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
                {
                    if (playerInfo.Object == null) continue;
                    (int playerCompleted, int playerTotal) = TaskInfo(playerInfo);
                    totalTasks += playerTotal;
                    completedTasks += playerCompleted;
                }

                __instance.TotalTasks = totalTasks;
                __instance.CompletedTasks = completedTasks;
                return false;
            }
        }

    }
}
