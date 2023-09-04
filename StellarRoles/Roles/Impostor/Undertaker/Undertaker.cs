using UnityEngine;

namespace StellarRoles
{
    public static class Undertaker
    {
        public static PlayerControl Player { get; set; }
        public static Color Color => IsNeutralKiller ? NeutralKiller.Color : Palette.ImpostorRed;
        public static float DraggingDelayAfterKill => CustomOptionHolder.UndertakerDragingDelaiAfterKill.GetFloat();
        public static float DraggingCooldown => CustomOptionHolder.UndertakerDragCooldown.GetFloat();
        public static DeadBody DeadBodyDragged { get; set; } = null;
        public static DeadBody DeadBodyCurrentTarget { get; set; } = null;
        public static bool CanDragAndVent => CustomOptionHolder.UndertakerCanDragAndVent.GetBool();
        public static bool IsNeutralKiller => CustomOptionHolder.UndertakerIsNeutral.GetBool();

        private static Sprite _DragbuttonSprite;
        private static Sprite _DropbuttonSprite;

        public static void GetDescription()
        {
            string settingsDescription =
                $"The {nameof(Undertaker)} is able to drag bodies in order to hide them in new locations. " +
                $"While dragging a body, the {nameof(Undertaker)} moves 50% slower. Kill cooldown is paused while dragging.";

            if (DraggingDelayAfterKill > 0)
                settingsDescription += $"\n\nKilling will put drag on a {Helpers.ColorString(Color.yellow, DraggingDelayAfterKill.ToString())} second cooldown.";

            if (CanDragAndVent)
                settingsDescription += $"\n\nThe {nameof(Undertaker)} is able to bring dragged bodies with them through vents.";
            else
                settingsDescription += $"\n\nThe {nameof(Undertaker)} cannot vent while dragging a body.";

            RoleInfo.Undertaker.SettingsDescription = RoleInfo.UndertakerNeutralKiller.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static Sprite GetDragButtonSprite()
        {
            return _DragbuttonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.UndertakerDrag.png", 115f);
        }

        public static Sprite GetDropButtonSprite()
        {
            return _DropbuttonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.UndertakerDrop.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            DeadBodyDragged = null;
            DeadBodyCurrentTarget = null;
        }
    }
}
