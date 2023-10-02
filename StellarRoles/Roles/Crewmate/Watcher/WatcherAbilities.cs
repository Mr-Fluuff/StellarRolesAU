using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class WatcherAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Watcher.Player) return;

            watcherUpdate();
            watcherBattery();
            Sensor.UpdateForWatcher();
            Sensor.UpdateActivateSensorPerPlayer();
        }

        public static bool isRoleBlocked()
        {
            return Watcher.RoleBlock && Helpers.IsCommsActive();
        }


        static void watcherBattery()
        {
            if (MeetingHud.Instance != null) return;

            if (Watcher.IsActive) Watcher.BatteryTime -= Time.deltaTime;
            if (Watcher.AllTasksCompleted && !Watcher.IsActive)
            {
                Watcher.SelfChargingTimer -= Time.deltaTime;

                if (Watcher.SelfChargingTimer == 0f)
                {
                    Watcher.SelfChargingTimer = Watcher.SelfChargingBatteryCooldown;
                    if (Ascended.IsAscended(Watcher.Player))
                    {
                        Watcher.SelfChargingTimer = Watcher.SelfChargingBatteryCooldown * .75f;
                    }
                    Watcher.BatteryTime += Watcher.SelfChargingBatteryDuration / Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                }
            }
        }

        public static void watcherUpdate()
        {
            (int playerCompleted, int totalTasks) = TasksHandler.TaskInfo(Watcher.Player.Data);
            Watcher.AllTasksCompleted = playerCompleted == totalTasks;

            if (playerCompleted == Watcher.RechargedTasks)
            {
                Watcher.RechargedTasks += 1;
                bool watcherDisabled = Watcher.DisableRoundOneAccess && MapOptions.IsFirstRound;

                if (!watcherDisabled)
                {
                    Watcher.BatteryTime += Watcher.BatteryTimePerTask / Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                }
            }

            if (Watcher.IsActive && Watcher.CameraMinigame != null && (Watcher.BatteryTime <= 0f || Watcher.CameraMinigame.amClosing == Minigame.CloseState.Closing))
            {
                Helpers.SetMovement(true);
                Watcher.IsActive = false;

                if (Watcher.BatteryTime <= 0f)
                Watcher.CameraMinigame.ForceClose();

                Watcher.CameraMinigame = null;
            }
        }
    }
}
