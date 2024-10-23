using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    public enum Alignment { Imposter, Crewmate, Invalid };
    /// <summary>
    /// Important!
    /// Change all references of Parity Cop in strings if role name changed
    /// </summary>
    public class ParityCop
    {
        public static readonly Dictionary<byte, ParityCop> ParityCopDictionary = new();
        public static Alignment NeutralAlignment => CustomOptionHolder.ParityCopNeutralsMatchKillers.GetBool() ? Alignment.Imposter : Alignment.Crewmate;
        public static readonly Color Color = new Color32(61, 109, 191, byte.MaxValue);

        private static Sprite _ButtonSprite;
        private static Sprite _FakeCompareSprite;
        private static Sprite _SameAlignmentSprite;
        private static Sprite _DifferentAlignmentSprite;
        private static Sprite _UnknownAlignmentSprite;
        private static Sprite _SameAlignmentToolTipSprite;
        private static Sprite _DifferentAlignmentToolTipSprite;
        private static Sprite _UnknownAlignmentToolTipSprite;
        private static Sprite _FakeCompareToolTipSprite;

        public static bool FakeCompare => CustomOptionHolder.ParityCopFakeCompare.GetBool();
        public static float CompareCooldown => CustomOptionHolder.ParityCopCompareCooldown.GetFloat();
        public static bool RoleBlock => CustomOptionHolder.ParityCopRoleBlock.GetBool();

        public readonly PlayerList ComparedPlayers = new();
        public readonly PlayerControl Player;
        public PlayerControl CurrentTarget;
        public bool PressedFakeCompare = false;
        public bool UsedFakeCompare = false;
        public float FakeCompareCharges = 0f;

        public static void GetDescription()
        {
            string description = $"The Parity Cop can compare players to one another based on whether or not they are able to kill other players.";

            if (CustomOptionHolder.ParityCopNeutralsMatchKillers.GetBool())
                description += " Neutral roles classify as killers on these settings.";

            description +=
                $"\n\nIf players you compare are in a matching group, they will have checkmarks next to their names in meeting. " +
                $"If they are not in a matching group, they will have X's instead. " +
                $"The two most recent players you compare will be selected as long as they survive to the next meeting.\n\n" +
                $"Be careful who you compare! " +
                $"Non-Crewmate players that you compare will see a '?' next to their name and the name of the person you have compared them to when the next meeting begins.\n\n";

            if (FakeCompare)
                description +=
                    "Fake Out allows the Parity Cop to try to fool their compare target at the cost of not getting compare information that round. " +
                    "It places you in a fake-comparison with your most recent compare target!";

            RoleInfo.ParityCop.SettingsDescription = Helpers.WrapText(description);
        }
        public ParityCop(PlayerControl player)
        {
            Player = player;
            PressedFakeCompare = false;
            ParityCopDictionary.Add(player.PlayerId, this);
            if (FakeCompare)
            {
                FakeCompareCharges = 1f;
            }
        }

        public static float CalculateParityCopCooldown(PlayerControl player)
        {
            float result = ParityCop.CompareCooldown;
            if (Ascended.IsAscended(player))
            {
                result = result * .5f;
            }
            return result;
        }


        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.ParityCopCompare.png", 115f);
        }


        public static Sprite GetFakeCompareSprite()
        {
            return _FakeCompareSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.FakeCompare.png", 115f);
        }

        public static Sprite GetPlayersSameAlignmentSprite()
        {
            return _SameAlignmentSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.SameAlignment.png", 150f);
        }

        public static Sprite GetPlayersDifferentAlignmentSprite()
        {
            return _DifferentAlignmentSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.DifferentAlignment.png", 150f);
        }

        public static Sprite GetPlayersUnknownAlignmentSprite()
        {
            return _UnknownAlignmentSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.UnknownAlignment.png", 150f);
        }

        public static Sprite GetPlayersSameAlignmentToolTipSprite()
        {
            return _SameAlignmentToolTipSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.ParityMatchToolTip.png", 350f);
        }

        public static Sprite GetPlayersDifferentAlignmentToolTipSprite()
        {
            return _DifferentAlignmentToolTipSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.ParityDoNotMatchToolTip.png", 350f);
        }

        public static Sprite GetPlayersUnknownAlignmentToolTipSprite()
        {
            return _UnknownAlignmentToolTipSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.ParityEvilToolTip.png", 350f);
        }

        public static Sprite GetPlayersFakeoutToolTipSprite()
        {
            return _FakeCompareToolTipSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.ParityCop.ParityFakeoutToolTip.png", 350f);
        }


        public Sprite RetrieveCompareSprite()
        {
            List<PlayerControl> players = FindPlayersToCompare();

            if (players == null || (players.Count < 1 && !PressedFakeCompare))
                return null;

            else if (PlayerControl.LocalPlayer != Player && !PlayerControl.LocalPlayer.Data.IsDead)
                return GetPlayersUnknownAlignmentSprite();

            else if (PressedFakeCompare)
                return GetPlayersUnknownAlignmentSprite();

            else if (players.Count < 2)
                return null;

            else if (ComparePlayers(players[0], players[1]))
                return GetPlayersSameAlignmentSprite();

            else
                return GetPlayersDifferentAlignmentSprite();
        }

        public Sprite RetrieveCompareToolTipSprite()
        {
            List<PlayerControl> players = FindPlayersToCompare();

            if (players == null || players.Count < 1 && !PressedFakeCompare)
                return null;

            else if (PlayerControl.LocalPlayer != Player && !PlayerControl.LocalPlayer.Data.IsDead)
                return GetPlayersUnknownAlignmentToolTipSprite();

            else if (PressedFakeCompare)
                return GetPlayersFakeoutToolTipSprite();

            else if (players.Count < 2)
                return null;

            else if (ComparePlayers(players[0], players[1]))
                return GetPlayersSameAlignmentToolTipSprite();

            else
                return GetPlayersDifferentAlignmentToolTipSprite();
        }

        public static bool ComparePlayers(PlayerControl target1, PlayerControl target2)
        {
            return !target1.IsParityCop(out _) && !target2.IsParityCop(out _)
                && target1.PlayerAlighnment().Equals(target2.PlayerAlighnment());
        }

        public bool IsValidTarget(PlayerControl target)
        {
            return ComparedPlayers.Count == 0
                || (ComparedPlayers.Count == 1 && ComparedPlayers.GetPlayerAt(ComparedPlayers.Count - 1) != target)
                || (ComparedPlayers.Count >= 2 && ComparedPlayers.GetPlayerAt(ComparedPlayers.Count - 1) != target && ComparedPlayers.GetPlayerAt(ComparedPlayers.Count - 2) != target);
        }


        public List<PlayerControl> FindPlayersToCompare()
        {
            List<PlayerControl> filterList = ComparedPlayers.Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();
            filterList.Reverse();

            int num = PressedFakeCompare ? 1 : 2;
            return filterList.Count < num ? null : filterList.GetRange(0, num);
        }


        public static void ClearParityCopLists()
        {
            foreach (ParityCop parityCop in ParityCopDictionary.Values)
                parityCop.ComparedPlayers.Clear();
        }

        public static bool IsParityCop(byte playerId, out ParityCop parityCop)
        {
            return ParityCopDictionary.TryGetValue(playerId, out parityCop);
        }

        public static void ClearAndReload()
        {
            ParityCopDictionary.Clear();
        }
    }

    public static class ParityCopExtensions
    {
        public static bool IsParityCop(this PlayerControl player, out ParityCop parityCop) => ParityCop.IsParityCop(player.PlayerId, out parityCop);
        public static bool IsParityCop(this PlayerControl player) => ParityCop.IsParityCop(player.PlayerId, out _);

        public static Alignment PlayerAlighnment(this PlayerControl player)
        {
            player.GetRoleDetails(out RoleInfo role, out Faction faction, out RoleId roleId);

            if (faction == Faction.Impostor || faction == Faction.NK || role.IsCrewKilling())
                return Alignment.Imposter;

            if (roleId == RoleId.Romantic)
                return Romantic.IsCrewmate ? Alignment.Crewmate
                    : Romantic.IsImpostor ? Alignment.Imposter
                    : Romantic.NeutralSided ? ParityCop.NeutralAlignment
                    : Alignment.Invalid;

            if (roleId == RoleId.VengefulRomantic)
                return VengefulRomantic.IsCrewmate ? Alignment.Crewmate
                    : VengefulRomantic.IsImpostor ? Alignment.Imposter
                    : Alignment.Invalid;

            if (roleId == RoleId.Refugee)
                return Alignment.Crewmate;

            if (faction == Faction.Neutral)
                return ParityCop.NeutralAlignment;

            return Alignment.Crewmate;
        }
    }
}
