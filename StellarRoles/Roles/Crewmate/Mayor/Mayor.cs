using UnityEngine;

namespace StellarRoles
{
    public static class Mayor
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(32, 77, 66, byte.MaxValue);
        public static bool CanSeeVoteColorsInMeeting => CustomOptionHolder.MayorTasksNeededToSeeVoteColors.GetBool();
        public static int TasksNeededToSeeVoteColors => CustomOptionHolder.MayorTasksNeededToSeeVoteColors.GetInt();
        public static bool Retired { get; set; }
        public static bool CanRetire => CustomOptionHolder.MayorCanRetire.GetBool();

        private static Sprite _ToolTipSprite;

        public static Sprite ToolTipSprite()
        {
            return _ToolTipSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.MayorToolTip.png", 250f);
        }
        public static void GetDescription()
        {
            string settingDescription =
                $"The {nameof(Mayor)} is a role who always double votes.\n\n" +
                $"The only way the {nameof(Mayor)} can vote normally is by being jailed or voting for a jailed player.\n\n" +
                $"When the {nameof(Mayor)} completes {Helpers.ColorString(Color.yellow, TasksNeededForHelpCard().ToString())} task(s), they are able to see through anonymous voting.";

            if (CanRetire)
            {
                settingDescription += $"\n\nThe {nameof(Mayor)} may choose to sacrifice its extra vote power when it completes tasks. This can be used to attempt to hide the {nameof(Mayor)}'s role from an {nameof(Assassin)}!";
            }

            RoleInfo.Mayor.SettingsDescription = Helpers.WrapText(settingDescription);
        }

        public static void ClearAndReload()
        {
            Player = null;
            Retired = false;
        }

        public static int TasksNeededForVoteColors()
        {
            int shorttasks = GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;
            int longtasks = GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;
            int commontasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks;
            int result = Mathf.Min(shorttasks + longtasks + commontasks, TasksNeededToSeeVoteColors);
            if (Ascended.IsAscended(Player))
            {
                result--;
            }
            return result;
        }

        public static int TasksNeededForHelpCard()
        {
            int shorttasks = GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;
            int longtasks = GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;
            int commontasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks;
            int result = Mathf.Min(shorttasks + longtasks + commontasks, TasksNeededToSeeVoteColors);
            return result;
        }
    }
}
