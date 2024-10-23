using System.Linq;
using UnityEngine;

namespace StellarRoles
{
    public class HelpInfo
    {
        private static Sprite menuBackground;
        private static Sprite __HelpMenuBackground;
        private static Sprite factionBackground;

        public static string[] CrewmateFunnyLines = new string[] { "Sucks To Suck",
            "You are the chosen one.",
            "Imagine being vanilla crew.",
            "lmao",
            "404 Role Not Found",
            "L plus ratio",
            Helpers.WrapText("Live Long and Prosper -Spock"),
            "Unlock role DLC for $4.99",
            "Well we can’t ALL be special",
        "Maybe next time....",
        "Unlock role DLC for $4.99",
        "Any Cultists?",
        "\"This is Fine...\"",
        "L Rizz.",
        Helpers.WrapText("The Vanilla Crewmate is a role that can summon the Ender Dragon if their tasks are left incomplete for 20 rounds"),
        "\"Mom can I have a role?\" \"We have roles at home.\"",
        "\"What do you call a fish wearing a bowtie?\" \"Sofishticated.\"",
        Helpers.WrapText("\"I once got fired from a canned juice company. Apparently I couldn't concentrate.\""),
        Helpers.WrapText("\"I made a pencil with two erasers. It was pointless.\""),
         Helpers.WrapText("\"What does garlic do when it gets hot?\" \"It takes its cloves off.\""),
            Helpers.WrapText("In every generation there is a chosen one. She alone will stand against the vampires the demons and the forces of darkness. She is the slayer."),
            Helpers.WrapText($"\"Why did the scarecrow win an award? Because he was outstanding in his field.\""),
            Helpers.WrapText("“Heroes are made by the paths they choose, not the power they are graced with.” –Brodi Ashton"),
            Helpers.WrapText("“A hero is an ordinary individual who finds the strength to persevere and endure in spite of overwhelming obstacles.” —Christopher Reeve"),
            Helpers.WrapText("The FIRST ORDER reigns. Having decimated the peaceful Republic, Supreme Leader Snoke now deploys his merciless legions to seize military control of the galaxy.") +"\n\n"
            + Helpers.WrapText("Only General Leia Organa's band of RESISTANCE fighters stand against the rising tyranny, certain that Jedi Master Luke Skywalker will return and restore a spark of hope to the fight.")
            +"\n\n"
            +Helpers.WrapText("But the Resistance has been exposed. As the First Order speeds toward the rebel base, the brave heroes mount a desperate escape....")
         +"\n-Star Wars",
        Helpers.WrapText("In a fight? Here is what you do: in a low voice begin to say wolowolowolowolowolo slowly increasing in volume. Begin to sway side to side. You should be pretty loud and your opponent will have stepped back and appear visibly shaken. Let your eyes roll to the back of your head. By now, you're chanting WOLOWOLOWOLOWO at the top of your lungs. They will run away. Everyone within a one mile radius will feel a terrifying presence within their soul. Ascend into your planar form."),

         Helpers.WrapText(@"Whenever I get a package of plain M&Ms, I make it my duty to continue the strength and robustness of the candy as a species. To this end, I hold M&M duels. Taking two candies between my thumb and forefinger, I apply pressure, squeezing them together until one of them cracks and splinters. That is the loser, and I eat the inferior one immediately. The winner gets to go another round. I have found that, in general, the brown and red M&Ms are tougher, and the newer blue ones are genetically weaker."),
         Helpers.WrapText($"\"Why can't the crewmates and the imposters just get along\" -Drunk Stell 2023"),
         Helpers.WrapText($"According to all known laws of aviation, there is no way a bee should be able to fly.") +"\n\n" + Helpers.WrapText("Its wings are too small to get its fat little body off the ground.")
            + "\n\n" + Helpers.WrapText("The bee, of course, flies anyway because bees don't care what humans think is impossible.") +
            "\n\n-The Bee Movie",
         "3/10",
         Helpers.WrapText("Neapolitan is made of chocolate, strawberry, and disappointment."),
         "DMCA WARNING!" + "\n" + Helpers.WrapText("Watch Stell Break Dance: https://youtu.be/dQw4w9WgXcQ"),
         "Very\nAnnoyed\nNon\nImpostor\nLooking\nListless\nAgain"
        };

        public static Sprite helpButtonSprite(PlayerControl player)
        {
            RoleId roleId = RoleInfo.GetRoleInfoForPlayer(player, false).Where(x => x.FactionId != Faction.Modifier).FirstOrDefault().RoleId;
            return helpButtonGetSprite(roleId);
        }

        public static Sprite getMenuBackground()
        {
            return menuBackground ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Screen.png", 110f);
        }
        public static Sprite getFactionBackground()
        {
            return factionBackground ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.FactionScreen.png", 110f);
        }

        public static Sprite getHelpMenuBackground()
        {
            return __HelpMenuBackground ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.HelpScreen.png", 100f);
        }

        public static Sprite helpButtonGetSprite(RoleId roleId)
        {
            Sprite helpSprite;
            switch (roleId)
            {
                case RoleId.Administrator:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenAdministrator.png", 100f);
                    break;
                case RoleId.Detective:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenDetective.png", 100f);
                    break;
                case RoleId.Engineer:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenEngineer.png", 100f);
                    break;
                case RoleId.Guardian:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenGuardian.png", 100f);
                    break;
                case RoleId.Investigator:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenInvestigator.png", 100f);
                    break;
                case RoleId.Jailor:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenJailor.png", 100f);
                    break;
                case RoleId.Mayor:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenMayor.png", 100f);
                    break;
                case RoleId.Medic:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenMedic.png", 100f);
                    break;
                case RoleId.ParityCop:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenParityCop.png", 100f);
                    break;
                case RoleId.Psychic:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenPsychic.png", 100f);
                    break;
                case RoleId.Sheriff:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenSheriff.png", 100f);
                    break;
                case RoleId.Spy:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenSpy.png", 100f);
                    break;
                case RoleId.Tracker:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenTracker.png", 100f);
                    break;
                case RoleId.Trapper:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenTrapper.png", 100f);
                    break;
                case RoleId.Watcher:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenWatcher.png", 100f);
                    break;
                case RoleId.Bomber:
                case RoleId.BomberNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenBomber.png", 100f);
                    break;
                case RoleId.Camouflager:
                case RoleId.CamouflagerNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenCamouflager.png", 100f);
                    break;
                case RoleId.Changeling:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenChangeling.png", 100f);
                    break;
                case RoleId.Cultist:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenCultist.png", 100f);
                    break;
                case RoleId.Follower:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenFollower.png", 100f);
                    break;
                case RoleId.Janitor:
                case RoleId.JanitorNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenJanitor.png", 100f);
                    break;
                case RoleId.Miner:
                case RoleId.MinerNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenMiner.png", 100f);
                    break;
                case RoleId.Morphling:
                case RoleId.MorphlingNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenMorphling.png", 100f);
                    break;
                case RoleId.Shade:
                case RoleId.ShadeNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenShade.png", 100f);
                    break;
                case RoleId.Undertaker:
                case RoleId.UndertakerNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenUndertaker.png", 100f);
                    break;
                case RoleId.Vampire:
                case RoleId.VampireNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenVampire.png", 100f);
                    break;
                case RoleId.Warlock:
                case RoleId.WarlockNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenWarlock.png", 100f);
                    break;
                case RoleId.Wraith:
                case RoleId.WraithNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenWraith.png", 100f);
                    break;
                case RoleId.Arsonist:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Neutral.ScreenArsonist.png", 100f);
                    break;
                case RoleId.Scavenger:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Neutral.ScreenScavenger.png", 100f);
                    break;
                case RoleId.Executioner:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Neutral.ScreenExecutioner.png", 100f);
                    break;
                case RoleId.Jester:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Neutral.ScreenJester.png", 100f);
                    break;
                case RoleId.Refugee:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Neutral.ScreenRefugee.png", 100f);
                    break;
                case RoleId.Romantic:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Neutral.ScreenRomantic.png", 100f);
                    break;
                case RoleId.VengefulRomantic:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Neutral.ScreenVengefulRomantic.png", 100f);
                    break;
                case RoleId.HeadHunter:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.NK.ScreenHeadhunter.png", 100f);
                    break;
                case RoleId.Nightmare:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.NK.ScreenNightmare.png", 100f);
                    break;
                case RoleId.Pyromaniac:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.NK.ScreenPyromaniac.png", 100f);
                    break;
                case RoleId.RuthlessRomantic:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.NK.ScreenRuthless_Romantic.png", 100f);
                    break;
                case RoleId.RogueImpostor:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.NK.ScreenRogueImpostor.png", 100f);
                    break;
                case RoleId.Impostor:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenImpostor.png", 100f);
                    break;
                case RoleId.Crewmate:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenCrewmate.png", 100f);
                    break;
                case RoleId.Vigilante:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Crew.ScreenVigilante.png", 100f);
                    break;
                case RoleId.Giant:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Modifier.ScreenGiant.png", 100f);
                    break;
                case RoleId.Mini:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Modifier.ScreenMini.png", 100f);
                    break;
                case RoleId.Clutch:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Modifier.ScreenClutch.png", 100f);
                    break;
                case RoleId.Sleepwalker:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Modifier.ScreenSleepwalker.png", 100f);
                    break;
                case RoleId.Spiteful:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Modifier.ScreenSpiteful.png", 100f);
                    break;
                case RoleId.Gopher:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Modifier.ScreenGopher.png", 100f);
                    break;
                case RoleId.Sniper:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Modifier.ScreenSniper.png", 100f);
                    break;
                case RoleId.Hacker:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenHacker.png", 100f);
                    break;
                case RoleId.Parasite:
                case RoleId.ParasiteNK:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.Impostor.ScreenParasite.png", 100f);
                    break;
                default:
                    helpSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.HelpMenu.HelpScreen.png", 100f);
                    break;
            }
            return helpSprite;
        }

        public static void RefreshInfo()
        {
            //Imps
            Bomber.GetDescription();
            Camouflager.GetDescription();
            Changeling.GetDescription();
            Cultist.GetDescription();
            Follower.GetDescription();
            Hacker.GetDescription();
            Janitor.GetDescription();
            Miner.GetDescription();
            Morphling.GetDescription();
            Shade.GetDescription();
            Parasite.GetDescription();
            Undertaker.GetDescription();
            Vampire.GetDescription();
            Warlock.GetDescription();
            Wraith.GetDescription();

            //Neutral
            Arsonist.GetDescription();
            Executioner.GetDescription();
            Jester.GetDescription();
            Refugee.GetDescription();
            Romantic.GetDescription();
            Scavenger.GetDescription();
            VengefulRomantic.GetDescription();

            //NK
            HeadHunter.GetDescription();
            Nightmare.GetDescription();
            Pyromaniac.GetDescription();
            RuthlessRomantic.GetDescription();
            NeutralKiller.GetDescription();

            //Crew
            Administrator.GetDescription();
            Detective.GetDescription();
            Engineer.GetDescription();
            Guardian.GetDescription();
            Investigator.GetDescription();
            Jailor.GetDescription();
            Mayor.GetDescription();
            Medic.GetDescription();
            ParityCop.GetDescription();
            Psychic.GetDescription();
            Sheriff.GetDescription();
            Spy.GetDescription();
            Tracker.GetDescription();
            Trapper.GetDescription();
            Vigilante.GetDescription();
            Watcher.GetDescription();

            //Modifier
            Clutch.GetDescription();
            Giant.GetDescription();
            Gopher.GetDescription();
            Mini.GetDescription();
            Sleepwalker.GetDescription();
            Sniper.GetDescription();
            Spiteful.GetDescription();
            Ascended.GetDescription();
        }


        public static string crewmateHelpText()
        {
            int randomIndex = StellarRoles.rnd.Next(0, CrewmateFunnyLines.Length);

            return CrewmateFunnyLines[randomIndex];
        }

        public static string helpText(RoleInfo roleinfo)
        {
            string result;

            if (roleinfo.RoleId == RoleId.Crewmate)
            {
                result = crewmateHelpText();
            }
            else
            {
                result = roleinfo.SettingsDescription;
            }

            return result;
        }

    }
}
