using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    public static class MedicAbilities
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class MedicHudManagerPatch
        {
            public static void Postfix(HudManager __instance)
            {
                if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Medic.Player) return;
                medicBattery();
                medicUpdate();
                medicSetCurrentTarget();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class ResetMedicAbilitiesMeeting
        {
            public static void Postfix()
            {
                ResetMedic();
            }
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
                ResetMedic();
            }
        }

        public static void ResetMedic()
        {
            if (Medic.VitalsMinigame)
            {
                Medic.VitalsMinigame.ForceClose();
                Medic.VitalsMinigame = null;
            }
            Medic.IsActive = false;
        }

        private static void medicBattery()
        {
            if (MeetingHud.Instance != null) return;

            if (Medic.IsActive) Medic.Battery -= Time.deltaTime;
            if (Medic.AllTasksCompleted && !Medic.IsActive)
            {
                Medic.SelfChargingTimer -= Time.deltaTime;
                if (Medic.SelfChargingTimer <= 0f)
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