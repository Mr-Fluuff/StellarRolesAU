using HarmonyLib;
using System;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class JailorAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || !PlayerControl.LocalPlayer.IsJailor(out Jailor jailor)) return;

            try
            {
                (int, int) tasks = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);

                if (tasks.Item1 == jailor.CurrentRechargeTasks)
                {
                    jailor.CurrentRechargeTasks += Jailor.TasksToRecharge;
                    jailor.Charges++;
                }
                if (tasks.Item1 == tasks.Item2 && tasks.Item2 > 0 && Ascended.IsAscended(jailor.Player))
                {
                    jailor.Charges++;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log(LogLevel.Error, "Jailor error: " + ex.StackTrace);
            }
        }
    }
}
