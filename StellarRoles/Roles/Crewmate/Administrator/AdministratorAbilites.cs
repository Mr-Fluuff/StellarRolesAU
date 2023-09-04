using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class AdministratorAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Administrator.Player) return;

            administratorBattery();
            administratorUpdate();
        }

        static void administratorBattery()
        {
            if (MeetingHud.Instance != null) return;

            if (Administrator.IsActive) Administrator.BatteryTime -= Time.deltaTime;
            if (Administrator.AllTasksCompleted && !Administrator.IsActive)
            {
                Administrator.SelfChargingTimer -= Time.deltaTime;
                if (Administrator.SelfChargingTimer == 0f)
                {
                    Administrator.SelfChargingTimer = Administrator.SelfChargingBatteryCooldown;
                    if (Ascended.IsAscended(Administrator.Player))
                    {
                        Administrator.SelfChargingTimer = Administrator.SelfChargingBatteryCooldown * .75f;
                    }
                    Administrator.BatteryTime += Administrator.SelfChargingBatteryDuration / Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                }
            }
        }

        public static void administratorUpdate()
        {
            (int playerCompleted, int totalTasks) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
            Administrator.AllTasksCompleted = playerCompleted == totalTasks;

            if (playerCompleted == Administrator.RechargedTasks)
            {
                Administrator.RechargedTasks += 1;
                bool disabled = Administrator.DisableRoundOneAccess && MapOptions.IsFirstRound;
                if (!disabled)
                {
                    Administrator.BatteryTime += Administrator.TimePerTask / Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                }
            }

            if (Administrator.IsActive && (MapBehaviour.Instance == null || !MapBehaviour.Instance.isActiveAndEnabled || Administrator.BatteryTime <= 0f))
            {
                Helpers.SetMovement(true);
                MapBehaviour.Instance.Close();
                Administrator.IsActive = false;
            }
        }
    }
}
