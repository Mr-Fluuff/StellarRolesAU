using StellarRoles.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Cultist
    {
        public static PlayerControl Player { get; set; }
        public static readonly Color Color = Palette.ImpostorRed;
        public static bool NeedsFollower { get; set; } = true;
        public static bool FollowerImpRolesEnabled = false;
        public static bool FollowerSpecialRoleAssigned = false;
        public static PlayerControl CurrentFollower { get; set; }
        public static readonly List<Arrow> LocalArrows = new();

        private static Sprite _ButtonSprite;

        public static Sprite GetRecruitButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Recruit.png", 115f);
        }

        public static RoleId CultistFollowerRole(RoleId roleId)
        {
            RoleId newRole = RoleId.Follower;
            switch (roleId)
            {
                case RoleId.Administrator:
                case RoleId.Medic:
                case RoleId.Watcher:
                    newRole = RoleId.Hacker;
                    break;
                case RoleId.Detective:
                case RoleId.Jailor:
                    if (!Undertaker.IsNeutralKiller)
                        newRole = RoleId.Undertaker;
                    break;
                case RoleId.Trapper:
                case RoleId.Engineer:
                    if (!Miner.IsNeutralKiller)
                        newRole = RoleId.Miner;
                    break;
                case RoleId.Guardian:
                    if (!Vampire.IsNeutralKiller)
                        newRole = RoleId.Vampire;
                    break;
                case RoleId.Investigator:
                    if (!Camouflager.IsNeutralKiller)
                        newRole = RoleId.Camouflager;
                    break;
                case RoleId.Mayor:
                    if (!Morphling.IsNeutralKiller)
                        newRole = RoleId.Morphling;
                    break;
                case RoleId.ParityCop:
                    if (!Wraith.IsNeutralKiller)
                        newRole = RoleId.Wraith;
                    break;
                case RoleId.Psychic:
                    if (!Warlock.IsNeutralKiller)
                        newRole = RoleId.Warlock;
                    break;
                case RoleId.Tracker:
                case RoleId.HeadHunter:
                    if (!BountyHunter.IsNeutralKiller)
                        newRole = RoleId.BountyHunter;
                    break;
                case RoleId.Arsonist:
                case RoleId.Pyromaniac:
                    if (!Bomber.IsNeutralKiller)
                        newRole = RoleId.Bomber;
                    break;
                case RoleId.Nightmare:
                    if (!Shade.IsNeutralKiller)
                        newRole = RoleId.Shade;
                    break;
                case RoleId.Sheriff:
                case RoleId.Executioner:
                case RoleId.Jester:
                        newRole = RoleId.Changeling;
                    break;

            }
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
            FollowerImpRolesEnabled = CustomOptionHolder.CultistSpecialRolesEnabled.GetBool();
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
