using StellarRoles.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Charlatan
    {
        public static PlayerControl Player { get; set; } = null;
        public static byte ReportTarget { get; set; } = byte.MaxValue;
        public static List<DeadBody> DeadBodies { get; set; } = new();
        public static List<byte> ConcealedBodies { get; set; } = new();

        public static bool IsNeutralKiller => CustomOptionHolder.CharlatanIsNeutral.GetBool();
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        //Conceal
        public static int ConcealChargesPerKill => CustomOptionHolder.CharlatanConcealChargesPerKill.GetInt();
        public static float ConcealChannelDuration=> CustomOptionHolder.CharlatanConcealChannelDuration.GetFloat();
        public static float ConcealReportRange => CustomOptionHolder.CharlatanConcealReportRange.GetSelection() == 0 ? 0.30f : 0.5f;


        public static int ConcealCharges { get; set; } = 1;
        public const float ConcealRange = 0.8f;

        //Deceive
        public static float DeceiveIncPerKill => CustomOptionHolder.CharlatanDeceiveDurationPerKill.GetFloat();
        public static float DeceiveTimer { get; set; } = 0f;
        public static float MaxDeceiveTimer { get; set; } = 0f;

        //Sprites
        private static Sprite _DeceiveButtonSprite;
        private static Sprite _ConcealButtonSprite;

        public static Sprite GetDeceiveButtonSprite()
        {
            return _DeceiveButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.DecieveButton.png", 115f);
        }

        public static Sprite GetConcealButtonSprite()
        {
            return _ConcealButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ConcealButton.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            DeceiveTimer = 0f;
            MaxDeceiveTimer = CustomOptionHolder.CharlatanDeceiveBaseDuration.GetFloat();
            ConcealCharges = CustomOptionHolder.CharlatanConcealBaseCharges.GetInt();
            ConcealedBodies = new();
            DeadBodies = new();
            ReportTarget = byte.MaxValue;
        }

        public static void GetDescription()
        {
            var timer = CustomOptionHolder.CharlatanDeceiveBaseDuration.GetFloat();
            var charges = CustomOptionHolder.CharlatanConcealBaseCharges.GetInt();
            string concealcolored = Helpers.ColorString(Color.yellow, charges.ToString());
            string concealchargestring = charges == 1 ? concealcolored + " charge" : concealcolored + " charges";

            string gaincolored = Helpers.ColorString(Color.yellow, ConcealChargesPerKill.ToString());
            string gainchargestring = ConcealChargesPerKill == 1 ? gaincolored + " more charge is" : gaincolored + " more charges are";

            string percent = CustomOptionHolder.CharlatanConcealReportRange.GetSelection() == 0 ? "70%" : "50%";
            string help = $"The Charlatan is a role centered around manipulating body reports." +
                $"\n\nThe first ability, Deceive, can be used to a report a body the player has killed regardless of range or line of sight. This ability originally lasts {Helpers.ColorString(Color.yellow, timer.ToString())} seconds after killing, but their next use's duration increases by {Helpers.ColorString(Color.yellow, DeceiveIncPerKill.ToString())} seconds every time they kill." +
                $"\n\nThe second ability, Conceal, will cause all bodies within .25 range of the Charlatan to have their report range reduced by {Helpers.ColorString(Color.yellow, percent.ToString())}. This ability starts with {concealchargestring}, and {gainchargestring} added whenever the Charlatan kills." +
                $"\n\nConceal can only be used near bodies and must be channeled for {Helpers.ColorString(Color.yellow, ConcealChannelDuration.ToString())} seconds. Conceal has a cooldown of {Helpers.ColorString(Color.yellow, Helpers.KillCooldown().ToString())} seconds.";

            RoleInfo.Charlatan.SettingsDescription = RoleInfo.CharlatanNeutralKiller.SettingsDescription = Helpers.WrapText(help);
        }
    }
}
