using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    public static class Trapper
    {
        public static readonly Color Color = new Color32(30, 140, 30, byte.MaxValue);

        public static PlayerControl Player { get; set; }
        public static float TrapCount { get; set; }
        public static float CoverCount { get; set; }
        public static float TrapCooldown => CustomOptionHolder.TrapperTrapCoverCooldown.GetFloat();
        public static float CoverCooldown => CustomOptionHolder.TrapperTrapCoverCooldown.GetFloat();
        public static float TrapRootDuration => CustomOptionHolder.TrapperRootDuration.GetFloat();
        public static bool RoleBlock => CustomOptionHolder.TrapperRoleBlock.GetBool() && CustomOptionHolder.CrewRoleBlock.GetBool();
        public static Vent VentTarget { get; set; } = null;

        private static Sprite _CloseVentButtonSprite;
        private static Sprite _AnimatedVentSealedSprite;
        private static Sprite _VentBoardButtonSprite;
        private static Sprite _StaticVentSealedSprite;
        private static Sprite _SubmergedCentralUpperVentSealedSprite;
        private static Sprite _SubmergedCentralLowerVentSealedSprite;
        private static float LastPPU;

        public static void GetDescription()
        {
            RoleInfo.Trapper.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Trapper)} is a role that can choose to cover up or trap a vent.\n\n" +
                $"When covering a vent, the cover will activate after the next meeting has been called. " +
                $"Covering restricts players from using that vent, but they may still travel through it.\n\n" +
                $"When trapping a vent, the trap is armed instantly but can only be seen by venting roles who spend at least 3 seconds in close proximity to it. " +
                $"Players who attempt to use a trapped vent will be frozen in place for {Helpers.ColorString(Color.yellow, TrapRootDuration.ToString())} seconds and will alert the {nameof(Trapper)} with a flash.");
        }

        public static Sprite GetCloseVentButtonSprite()
        {
            return _CloseVentButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.settrap.png", 115f);
        }

        public static Sprite GetVentBoardButton()
        {
            return _VentBoardButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.VentBoardsButton.png", 100f);
        }

        public static Sprite GetAnimatedVentSealedSprite()
        {
            float ppu = 185f;
            if (SubmergedCompatibility.IsSubmerged) ppu = 120f;
            if (LastPPU != ppu)
            {
                _AnimatedVentSealedSprite = null;
                LastPPU = ppu;
            }
            return _AnimatedVentSealedSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.AnimatedVentSealed.png", ppu);
        }

        public static Sprite GetStaticVentSealedSprite()
        {
            return _StaticVentSealedSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.vent_cover.png", 160f);
        }

        public static Sprite GetSubmergedCentralUpperSealedSprite()
        {
            return _SubmergedCentralUpperVentSealedSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.CentralUpperBlocked.png", 145f);
        }

        public static Sprite GetSubmergedCentralLowerSealedSprite()
        {
            return _SubmergedCentralLowerVentSealedSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.CentralLowerBlocked.png", 145f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            TrapCount = CustomOptionHolder.TrapperNumberOfTraps.GetFloat();
            CoverCount = CustomOptionHolder.TrapperNumberOfCovers.GetFloat();
            VentTrap.ClearAllVentTraps();
        }
    }
}
