using StellarRoles.Objects;
using StellarRoles.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Cultist
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = Palette.ImpostorRed;
        public static bool NeedsFollower { get; set; } = true;
        public static bool FollowerImpRolesEnabled => CustomOptionHolder.CultistSpecialRolesEnabled.GetBool();
        public static bool FollowerSpecialRoleAssigned = false;
        public static bool CultistMeetingChat => CustomOptionHolder.CultistChatInMeeting.GetBool();
        public static PlayerControl CurrentFollower { get; set; } = null;
        public static readonly List<Arrow> LocalArrows = new();

        private static Sprite _ButtonSprite;

        public static Sprite GetRecruitButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Recruit.png", 115f);
        }

        public static RoleId CultistFollowerRole(RoleId roleId)
        {
            RoleId newRole = RoleId.Follower;
            var roleinfo = RoleManagerSelectRolesPatch.GetRoleAssignmentData();
            switch (roleId)
            {
                case RoleId.Administrator:
                case RoleId.Medic:
                case RoleId.Watcher:
                    newRole = RoleId.Hacker;
                    break;
                case RoleId.Detective:
                    newRole = RoleId.Undertaker;
                    break;
                case RoleId.Jailor:
                    newRole = RoleId.Parasite;
                    break;
                case RoleId.Trapper:
                case RoleId.Engineer:
                    newRole = RoleId.Miner;
                    break;
                case RoleId.Guardian:
                    newRole = RoleId.Vampire;
                    break;
                case RoleId.Investigator:
                    newRole = RoleId.Camouflager;
                    break;
                case RoleId.Mayor:
                    newRole = RoleId.Morphling;
                    break;
                case RoleId.ParityCop:
                    newRole = RoleId.Wraith;
                    break;
                case RoleId.Psychic:
                    newRole = RoleId.Warlock;
                    break;
                case RoleId.Arsonist:
                case RoleId.Pyromaniac:
                    if (!Bomber.IsNeutralKiller)
                        newRole = RoleId.Bomber;
                    break;
                case RoleId.Nightmare:
                    newRole = RoleId.Shade;
                    break;
                case RoleId.Sheriff:
                case RoleId.Executioner:
                case RoleId.Jester:
                    newRole = RoleId.Changeling;
                    break;
                default:
                    newRole = RoleId.Follower;
                    break;
            }
            if (!roleinfo.ImpSettings.ContainsKey(newRole) || roleinfo.ImpSettings[newRole] == 0)
                newRole = RoleId.Follower;
            return newRole;
        }


        public static void ClearAndReload()
        {
            foreach (Arrow arrow in LocalArrows)
                Object.Destroy(arrow.Object);

            LocalArrows.Clear();

            Arrow arrow1 = new(Palette.ImpostorRed);
            arrow1.Object.SetActive(false);
            Arrow arrow2 = new(Palette.ImpostorRed);
            arrow2.Object.SetActive(false);

            LocalArrows.Add(arrow1);
            LocalArrows.Add(arrow2);
            Player = null;
            CurrentFollower = null;
            NeedsFollower = true;
            FollowerSpecialRoleAssigned = false;
        }

        public static void GetDescription()
        {
            RoleInfo.Cultist.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Cultist)} is a player who spawns as a solo impostor with no kill button. " +
                $"The {nameof(Cultist)} must use its convert ability to choose its impostor partner, the {nameof(Follower)}, to get its kill button.\n\n" +
                $"The {nameof(Follower)} cannot be killed in a meeting by the {nameof(Vigilante)}.\n\n" +
                $"The {nameof(Cultist)} and {nameof(Follower)} have arrows pointing to one another and are alerted when the other kills with a red flash on their screen. " +
                $"The {nameof(Cultist)} and {nameof(Follower)} can mid round chat with one another.");
        }
    }
}
