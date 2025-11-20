using AmongUs.GameOptions;
using Cpp2IL.Core.Extensions;
using HarmonyLib;
using StellarRoles.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StellarRoles.StellarRoles;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    class GameOptionsDataGetAdjustedNumImpostorsPatch
    {
        public static void Postfix(ref int __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal)
            {  // Ignore Vanilla impostor limits in Games.
                __result = Mathf.Clamp(GameOptionsManager.Instance.CurrentGameOptions.NumImpostors, 1, 3);
            }
        }
    }

    /*[HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Validate))]
    class GameOptionsDataValidatePatch
    {
        public static void Postfix(GameOptionsData __instance)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.Normal) return;
            __instance.NumImpostors = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
        }
    }*/

    [HarmonyPatch(typeof(RoleOptionsCollectionV08), nameof(RoleOptionsCollectionV08.GetNumPerGame))]
    class RoleOptionsDataGetNumPerGamePatch
    {
        public static void Postfix(ref int __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) __result = 0; // Deactivate Vanilla Roles if the mod roles are active
        }
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    class RoleManagerSelectRolesPatch
    {
        public static void Postfix()
        {
            RPCProcedure.Send(CustomRPC.ResetVaribles);
            RPCProcedure.ResetVariables();

            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
                AssignRoles();
        }

        private static void AssignRoles()
        {
            RoleAssignmentData data = GetRoleAssignmentData();
            if (Spectator.ToBecomeSpectator.Count > 0)
                AssignSpectators(data);
            AssignEnsuredRoles(data); // Assign roles that should always be in the game first
            AssignChanceRoles(data); // Assign roles that may or may not be in the game last
            AssignRoleTargets();
            AssignAssassins();
            AssignModifiers();
        }

        public static bool BlockedRole(RoleId roleId)
        {
            var map = Helpers.CurrentMap();
            var block = false;
            switch (roleId)
            {
                case RoleId.Miner:
                    if (map == Map.Fungal && CustomOptionHolder.DisableMinerOnFungle.GetBool()) block = true;
                    if (map == Map.Mira && CustomOptionHolder.DisableMinerOnMira.GetBool()) block = true;
                    break;
                case RoleId.Janitor:
                    if (map == Map.Skeld && CustomOptionHolder.DisableJanitorOnSkeld.GetBool()) block = true;
                    if (map == Map.Mira && CustomOptionHolder.DisableJanitorOnMira.GetBool()) block = true;
                    if (map == Map.Fungal && CustomOptionHolder.DisableJanitorOnFungle.GetBool()) block = true;
                    break;
                case RoleId.Arsonist:
                    if (map == Map.Skeld && CustomOptionHolder.DisableArsonistOnSkeld.GetBool()) block = true;
                    if (map == Map.Mira && CustomOptionHolder.DisableArsonistOnMira.GetBool()) block = true;
                    break;
                case RoleId.Scavenger:
                    if (map == Map.Skeld && CustomOptionHolder.DisableScavengerOnSkeld.GetBool()) block = true;
                    if (map == Map.Mira && CustomOptionHolder.DisableScavengerOnMira.GetBool()) block = true;
                    if (map == Map.Fungal && CustomOptionHolder.DisableScavengerOnFungle.GetBool()) block = true;
                    break;
                case RoleId.Medic:
                    if (map == Map.Skeld && CustomOptionHolder.DisableMedicOnSkeld.GetBool()) block = true;
                    if (map == Map.Mira && CustomOptionHolder.DisableMedicOnMira.GetBool()) block = true;
                    break;
                case RoleId.Administrator:
                    if (map == Map.Mira && CustomOptionHolder.DisableAdministratorOnMira.GetBool()) block = true;
                    if (map == Map.Fungal && CustomOptionHolder.DisableAdministratorOnFungle.GetBool()) block = true;
                    break;
                case RoleId.Watcher:
                    if (map == Map.Skeld && CustomOptionHolder.DisableWatcherOnSkeld.GetBool()) block = true;
                    break;
                case RoleId.Mayor:
                    if (!GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes) block = true;
                    break;
                case RoleId.Hacker:
                    if (SubmergedCompatibility.IsSubmerged) block = true;
                    break;
            }
            return block;
        }

        public static RoleAssignmentData GetRoleAssignmentData()
        {
            bool isMira = Helpers.IsMap(Map.Mira);
            bool isSkeld = Helpers.IsMap(Map.Skeld);
            bool isFungle = Helpers.IsMap(Map.Fungal);

            // Get the players that we want to assign the roles to. Crewmate and Neutral roles are assigned to natural crewmates. Impostor roles to impostors.
            List<PlayerControl> crewmates = new();
            List<PlayerControl> impostors = new();

            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator().OrderBy(x => rnd.Next()))
                (player.Data.Role.IsImpostor ? impostors : crewmates).Add(player);

            int crewmateMin = CustomOptionHolder.CrewmateRolesCountMin.GetSelection();
            int crewmateMax = CustomOptionHolder.CrewmateRolesCountMax.GetSelection();
            int neutralMin = CustomOptionHolder.NeutralRolesCountMin.GetSelection();
            int neutralMax = CustomOptionHolder.NeutralRolesCountMax.GetSelection();
            int neutralKMin = CustomOptionHolder.NeutralKillerRolesCountMin.GetSelection();
            int neutralKMax = CustomOptionHolder.NeutralKillerRolesCountMax.GetSelection();
            int impostorMin = CustomOptionHolder.ImpostorRolesCountMin.GetSelection();
            int impostorMax = CustomOptionHolder.ImpostorRolesCountMax.GetSelection();

            // Get the maximum allowed count of each role type based on the minimum and maximum option
            int crewCountSettings = crewmateMin < crewmateMax ? rnd.Next(crewmateMin, crewmateMax) : crewmateMin;
            int neutralCountSettings = neutralMin < neutralMax ? rnd.Next(neutralMin, neutralMax) : neutralMin;
            int neutralKCountSettings = neutralKMin < neutralKMax ? rnd.Next(neutralKMin, neutralKMax) : neutralKMin;
            int impCountSettings = impostorMin < impostorMax ? rnd.Next(impostorMin, impostorMax) : impostorMin;

            // Fill in the lists with the roles that should be assigned to players. Note that the special roles are NOT included in these lists
            Dictionary<RoleId, int> impSettings = new();
            Dictionary<RoleId, int> neutralSettings = new();
            Dictionary<RoleId, int> neutralKillerSettings = new();
            Dictionary<RoleId, int> crewSettings = new();

            //Impostor/Rogue
            bool BomberNeutral = CustomOptionHolder.BomberIsNeutral.GetBool();
            bool CamouflagerNeutral = CustomOptionHolder.CamouflagerIsNeutral.GetBool();
            bool JanitorNeutral = CustomOptionHolder.JanitorIsNeutral.GetBool();
            bool MinerNeutral = CustomOptionHolder.MinerIsNeutral.GetBool();
            bool MorphlingNeutral = CustomOptionHolder.MorphlingIsNeutral.GetBool();
            bool ParasiteNeutral = CustomOptionHolder.ParasiteIsNeutral.GetBool();
            bool ShadeNeutral = CustomOptionHolder.ShadeIsNeutral.GetBool();
            bool UndertakerNeutral = CustomOptionHolder.UndertakerIsNeutral.GetBool();
            bool VampireNeutral = CustomOptionHolder.VampireIsNeutral.GetBool();
            bool WarlockNeutral = CustomOptionHolder.WarlockIsNeutral.GetBool();
            bool WraithNeutral = CustomOptionHolder.WraithIsNeutral.GetBool();

            //Impostor
            impSettings.Add(RoleId.Bomber, BomberNeutral ? 0 : CustomOptionHolder.BomberSpawnRate.GetSelection());
            impSettings.Add(RoleId.Camouflager, CamouflagerNeutral ? 0 : CustomOptionHolder.CamouflagerSpawnRate.GetSelection());
            impSettings.Add(RoleId.Changeling, CustomOptionHolder.ChangelingSpawnRate.GetSelection());
            impSettings.Add(RoleId.Morphling, MorphlingNeutral ? 0 : CustomOptionHolder.MorphlingSpawnRate.GetSelection());
            impSettings.Add(RoleId.Parasite, ParasiteNeutral ? 0 : CustomOptionHolder.ParasiteSpawnRate.GetSelection());
            impSettings.Add(RoleId.Shade, ShadeNeutral ? 0 : CustomOptionHolder.ShadeSpawnRate.GetSelection());
            impSettings.Add(RoleId.Undertaker, UndertakerNeutral ? 0 : CustomOptionHolder.UndertakerSpawnRate.GetSelection());
            impSettings.Add(RoleId.Vampire, VampireNeutral ? 0 : CustomOptionHolder.VampireSpawnRate.GetSelection());
            impSettings.Add(RoleId.Warlock, WarlockNeutral ? 0 : CustomOptionHolder.WarlockSpawnRate.GetSelection());
            impSettings.Add(RoleId.Wraith, WraithNeutral ? 0 : CustomOptionHolder.WraithSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Hacker))
                impSettings.Add(RoleId.Hacker, CustomOptionHolder.HackerSpawnRate.GetSelection());

            if (impostors.Count > 1)
                impSettings.Add(RoleId.Cultist, CustomOptionHolder.CultistSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Miner))
            {
                impSettings.Add(RoleId.Miner, MinerNeutral ? 0 : CustomOptionHolder.MinerSpawnRate.GetSelection());
                neutralKillerSettings.Add(RoleId.MinerNK, MinerNeutral ? CustomOptionHolder.MinerSpawnRate.GetSelection() : 0);
            }

            if (!BlockedRole(RoleId.Janitor))
            {
                impSettings.Add(RoleId.Janitor, JanitorNeutral ? 0 : CustomOptionHolder.JanitorSpawnRate.GetSelection());
                neutralKillerSettings.Add(RoleId.JanitorNK, JanitorNeutral ? CustomOptionHolder.JanitorSpawnRate.GetSelection() : 0);
            }

            //Neutral
            neutralSettings.Add(RoleId.Jester, CustomOptionHolder.JesterSpawnRate.GetSelection());
            neutralSettings.Add(RoleId.Executioner, CustomOptionHolder.ExecutionerSpawnRate.GetSelection());
            neutralSettings.Add(RoleId.Romantic, CustomOptionHolder.RomanticSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Scavenger))
                neutralSettings.Add(RoleId.Scavenger, CustomOptionHolder.ScavengerSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Arsonist))
                neutralSettings.Add(RoleId.Arsonist, CustomOptionHolder.ArsonistSpawnRate.GetSelection());

            //Neutral Killer
            neutralKillerSettings.Add(RoleId.BomberNK, BomberNeutral ? CustomOptionHolder.BomberSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.CamouflagerNK, CamouflagerNeutral ? CustomOptionHolder.CamouflagerSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.HeadHunter, CustomOptionHolder.HeadHunterSpawnRate.GetSelection());
            neutralKillerSettings.Add(RoleId.MorphlingNK, MorphlingNeutral ? CustomOptionHolder.MorphlingSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.Nightmare, CustomOptionHolder.NightmareSpawnRate.GetSelection());
            neutralKillerSettings.Add(RoleId.Pyromaniac, CustomOptionHolder.PyromaniacSpawnRate.GetSelection());
            neutralKillerSettings.Add(RoleId.ParasiteNK, ParasiteNeutral ? CustomOptionHolder.ParasiteSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.ShadeNK, ShadeNeutral ? CustomOptionHolder.ShadeSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.UndertakerNK, UndertakerNeutral ? CustomOptionHolder.UndertakerSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.VampireNK, VampireNeutral ? CustomOptionHolder.VampireSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.WarlockNK, WarlockNeutral ? CustomOptionHolder.WarlockSpawnRate.GetSelection() : 0);
            neutralKillerSettings.Add(RoleId.WraithNK, WraithNeutral ? CustomOptionHolder.WraithSpawnRate.GetSelection() : 0);


            //Crewmate
            crewSettings.Add(RoleId.Engineer, CustomOptionHolder.EngineerSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Investigator, CustomOptionHolder.InvestigatorSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Guardian, CustomOptionHolder.GuardianSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Tracker, CustomOptionHolder.TrackerSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Vigilante, CustomOptionHolder.VigilanteSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Sheriff, CustomOptionHolder.SheriffSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Trapper, CustomOptionHolder.TrapperSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Jailor, CustomOptionHolder.JailorSpawnRate.GetSelection());
            crewSettings.Add(RoleId.ParityCop, CustomOptionHolder.ParityCopSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Psychic, CustomOptionHolder.PsychicSpawnRate.GetSelection());
            crewSettings.Add(RoleId.Detective, CustomOptionHolder.DetectiveSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Mayor))
                crewSettings.Add(RoleId.Mayor, CustomOptionHolder.MayorSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Watcher))
                crewSettings.Add(RoleId.Watcher, CustomOptionHolder.WatcherSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Administrator))
                crewSettings.Add(RoleId.Administrator, CustomOptionHolder.AdministratorSpawnRate.GetSelection());

            if (!BlockedRole(RoleId.Medic))
                crewSettings.Add(RoleId.Medic, CustomOptionHolder.MedicSpawnRate.GetSelection());

            if (impostors.Count > 1) // Only add Spy if more than 1 impostor as the spy role is otherwise useless
                crewSettings.Add(RoleId.Spy, CustomOptionHolder.SpySpawnRate.GetSelection());

            return new RoleAssignmentData
            {
                Crewmates = crewmates,
                Impostors = impostors,
                CrewSettings = crewSettings,
                NeutralSettings = neutralSettings,
                NeutralKillerSettings = neutralKillerSettings,
                ImpSettings = impSettings,
                MaxCrewmateRoles = crewCountSettings,
                MaxNeutralRoles = neutralCountSettings,
                MaxNeutralKillerRoles = neutralKCountSettings,
                MaxImpostorRoles = impCountSettings
            };
        }

        private static void RemoveSpectatingImpostors(RoleAssignmentData data)
        {
            foreach (var player in Spectator.ToBecomeSpectator.GetPlayerEnumerator().Where(p => p != null && p.Data.Role.IsImpostor && !p.Data.Disconnected))
            {
                player.TurnToCrewmate();
                data.Impostors.Remove(player);
            }
        }

        private static void RemoveSpecatingCrew(RoleAssignmentData data)
        {
            data.Crewmates.RemoveAll(player => Spectator.ToBecomeSpectator.Contains(player.PlayerId));
        }

        private static void BalanceImpostors(RoleAssignmentData data)
        {
            List<PlayerControl> crewmates = data.Crewmates;
            List<PlayerControl> impostors = data.Impostors;
            int playercount = impostors.Count + crewmates.Count;

            if ((playercount < 6 && impostors.Count > 1) || playercount < 9 && impostors.Count > 2)
            {
                PlayerControl nextimp = impostors.RemoveAndReturn(rnd.Next(0, impostors.Count));
                nextimp.TurnToCrewmate();
                crewmates.Add(nextimp);
            }

            else if ((playercount >= 3 && impostors.Count < 1) || (playercount > 7 && impostors.Count < 2 && GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors > 1))
            {
                PlayerControl nextcrew = crewmates.RemoveAndReturn(rnd.Next(0, crewmates.Count));
                RPCProcedure.Send(CustomRPC.CreateImpostor, nextcrew);
                RPCProcedure.CreateImpostor(nextcrew);
                impostors.Add(nextcrew);
            }
        }

        private static void AssignSpectators(RoleAssignmentData data)
        {
            RemoveSpectatingImpostors(data);
            RemoveSpecatingCrew(data);
            BalanceImpostors(data);

            foreach (byte playerId in Spectator.ToBecomeSpectator.Clone())
            {
                PlayerControl player = Helpers.PlayerById(playerId);

                RPCProcedure.Send(CustomRPC.SetRole, RoleId.Spectator, player);
                RPCProcedure.SetRole(RoleId.Spectator, player);
            }

            RPCProcedure.Send(CustomRPC.ClearToBeSpectators);
            Spectator.ToBecomeSpectator.Clear();
        }

        private static void AssignEnsuredRoles(RoleAssignmentData data)
        {
            // Get all roles where the chance to occur is set to 100%
            List<RoleId> ensuredCrewmateRoles = data.CrewSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<RoleId> ensuredNeutralRoles = data.NeutralSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<RoleId> ensuredNeutralKRoles = data.NeutralKillerSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<RoleId> ensuredImpostorRoles = data.ImpSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();


            //Assign Impostor First
            while (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0)
            {
                Dictionary<RoleType, List<RoleId>> ImpRolesToAssign = new();
                if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0)
                    ImpRolesToAssign.Add(RoleType.Impostor, ensuredImpostorRoles);

                // Randomly select a pool of roles to assign a role from next (Crewmate role, Neutral role or Impostor role) 
                // then select one of the roles from the selected pool to a player 
                // and remove the role (and any potentially blocked role pairings) from the pool(s)
                RoleType roleType = ImpRolesToAssign.Keys.ElementAt(rnd.Next(0, ImpRolesToAssign.Keys.Count));
                List<PlayerControl> players = data.Impostors;
                RoleId roleId = ImpRolesToAssign[roleType].RemoveAndReturn(rnd.Next(0, ImpRolesToAssign[roleType].Count));
                SetRoleToRandomPlayer(roleId, players);

                if (roleId == RoleId.Cultist)
                {
                    PlayerControl player = data.Impostors.FirstOrDefault();

                    player.TurnToCrewmate();

                    data.Impostors.Remove(player);
                    data.Crewmates.Add(player);
                }
                else
                {
                    ImpRolesToAssign[RoleType.Impostor].RemoveAll(x => x == RoleId.Cultist);
                    if (data.ImpSettings.ContainsKey(RoleId.Cultist))
                        data.ImpSettings[RoleId.Cultist] = 0;
                }

                if (CustomOptionHolder.BlockedRolePairings.TryGetValue(roleId, out RoleId[] pairings))
                {
                    foreach (RoleId blockedRoleId in pairings)
                    {
                        // Set chance for the blocked roles to 0 for chances less than 100%
                        if (data.ImpSettings.ContainsKey(blockedRoleId))
                            data.ImpSettings[blockedRoleId] = 0;
                        if (data.NeutralSettings.ContainsKey(blockedRoleId))
                            data.NeutralSettings[blockedRoleId] = 0;
                        if (data.NeutralKillerSettings.ContainsKey(blockedRoleId))
                            data.NeutralKillerSettings[blockedRoleId] = 0;
                        if (data.CrewSettings.ContainsKey(blockedRoleId))
                            data.CrewSettings[blockedRoleId] = 0;
                        // Remove blocked roles even if the chance was 100%
                        foreach (List<RoleId> ensuredRolesList in ImpRolesToAssign.Values)
                            ensuredRolesList.RemoveAll(x => x == blockedRoleId);
                    }
                }
                data.MaxImpostorRoles--;
            }

            // Assign Crewmates/Neutrals/Nks roles until we run out of either players we can assign roles to
            // or run out of roles we can assign to players
            while (data.Crewmates.Count > 0 && (
                (data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0)
                || (data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)
                || (data.MaxNeutralKillerRoles > 0 && ensuredNeutralKRoles.Count > 0)))
            {

                // Add Crewmate/Neutral/Nk Roles To List
                Dictionary<RoleType, List<RoleId>> CrewRolesToAssign = new();
                if (data.Crewmates.Count > 0)
                {
                    if (data.MaxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0)
                        CrewRolesToAssign.Add(RoleType.Crewmate, ensuredCrewmateRoles);
                    if (data.MaxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)
                        CrewRolesToAssign.Add(RoleType.Neutral, ensuredNeutralRoles);
                    if (data.MaxNeutralKillerRoles > 0 && ensuredNeutralKRoles.Count > 0)
                        CrewRolesToAssign.Add(RoleType.NeutralKiller, ensuredNeutralKRoles);
                }

                // Randomly select a pool of roles to assign a role from next (Crewmate, Neutral, or NK role)
                // then select one of the roles from the selected pool to a player 
                // and remove the role (and any potentially blocked role pairings) from the pool(s)
                RoleType roleType = CrewRolesToAssign.Keys.ElementAt(rnd.Next(0, CrewRolesToAssign.Keys.Count));
                List<PlayerControl> players = roleType == RoleType.Crewmate || roleType == RoleType.Neutral || roleType == RoleType.NeutralKiller ? data.Crewmates : data.Impostors;
                RoleId roleId = CrewRolesToAssign[roleType].RemoveAndReturn(rnd.Next(0, CrewRolesToAssign[roleType].Count));
                SetRoleToRandomPlayer(roleId, players);

                if (CustomOptionHolder.BlockedRolePairings.TryGetValue(roleId, out RoleId[] pairings))
                {
                    foreach (RoleId blockedRoleId in pairings)
                    {
                        // Set chance for the blocked roles to 0 for chances less than 100%
                        if (data.ImpSettings.ContainsKey(blockedRoleId))
                            data.ImpSettings[blockedRoleId] = 0;
                        if (data.NeutralSettings.ContainsKey(blockedRoleId))
                            data.NeutralSettings[blockedRoleId] = 0;
                        if (data.NeutralKillerSettings.ContainsKey(blockedRoleId))
                            data.NeutralKillerSettings[blockedRoleId] = 0;
                        if (data.CrewSettings.ContainsKey(blockedRoleId))
                            data.CrewSettings[blockedRoleId] = 0;
                        // Remove blocked roles even if the chance was 100%
                        foreach (List<RoleId> ensuredRolesList in CrewRolesToAssign.Values)
                            ensuredRolesList.RemoveAll(x => x == blockedRoleId);
                    }
                }

                // Adjust the role limit
                switch (roleType)
                {
                    case RoleType.Crewmate:
                        data.MaxCrewmateRoles--;
                        break;
                    case RoleType.Neutral:
                        data.MaxNeutralRoles--;
                        break;
                    case RoleType.NeutralKiller:
                        data.MaxNeutralKillerRoles--;
                        break;
                }
            }
        }

        private static void AssignChanceRoles(RoleAssignmentData data)
        {
            // Get all roles where the chance to occur is set grater than 0% but not 100% and build a ticket pool based on their weight
            List<RoleId> crewmateTickets = data.CrewSettings.Where(x => x.Value > 0 && x.Value < 10).SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToList();
            List<RoleId> neutralTickets = data.NeutralSettings.Where(x => x.Value > 0 && x.Value < 10).SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToList();
            List<RoleId> neutralKTickets = data.NeutralKillerSettings.Where(x => x.Value > 0 && x.Value < 10).SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToList();
            List<RoleId> impostorTickets = data.ImpSettings.Where(x => x.Value > 0 && x.Value < 10).SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToList();


            //Assign Impostors First
            while (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0)
            {
                // Imp Roles To List
                Dictionary<RoleType, List<RoleId>> ImpRolesToAssign = new();
                if (data.Impostors.Count > 0 && data.MaxImpostorRoles > 0 && impostorTickets.Count > 0)
                    ImpRolesToAssign.Add(RoleType.Impostor, impostorTickets);

                // Randomly select a pool of role tickets to assign a role from next Impostor role
                // then select one of the roles from the selected pool to a player 
                // and remove all tickets of this role (and any potentially blocked role pairings) from the pool(s)
                RoleType roleType = ImpRolesToAssign.Keys.ElementAt(rnd.Next(0, ImpRolesToAssign.Keys.Count));
                List<PlayerControl> players = data.Impostors;
                RoleId roleId = ImpRolesToAssign[roleType][rnd.Next(0, ImpRolesToAssign[roleType].Count)];
                SetRoleToRandomPlayer(roleId, players);
                ImpRolesToAssign[roleType].RemoveAll(x => x == roleId);

                if (roleId == RoleId.Cultist)
                {
                    PlayerControl player = data.Impostors.FirstOrDefault();

                    player.TurnToCrewmate();

                    data.Impostors.Remove(player);
                    data.Crewmates.Add(player);
                }
                else
                {
                    ImpRolesToAssign[RoleType.Impostor].RemoveAll(x => x == RoleId.Cultist);
                }

                if (CustomOptionHolder.BlockedRolePairings.TryGetValue(roleId, out RoleId[] pairings))
                {
                    foreach (RoleId blockedRoleId in pairings)
                    {
                        // Remove tickets of blocked roles from all pools
                        crewmateTickets.RemoveAll(x => x == blockedRoleId);
                        neutralTickets.RemoveAll(x => x == blockedRoleId);
                        neutralKTickets.RemoveAll(x => x == blockedRoleId);
                        impostorTickets.RemoveAll(x => x == blockedRoleId);
                    }
                }
                data.MaxImpostorRoles--;
            }

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while (
                data.Crewmates.Count > 0 && (
                    (data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0)
                    || (data.MaxNeutralRoles > 0 && neutralTickets.Count > 0)
                    || (data.MaxNeutralKillerRoles > 0 && neutralKTickets.Count > 0)))
            {

                // Chance Crew/Neutral/Nk roles to list
                Dictionary<RoleType, List<RoleId>> CrewRolesToAssign = new();
                if (data.Crewmates.Count > 0)
                {
                    if (data.MaxCrewmateRoles > 0 && crewmateTickets.Count > 0)
                        CrewRolesToAssign.Add(RoleType.Crewmate, crewmateTickets);
                    if (data.MaxNeutralRoles > 0 && neutralTickets.Count > 0)
                        CrewRolesToAssign.Add(RoleType.Neutral, neutralTickets);
                    if (data.MaxNeutralKillerRoles > 0 && neutralKTickets.Count > 0)
                        CrewRolesToAssign.Add(RoleType.NeutralKiller, neutralKTickets);
                }

                // Randomly select a pool of role tickets to assign a role from next (Crewmate, Neutral, or NK role)
                // then select one of the roles from the selected pool to a player 
                // and remove all tickets of this role (and any potentially blocked role pairings) from the pool(s)
                RoleType roleType = CrewRolesToAssign.Keys.ElementAt(rnd.Next(0, CrewRolesToAssign.Keys.Count));
                List<PlayerControl> players = data.Crewmates;
                RoleId roleId = CrewRolesToAssign[roleType][rnd.Next(0, CrewRolesToAssign[roleType].Count)];
                SetRoleToRandomPlayer(roleId, players);
                CrewRolesToAssign[roleType].RemoveAll(x => x == roleId);

                if (CustomOptionHolder.BlockedRolePairings.TryGetValue(roleId, out RoleId[] pairings))
                    foreach (RoleId blockedRoleId in pairings)
                    {
                        // Remove tickets of blocked roles from all pools
                        crewmateTickets.RemoveAll(x => x == blockedRoleId);
                        neutralTickets.RemoveAll(x => x == blockedRoleId);
                        neutralKTickets.RemoveAll(x => x == blockedRoleId);
                        impostorTickets.RemoveAll(x => x == blockedRoleId);
                    }

                // Adjust the role limit
                switch (roleType)
                {
                    case RoleType.Crewmate:
                        data.MaxCrewmateRoles--;
                        break;
                    case RoleType.Neutral:
                        data.MaxNeutralRoles--;
                        break;
                    case RoleType.NeutralKiller:
                        data.MaxNeutralKillerRoles--;
                        break;
                }
            }
        }

        private static void AssignRoleTargets()
        {
            if (Executioner.Player != null)
            {
                List<PlayerControl> possibleTargets = PlayerControl.AllPlayerControls.GetFastEnumerator().Where(player =>
                {
                    NetworkedPlayerInfo data = player.Data;
                    if (data.IsDead || data.Disconnected || data.Role.IsImpostor)
                        return false;

                    return
                        !player.IsNeutral() &&
                        player != Romantic.Lover &&
                        player != Spy.Player &&
                        player != Mayor.Player &&
                        player != Vigilante.Player &&
                        player != Sheriff.Player &&
                        !Spectator.IsSpectator(player.PlayerId);
                }).ToList();

                if (possibleTargets.Count == 0)
                {
                    RPCProcedure.ExecutionerChangeRole();
                    RPCProcedure.Send(CustomRPC.ExecutionerChangeRole);
                }
                else
                {
                    PlayerControl target = possibleTargets[rnd.Next(0, possibleTargets.Count)];
                    RPCProcedure.Send(CustomRPC.ExecutionerSetTarget, target);
                    Executioner.Target = target;
                }
            }
        }

        private static void AssignAssassins()
        {
            List<PlayerControl> impPlayers = PlayerControl.AllPlayerControls.GetFastEnumerator().Where(player => player.Data.Role.IsImpostor).ToList();

            int assassincount = CustomOptionHolder.AssassinCount.GetSelection();
            int modifierCount = Mathf.Min(impPlayers.Count, assassincount);

            AssignModifiers(impPlayers, new List<RoleId> { RoleId.Assassin }, modifierCount, modifierCount); // Assign Assassin
        }

        private static void AssignModifiers()
        {
            //Assign Cosmetic Modifiers
            AssignModifiers(
                PlayerControl.AllPlayerControls.GetFastEnumerator().ToList(),
                new List<RoleId> { RoleId.Mini, RoleId.Giant },
                CustomOptionHolder.ModifierCosmeticMin.GetSelection(),
                CustomOptionHolder.ModifierCosmeticMax.GetSelection()
            );

            //Assign Evil Modifiers
            List<PlayerControl> players = PlayerControl.AllPlayerControls.GetFastEnumerator().ToList(); // New player list because cosmetic modifiers don't count

            AssignModifiers(
                players,
                new List<RoleId>() { RoleId.Clutch },
                CustomOptionHolder.ModifiersImpCountMin.GetSelection(),
                CustomOptionHolder.ModifiersImpCountMax.GetSelection()
            );

            //Assign Other Modifiers
            AssignModifiers(
                players,
                new List<RoleId> { RoleId.Spiteful, RoleId.Sleepwalker, RoleId.Gopher, RoleId.Sniper, RoleId.Ascended },
                CustomOptionHolder.ModifiersMiscCountMin.GetSelection(),
                CustomOptionHolder.ModifiersMiscCountMax.GetSelection()
            );
        }

        private static void AssignModifiers(List<PlayerControl> players, List<RoleId> allModifiers, int modifierMin, int modifierMax)
        {
            modifierMin = Mathf.Min(modifierMin, modifierMax);
            int modifierCount = Mathf.Min(players.Count, rnd.Next(modifierMin, modifierMax));
            if (modifierCount == 0)
                return;

            List<RoleId> ensuredModifiers = new();
            List<RoleId> chanceModifiers = new();

            foreach (RoleId modifier in allModifiers)
            {
                if (GetSelectionForRoleId(modifier) >= 10)
                    ensuredModifiers.AddRange(Enumerable.Repeat(modifier, GetSelectionForRoleId(modifier, true) / 10));
                else
                    chanceModifiers.AddRange(Enumerable.Repeat(modifier, GetSelectionForRoleId(modifier, true)));
            }

            modifierCount -= AssignModifiersToPlayers(ensuredModifiers, players, modifierCount); // Assign ensured modifier

            if (modifierCount <= 0)
                return;
            int chanceModifierCount = Mathf.Min(modifierCount, chanceModifiers.Count);
            List<RoleId> chanceModifierToAssign = new();
            while (chanceModifierCount > 0 && chanceModifiers.Count > 0)
            {
                RoleId modifierId = chanceModifiers[rnd.Next(0, chanceModifiers.Count)];
                chanceModifierToAssign.Add(modifierId);

                int modifierSelection = GetSelectionForRoleId(modifierId);
                while (modifierSelection > 0)
                {
                    chanceModifiers.Remove(modifierId);
                    modifierSelection--;
                }
                chanceModifierCount--;
            }

            AssignModifiersToPlayers(chanceModifierToAssign, players, modifierCount); // Assign chance modifier

        }

        private static void SetRoleToRandomPlayer(RoleId roleId, List<PlayerControl> playerList)
        {
            PlayerControl player = playerList.RemoveAndReturn(rnd.Next(0, playerList.Count));

            RPCProcedure.Send(CustomRPC.SetRole, (byte)roleId, player);
            RPCProcedure.SetRole(roleId, player);
        }

        private static PlayerControl SetModifierToRandomPlayer(RoleId modifierId, List<PlayerControl> playerList)
        {
            if (playerList.Count == 0)
                return null;

            PlayerControl player = playerList.RemoveAndReturn(rnd.Next(0, playerList.Count));

            RPCProcedure.Send(CustomRPC.SetModifier, (byte)modifierId, player);
            RPCProcedure.SetModifier(modifierId, player);

            return player;
        }

        private static int AssignModifiersToPlayers(List<RoleId> modifiers, List<PlayerControl> playerList, int modifierCount)
        {
            modifiers = modifiers.OrderBy(x => rnd.Next()).ToList(); // randomize list
            int used = 0;

            while (modifierCount > 0 && modifiers.Count > 0 && playerList.Count > 0)
            {
                var modifier = modifiers[rnd.Next(0, modifiers.Count)];
                bool handled = false;

                if (modifier == RoleId.Clutch)
                {
                    List<PlayerControl> impPlayers = playerList.Where(player => player.IsClutchEligible()).ToList();
                    if (impPlayers.Count > 0)
                    {
                        playerList.Remove(SetModifierToRandomPlayer(modifier, impPlayers));
                        handled = true;
                    }
                }
                else if (modifier == RoleId.Gopher)
                {
                    List<PlayerControl> playersWhoCanVent = playerList.Where(player => player.IsGopherEligible()).ToList();

                    if (playersWhoCanVent.Count > 0)
                    {
                        playerList.Remove(SetModifierToRandomPlayer(modifier, playersWhoCanVent));
                        handled = true;
                    }
                }
                else if (modifier == RoleId.Ascended)
                {
                    List<PlayerControl> canGetAscended = playerList.Where(player => player.IsAscendedEligible()).ToList();
                    if (canGetAscended.Count > 0)
                    {
                        playerList.Remove(SetModifierToRandomPlayer(modifier, canGetAscended));
                        handled = true;
                    }
                }
                else if (modifier == RoleId.Sniper)
                {
                    List<PlayerControl> canGetSniper = playerList.Where(player => player.IsKiller()).ToList();
                    if (canGetSniper.Count > 0)
                    {
                        playerList.Remove(SetModifierToRandomPlayer(modifier, canGetSniper));
                        handled = true;
                    }
                }
                else if (modifier == RoleId.Spiteful)
                {
                    List<PlayerControl> canGetSpiteful = playerList.Where(player => !player.IsJester()).ToList();
                    if (canGetSpiteful.Count > 0)
                    {
                        playerList.Remove(SetModifierToRandomPlayer(modifier, canGetSpiteful));
                        handled = true;
                    }
                }
                else if (playerList.Count > 0)
                {
                    playerList.Remove(SetModifierToRandomPlayer(modifier, playerList));
                    handled = true;
                }
                modifiers.Remove(modifier);

                if (handled == true)
                {
                    used++;
                    modifierCount--;
                }
            }
            return used;
        }

        public static int GetSelectionForRoleId(RoleId roleId, bool multiplyQuantity = false)
        {
            int selection = 0;

            switch (roleId)
            {
                case RoleId.Mini:
                    selection = CustomOptionHolder.ModifierMini.GetSelection();
                    break;
                case RoleId.Giant:
                    selection = CustomOptionHolder.ModifierGiant.GetSelection();
                    break;
                case RoleId.Sleepwalker:
                    selection = CustomOptionHolder.ModifierSleepwalker.GetSelection();
                    if (multiplyQuantity)
                        selection *= CustomOptionHolder.ModifierSleepwalkerQuantity.GetQuantity();
                    break;
                case RoleId.Assassin:
                    selection = CustomOptionHolder.AssassinCount.GetSelection() * 10;
                    if (multiplyQuantity)
                        selection *= CustomOptionHolder.AssassinCount.GetQuantity();
                    break;
                case RoleId.Spiteful:
                    selection = CustomOptionHolder.ModifierSpiteful.GetSelection();
                    if (multiplyQuantity)
                        selection *= CustomOptionHolder.ModifierSpitefulCount.GetQuantity();
                    break;
                case RoleId.Clutch:
                    selection = CustomOptionHolder.ModifierClutch.GetSelection();
                    if (multiplyQuantity)
                        selection *= CustomOptionHolder.ModifierClutch.GetQuantity();
                    break;
                case RoleId.Gopher:
                    selection = CustomOptionHolder.ModifierGopher.GetSelection();
                    if (multiplyQuantity)
                        selection *= CustomOptionHolder.ModifierGopherQuantity.GetQuantity();
                    break;
                case RoleId.Sniper:
                    selection = CustomOptionHolder.ModifierSniper.GetSelection();
                    if (multiplyQuantity)
                        selection *= CustomOptionHolder.ModifierSniperQuantity.GetQuantity();
                    break;
                case RoleId.Ascended:
                    selection = CustomOptionHolder.ModifierAscended.GetSelection();
                    if (multiplyQuantity)
                        selection *= CustomOptionHolder.ModifierAscendedQuantity.GetQuantity();
                    break;
            }

            return selection;
        }

        public class RoleAssignmentData
        {
            public List<PlayerControl> Crewmates { get; set; }
            public List<PlayerControl> Impostors { get; set; }
            public Dictionary<RoleId, int> ImpSettings = new();
            public Dictionary<RoleId, int> NeutralSettings = new();
            public Dictionary<RoleId, int> NeutralKillerSettings = new();
            public Dictionary<RoleId, int> CrewSettings = new();
            public int MaxCrewmateRoles { get; set; }
            public int MaxNeutralRoles { get; set; }
            public int MaxNeutralKillerRoles { get; set; }
            public int MaxImpostorRoles { get; set; }
        }

        private enum RoleType
        {
            Crewmate,
            Neutral,
            NeutralKiller,
            Impostor
        }
    }
}
