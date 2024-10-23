using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Roles = StellarRoles;

namespace StellarRoles
{
    public class RoleInfo
    {
        public readonly Color Color;
        public readonly string Name;
        public readonly string IntroDescription;
        public readonly string ShortDescription;
        public readonly RoleId RoleId;
        public readonly Faction FactionId;
        public string SettingsDescription;

        RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId, Faction factionId, string settingsDescription = "")
        {
            Color = color;
            Name = name;
            IntroDescription = introDescription;
            SettingsDescription = settingsDescription;
            RoleId = roleId;
            FactionId = factionId;
            ShortDescription = shortDescription;
        }

        public static readonly Color ModifierColor = Roles.Spiteful.Color;

        public static readonly RoleInfo Spectator = new("Spectator", Roles.Spectator.Color, "Watch the Game", "Watch the Game", RoleId.Spectator, Faction.Other);

        // Impostor Roles
        public static readonly RoleInfo Bomber = new("Bomber", Palette.ImpostorRed, "Play extreme hot potato with crewmates!", "Play extreme hot potato with crewmates!", RoleId.Bomber, Faction.Impostor);
        public static readonly RoleInfo Camouflager = new("Camouflager", Palette.ImpostorRed, "Camouflage and kill the Crewmates", "Hide among others", RoleId.Camouflager, Faction.Impostor);
        public static readonly RoleInfo Changeling = new("Changeling", Palette.ImpostorRed, "Choose Your Style and Kill", "Choose Your Style and Kill", RoleId.Changeling, Faction.Impostor);
        public static readonly RoleInfo Cultist = new("Cultist", Palette.ImpostorRed, "Recruit for your cause", "Recruit for your cause", RoleId.Cultist, Faction.Impostor);
        public static readonly RoleInfo Follower = new("Follower", Palette.ImpostorRed, "", "Follow your leader", RoleId.Follower, Faction.Impostor);
        public static readonly RoleInfo Hacker = new("Hacker", Palette.ImpostorRed, "Download and disrupt system information", "Download and disrupt system information", RoleId.Hacker, Faction.Impostor);
        public static readonly RoleInfo Impostor = new("Impostor", Palette.ImpostorRed, Helpers.ColorString(Palette.ImpostorRed, "Sabotage and kill everyone"), "Sabotage and kill everyone", RoleId.Impostor, Faction.Impostor, "Oof Normal Imp.....");
        public static readonly RoleInfo Janitor = new("Janitor", Palette.ImpostorRed, "Kill everyone and leave no traces", "Clean up dead bodies", RoleId.Janitor, Faction.Impostor);
        public static readonly RoleInfo Miner = new("Miner", Palette.ImpostorRed, "Make new Vents", "Create Vents", RoleId.Miner, Faction.Impostor);
        public static readonly RoleInfo Morphling = new("Morphling", Palette.ImpostorRed, "Change your look to not get caught", "Change your look", RoleId.Morphling, Faction.Impostor);
        public static readonly RoleInfo Shade = new("Shade", Palette.ImpostorRed, "Disappear and Kill", "Disappear and Kill", RoleId.Shade, Faction.Impostor);
        public static readonly RoleInfo Parasite = new("Parasite", Palette.ImpostorRed, "Infest and Kill", "Infest and Kill", RoleId.Parasite, Faction.Impostor);
        public static readonly RoleInfo Undertaker = new("Undertaker", Palette.ImpostorRed, "Kill everyone and leave no traces", "Drag up dead bodies to hide them", RoleId.Undertaker, Faction.Impostor);
        public static readonly RoleInfo Vampire = new("Vampire", Palette.ImpostorRed, "Kill the Crewmates with your bites", "Bite your enemies", RoleId.Vampire, Faction.Impostor);
        public static readonly RoleInfo Warlock = new("Warlock", Palette.ImpostorRed, "Curse other players and kill everyone", "Curse and kill everyone", RoleId.Warlock, Faction.Impostor);
        public static readonly RoleInfo Wraith = new("Wraith", Palette.ImpostorRed, "Haunt Crewmates with blinding speed", "Haunt the Crew", RoleId.Wraith, Faction.Impostor);

        // Rogue Impostor Roles
        public static readonly RoleInfo BomberNeutralKiller = new("Bomber", NeutralKiller.Color, "Play extreme hot potato with crewmates!", "Play extreme hot potato with crewmates!", RoleId.BomberNK, Faction.NK);
        public static readonly RoleInfo CamouflagerNeutralKiller = new("Camouflager", NeutralKiller.Color, "Camouflage and kill the Crewmates", "Hide among others", RoleId.CamouflagerNK, Faction.NK);
        public static readonly RoleInfo JanitorNeutralKiller = new("Janitor", NeutralKiller.Color, "Kill everyone and leave no traces", "Clean up dead bodies", RoleId.JanitorNK, Faction.NK);
        public static readonly RoleInfo MinerNeutralKiller = new("Miner", NeutralKiller.Color, "Make new Vents", "Create Vents", RoleId.MinerNK, Faction.NK);
        public static readonly RoleInfo MorphlingNeutralKiller = new("Morphling", NeutralKiller.Color, "Change your look to not get caught", "Change your look", RoleId.MorphlingNK, Faction.NK);
        public static readonly RoleInfo RuthlessRomantic = new("Ruthless Romantic", Roles.RuthlessRomantic.Color, "Kill all players!", "Kill all players!", RoleId.RuthlessRomantic, Faction.NK);
        public static readonly RoleInfo ShadeNeutralKiller = new("Shade", NeutralKiller.Color, "Disappear and Kill", "Disappear and Kill", RoleId.ShadeNK, Faction.NK);
        public static readonly RoleInfo ParasiteNeutralKiller = new("Parasite", NeutralKiller.Color, "Infest and Kill", "Infest and Kill", RoleId.ParasiteNK, Faction.NK);
        public static readonly RoleInfo UndertakerNeutralKiller = new("Undertaker", NeutralKiller.Color, "Kill everyone and leave no traces", "Drag up dead bodies to hide them", RoleId.UndertakerNK, Faction.NK);
        public static readonly RoleInfo VampireNeutralKiller = new("Vampire", NeutralKiller.Color, "Kill the Crewmates with your bites", "Bite your enemies", RoleId.VampireNK, Faction.NK);
        public static readonly RoleInfo WarlockNeutralKiller = new("Warlock", NeutralKiller.Color, "Curse other players and kill everyone", "Curse and kill everyone", RoleId.WarlockNK, Faction.NK);
        public static readonly RoleInfo WraithNeutralKiller = new("Wraith", NeutralKiller.Color, "Haunt Crewmates with blinding speed", "Haunt the Crew", RoleId.WraithNK, Faction.NK);

        public static readonly RoleInfo RogueImpostor = new("Rogue Impostor", NeutralKiller.Color, " ", " ", RoleId.RogueImpostor, Faction.NK);

        // Crew Roles
        public static readonly RoleInfo Administrator = new("Administrator", Roles.Administrator.Color, "Access Admin Table", "Access Admin Table", RoleId.Administrator, Faction.Crewmate);
        public static readonly RoleInfo Crewmate = new("Crewmate", Palette.CrewmateBlue, "Find the Impostors", "Find the Impostors", RoleId.Crewmate, Faction.Crewmate);
        public static readonly RoleInfo Investigator = new("Investigator", Roles.Investigator.Color, "Find the <color=#FF1919FF>Impostors</color> by examining footprints", "Examine footprints", RoleId.Investigator, Faction.Crewmate);
        public static readonly RoleInfo Engineer = new("Engineer", Roles.Engineer.Color, "Maintain important systems on the ship", "Repair the ship", RoleId.Engineer, Faction.Crewmate);
        public static readonly RoleInfo Vigilante = new("Vigilante", Roles.Vigilante.Color, "Guess and shoot", "Guess and shoot", RoleId.Vigilante, Faction.Crewmate);
        public static readonly RoleInfo Guardian = new("Guardian", Roles.Guardian.Color, "Protect someone with your shield", "Protect other players", RoleId.Guardian, Faction.Crewmate);
        public static readonly RoleInfo Jailor = new("Jailor", Roles.Jailor.Color, "Lock down the assassin!", "Lock down the assassin!", RoleId.Jailor, Faction.Crewmate);
        public static readonly RoleInfo Mayor = new("Mayor", Roles.Mayor.Color, "Your vote counts twice", "Your vote counts twice", RoleId.Mayor, Faction.Crewmate);
        public static readonly RoleInfo Medic = new("Medic", Roles.Medic.Color, "Monitor crew health", "Monitor crew health", RoleId.Medic, Faction.Crewmate);
        public static readonly RoleInfo Detective = new("Detective", Roles.Detective.Color, "Inspect the Crime Scenes", "Inspect the Crime Scenes", RoleId.Detective, Faction.Crewmate);
        public static readonly RoleInfo Sheriff = new("Sheriff", Roles.Sheriff.Color, "Shoot the <color=#FF1919FF>Impostors</color>", "Shoot the Impostors", RoleId.Sheriff, Faction.Crewmate);
        public static readonly RoleInfo Spy = new("Spy", Roles.Spy.Color, "Confuse the <color=#FF1919FF>Impostors</color>", "Confuse the Impostors", RoleId.Spy, Faction.Crewmate);
        public static readonly RoleInfo Tracker = new("Tracker", Roles.Tracker.Color, "Track the <color=#FF1919FF>Impostors</color> down", "Track the Impostors down", RoleId.Tracker, Faction.Crewmate);
        public static readonly RoleInfo Trapper = new("Trapper", Roles.Trapper.Color, "Cover and Trap Vents", "Cover and Trap Vents", RoleId.Trapper, Faction.Crewmate);
        public static readonly RoleInfo Watcher = new("Watcher", Roles.Watcher.Color, "Keep Watch on your fellow crewmates", "Keep Watch on your fellow crewmates", RoleId.Watcher, Faction.Crewmate);
        public static readonly RoleInfo ParityCop = new("Parity Cop", Roles.ParityCop.Color, "Compare the alignments of other players!", "Compare the alignments of other players!", RoleId.ParityCop, Faction.Crewmate);
        public static readonly RoleInfo Psychic = new("Psychic", Roles.Psychic.Color, "Keep An Eye On Evil", "Keep An Eye On Evil", RoleId.Psychic, Faction.Crewmate);
        public static readonly RoleInfo Refugee = new("Refugee", Roles.Refugee.Color, "Survive and Exile Killer Threats", "Survive and Exile Killer Threats", RoleId.Refugee, Faction.Crewmate);

        // Neutral Roles
        public static readonly RoleInfo Arsonist = new("Arsonist", Roles.Arsonist.Color, "Let them burn", "Let them burn", RoleId.Arsonist, Faction.Neutral);
        public static readonly RoleInfo Beloved = new("♥", Roles.Beloved.Color, "", "", RoleId.Beloved, Faction.Neutral);
        public static readonly RoleInfo Executioner = new("Executioner", Roles.Executioner.Color, "Vote out your target", "Vote out your target", RoleId.Executioner, Faction.Neutral);
        public static readonly RoleInfo Jester = new("Jester", Roles.Jester.Color, "Get voted out", "Get voted out", RoleId.Jester, Faction.Neutral);
        public static readonly RoleInfo Romantic = new("Romantic", Roles.Romantic.Color, "Protect and Assist Your Love", "Protect and Assist Your Love", RoleId.Romantic, Faction.Neutral);
        public static readonly RoleInfo Scavenger = new("Scavenger", Roles.Scavenger.Color, "Eat corpses to win", "Eat dead bodies", RoleId.Scavenger, Faction.Neutral);
        public static readonly RoleInfo VengefulRomantic = new("Vengeful Romantic", Roles.VengefulRomantic.Color, "Avenge Your Beloved!", "Avenge Your Beloved!", RoleId.VengefulRomantic, Faction.Neutral);

        //Neutral Killing Roles
        public static readonly RoleInfo HeadHunter = new("HeadHunter", Roles.HeadHunter.Color, "Hunt and Kill All Players!", "Hunt and Kill All Players!", RoleId.HeadHunter, Faction.NK);
        public static readonly RoleInfo Nightmare = new("Nightmare", Roles.Nightmare.Color, "Becomes the players worst nightmare!", "Becomes the players worst nightmare!", RoleId.Nightmare, Faction.NK);
        public static readonly RoleInfo Pyromaniac = new("Pyromaniac", Roles.Pyromaniac.Color, "Burn all players!", "Burn all players!", RoleId.Pyromaniac, Faction.NK);

        // Modifier
        public static readonly RoleInfo Clutch = new("Clutch", ModifierColor, "Ability cooldowns are reduced when solo!", "Ability cooldowns are reduced when solo!", RoleId.Clutch, Faction.Modifier);
        public static readonly RoleInfo Giant = new("Giant", ModifierColor, "You are Big Boi!", "You are Big Boi!", RoleId.Giant, Faction.Modifier);
        public static readonly RoleInfo Gopher = new("Gopher", ModifierColor, "Your cooldowns progress while venting!", "Your cooldowns progress while venting!", RoleId.Gopher, Faction.Modifier);
        public static readonly RoleInfo Mini = new("Mini", ModifierColor, "You are Smol!", "You are Smol!", RoleId.Mini, Faction.Modifier);
        public static readonly RoleInfo Sleepwalker = new("Sleepwalker", ModifierColor, "Where am I? How did I get here?", "Where am I? How did I get here?", RoleId.Sleepwalker, Faction.Modifier);
        public static readonly RoleInfo Sniper = new("Sniper", ModifierColor, "You have a higher kill range!", "You have a higher kill range!", RoleId.Sniper, Faction.Modifier);
        public static readonly RoleInfo Spiteful = new("Spiteful", ModifierColor, "They will regret voting for you!", "They will regret voting for you!", RoleId.Spiteful, Faction.Modifier);
        public static readonly RoleInfo Ascended = new("Ascended", ModifierColor, "You role is improved! Check the in-game wiki to see how!", "You role is improved! Check the in-game wiki to see how!", RoleId.Ascended, Faction.Modifier);

        public static readonly RoleInfo WasRomantic = new("♥", Roles.Romantic.Color, "", "", RoleId.Romantic, Faction.Modifier);

        public static List<RoleInfo> AllRoleInfos => new()
        {
            // Impostor Roles
            Bomber,
            Camouflager,
            Changeling,
            Cultist,
            Follower,
            Hacker,
            Impostor,
            Janitor,
            Miner,
            Morphling,
            Shade,
            Parasite,
            Undertaker,
            Vampire,
            Warlock,
            Wraith,

            // Crewmate Roles
            Administrator,
            Crewmate,
            Detective,
            Engineer,
            Guardian,
            Investigator,
            Jailor,
            Mayor,
            Medic,
            ParityCop,
            Psychic,
            Sheriff,
            Spy,
            Tracker,
            Trapper,
            VengefulRomantic,
            Vigilante,
            Watcher,

            // Neutral Roles
            Arsonist,
            Executioner,
            Jester,
            Refugee,
            Romantic,
            Scavenger,

            // Neutral Killing Roles
            HeadHunter,
            Nightmare,
            Pyromaniac,
            RuthlessRomantic,

            // Rogue Impostor Roles
            BomberNeutralKiller,
            CamouflagerNeutralKiller,
            JanitorNeutralKiller,
            MinerNeutralKiller,
            MorphlingNeutralKiller,
            ShadeNeutralKiller,
            ParasiteNeutralKiller,
            UndertakerNeutralKiller,
            VampireNeutralKiller,
            WarlockNeutralKiller,
            WraithNeutralKiller,
            RogueImpostor,

            // Modifiers
            Clutch,
            Giant,
            Gopher,
            Mini,
            Sleepwalker,
            Sniper,
            Spiteful,
            Ascended
        };

        public static List<RoleInfo> GetRoleInfoForPlayer(PlayerControl player, bool showModifier = true)
        {
            List<RoleInfo> infos = new();


            // Modifier
            if (showModifier)
            {
                if (CustomOptionHolder.ModifierSleepwalkerSeesModifier.GetBool() && Roles.Sleepwalker.Players.Contains(player.PlayerId))
                    infos.Add(Sleepwalker);

                if (CustomOptionHolder.ModifierSpitefulSeeModifier.GetBool() && player.IsSpiteful(out _))
                    infos.Add(Spiteful);

                if (Roles.Gopher.Players.Contains(player)) infos.Add(Gopher);
                if (Roles.Sniper.Players.Contains(player)) infos.Add(Sniper);

                if (CustomOptionHolder.ModifierClutchSeeModifier.GetBool() && Roles.Clutch.Players.Contains(player))
                    infos.Add(Clutch);
                if (Roles.Ascended.Players.Contains(player))
                {
                    infos.Add(Ascended);
                }
            }

            if (Roles.Spectator.IsSpectator(player.PlayerId)) infos.Add(Spectator);

            // Impostor roles
            if (player == Roles.Morphling.Player) infos.Add(Roles.Morphling.IsNeutralKiller ? MorphlingNeutralKiller : Morphling);
            else if (player == Roles.Camouflager.Player) infos.Add(Roles.Camouflager.IsNeutralKiller ? CamouflagerNeutralKiller : Camouflager);
            else if (player == Roles.Vampire.Player) infos.Add(Roles.Vampire.IsNeutralKiller ? VampireNeutralKiller : Vampire);
            else if (player == Roles.Janitor.Player) infos.Add(Roles.Janitor.IsNeutralKiller ? JanitorNeutralKiller : Janitor);
            else if (player == Roles.Warlock.Player) infos.Add(Roles.Warlock.IsNeutralKiller ? WarlockNeutralKiller : Warlock);
            else if (player == Roles.Undertaker.Player) infos.Add(Roles.Undertaker.IsNeutralKiller ? UndertakerNeutralKiller : Undertaker);
            else if (player == Roles.Wraith.Player) infos.Add(Roles.Wraith.IsNeutralKiller ? WraithNeutralKiller : Wraith);
            else if (player == Roles.Hacker.Player) infos.Add(Hacker);
            else if (player == Roles.Miner.Player) infos.Add(Roles.Miner.IsNeutralKiller ? MinerNeutralKiller : Miner);
            else if (player == Roles.Shade.Player) infos.Add(Roles.Shade.IsNeutralKiller ? ShadeNeutralKiller : Shade);
            else if (player == Roles.Parasite.Player) infos.Add(Roles.Parasite.IsNeutralKiller ? ParasiteNeutralKiller : Parasite);
            else if (player == Roles.Bomber.Player) infos.Add(Roles.Bomber.IsNeutralKiller ? BomberNeutralKiller : Bomber);
            else if (player == Roles.Changeling.Player) infos.Add(Changeling);
            else if (player == Roles.Cultist.Player) infos.Add(Cultist);
            else if (player == Roles.Follower.Player) infos.Add(Follower);

            // Crewmate roles
            else if (player == Roles.Mayor.Player) infos.Add(Mayor);
            else if (player == Roles.Engineer.Player) infos.Add(Engineer);
            else if (player == Roles.Investigator.Player) infos.Add(Investigator);
            else if (player == Roles.Guardian.Player) infos.Add(Guardian);
            else if (player == Roles.Tracker.Player) infos.Add(Tracker);
            else if (player == Roles.Sheriff.Player) infos.Add(Sheriff);
            else if (player == Roles.Spy.Player) infos.Add(Spy);
            else if (player == Roles.Vigilante.Player) infos.Add(Vigilante);
            else if (player == Roles.Detective.Player) infos.Add(Detective);
            else if (player == Roles.Administrator.Player) infos.Add(Administrator);
            else if (player == Roles.Watcher.Player) infos.Add(Watcher);
            else if (player == Roles.Trapper.Player) infos.Add(Trapper);
            else if (player == Roles.Medic.Player) infos.Add(Medic);
            else if (player.IsJailor(out _)) infos.Add(Jailor);
            else if (player.IsParityCop(out _)) infos.Add(ParityCop);
            else if (player == Roles.Psychic.Player) infos.Add(Psychic);

            // Neutral Roles
            else if (player.IsJester(out _)) infos.Add(Jester);
            else if (player.IsRefugee(out _)) infos.Add(Refugee);
            else if (player.IsRuthlessRomantic(out _)) infos.Add(RuthlessRomantic);
            else if (player.IsNightmare(out _)) infos.Add(Nightmare);
            else if (player.IsPyromaniac(out _)) infos.Add(Pyromaniac);
            else if (player == Roles.Executioner.Player) infos.Add(Executioner);
            else if (player == Roles.Arsonist.Player) infos.Add(Arsonist);
            else if (player == Roles.Scavenger.Player) infos.Add(Scavenger);
            else if (player == Roles.Romantic.Player) infos.Add(Romantic);
            else if (player == Roles.VengefulRomantic.Player) infos.Add(VengefulRomantic);
            else if (player == Roles.HeadHunter.Player) infos.Add(HeadHunter);

            if (player == Roles.Beloved.Player)
            {
                if (Roles.Beloved.WasArsonist)
                    infos.Add(Arsonist);

                else if (Roles.Beloved.EasExecutioner)
                    infos.Add(Executioner);

                else if (Roles.Beloved.WasJester)
                    infos.Add(Jester);

                else if (Roles.Beloved.WasScavenger)
                    infos.Add(Scavenger);

                else if (Roles.Beloved.WasRefugee)
                    infos.Add(Refugee);

                infos.Add(Beloved);
            }

            else if (player == Roles.Beloved.Romantic && Roles.Romantic.SetNameFirstMeeting)
                infos.Add(WasRomantic);

            // Default roles
            if (infos.Count == 0)
                infos.Add(player.Data.Role.IsImpostor ? Impostor : Crewmate); // Just Impostor/Crewmate

            return infos;
        }

        public static string GetRolesString(PlayerControl player, bool useColors, bool showModifier = true)
        {
            return string.Join(" ", GetRoleInfoForPlayer(player, showModifier).Select(x => useColors ? Helpers.ColorString(x.Color, x.Name) : x.Name).ToArray());
        }
    }

    public static class RoleInfoExtensions
    {
        public static bool IsCrewKilling(this RoleInfo role)
        {
            return role.RoleId == RoleId.Sheriff || role.RoleId == RoleId.Vigilante;
        }

        public static void GetRoleDetails(this PlayerControl player, out RoleInfo role, out Faction faction, out RoleId RoleId)
        {
            role = RoleInfo.GetRoleInfoForPlayer(player, false).FirstOrDefault();
            faction = role.FactionId;
            RoleId = role.RoleId;
        }
    }
}
