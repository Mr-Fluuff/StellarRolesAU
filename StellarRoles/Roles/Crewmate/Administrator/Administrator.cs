using UnityEngine;

namespace StellarRoles
{
    public static class Administrator
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(160, 115, 215, byte.MaxValue);

        private static Sprite _AdminSprite;
        private static Sprite _RoundOneAdminSprite;

        public static float BatteryTime { get; set; }

        public static bool IsActive { get; set; }
        public static int RechargedTasks { get; set; } = 1;
        public static float SelfChargingBatteryCooldown => CustomOptionHolder.AdministratorSelfChargingBatteryCooldown.GetFloat();
        public static float SelfChargingBatteryDuration => CustomOptionHolder.AdministratorBatteryTimePerTask.GetFloat();
        public static bool DisableRoundOneAccess => CustomOptionHolder.AdministratorDisableRoundOneAccess.GetBool();
        public static float TimePerTask => CustomOptionHolder.AdministratorBatteryTimePerTask.GetFloat();
        public static float SelfChargingTimer { get; set; }
        public static bool AllTasksCompleted { get; set; }

        public static void GetDescription()
        {
            string description =
                $"By doing tasks, the {nameof(Administrator)} charges up a battery that powers a mobile admin table. " +
                $"It costs one second of charge to open the table, and the battery loses charge the longer it is open. " +
                $"The battery charges itself every {Helpers.ColorString(Color.yellow, SelfChargingBatteryCooldown.ToString())} seconds when the {nameof(Administrator)} is done with tasks.\n\n";

            if (DisableRoundOneAccess)
                description += "Access to the mobile admin table is disabled in the first round.";

            RoleInfo.Administrator.SettingsDescription = Helpers.WrapText(description);
        }

        public static Sprite GetAdminSprite()
        {
            if (_AdminSprite == null || _RoundOneAdminSprite == null)
                SetAdminSprite();

            return DisableRoundOneAccess && MapOptions.IsFirstRound ? _RoundOneAdminSprite : _AdminSprite;
        }

        public static void SetAdminSprite()
        {
            if (!Helpers.GameStarted) return;
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            switch (mapId)
            {
                case 0:
                case 3:
                    _RoundOneAdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1AdminSkeld.png", 115f);
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Administrator.AdminSkeld.png", 115f);
                    break;

                case 1:
                    _RoundOneAdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1AdminMira.png", 115f);
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Administrator.AdminMira.png", 115f);
                    break;

                case 2:
                    _RoundOneAdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1AdminPolus.png", 115f);
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Administrator.AdminPolus.png", 115f);
                    break;

                case 4:
                    _RoundOneAdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1AdminAirship.png", 115f);
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Administrator.AdminAirship.png", 115f);
                    break;

                case 5:
                    _RoundOneAdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1AdminSubmerged.png", 115f);
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Administrator.AdminSubmerged.png", 115f);
                    break;
                default:
                    _RoundOneAdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.RoundOne.R1AdminSkeld.png", 115f);
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Administrator.AdminSkeld.png", 115f);
                    break;
            }
        }

        public static void ClearAndReload()
        {
            Player = null;
            BatteryTime = CustomOptionHolder.AdministratorInitialBatteryTime.GetFloat();
            SelfChargingTimer = SelfChargingBatteryCooldown;
            AllTasksCompleted = false;
            IsActive = false;
            RechargedTasks = 1;
            SetAdminSprite();
        }
    }
}
