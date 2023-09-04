using StellarRoles.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Watcher
    {

        public static readonly Dictionary<byte, Arrow> TrippedTrapWires = new();
        public static readonly PlayerList TrackedPlayers = new();

        public static PlayerControl Player { get; set; }
        public static readonly Color Color = new Color32(10, 235, 235, byte.MaxValue);

        public static float BatteryTime { get; set; }
        public static float BatteryTimePerTask => CustomOptionHolder.WatcherBatteryTimePerTask.GetFloat();
        public static float SensorCount { get; set; }
        public static bool AnonymousArrows => CustomOptionHolder.WatcherAnonymousArrows.GetBool();
        public static bool IsActive { get; set; } = false;
        public static int RechargedTasks { get; set; }
        public static Minigame CameraMinigame { get; set; } = null;
        public static float SelfChargingBatteryCooldown => CustomOptionHolder.WatcherSelfChargingBatteryCooldown.GetFloat();
        public static float SelfChargingBatteryDuration => CustomOptionHolder.WatcherBatteryTimePerTask.GetFloat();
        public static float SelfChargingTimer { get; set; }
        public static bool AllTasksCompleted { get; set; }
        public static bool DisableRoundOneAccess => CustomOptionHolder.WatcherRoundOneAccess.GetBool();
        public static bool RoleBlock => CustomOptionHolder.WatcherRoleBlock.GetBool() && CustomOptionHolder.CrewRoleBlock.GetBool();
        public static bool NonCrewFlash => CustomOptionHolder.WatcherNonCrewFlash.GetBool();
        public static float NonCrewFlashDelay => NonCrewFlash ? (CustomOptionHolder.WatcherNonCrewFlash.GetSelection() - 1) * 2.5f : 0;

        private static Sprite _CamSprite;
        private static Sprite _RoundOneCamSprite;
        private static Sprite _LogSprite;
        private static Sprite _RoundOneLogSprite;
        private static Sprite _TrapWireButtonSprite;
        private static Sprite _TrippedWireOverlay;
        private static Sprite _TrippedWireToolTip;

        public static void GetDescription()
        {
            string informationSource = Helpers.GameStarted ? (Helpers.IsMap(Map.Mira) ? "door logs" : "cameras") : "logs/cameras";
            string settingsDescription =
                $"The {nameof(Watcher)} has two main abilities: sensors and mobile {informationSource}.\n\n" +
                $"The {nameof(Watcher)} gets 1 sensor that is reset every round. " +
                $"When a player walks near that sensor, " +
                $"the {nameof(Watcher)} is alerted and {(AnonymousArrows ? "an anonymous" : "a colored")} arrow will point to the location of where the sensor was.\n\n" +
                $"By doing tasks, the {nameof(Watcher)} charges up a battery that powers a mobile {informationSource} panel! " +
                $"It costs one second of charge to open this panel, and the battery loses charge the longer it is open. " +
                $"The battery charges itself every {Helpers.ColorString(Color.yellow, SelfChargingBatteryCooldown.ToString())} seconds when the {nameof(Watcher)} is done with tasks.";

            if (NonCrewFlash)
                settingsDescription += $"\n\nPlayers who trip your sensor will be notified {(NonCrewFlashDelay == 0 ? "immediately" : $"after {Helpers.ColorString(Color.yellow, NonCrewFlashDelay.ToString())} seconds")}";

            RoleInfo.Watcher.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static Sprite GetCamSprite()
        {
            return DisableRoundOneAccess && MapOptions.IsFirstRound
                ? (_RoundOneCamSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1WatcherCamera.png", 115f))
                : (_CamSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Watcher.WatcherCamera.png", 115f));
        }

        public static Sprite GetTrippedWireOverlay()
        {
            return _TrippedWireOverlay ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Watcher.WatcherSensorOverlay.png", 180f);
        }

        public static Sprite GetTrippedWireToolTip()
        {
            return _TrippedWireToolTip ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Watcher.WatcherToolTip.png", 350f);
        }

        public static Sprite GetLogSprite()
        {
            return DisableRoundOneAccess && MapOptions.IsFirstRound
                ? (_RoundOneLogSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1Doorlogs.png", 115f))
                : (_LogSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Watcher.Doorlogs.png", 115f));
        }

        public static void ResetSensors()
        {
            SensorCount = 1;
            Sensor.ClearTripWires();
            TrackedPlayers.Clear();
        }

        private static void DestroyArrows()
        {
            foreach (Arrow arrow in TrippedTrapWires.Values)
                Object.Destroy(arrow.Object);
        }

        public static Sprite GetTrapWireSprite()
        {
            return _TrapWireButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Watcher.WatcherSensorButton.png", 135f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            BatteryTime = CustomOptionHolder.WatcherInitialBatteryTime.GetFloat();
            SensorCount = 1;
            SelfChargingTimer = SelfChargingBatteryCooldown;
            IsActive = false;
            RechargedTasks = 1;
            CameraMinigame = null;
            AllTasksCompleted = false;
            DestroyArrows();
            TrippedTrapWires.Clear();
            TrackedPlayers.Clear();

        }
    }
}
