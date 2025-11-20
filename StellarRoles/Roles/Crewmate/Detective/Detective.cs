using Cpp2IL.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    public static class Detective
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(35, 52, 81, byte.MaxValue);


        public const float Cooldown = 3f;

        public static float InspectsPerRound { get; set; }
        public static float Duration => CustomOptionHolder.DetectiveInspectDuration.GetFloat();
        public static bool IsCrimeSceneEnabled => CustomOptionHolder.DetectiveEnableCrimeScenes.GetBool();
        public static float InspectsPerRoundDefault => CustomOptionHolder.DetectiveInspectsPerRound.GetFloat();
        public static bool RoleBlock => CustomOptionHolder.DetectiveRoleBlock.GetBool();
        private static Sprite _CrimeSceneSprite;


        public static DeadPlayer Target { get; set; } = null;
        public static bool TargetIsFresh { get; set; }
        public static DeadPlayer InspectTarget { get; set; }

        public static DateTime MeetingStartTime { get; set; } = DateTime.UtcNow;
        public static readonly List<SpriteRenderer> CrimeScenes = new();

        public static readonly List<DeadPlayer> OldDeadBodies = new();
        public static readonly List<DeadPlayer> FreshDeadBodies = new();

        //QuestionInfo
        public static readonly Dictionary<byte, PlayerList> KillersLinkToKills = new();
        public static readonly Dictionary<byte, int> PlayerIdToKillerCountQuestion = new();
        public static readonly Dictionary<byte, string> PlayerIdToKillerKilledQuestion = new();
        public static readonly PlayerList PlayersKilledByAbility = new();
        public static readonly Dictionary<byte, KillerEscapeByVent> KillerEscapeRoute = new();
        public static readonly Dictionary<byte, KillerDirection> KillerEscapeDirection = new();
        public static readonly Dictionary<byte, List<int>> QuestionsForPlayer = new();

        //Inspecting Info
        public static readonly Dictionary<byte, int> BodyToInspects = new();
        public static readonly Dictionary<byte, int> CrimeSceneToInspect = new();

        public static void GetDescription()
        {
            string description =
                $"When pressing Inspect while near a dead body, extra information about the murder will be added to the {nameof(Detective)}'s chat. " +
                $"Inspections have a {Helpers.ColorString(Color.yellow, Duration.ToString())} second duration and a 3 second cooldown.\n\n";

            if (IsCrimeSceneEnabled)
                description +=
                    $"After one meeting has past, the {nameof(Detective)} can return to the place where a player was killed to inspect a crime scene (depicted by a pool of blood). " +
                    $"Information gathered from a body is more likely to have a high-impact on the game than information gathered from a crime scene.";

            RoleInfo.Detective.SettingsDescription = Helpers.WrapText(description);
        }

        public static float CalculateDetectiveDuration()
        {
            float result = Detective.Duration;
            if (Ascended.IsAscended(Detective.Player))
            {
                result = result * .70f;
            }
            return result;
        }

        public static Sprite GetCrimeSceneSprite()
        {
            return _CrimeSceneSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.blood.png", 200f);
        }

        private static Sprite _QuestionSprite;
        public static Sprite GetQuestionSprite()
        {
            return _QuestionSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.inspect.png", 115f);
        }

        public static string DetectiveQuestion()
        {
            GenerateInformationOrder();
            if (!QuestionsForPlayer.TryGetValue(Target.Player.PlayerId, out List<int> questions))
                return "Something has gone horribly horribly wrong.";
            else if (questions.Count == 0)
                return "I have no more information to give you. When in a mix, vote Kwix.";
            else
                return GetDetectiveAnswer(questions.RemoveAndReturn(0));
        }

        public static string GetDetectiveAnswer(int questionNumber)
        {
            bool killedByAbility = PlayersKilledByAbility.Contains(Target.Player);
            string killedByAbilitystring = killedByAbility ? "Cause of Death: Ability" : "Cause of Death: Normal Kill";
            float timeSinceDeathMeeting = (float)(MeetingStartTime - Target.TimeOfDeath).TotalMilliseconds;
            string deathMeeting = $"{Math.Round(timeSinceDeathMeeting / 1000)}s before meeting started";
            timeSinceDeathMeeting = (float)Math.Round(timeSinceDeathMeeting / 1000);
            float timeSinceDeath = (float)Math.Round((DateTime.UtcNow - Target.TimeOfDeath).TotalMilliseconds / 1000);
            bool isCorpse = IsCorpse(Target.Player.PlayerId);
            bool fresh = isCorpse ? timeSinceDeath <= 10 : timeSinceDeathMeeting <= 10;
            bool older = isCorpse ? timeSinceDeath >= 10 && timeSinceDeath <= 20 : timeSinceDeathMeeting >= 10 && timeSinceDeathMeeting <= 20;
            bool dead = isCorpse ? timeSinceDeath >= 20 : timeSinceDeathMeeting >= 20;
            string name = $"({Target.Player.Data.PlayerName})";
            switch (questionNumber)
            {
                case 0:
                    if (IsCorpse(Target.Player.PlayerId))
                    {
                        if (fresh)
                            return $"This player was killed very recently!\n{CorpseTime()}\n{name}.";
                        else if (older)
                            return $"This player's wounds are still somewhat fresh!\n{CorpseTime()}\n{name}.";
                        else if (dead)
                            return $"This player's injuries are not fresh, they seem to have been lost or forgotten?\n{CorpseTime()}\n{name}.";
                    }
                    else
                    {
                        if (fresh)
                            return $"The crime scene was still fresh! The killer couldn't have gotten far... right?\n{deathMeeting}\n{name}.";
                        else if (older)
                            return $"The crime scene has aged a little, but not too long to get information!\n{deathMeeting}\n{name}.";
                        else if (dead)
                            return $"The crime scene has aged a lot!\n{deathMeeting}\n{name}.";
                    }
                    return "The answer is 42";
                case 1:
                    {
                        bool rnd = StellarRoles.rnd.Next(2) == 0;
                        string ability;
                        if (killedByAbility)
                            ability = rnd
                                ? "This player was not killed by a normal weapon."
                                : "It is hard to tell what exactly caused this player's death, but it definitely was not normal!";
                        else
                            ability = rnd
                                ? "The player's wounds look pretty gruesome, but nothing unusual to see here."
                                : "Just a simple kill, nothing crazy or magical about it.";

                        return $"{ability}\n{killedByAbilitystring}\n{name}";
                    }
                case 2:
                    {
                        bool isImpostor = Target.Player.Data.Role.IsImpostor;
                        bool isNeutral = Helpers.IsNeutral(Target.Player);
                        bool IsNeutralKiller = Helpers.IsNeutralKiller(Target.Player);

                        string start = $"It looks like {Target.Player.Data.PlayerName} was the {RoleInfo.GetRolesString(Target.Player, false)}.\n";
                        if (!isImpostor && (!isNeutral || Target.Player.IsRefugee(out _)))
                            return $"{start}They will be missed.";
                        else if (isImpostor || IsNeutralKiller)
                            return $"{start}I feel a little safer!";
                        else if (isNeutral && !IsNeutralKiller)
                            return $"{start}We are better off without that in our way!";

                        return $"{Target.Player.Data.PlayerName} Does not have a role.";
                    }
                case 3:
                    return $"{WhatIsKillersAlighment(Target.Player.PlayerId)}\n{name}.";
                case 4:
                    if (IsCorpse(Target.Player.PlayerId))
                    {
                        return $"This body has been tampered with!\n(Decayed, Dragged, or Concealed)\n{name}";
                    }
                    else return $"This crime scene has been tampered with!\n(Decayed, Dragged, or Concealed)\n{name}";
                case 5:
                    return $"{FindOtherPlayerQuestion(Target.Player.PlayerId)}\n{name}.";
                case 6:
                    return $"{HowManyPlayersKillerKilledQuestion(Target.Player.PlayerId)}\n{name}.";
                case 7:
                    {
                        int rnd = StellarRoles.rnd.Next(3);
                        if (rnd == 0)
                            return $"It looks like the killer traveled to {KillerEscapeDirection[Target.Player.PlayerId].GetDirection()}.\n{name}.";
                        else if (rnd == 1)
                            return $"The suspect's trail went to {KillerEscapeDirection[Target.Player.PlayerId].GetDirection()} after killing!\n{name}.";
                        else
                            return $"It looks like the killer made an escape to {KillerEscapeDirection[Target.Player.PlayerId].GetDirection()}.\n{name}.";
                    }
                case 8:
                    return $"{DidKillerUseVent(Target.Player.PlayerId)}\n{name}.";
                default:
                    return "The answer is 42.";
            }

        }

        public static void GenerateInformationOrder()
        {
            if (QuestionsForPlayer.ContainsKey(Target.Player.PlayerId)) return;

            QuestionsForPlayer[Target.Player.PlayerId] = QuestionOrderCalculation();
        }

        public static List<int> QuestionOrderCalculation()
        {
            List<int> questionOrder = new();
            List<int> groupA = new() { 0, 1, 2, 3};
            int a = 0;
            int b = 0;
            if (GameHistory.DeadPlayers.Any(p => p.Data.PlayerId == Target.Data.PlayerId && p.Tampered)) 
            {
                groupA.Add(4);
            }
            List<int> groupB = new() { 5, 6, 7, 8 };
            if (TargetIsFresh || Ascended.IsAscended(Player))
            {
                while (a < 2 || b < 2)
                {
                    int achance = (a < 2) ? b == 2 ? 0 : 30 : 101;
                    if (Helpers.TrueRandom(1, 100) >= achance)
                    {
                        AddRandomElementFromGroup(questionOrder, groupA);
                        a++;
                    }
                    else
                    {
                        AddRandomElementFromGroup(questionOrder, groupB);
                        b++;
                    }
                }
            }
            else
            {
                while (a < 1 || b < 3)
                {
                    int bchance = (a < 1) ? b == 3 ? 0 : 70 : 101;
                    if (Helpers.TrueRandom(1, 100) >= bchance)
                    {
                        AddRandomElementFromGroup(questionOrder, groupA);
                        a++;
                    }
                    else
                    {
                        AddRandomElementFromGroup(questionOrder, groupB);
                        b++;
                    }
                }
            }
            return questionOrder;
        }

        private static void AddRandomElementFromGroup(List<int> questions, List<int> pool)
        {
            questions.Add(pool.RemoveAndReturn(StellarRoles.rnd.Next(pool.Count)));
        }

        public static string DidKillerUseVent(byte playerId)
        {
            bool rnd = StellarRoles.rnd.Next(2) == 0;
            if (KillerEscapeRoute.TryGetValue(playerId, out KillerEscapeByVent killerDirection))
            {
                if (killerDirection.UsedVent)
                    return rnd ? "The killer used a vent for a quick getaway!" : "The killer escaped through the vents!";
                else
                    return rnd
                        ? "The nearest vents don't appear to be disturbed."
                        : "The nearest vents are untouched.\nThey must have escaped another way!";
            }

            return "The killer entered the Shadow Realm.";
        }

        public static bool IsCorpse(byte playerId)
        {
            return FreshDeadBodies.Any(body => body.Player.PlayerId == playerId);
        }

        public static string CorpseTime()
        {
            // TODO: does this need to be converted into a float.
            return $"They have died {Math.Round(((float)(DateTime.UtcNow - Target.TimeOfDeath).TotalMilliseconds) / 1000)}s ago.";
        }

        public static string FindOtherPlayerQuestion(byte playerId)
        {
            bool rnd = StellarRoles.rnd.Next(2) == 0;
            string name = "";

            if (TargetIsFresh)
                name = FindOtherPlayer(playerId);
            else if (PlayerIdToKillerKilledQuestion.TryGetValue(playerId, out string nme))
                name = nme;

            if (name != "")
                return rnd
                    ? $"The wounds on this player are very similar to the wounds on {name}."
                    : $"This player died to the same killer as {name}.";
            else
                return rnd
                    ? "No one else seems to have been killed by this player's murderer."
                    : "This killer's handiwork is not something you have seen before.";
        }

        public static string FindOtherPlayer(byte playerId)
        {
            PlayerList list = [];
            foreach (PlayerList l in KillersLinkToKills.Values)
            {
                if (l.Contains(playerId))
                {
                    list.AddRange(l);
                }
            }
            if (list.Remove(playerId))
                return list.Count != 0 ? list.GetPlayerAt(StellarRoles.rnd.Next(list.Count)).Data.PlayerName : "";

            return "";

        }

        public static string HowManyPlayersKillerKilledQuestion(byte playerId)
        {
            bool rnd = StellarRoles.rnd.Next(2) == 0;
            if (TargetIsFresh)
            {
                int killCount = HowManyPlayersHasKillerKilled(playerId);
                return rnd ? $"This murderer has killed {killCount} players." : $"This player's killer has murdered {killCount} players.";
            }
            else if (PlayerIdToKillerCountQuestion.TryGetValue(playerId, out int killCount))
                return rnd
                    ? $"This murderer has killed {killCount} people prior to the end of last round."
                    : $"This player's killer has murdered {killCount} players prior to the end of last round.";
            else
                return "The people on the death star were innocent.";
        }

        public static int HowManyPlayersHasKillerKilled(byte playerId)
        {
            int count = 0;
            foreach (PlayerList list in KillersLinkToKills.Values)
            {
                if (list.Contains(playerId))
                    count += list.Count;
            }

/*            if (PlayerIdToKillerCountQuestion.TryGetValue(playerId, out int killCount))
            {
                count += killCount;
            }*/

            return count;
        }


        public static string WhatIsKillersAlighment(byte playerId)
        {
            PlayerControl killer = Helpers.PlayerById(KillersLinkToKills.First(pair => pair.Value.Contains(playerId)).Key);
            bool rnd = StellarRoles.rnd.Next(2) == 0;

            if (Helpers.IsNeutral(killer))
                return rnd
                    ? "This killer works alone.\nKiller Alignment: Neutral Killer"
                    : "This killer works for no one.\nKiller Alignment: Neutral Killer";
            else if (killer.Data.Role.IsImpostor)
                return rnd
                    ? "The wounds on this body appear alien.\nKiller Alignment: Impostor"
                    : "Just another classic murder scene here.\nKiller Alignment: Impostor";
            else
                return rnd
                    ? "An act of justice was made here.\nKiller Alignment: Crewmate"
                    : "Whoever killed this may have done the right thing!\nKiller Alignment: Crewmate";
        }

        public static void ClearAndReload()
        {
            PlayersKilledByAbility.Clear();
            Player = null;
            Target = null;
            InspectTarget = null;
            OldDeadBodies.Clear();
            FreshDeadBodies.Clear();
            CrimeScenes.Clear();
            MeetingStartTime = DateTime.UtcNow;
            KillersLinkToKills.Clear();
            InspectsPerRound = InspectsPerRoundDefault;
            PlayersKilledByAbility.Clear();
            CrimeScenes.Clear();
            MeetingStartTime = DateTime.UtcNow;
            KillerEscapeRoute.Clear();
            KillerEscapeDirection.Clear();
            TargetIsFresh = false;
            QuestionsForPlayer.Clear();
            BodyToInspects.Clear();
            CrimeSceneToInspect.Clear();
            PlayerIdToKillerCountQuestion.Clear();
            PlayerIdToKillerKilledQuestion.Clear();
        }
    }

    public class KillerDirection
    {
        private Vector2 KillerStartingLocation;
        private Vector2 KillerEndingLocation;
        public KillerDirection(Vector2 killerStartingLocation, Vector2 killerEndingLocation)
        {
            KillerStartingLocation = killerStartingLocation;
            KillerEndingLocation = killerEndingLocation;
        }


        public string GetDirection()
        {
            float y = KillerEndingLocation.y - KillerStartingLocation.y;
            float x = KillerEndingLocation.x - KillerStartingLocation.x;
            if (x == 0 && y == 0)
                return "an Unkown Location";
            else if (Math.Abs(x) > Math.Abs(y))
                return x >= 0 ? "the East" : "the West";
            else
                return y >= 0 ? "the North" : "the South";
        }
    }

    public class KillerEscapeByVent
    {
        public readonly PlayerControl Venter;
        public float Timer = 5f;
        public bool UsedVent = false;
        public KillerEscapeByVent(PlayerControl venter)
        {
            Venter = venter;
        }
    }
}
