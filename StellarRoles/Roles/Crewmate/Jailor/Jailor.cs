using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    public class Jailor
    {
        public static readonly Dictionary<byte, Jailor> PlayerIdToJailor = new();
        public static readonly Color Color = new Color32(135, 135, 135, byte.MaxValue);

        public static float InitialCharges => CustomOptionHolder.InitialJailCharges.GetFloat();
        public static float TasksToRecharge => CustomOptionHolder.JailorTasksPerRecharge.GetFloat();
        public static bool CanJailSelf => CustomOptionHolder.JailorCanJailSelf.GetBool();
        public static bool JailedTargetsGuessedAsJailor => CustomOptionHolder.JailedTargetsGuessed.GetBool() && CanJailSelf;
        private static Sprite _CheckSprite;

        public bool TargetJailedIcons = false;
        public readonly List<GameObject> Bars = new();
        public readonly PlayerControl Player;
        public PlayerControl Target;
        public float Charges;
        public float CurrentRechargeTasks;
        public bool HasJailed;

        public static void GetDescription()
        {
            string description =
                $"The {nameof(Jailor)} is designed to protect players from meeting-based powers. " +
                $"After doing {Helpers.ColorString(Color.yellow, TasksToRecharge.ToString())} task(s), the {nameof(Jailor)} gains a charge of jail to be used in the meeting.\n\n" +
                $"Putting a player in jail will protect them from {nameof(Vigilante)} and {nameof(Assassin)} shots, also extra {nameof(Mayor)} votes. " +
                $"The {nameof(Mayor)} may not use their powers while jailed.\n\n" +
                $"If the {nameof(Vigilante)} or {nameof(Assassin)} are jailed, their menu will change and they will only be allowed to guess the {nameof(Jailor)}. " +
                $"If the {nameof(Jailor)} is killed in meeting, the jail will break.";

            if (JailedTargetsGuessedAsJailor)
                description += $"\n\nJailed targets can be guessed as {nameof(Jailor)}.";

            RoleInfo.Jailor.SettingsDescription = Helpers.WrapText(description);
        }

        public Jailor(PlayerControl playerControl)
        {
            Player = playerControl;
            PlayerIdToJailor.Add(playerControl.PlayerId, this);
            Charges = InitialCharges;
            CurrentRechargeTasks = TasksToRecharge;
            HasJailed = false;
            TargetJailedIcons = false;
        }

        public static Sprite GetCheckSprite()
        {
            return _CheckSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.jailbars.png", 135f);
        }

        public static bool IsJailor(byte playerId, out Jailor jailor)
        {
            return PlayerIdToJailor.TryGetValue(playerId, out jailor);
        }

        public static bool IsJailorTarget(PlayerControl player)
        {
            return PlayerIdToJailor.Any(jailor => jailor.Value.Target == player);
        }

        public static void ClearAndReload()
        {
            PlayerIdToJailor.Clear();
        }
    }

    public static class JailorExtensions
    {
        public static bool IsJailor(this PlayerControl player, out Jailor jailor) => Jailor.IsJailor(player.PlayerId, out jailor);
        public static bool IsJailed(this PlayerControl player) => Jailor.IsJailorTarget(player);

    }
}
