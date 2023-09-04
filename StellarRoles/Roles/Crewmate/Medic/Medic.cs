using UnityEngine;

namespace StellarRoles
{
    public static class Medic
    {
        public static PlayerControl Player { get; set; }
        public static Minigame VitalsMinigame { get; set; } = null;
        public static PlayerControl Target { get; set; } = null;
        public static PlayerControl CurrentTarget { get; set; } = null;
        public static bool UsedHeartMonitor { get; set; } = false;

        public static readonly Color Color = new Color32(10, 180, 110, byte.MaxValue);

        public static float Battery { get; set; } = 20f;
        public static float TimePerTask => CustomOptionHolder.MedicBatteryTimePerTask.GetFloat();
        public static float SelfChargingBatteryCooldown => CustomOptionHolder.MedicSelfChargingBatteryCooldown.GetFloat();
        public static float SelfChargingBatteryDuration => CustomOptionHolder.MedicBatteryTimePerTask.GetFloat();
        public static float SelfChargingTimer { get; set; }
        public static bool AllTasksCompleted { get; set; }
        public static bool DisableRoundOneAccess => CustomOptionHolder.MedicDisableRoundOneAccess.GetBool();
        public static bool NonCrewFlash => CustomOptionHolder.MedicNonCrewFlash.GetBool();
        public static float NonCrewFlashDelay => NonCrewFlash ? (CustomOptionHolder.MedicNonCrewFlash.GetSelection() - 1) * 2.5f : 0;
        public static bool RoleBlock => CustomOptionHolder.MedicRoleBlock.GetBool() && CustomOptionHolder.CrewRoleBlock.GetBool();

        public static bool IsActive { get; set; }
        public static int RechargedTasks { get; set; } = 1;

        private static Sprite _ButtonSprite;
        private static Sprite _VitalsSprite;
        private static Sprite _RoundOneVitalsSprite;

        public static void GetDescription()
        {
            string description = Helpers.WrapText(
                $"The {nameof(Medic)} has two main abilities.\n\n" +
                $"Heart monitor - Can be placed on one player per round and notifies the {nameof(Medic)} if the player is killed.\n\n" +
                $"Mobile Vitals Panel - By doing tasks, the {nameof(Medic)} charges up a battery that powers a mobile vitals panel! " +
                $"It costs one second of charge to open this panel, and the battery loses charge the longer it is open. " +
                $"The battery charges itself every {Helpers.ColorString(Color.yellow, SelfChargingBatteryCooldown.ToString())} seconds when the {nameof(Medic)} is done with tasks.\n\n");

            if (DisableRoundOneAccess)
                description += "Access to the mobile vitals panel is disabled in the first round.\n\n";

            float flashDelay = NonCrewFlashDelay;
            if (NonCrewFlash)
                description += $"The player who kills your monitor target will be notificed {(flashDelay == 0 ? "immediately" : $"after {Helpers.ColorString(Color.yellow, flashDelay.ToString())} seconds")}";

            RoleInfo.Medic.SettingsDescription = Helpers.WrapText(description);
        }

        public static Sprite GetHeartMonitorSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.MedicHeartMonitor.png", 115f);
        }

        public static Sprite GetVitalsSprite()
        {
            return DisableRoundOneAccess && MapOptions.IsFirstRound
                ? (_RoundOneVitalsSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1MedicVitals.png", 115f))
                : (_VitalsSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.MedicVitals.png", 115f));
        }

        public static void ResetHeartMonitor()
        {
            CurrentTarget = null;
            Target = null;
            UsedHeartMonitor = false;
        }

        public static void ClearAndReload()
        {
            Player = null;
            Target = null;
            CurrentTarget = null;
            VitalsMinigame = null;
            Battery = CustomOptionHolder.MedicInitialBatteryTime.GetFloat();
            UsedHeartMonitor = false;
            SelfChargingTimer = SelfChargingBatteryCooldown;
            AllTasksCompleted = false;
            IsActive = false;
            RechargedTasks = 1;
        }
    }
}
