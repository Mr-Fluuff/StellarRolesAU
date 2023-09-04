using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    public enum RemoteFix
    {
        Off,
        On,
        TaskCompletion
    }

    public static class Engineer
    {
        public static PlayerControl Player { get; set; }
        public static readonly Color Color = new Color32(0, 40, 245, byte.MaxValue);
        private static Sprite _ButtonSprite;
        private static Sprite _VentButtonSprite;

        public static bool HighlightForEvil => CustomOptionHolder.EngineerHighlightForEvil.GetBool();
        public static bool CanVent => CustomOptionHolder.EngineerCanVent.GetBool();
        public static bool AdvancedSabotageRepair => CustomOptionHolder.EngineerAdvancedSabotageRepairs.GetBool();
        public static bool RoleBlock => CustomOptionHolder.EngineerRoleBlock.GetBool() && CustomOptionHolder.CrewRoleBlock.GetBool();

        public static float EngineerVentTimer = 0f;
        public static RemoteFix GetsFix => (RemoteFix)CustomOptionHolder.EngineerHasFix.GetSelection();


        public static bool HasFix { get; set; } = false;
        public static bool GaveFix { get; set; } = false;


        public static void GetDescription()
        {
            string description = $"The {nameof(Engineer)} specializes in fixing sabotages.";
            if (CanVent)
                description += $" The {nameof(Engineer)} can also vent!";

            description += "\n\n";

            if (AdvancedSabotageRepair)
                description +=
                    $"When interacting with a single part of any sabotage, all other parts of the sabotage will be fixed as well. " +
                    $"This works on reactor, O2, 2-part comms, lights, and Polus doors. " +
                    $"This power is disabled when the {nameof(Engineer)} is close to other players to prevent hard clears and easy assassinations.\n\n";

            if (CanVent && HighlightForEvil)
                description += $"While occupying a vent, all vents will be highlighted blue for the {nameof(Engineer)} and all evil killing roles.\n\n";

            if (GetsFix != RemoteFix.Off)
                description += $"Once per game, the {nameof(Engineer)} may fix a sabotage from anywhere using their fix ability. ";

            if (GetsFix == RemoteFix.TaskCompletion)
                description += $"This ability is granted after tasks have been completed";


            RoleInfo.Engineer.SettingsDescription = Helpers.WrapText(description);
        }

        public static bool IsAlone(float distance)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player.Data.IsDead) continue;
                if (player == Player) continue;
                if (PhysicsHelpers.AnythingBetween(player.GetTruePosition(), Player.GetTruePosition(), Constants.ShadowMask, false)) continue;

                if (Vector2.Distance(Player.GetTruePosition(), player.GetTruePosition()) <= distance + 0.01f) return false;
            }

            return true;
        }

        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.RepairButton.png", 115f);
        }

        public static Sprite GetVentButtonSprite()
        {
            return _VentButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.VentButtons.EngineerVentButton.png", 115f);
        }

        public static void ClearAndReload()
        {
            Player = null;
            HasFix = GetsFix == RemoteFix.On;
            GaveFix = false;
        }
    }
}

