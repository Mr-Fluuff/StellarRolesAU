using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public enum Misfire
    {
        Self,
        Target,
        Both
    }

    public static class Sheriff
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(248, 205, 70, byte.MaxValue);

        public static bool CanKillArsonist => CustomOptionHolder.SheriffCanKillArsonist.GetBool();
        public static bool CanKillJester => CustomOptionHolder.SheriffCanKillJester.GetBool();
        public static bool CanKillExecutioner => CustomOptionHolder.SheriffCanKillExecutioner.GetBool();
        public static bool CanKillScavenger => CustomOptionHolder.SheriffCanKillScavenger.GetBool();
        public static bool SpyCanDieToSheriff => CustomOptionHolder.SpyCanDieToSheriff.GetBool();
        public static Misfire MisfireKills => (Misfire)CustomOptionHolder.SheriffMisfireKills.GetSelection();
        public static bool Haskilled { get; set; } = false;
        public static PlayerControl CurrentTarget { get; set; } = null;

        private static Sprite _KillButtonSprite;

        public static void GetDescription()
        {
            string description =
                $"The {nameof(Sheriff)} is a role whose goal is to directly kill non-crewmates, with the main focus being Impostors and Neutral Killers." +
                $"\n\n{SheriffMisfire(MisfireKills)}\n\n";
            if (SpyCanDieToSheriff || CanKillArsonist || CanKillJester || CanKillExecutioner || CanKillScavenger)
                description += SheriffCanKillString();

            RoleInfo.Sheriff.SettingsDescription = Helpers.WrapText(description);
        }

        public static Sprite GetKillButtonSprite()
        {
            return _KillButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.SheriffGun.png", 115f);
        }

        public static bool CanBeKilledBySheriff(PlayerControl player)
        {
            if (player.Data.Role.IsImpostor)
                return true;
            else if (player == HeadHunter.Player)
                return true;
            else if (NeutralKiller.Players.Contains(player))
                return true;
            else if (player == Spy.Player)
                return SpyCanDieToSheriff;
            else if (player == Arsonist.Player)
                return CanKillArsonist;
            else if (player.IsJester(out _))
                return CanKillJester;
            else if (player == Scavenger.Player)
                return CanKillScavenger;
            else if (player == Executioner.Player)
                return CanKillExecutioner;
            else if (player == Romantic.Player)
                return Romantic.CanBeKilledBySheriff();
            else if (player == VengefulRomantic.Player)
                return VengefulRomantic.CanBeKilledBySheriff();
            else if (player.IsPyromaniac(out _))
                return true;
            else
                return false;
        }

        private static string SheriffMisfire(Misfire kills)
        {
            return kills switch
            {
                Misfire.Self => $"Killing the wrong player will cause a misfire, killing the {nameof(Sheriff)} instead. Choose wisely!",
                Misfire.Target => $"The {nameof(Sheriff)} may kill one player per game with no repercussions on these settings. Choose wisely!",
                _ => "Killing the wrong player will cause a misfire, killing both players. Choose wisely!",
            };
        }

        private static string SheriffCanKillString()
        {
            List<string> roles = new();
            if (CanKillArsonist)
                roles.Add(nameof(Arsonist));
            if (CanKillJester)
                roles.Add(nameof(Jester));
            if (CanKillExecutioner)
                roles.Add(nameof(Executioner));
            if (CanKillScavenger)
                roles.Add(nameof(Scavenger));

            string description = roles.Count > 0
                ? $"The {nameof(Sheriff)} may only kill these neutral roles: {string.Join(", ", roles)}."
                : "";

            if (SpyCanDieToSheriff)
                description += $"{(description.Length > 0 ? "\n\n" : "")}The {nameof(Sheriff)} can also kill the {nameof(Spy)}.";

            return description;
        }

        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            Haskilled = false;
        }
    }
}
