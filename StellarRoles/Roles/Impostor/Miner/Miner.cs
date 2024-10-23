using StellarRoles.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Miner
    {
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;

        public readonly static List<Vent> Vents = new();
        public static PlayerControl Player { get; set; } = null;
        public static int ChargesRemaining { get; set; }
        // TODO: convert into enum usage
        public static bool VisibleInstantly => CustomOptionHolder.MinerVentsDelay.GetSelection() != 0 && CustomOptionHolder.MinerVentsDelay.GetFloat() == 0;
        public static bool VisibleDelay => CustomOptionHolder.MinerVentsDelay.GetBool() && CustomOptionHolder.MinerVentsDelay.GetFloat() > 0;
        public static bool VisibleAfterMeeting => CustomOptionHolder.MinerVentsDelay.GetSelection() == 0;
        public static float Delay => CustomOptionHolder.MinerVentsDelay.GetFloat();
        public static float Cooldown => CustomOptionHolder.MinerCooldown.GetFloat();
        public static bool IsNeutralKiller => CustomOptionHolder.MinerIsNeutral.GetBool();

        private static Sprite _ButtonSprite;


        public static void GetDescription()
        {
            RoleInfo.MinerNeutralKiller.SettingsDescription = RoleInfo.Miner.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Miner)} has {Helpers.ColorString(Color.yellow, CustomOptionHolder.MinerCharges.GetFloat().ToString())} vents they may place to create their own vent system. " +
                $"Placing a vent has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown.") + "\n\n" +
                $"Placed vents are active {(VisibleInstantly ? "immediately." : VisibleAfterMeeting ? "after a meeting has passed." : $"after {Helpers.ColorString(Color.yellow, Delay.ToString())} seconds.")}";
        }

        public static void ClearAndReload()
        {
            Player = null;
            MinerVent.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
            ChargesRemaining = CustomOptionHolder.MinerCharges.GetInt();

            if (Ascended.IsAscended(PlayerControl.LocalPlayer))
            {
                Miner.ChargesRemaining += 2;
            }
        }

        public static Sprite GetMineButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Mine.png", 115f);
        }
    }
}
