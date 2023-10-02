using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class MedicAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Medic.Player) return;
            medicBattery();
            medicUpdate();
            medicSetCurrentTarget();
        }

        private static void medicSetCurrentTarget()
        {
            if (Medic.UsedHeartMonitor) return;

            Medic.CurrentTarget = Helpers.SetTarget();
        }

        public static bool isRoleBlocked()
        {
            return Medic.RoleBlock && Helpers.IsCommsActive();
        }

        private static void medicUpdate()
        {
            (int playerCompleted, int totalTasks) = TasksHandler.TaskInfo(Medic.Player.Data);
            Medic.AllTasksCompleted = playerCompleted == totalTasks;
            if (playerCompleted == Medic.RechargedTasks)
            {
                Medic.RechargedTasks += 1;
                bool medicDisabled = Medic.DisableRoundOneAccess && MapOptions.IsFirstRound;

                if (!medicDisabled)
                {
                    Medic.Battery += Medic.TimePerTask / Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                }
            }

            if (Medic.IsActive && Medic.VitalsMinigame != null && (Medic.Battery <= 0f || Medic.VitalsMinigame.amClosing == Minigame.CloseState.Closing))
            {
                Helpers.SetMovement(true);
                Medic.IsActive = false;

                if (Medic.Battery <= 0f)
                Medic.VitalsMinigame.ForceClose();

                Medic.VitalsMinigame = null;
            }
        }

        private static void medicBattery()
        {
            if (MeetingHud.Instance != null) return;

            if (Medic.IsActive) Medic.Battery -= Time.deltaTime;
            if (Medic.AllTasksCompleted && !Medic.IsActive)
            {
                Medic.SelfChargingTimer -= Time.deltaTime;
                if (Medic.SelfChargingTimer == 0f)
                {
                    Medic.SelfChargingTimer = Medic.SelfChargingBatteryCooldown;
                    if (Ascended.IsAscended(Medic.Player))
                    {
                        Medic.SelfChargingTimer = Medic.SelfChargingBatteryCooldown * .75f;
                    }

                    bool disabled = Medic.DisableRoundOneAccess && MapOptions.IsFirstRound;
                    if (!disabled)
                    {
                        Medic.Battery += Medic.SelfChargingBatteryDuration / Helpers.SpitefulMultiplier(PlayerControl.LocalPlayer);
                    }
                }
            }
        }
    }
}
