using System.Collections.Generic;

namespace StellarRoles
{
    public static class CustomOptionDefaultSettings
    {

        public static Dictionary<float, PresetMapping> PresetIdToMapping;


        public static object GetDefaultPresetValue(float id, int preset)
        {
            PresetIdToMapping ??= LoadPresets();

            return PresetIdToMapping.TryGetValue(id, out PresetMapping mapping) ? mapping.GetDefaultValue(preset) : null;
        }

        public static Dictionary<float, PresetMapping> LoadPresets()
        {
            Dictionary<float, PresetMapping> presetIdToMapping = new();

            Dictionary<string, object> crewmateRolesCountMin = new()
            {
                {"Stell's Lobby", 6f },
                {"Streamer Non-Prox", 9f },
                {"Streamer Prox", 9f },
                {"Chaotic", 9f },
                {"Beginner", 9f }
            };
            presetIdToMapping.Add(CustomOptionHolder.CrewmateRolesCountMin.Id, new PresetMapping(crewmateRolesCountMin, 0));

            Dictionary<string, object> crewmateRolesCountMax = new()
            {
                {"Stell's Lobby", 9f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.CrewmateRolesCountMax.Id, new PresetMapping(crewmateRolesCountMax, 0));

            Dictionary<string, object> neutralRolesCountMin = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NeutralRolesCountMin.Id, new PresetMapping(neutralRolesCountMin, 0));


            Dictionary<string, object> neutralRolesCountMax = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 2f },
                {"Streamer Prox", 2f },
                {"Chaotic", 2f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NeutralRolesCountMax.Id, new PresetMapping(neutralRolesCountMax, 0));

            Dictionary<string, object> nKRolesCountMin = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NeutralKillerRolesCountMin.Id, new PresetMapping(nKRolesCountMin, 0));

            Dictionary<string, object> nKRolesCountMax = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 2f },
                {"Chaotic", 3f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NeutralKillerRolesCountMax.Id, new PresetMapping(nKRolesCountMax, 0));

            Dictionary<string, object> nkGainAssassin = new()
            {
                {"Stell's Lobby", "Imp Team Wipe" },
                {"Streamer Non-Prox", "Imp Team Wipe" },
                {"Streamer Prox", "Game Start" },
                {"Chaotic", "Game Start" },
                {"Beginner", "Never" }
            };
            presetIdToMapping.Add(CustomOptionHolder.NeutralKillerGainsAssassin.Id, new PresetMapping(nkGainAssassin, 0));

            //ASSASSIN
            Dictionary<string, object> Assassin = new()
            {
                {"Stell's Lobby", 2f },
                {"Streamer Non-Prox", 2f },
                {"Streamer Prox", 2f },
                {"Chaotic", 2f },
                {"Beginner", 0f }
            };
            presetIdToMapping.Add(CustomOptionHolder.AssassinCount.Id, new PresetMapping(Assassin, 0));

            Dictionary<string, object> AssassinMultipleShotsPerMeeting = new()
            {
                {"Stell's Lobby", false },
                {"Streamer Non-Prox", false },
                {"Streamer Prox", false },
                {"Chaotic", false },
                {"Beginner", false }
            };
            presetIdToMapping.Add(CustomOptionHolder.AssassinMultipleShotsPerMeeting.Id, new PresetMapping(AssassinMultipleShotsPerMeeting, 0));

            Dictionary<string, object> AssassinNumberOfShots = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 5f },
                {"Streamer Prox", 5f },
                {"Chaotic", 5f },
                {"Beginner", 0f }
            };
            presetIdToMapping.Add(CustomOptionHolder.AssassinNumberOfShots.Id, new PresetMapping(AssassinNumberOfShots, 0));

            Dictionary<string, object> guesserSpawnRate = new()
            {
                {"Stell's Lobby", "50%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "40%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.VigilanteSpawnRate.Id, new PresetMapping(guesserSpawnRate, 0));

            Dictionary<string, object> guesserNumberOfShots = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 5f },
                {"Streamer Prox", 5f },
                {"Chaotic", 5f },
                {"Beginner", 5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.VigilanteNumberOfShots.Id, new PresetMapping(guesserNumberOfShots, 0));

            //MORPHLING
            Dictionary<string, object> morphlingSpawnRate = new()
            {
                {"Stell's Lobby", "50%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "50%" },
                {"Chaotic", "40%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MorphlingSpawnRate.Id, new PresetMapping(morphlingSpawnRate, 0));

            Dictionary<string, object> morphlingCooldown = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MorphlingCooldown.Id, new PresetMapping(morphlingCooldown, 0));

            //ask
            Dictionary<string, object> morphlingDuration = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MorphlingDuration.Id, new PresetMapping(morphlingDuration, 0));

            //BOMBER
            Dictionary<string, object> bomberSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "40%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.BomberSpawnRate.Id, new PresetMapping(bomberSpawnRate, 0));

            Dictionary<string, object> bomberBombCooldown = new()
            {
                {"Stell's Lobby", 25f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 25f },
                {"Chaotic", 25f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.BomberBombCooldown.Id, new PresetMapping(bomberBombCooldown, 0));

            Dictionary<string, object> bomberDelay = new()
            {
                {"Stell's Lobby", 7.5f },
                {"Streamer Non-Prox", 7.5f },
                {"Streamer Prox", 7.5f },
                {"Chaotic", 7.5f },
                {"Beginner", 7.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.BomberDelay.Id, new PresetMapping(bomberDelay, 0));

            Dictionary<string, object> bomberTimer = new()
            {
                {"Stell's Lobby", 25f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 25f },
                {"Chaotic", 25f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.BomberTimer.Id, new PresetMapping(bomberTimer, 0));

            /*            Dictionary<string, Object> bomberImpsSeeBombed = new Dictionary<string, Object>()
                          {
                              {"Stell's Lobby", 3f },
                              {"Streamer Non-Prox", 5f },
                              {"Streamer Prox", 5f },
                              {"Chaotic", 1f },
                              {"Beginner", 2f }
                          };

                        presetIdToMapping.Add(CustomOptionHolder.bomberImpsSeeBombed.id, new PresetMapping(bomberImpsSeeBombed, 0));*/

            Dictionary<string, object> bomberCanReport = new()
              {
                  {"Stell's Lobby", false },
                  {"Streamer Non-Prox", false },
                  {"Streamer Prox", false },
                  {"Chaotic", false },
                  {"Beginner", false }
              };

            presetIdToMapping.Add(CustomOptionHolder.BomberCanReport.Id, new PresetMapping(bomberCanReport, 0));

            //CHANGELING
            Dictionary<string, object> changelingSpawnRate = new()
            {
                {"Stell's Lobby", "30%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ChangelingSpawnRate.Id, new PresetMapping(changelingSpawnRate, 0));

            //HACKER
            Dictionary<string, object> hackerSpawnRate = new()
            {
                {"Stell's Lobby", "60%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.HackerSpawnRate.Id, new PresetMapping(hackerSpawnRate, 0));

            //WRAITH
            Dictionary<string, object> wraithSpawnRate = new()
            {
                {"Stell's Lobby", "60%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "40%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithSpawnRate.Id, new PresetMapping(wraithSpawnRate, 0));

            Dictionary<string, object> wraithPhaseDuration = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 4f },
                {"Streamer Prox", 3f },
                {"Chaotic", 4f },
                {"Beginner", 4f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithPhaseDuration.Id, new PresetMapping(wraithPhaseDuration, 0));

            Dictionary<string, object> wraithPhaseCooldown = new()
            {
                {"Stell's Lobby", 15f },
                {"Streamer Non-Prox", 15f },
                {"Streamer Prox", 15f },
                {"Chaotic", 15f },
                {"Beginner", 15f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithPhaseCooldown.Id, new PresetMapping(wraithPhaseCooldown, 0));

            Dictionary<string, object> wraithLantern = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithLantern.Id, new PresetMapping(wraithLantern, 0));

            Dictionary<string, object> wraithLanternCooldown = new()
            {
                {"Stell's Lobby", 27.5f },
                {"Streamer Non-Prox", 27.5f },
                {"Streamer Prox", 25f },
                {"Chaotic", 20f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithLanternCooldown.Id, new PresetMapping(wraithLanternCooldown, 0));

            Dictionary<string, object> wraithLanternDuration = new()
            {
                {"Stell's Lobby",8f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 12f },
                {"Chaotic", 15f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithLanternDuration.Id, new PresetMapping(wraithLanternDuration, 0));

            Dictionary<string, object> wraithInvisibleDuration = new()
            {
                {"Stell's Lobby", 2f },
                {"Streamer Non-Prox", 2f },
                {"Streamer Prox", 3f },
                {"Chaotic", 3f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithInvisibleDuration.Id, new PresetMapping(wraithInvisibleDuration, 0));

            Dictionary<string, object> shadeSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "10%" },
                {"Streamer Prox", "10%" },
                {"Chaotic", "20%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeSpawnRate.Id, new PresetMapping(shadeSpawnRate, 0));

            Dictionary<string, object> shadeCooldown = new()
            {
                {"Stell's Lobby", 17.5f },
                {"Streamer Non-Prox", 17.5f },
                {"Streamer Prox", 17.5f },
                {"Chaotic", 20f },
                {"Beginner", 17.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeCooldown.Id, new PresetMapping(shadeCooldown, 0));

            Dictionary<string, object> shadeDuration = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeDuration.Id, new PresetMapping(shadeDuration, 0));

            Dictionary<string, object> shadeEvidence = new()
            {
                {"Stell's Lobby", 30f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 30f },
                {"Chaotic", 30f },
                {"Beginner", 30f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeEvidence.Id, new PresetMapping(shadeEvidence, 0));

            Dictionary<string, object> shadeKillsToGainBlind = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 3f },
                {"Chaotic", 2f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeKillsToGainBlind.Id, new PresetMapping(shadeKillsToGainBlind, 0));

            Dictionary<string, object> shadeBlindCooldown = new()
            {
                {"Stell's Lobby", 27.5f },
                {"Streamer Non-Prox", 27.5f },
                {"Streamer Prox", 27.5f },
                {"Chaotic", 27.5f },
                {"Beginner", 27.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeBlindCooldown.Id, new PresetMapping(shadeBlindCooldown, 0));

            Dictionary<string, object> shadeBlindRange = new()
            {
                {"Stell's Lobby", ".75x" },
                {"Streamer Non-Prox", ".75x" },
                {"Streamer Prox", ".75x" },
                {"Chaotic", ".75x" },
                {"Beginner", "75x" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeBlindRange.Id, new PresetMapping(shadeBlindRange, 0));


            Dictionary<string, object> shadeBlindDuration = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeBlindDuration.Id, new PresetMapping(shadeBlindDuration, 0));

            Dictionary<string, object> camouflagerSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "10%" },
                {"Streamer Prox", "20%" },
                {"Chaotic", "20%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.CamouflagerSpawnRate.Id, new PresetMapping(camouflagerSpawnRate, 0));

            Dictionary<string, object> camouflagerCooldown = new()
            {
                {"Stell's Lobby", 22.5f },
                {"Streamer Non-Prox", 22.5f },
                {"Streamer Prox", 22.5f },
                {"Chaotic", 22.5f },
                {"Beginner", 22.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.CamouflagerCooldown.Id, new PresetMapping(camouflagerCooldown, 0));

            Dictionary<string, object> camouflagerDuration = new()
            {
                {"Stell's Lobby", 12.5f },
                {"Streamer Non-Prox", 12.5f },
                {"Streamer Prox", 12.5f },
                {"Chaotic", 12.5f },
                {"Beginner", 12.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.CamouflagerDuration.Id, new PresetMapping(camouflagerDuration, 0));

            Dictionary<string, object> vampireSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "0%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "20%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.VampireSpawnRate.Id, new PresetMapping(vampireSpawnRate, 0));

            Dictionary<string, object> vampireKillDelay = new()
            {
                {"Stell's Lobby", 5f },
                {"Streamer Non-Prox", 5f },
                {"Streamer Prox", 5f },
                {"Chaotic", 5f },
                {"Beginner", 5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.VampireKillDelay.Id, new PresetMapping(vampireKillDelay, 0));

            Dictionary<string, object> vampireCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 20f },
                {"Streamer Prox", 20f },
                {"Chaotic", 20f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.VampireCooldown.Id, new PresetMapping(vampireCooldown, 0));

            Dictionary<string, object> vampireKillButton = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off"},
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.VampireKillButton.Id, new PresetMapping(vampireKillButton, 0));

            Dictionary<string, object> jesterSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "40%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.JesterSpawnRate.Id, new PresetMapping(jesterSpawnRate, 0));

            Dictionary<string, object> jesterCanCallEmergency = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.JesterCanCallEmergency.Id, new PresetMapping(jesterCanCallEmergency, 0));

            Dictionary<string, object> jesterCanEnterVents = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.JesterCanEnterVents.Id, new PresetMapping(jesterCanEnterVents, 0));

            Dictionary<string, object> jesterLightsOnVision = new()
            {
                {"Stell's Lobby", .75f },
                {"Streamer Non-Prox", .75f },
                {"Streamer Prox", .75f },
                {"Chaotic", .75f },
                {"Beginner", .75f }
            };
            presetIdToMapping.Add(CustomOptionHolder.JesterLightsOnVision.Id, new PresetMapping(jesterLightsOnVision, 0));

            Dictionary<string, object> jesterLightsOffVision = new()
            {
                {"Stell's Lobby", .5f },
                {"Streamer Non-Prox", .5f },
                {"Streamer Prox", .5f },
                {"Chaotic", .5f },
                {"Beginner", .5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.JesterLightsOffVision.Id, new PresetMapping(jesterLightsOffVision, 0));

            Dictionary<string, object> executionerSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "0%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "10%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ExecutionerSpawnRate.Id, new PresetMapping(executionerSpawnRate, 0));

            Dictionary<string, object> executionerPromotesTo = new()
            {
                {"Stell's Lobby", "Refugee" },
                {"Streamer Non-Prox", "Refugee" },
                {"Streamer Prox", "Refugee" },
                {"Chaotic", "Jester" },
                {"Beginner", "Refugee" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ExecutionerPromotesTo.Id, new PresetMapping(executionerPromotesTo, 0));

            //ASK
            Dictionary<string, object> executionerConvertsImmediately = new()
            {
                {"Stell's Lobby", "Next Meeting" },
                {"Streamer Non-Prox", "Next Meeting" },
                {"Streamer Prox", "Next Meeting" },
                {"Chaotic", "Immediately" },
                {"Beginner", "Next Meeting" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ExecutionerConvertsImmediately.Id, new PresetMapping(executionerConvertsImmediately, 0));

            Dictionary<string, object> arsonistSpawnRate = new()
            {
                {"Stell's Lobby", "40%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "40%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ArsonistSpawnRate.Id, new PresetMapping(arsonistSpawnRate, 0));

            Dictionary<string, object> arsonistCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 20f },
                {"Streamer Prox", 22.5f },
                {"Chaotic", 20f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ArsonistCooldown.Id, new PresetMapping(arsonistCooldown, 0));

            Dictionary<string, object> arsonistDuration = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ArsonistDuration.Id, new PresetMapping(arsonistDuration, 0));

            Dictionary<string, object> arsonistDouseIgniteRoundCooldown = new()
            {
                {"Stell's Lobby", 27.5f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 25f },
                {"Chaotic", 22.5f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ArsonistDouseIgniteRoundCooldown.Id, new PresetMapping(arsonistDouseIgniteRoundCooldown, 0));

            Dictionary<string, object> arsonistMira = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "Off" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DisableArsonistOnMira.Id, new PresetMapping(arsonistMira, 0));

            Dictionary<string, object> bountyHunterSpawnRate = new()
            {
                {"Stell's Lobby", "40%" },
                {"Streamer Non-Prox", "20%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "30%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.BountyHunterSpawnRate.Id, new PresetMapping(bountyHunterSpawnRate, 0));

            Dictionary<string, object> bountyHunterBountyDuration = new()
            {
                {"Stell's Lobby", 25f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 25f },
                {"Chaotic", 25f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.BountyHunterBountyDuration.Id, new PresetMapping(bountyHunterBountyDuration, 0));

            Dictionary<string, object> bountyHunterReducedCooldown = new()
            {
                {"Stell's Lobby", 5f },
                {"Streamer Non-Prox", 7.5f },
                {"Streamer Prox", 5f },
                {"Chaotic", 17.5f },
                {"Beginner", 5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.BountyHunterReducedCooldown.Id, new PresetMapping(bountyHunterReducedCooldown, 0));

            Dictionary<string, object> bountyHunterPunishmentTime = new()
            {
                {"Stell's Lobby", 7.5f },
                {"Streamer Non-Prox", 7.5f },
                {"Streamer Prox", 7.5f },
                {"Chaotic", 32.5f },
                {"Beginner", 7.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.BountyHunterPunishmentTime.Id, new PresetMapping(bountyHunterPunishmentTime, 0));

            Dictionary<string, object> bountyHunterShowArrow = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.BountyHunterShowArrow.Id, new PresetMapping(bountyHunterShowArrow, 0));

            Dictionary<string, object> bountyHunterArrowUpdateIntervall = new()
            {
                {"Stell's Lobby", 2.5f },
                {"Streamer Non-Prox", 2.5f },
                {"Streamer Prox", 2.5f },
                {"Chaotic", 2.5f },
                {"Beginner", 2.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.BountyHunterArrowUpdateIntervall.Id, new PresetMapping(bountyHunterArrowUpdateIntervall, 0));

            Dictionary<string, object> undertakerSpawnRate = new()
            {
                {"Stell's Lobby", "50%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "40%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.UndertakerSpawnRate.Id, new PresetMapping(undertakerSpawnRate, 0));

            Dictionary<string, object> undertakerDragingDelaiAfterKill = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 2f },
                {"Streamer Prox", 4f },
                {"Chaotic", 0f },
                {"Beginner", 4f }
            };
            presetIdToMapping.Add(CustomOptionHolder.UndertakerDragingDelaiAfterKill.Id, new PresetMapping(undertakerDragingDelaiAfterKill, 0));

            Dictionary<string, object> undertakerDragCooldown = new()
            {
                {"Stell's Lobby",   15f },
                {"Streamer Non-Prox", 15f },
                {"Streamer Prox", 15f },
                {"Chaotic", 15f },
                {"Beginner", 15f }
            };
            presetIdToMapping.Add(CustomOptionHolder.UndertakerDragCooldown.Id, new PresetMapping(undertakerDragCooldown, 0));

            Dictionary<string, object> undertakerCanDragAndVent = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.UndertakerCanDragAndVent.Id, new PresetMapping(undertakerCanDragAndVent, 0));

            Dictionary<string, object> cultistSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "20%" },
                {"Streamer Prox", "20%" },
                {"Chaotic", "20%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.CultistSpawnRate.Id, new PresetMapping(cultistSpawnRate, 0));

            Dictionary<string, object> minerSpawnRate = new()
            {
                {"Stell's Lobby", "60%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "40%"},
                {"Chaotic", "50%" },
                {"Beginner", "50%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MinerSpawnRate.Id, new PresetMapping(minerSpawnRate, 0));

            Dictionary<string, object> minerCooldown = new()
            {
                {"Stell's Lobby", 15f },
                {"Streamer Non-Prox", 15f },
                {"Streamer Prox", 15f },
                {"Chaotic", 15f },
                {"Beginner", 15f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MinerCooldown.Id, new PresetMapping(minerCooldown, 0));

            Dictionary<string, object> minerCharges = new()
            {
                {"Stell's Lobby", 4f },
                {"Streamer Non-Prox", 30f },
                {"Streamer Prox", 3f },
                {"Chaotic", 60f },
                {"Beginner", 4f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MinerCharges.Id, new PresetMapping(minerCharges, 0));

            Dictionary<string, object> minerVentsActiveInstantly = new()
            {
                {"Stell's Lobby", "After Meeting" },
                {"Streamer Non-Prox", "On Place" },
                {"Streamer Prox", "On Place" },
                {"Chaotic", "On Place" },
                {"Beginner", "On Place" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MinerVentsActiveWhen.Id, new PresetMapping(minerVentsActiveInstantly, 0));

            Dictionary<string, object> mayorSpawnRate = new()
            {
                {"Stell's Lobby", "30%" },
                {"Streamer Non-Prox", "60%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "50%" },
                {"Beginner", "50%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MayorSpawnRate.Id, new PresetMapping(mayorSpawnRate, 0));

            Dictionary<string, object> mayorCanSeeVoteColors = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MayorCanSeeVoteColors.Id, new PresetMapping(mayorCanSeeVoteColors, 0));

            Dictionary<string, object> mayorTasksNeededToSeeVoteColors = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 8f },
                {"Streamer Prox", 5f },
                {"Chaotic", 6f },
                {"Beginner", 7f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MayorTasksNeededToSeeVoteColors.Id, new PresetMapping(mayorTasksNeededToSeeVoteColors, 0));

            Dictionary<string, object> engineerSpawnRate = new()
            {
                {"Stell's Lobby", "70%" },
                {"Streamer Non-Prox", "80%" },
                {"Streamer Prox", "60%" },
                {"Chaotic", "80%" },
                {"Beginner", "80%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.EngineerSpawnRate.Id, new PresetMapping(engineerSpawnRate, 0));

            Dictionary<string, object> engineerHasFix = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.EngineerHasFix.Id, new PresetMapping(engineerHasFix, 0));

            Dictionary<string, object> engineerCanVent = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.EngineerCanVent.Id, new PresetMapping(engineerCanVent, 0));

            Dictionary<string, object> engineerHighlightForEvil = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox",  "On" },
                {"Streamer Prox",  "On" },
                {"Chaotic",  "Off" },
                {"Beginner",  "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.EngineerHighlightForEvil.Id, new PresetMapping(engineerHighlightForEvil, 0));

            Dictionary<string, object> engineerAdvancedSabotageRepairs = new()
            {
                {"Stell's Lobby",  "On" },
                {"Streamer Non-Prox",  "On" },
                {"Streamer Prox",  "On" },
                {"Chaotic",  "On" },
                {"Beginner",  "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.EngineerAdvancedSabotageRepairs.Id, new PresetMapping(engineerAdvancedSabotageRepairs, 0));

            Dictionary<string, object> sheriffSpawnRate = new()
            {
                {"Stell's Lobby", "100%" },
                {"Streamer Non-Prox", "90%" },
                {"Streamer Prox", "90%" },
                {"Chaotic", "100%" },
                {"Beginner", "100%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SheriffSpawnRate.Id, new PresetMapping(sheriffSpawnRate, 0));

            Dictionary<string, object> sheriffMisfireKills = new()
            {
                {"Stell's Lobby", "Self" },
                {"Streamer Non-Prox", "Self" },
                {"Streamer Prox", "Self" },
                {"Chaotic", "Both" },
                {"Beginner", "Self" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SheriffMisfireKills.Id, new PresetMapping(sheriffMisfireKills, 0));

            Dictionary<string, object> sheriffCanKillNeutrals = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SheriffCanKillNeutrals.Id, new PresetMapping(sheriffCanKillNeutrals, 0));

            Dictionary<string, object> sheriffCanKillArsonist = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SheriffCanKillArsonist.Id, new PresetMapping(sheriffCanKillArsonist, 0));

            Dictionary<string, object> sheriffCanKillJester = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SheriffCanKillJester.Id, new PresetMapping(sheriffCanKillJester, 0));

            Dictionary<string, object> sheriffCanKillExecutioner = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SheriffCanKillExecutioner.Id, new PresetMapping(sheriffCanKillExecutioner, 0));

            Dictionary<string, object> sheriffCanKilScavengerl = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SheriffCanKillScavenger.Id, new PresetMapping(sheriffCanKilScavengerl, 0));

            Dictionary<string, object> investigatorSpawnRate = new()
            {
                {"Stell's Lobby", "50%" },
                {"Streamer Non-Prox", "60%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "50%"  },
                {"Beginner", "60%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.InvestigatorSpawnRate.Id, new PresetMapping(investigatorSpawnRate, 0));

            Dictionary<string, object> investigatorAnonymousFootprints = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.InvestigatorAnonymousFootprints.Id, new PresetMapping(investigatorAnonymousFootprints, 0));

            Dictionary<string, object> investigatorFootprintInterval = new()
            {
                {"Stell's Lobby", .25f },
                {"Streamer Non-Prox", .25f },
                {"Streamer Prox", .25f },
                {"Chaotic", .25f },
                {"Beginner", .25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.InvestigatorFootprintInterval.Id, new PresetMapping(investigatorFootprintInterval, 0));

            Dictionary<string, object> investigatorFootprintDuration = new()
            {
                {"Stell's Lobby", 2.5f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 4f },
                {"Chaotic", 4f },
                {"Beginner", 3.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.InvestigatorFootprintDuration.Id, new PresetMapping(investigatorFootprintDuration, 0));

            Dictionary<string, object> guardianSpawnRate = new()
           {
                {"Stell's Lobby", "30%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "40%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.GuardianSpawnRate.Id, new PresetMapping(guardianSpawnRate, 0));

            Dictionary<string, object> guardianVisibleToProtected = new()
            {
                {"Stell's Lobby", "Unprotected" },
                {"Streamer Non-Prox", "Unprotected" },
                {"Streamer Prox", "Unprotected" },
                {"Chaotic", "Unprotected" },
                {"Beginner", "Unprotected" }
            };
            presetIdToMapping.Add(CustomOptionHolder.GuardianShieldIsVisibleTo.Id, new PresetMapping(guardianVisibleToProtected, 0));

            Dictionary<string, object> guardianVisionRangeOfShield = new()
            {
                {"Stell's Lobby", 0.5f },
                {"Streamer Non-Prox", 0.5f },
                {"Streamer Prox", 0.5f },
                {"Chaotic", 0.75f },
                {"Beginner", 0.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.GuardianVisionRangeOfShield.Id, new PresetMapping(guardianVisionRangeOfShield, 0));

            Dictionary<string, object> administratorSpawnRate = new()
            {
                {"Stell's Lobby", "40%" },
                {"Streamer Non-Prox", "50%" },
                {"Streamer Prox",  "30%" },
                {"Chaotic",  "30%" },
                {"Beginner",  "50%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.AdministratorSpawnRate.Id, new PresetMapping(administratorSpawnRate, 0));

            Dictionary<string, object> administratorInitialBatteryTime = new()
            {
                {"Stell's Lobby", 4f },
                {"Streamer Non-Prox", 4f },
                {"Streamer Prox", 5f },
                {"Chaotic", 5f },
                {"Beginner", 5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.AdministratorInitialBatteryTime.Id, new PresetMapping(administratorInitialBatteryTime, 0));

            Dictionary<string, object> administratorBatteryTimePerTask = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1.5f },
                {"Streamer Prox", 1.5f },
                {"Chaotic", 2f },
                {"Beginner", 2f }
            };
            presetIdToMapping.Add(CustomOptionHolder.AdministratorBatteryTimePerTask.Id, new PresetMapping(administratorBatteryTimePerTask, 0));

            Dictionary<string, object> administratorSelfChargingBatteryCooldown = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.AdministratorSelfChargingBatteryCooldown.Id, new PresetMapping(administratorSelfChargingBatteryCooldown, 0));

            Dictionary<string, object> administratorDisableRoundOneAccess = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.AdministratorDisableRoundOneAccess.Id, new PresetMapping(administratorDisableRoundOneAccess, 0));

            Dictionary<string, object> medicSpawnRate = new()
            {
                {"Stell's Lobby", "40%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MedicSpawnRate.Id, new PresetMapping(medicSpawnRate, 0));

            Dictionary<string, object> medicInitialBatteryTime = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 4f },
                {"Streamer Prox", 5f },
                {"Chaotic", 5f },
                {"Beginner", 5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MedicInitialBatteryTime.Id, new PresetMapping(medicInitialBatteryTime, 0));

            Dictionary<string, object> medicBatteryTimePerTask = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1.5f },
                {"Streamer Prox", 2f },
                {"Chaotic", 3f },
                {"Beginner", 2f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MedicBatteryTimePerTask.Id, new PresetMapping(medicBatteryTimePerTask, 0));

            Dictionary<string, object> medicSelfChargingBatteryCooldown = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MedicSelfChargingBatteryCooldown.Id, new PresetMapping(medicSelfChargingBatteryCooldown, 0));

            Dictionary<string, object> medicDisableRoundOneAccess = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MedicDisableRoundOneAccess.Id, new PresetMapping(medicDisableRoundOneAccess, 0));

            Dictionary<string, object> medicNonCrewFlash = new()
            {
                {"Stell's Lobby", "0" },
                {"Streamer Non-Prox", "0" },
                {"Streamer Prox", "0" },
                {"Chaotic", "0" },
                {"Beginner", "0" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MedicNonCrewFlash.Id, new PresetMapping(medicNonCrewFlash, 0));

            Dictionary<string, object> trackerSpawnRate = new()
            {
                {"Stell's Lobby", "50%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "20%" },
                {"Chaotic", "30%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.TrackerSpawnRate.Id, new PresetMapping(trackerSpawnRate, 0));


            Dictionary<string, object> numberOfMeetings = new()
            {
                {"Stell's Lobby", 4f },
                {"Streamer Non-Prox", 5f },
                {"Streamer Prox", 4f },
                {"Chaotic", 6f },
                {"Beginner", 4f }
            };
            presetIdToMapping.Add(CustomOptionHolder.MaxNumberOfMeetings.Id, new PresetMapping(numberOfMeetings, 0));

            Dictionary<string, object> spySpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "0%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "20%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.SpySpawnRate.Id, new PresetMapping(spySpawnRate, 0));

            /**
                        Dictionary<string, Object> spyCanDieToSheriff = new Dictionary<string, Object>()
                        {
                            {"Stell's Lobby", 3f },
                            {"Streamer Non-Prox", 5f },
                            {"Streamer Prox", 5f },
                            {"Chaotic", 1f },
                            {"Beginner", 2f }
                        };

                        presetIdToMapping.Add(CustomOptionHolder.spyCanDieToSheriff.id, new PresetMapping(spyCanDieToSheriff, 0));


                        Dictionary<string, Object> spyImpostorsCanKillAnyone = new Dictionary<string, Object>()
                        {
                            {"Stell's Lobby", 3f },
                            {"Streamer Non-Prox", 5f },
                            {"Streamer Prox", 5f },
                            {"Chaotic", 1f },
                            {"Beginner", 2f }
                        };

                        presetIdToMapping.Add(CustomOptionHolder.spyImpostorsCanKillAnyone.id, new PresetMapping(spyImpostorsCanKillAnyone, 0));


                        Dictionary<string, Object> spyCanEnterVents = new Dictionary<string, Object>()
                        {
                            {"Stell's Lobby", 3f },
                            {"Streamer Non-Prox", 5f },
                            {"Streamer Prox", 5f },
                            {"Chaotic", 1f },
                            {"Beginner", 2f }
                        };

                        presetIdToMapping.Add(CustomOptionHolder.spyCanEnterVents.id, new PresetMapping(spyCanEnterVents, 0));


                        Dictionary<string, Object> spyHasImpostorVision = new Dictionary<string, Object>()
                        {
                            {"Stell's Lobby", 3f },
                            {"Streamer Non-Prox", 5f },
                            {"Streamer Prox", 5f },
                            {"Chaotic", 1f },
                            {"Beginner", 2f }
                        };

                        presetIdToMapping.Add(CustomOptionHolder.spyHasImpostorVision.id, new PresetMapping(spyHasImpostorVision, 0));
*/
            Dictionary<string, object> janitorSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "0%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "10%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.JanitorSpawnRate.Id, new PresetMapping(janitorSpawnRate, 0));

            Dictionary<string, object> janitorCooldown = new()
            {
                {"Stell's Lobby", 15f },
                {"Streamer Non-Prox", 15f },
                {"Streamer Prox", 15f },
                {"Chaotic", 15f },
                {"Beginner", 15f }
            };
            presetIdToMapping.Add(CustomOptionHolder.JanitorCooldown.Id, new PresetMapping(janitorCooldown, 0));

            Dictionary<string, object> warlockSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "10%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "0%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WarlockSpawnRate.Id, new PresetMapping(warlockSpawnRate, 0));

            Dictionary<string, object> warlockCooldown = new()
            {
                {"Stell's Lobby", 27.5f },
                {"Streamer Non-Prox", 27.5f },
                {"Streamer Prox", 20f },
                {"Chaotic", 25f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WarlockCooldown.Id, new PresetMapping(warlockCooldown, 0));

            Dictionary<string, object> warlockRootTime = new()
            {
                {"Stell's Lobby", 4f },
                {"Streamer Non-Prox", 4f },
                {"Streamer Prox", 4f },
                {"Chaotic", 4f },
                {"Beginner", 4f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WarlockRootTime.Id, new PresetMapping(warlockRootTime, 0));

            Dictionary<string, object> scavengerSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "10%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "20%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ScavengerSpawnRate.Id, new PresetMapping(scavengerSpawnRate, 0));

            Dictionary<string, object> scavengerCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 20f },
                {"Streamer Prox", 20f },
                {"Chaotic", 20f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ScavengerCooldown.Id, new PresetMapping(scavengerCooldown, 0));

            Dictionary<string, object> scavengerNumberToWin = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 4f },
                {"Chaotic", 3f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ScavengerNumberToWin.Id, new PresetMapping(scavengerNumberToWin, 0));

            Dictionary<string, object> scavengerCanUseVents = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ScavengerCanUseVents.Id, new PresetMapping(scavengerCanUseVents, 0));

            Dictionary<string, object> scavengerCorpsesTrackingCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 15f },
                {"Streamer Prox", 15f },
                {"Chaotic", 15f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ScavengerCorpsesTrackingCooldown.Id, new PresetMapping(scavengerCorpsesTrackingCooldown, 0));

            Dictionary<string, object> scavengerCorpsesTrackingDuration = new()
            {
                {"Stell's Lobby", 5f },
                {"Streamer Non-Prox", 2.5f },
                {"Streamer Prox", 5f },
                {"Chaotic", 5f },
                {"Beginner", 7.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ScavengerCorpsesTrackingDuration.Id, new PresetMapping(scavengerCorpsesTrackingDuration, 0));

            Dictionary<string, object> refugeeVestCooldown = new()
            {
                {"Stell's Lobby", 25f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 25f },
                {"Chaotic", 25f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.VestCooldown.Id, new PresetMapping(refugeeVestCooldown, 0));

            Dictionary<string, object> refugeeVestDuration = new()
            {
                {"Stell's Lobby", 7f },
                {"Streamer Non-Prox", 7f },
                {"Streamer Prox", 7f },
                {"Chaotic", 7f },
                {"Beginner", 7f }
            };
            presetIdToMapping.Add(CustomOptionHolder.VestDuration.Id, new PresetMapping(refugeeVestDuration, 0));

            Dictionary<string, object> refugeeCanBeGuessed = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.CanBeGuessed.Id, new PresetMapping(refugeeCanBeGuessed, 0));

            Dictionary<string, object> watcherSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "20%" },
                {"Chaotic", "30%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WatcherSpawnRate.Id, new PresetMapping(watcherSpawnRate, 0));

            Dictionary<string, object> watcherInitialBatteryTime = new()
            {
                {"Stell's Lobby", 6f },
                {"Streamer Non-Prox", 6f },
                {"Streamer Prox", 6f },
                {"Chaotic", 6f },
                {"Beginner", 6f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WatcherInitialBatteryTime.Id, new PresetMapping(watcherInitialBatteryTime, 0));

            Dictionary<string, object> watcherBatteryTimePerTask = new()
            {
                {"Stell's Lobby", 1.5f },
                {"Streamer Non-Prox", 2.5f },
                {"Streamer Prox", 2.5f },
                {"Chaotic", 2.5f },
                {"Beginner", 2.5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WatcherBatteryTimePerTask.Id, new PresetMapping(watcherBatteryTimePerTask, 0));

            Dictionary<string, object> watcherAnonymousArrows = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WatcherAnonymousArrows.Id, new PresetMapping(watcherAnonymousArrows, 0));

            Dictionary<string, object> watcherSelfChargingBatteryCooldown = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.WatcherSelfChargingBatteryCooldown.Id, new PresetMapping(watcherSelfChargingBatteryCooldown, 0));

            Dictionary<string, object> watcherRoundOneAccess = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WatcherRoundOneAccess.Id, new PresetMapping(watcherRoundOneAccess, 0));

            Dictionary<string, object> watcherNonCrewFlash = new()
            {
                {"Stell's Lobby", "10" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "10" },
                {"Chaotic", "Off" },
                {"Beginner", "10" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WatcherNonCrewFlash.Id, new PresetMapping(watcherNonCrewFlash, 0));


            Dictionary<string, object> detectiveSpawnRate = new()
            {
                {"Stell's Lobby", "30%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "40%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DetectiveSpawnRate.Id, new PresetMapping(detectiveSpawnRate, 0));

            Dictionary<string, object> detectiveInspectDuration = new()
            {
                {"Stell's Lobby", 2.5f },
                {"Streamer Non-Prox", 2f },
                {"Streamer Prox", 2f },
                {"Chaotic", 2f },
                {"Beginner", 2f }
            };
            presetIdToMapping.Add(CustomOptionHolder.DetectiveInspectDuration.Id, new PresetMapping(detectiveInspectDuration, 0));

            Dictionary<string, object> detectiveInspectsPerRound = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 6f },
                {"Streamer Prox", 8f },
                {"Chaotic", 8f },
                {"Beginner", 8f }
            };
            presetIdToMapping.Add(CustomOptionHolder.DetectiveInspectsPerRound.Id, new PresetMapping(detectiveInspectsPerRound, 0));

            Dictionary<string, object> detectiveEnableCrimeScenes = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DetectiveEnableCrimeScenes.Id, new PresetMapping(detectiveEnableCrimeScenes, 0));

            Dictionary<string, object> trapperSpawnRate = new()
            {
                {"Stell's Lobby", "30%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "40%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.TrapperSpawnRate.Id, new PresetMapping(trapperSpawnRate, 0));

            Dictionary<string, object> numberOfTraps = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 3f },
                {"Chaotic", 3f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.TrapperNumberOfTraps.Id, new PresetMapping(numberOfTraps, 0));

            Dictionary<string, object> trapperNumberOfCovers = new()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 3f },
                {"Chaotic", 3f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.TrapperNumberOfCovers.Id, new PresetMapping(trapperNumberOfCovers, 0));

            Dictionary<string, object> trapCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 20f },
                {"Streamer Prox", 20f },
                {"Chaotic", 20f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.TrapperTrapCoverCooldown.Id, new PresetMapping(trapCooldown, 0));

            Dictionary<string, object> trapRootDuration = new()
            {
                {"Stell's Lobby", 4f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 4f },
                {"Chaotic", 3f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.TrapperRootDuration.Id, new PresetMapping(trapRootDuration, 0));

            Dictionary<string, object> romanticSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "40%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.RomanticSpawnRate.Id, new PresetMapping(romanticSpawnRate, 0));

            Dictionary<string, object> romanticProtectCooldown = new()
            {
                {"Stell's Lobby", 25f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 25f },
                {"Chaotic", 25f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.RomanticProtectCooldown.Id, new PresetMapping(romanticProtectCooldown, 0));

            Dictionary<string, object> romanticProtectDuration = new()
            {
                {"Stell's Lobby", 7f },
                {"Streamer Non-Prox", 7f },
                {"Streamer Prox", 7f },
                {"Chaotic", 7f },
                {"Beginner", 7f }
            };
            presetIdToMapping.Add(CustomOptionHolder.RomanticProtectDuration.Id, new PresetMapping(romanticProtectDuration, 0));

            Dictionary<string, object> romanticKnowsTargetRoleWhen = new()
            {
                {"Stell's Lobby", "Instantly" },
                {"Streamer Non-Prox", "Instantly" },
                {"Streamer Prox", "Instantly" },
                {"Chaotic", "Instantly" },
                {"Beginner", "Instantly" }
            };
            presetIdToMapping.Add(CustomOptionHolder.RomanticKnowsTargetRoleWhen.Id, new PresetMapping(romanticKnowsTargetRoleWhen, 0));

            Dictionary<string, object> romanticOnAllImpsDead = new()
            {
                {"Stell's Lobby", "Dead" },
                {"Streamer Non-Prox", "Dead" },
                {"Streamer Prox", "Dead" },
                {"Chaotic", "Dead" },
                {"Beginner", "Dead" }
            };
            presetIdToMapping.Add(CustomOptionHolder.RomanticOnAllImpsDead.Id, new PresetMapping(romanticOnAllImpsDead, 0));

            Dictionary<string, object> romanticLoverSeesLove = new()
            {
                {"Stell's Lobby", "Next Meeting" },
                {"Streamer Non-Prox", "Next Meeting" },
                {"Streamer Prox", "Next Meeting" },
                {"Chaotic", "Next Meeting" },
                {"Beginner", "Next Meeting" }
            };
            presetIdToMapping.Add(CustomOptionHolder.RomanticLoverSeesLove.Id, new PresetMapping(romanticLoverSeesLove, 0));

            /* Dictionary<string, Object> eveyoneKnowsRomanticTargetIsAlive = new Dictionary<string, Object>()
             {
                 {"Stell's Lobby", "On" },
                 {"Streamer Non-Prox", "On" },
                 {"Streamer Prox", "On" },
                 {"Chaotic", "On" },
                 {"Beginner", "On" }
             };
             presetIdToMapping.Add(CustomOptionHolder.eveyoneKnowsRomanticTargetIsAlive.id, new PresetMapping(eveyoneKnowsRomanticTargetIsAlive, 0));*/

            Dictionary<string, object> headHunterSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "20%" },
                {"Streamer Prox", "20%" },
                {"Chaotic", "20%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.HeadHunterSpawnRate.Id, new PresetMapping(headHunterSpawnRate, 0));

            Dictionary<string, object> headHunterTrackingCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 20f },
                {"Streamer Prox", 20f },
                {"Chaotic", 20f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.HeadHunterTrackingCooldown.Id, new PresetMapping(headHunterTrackingCooldown, 0));

            Dictionary<string, object> headHunterTrackerDuration = new()
            {
                {"Stell's Lobby", 5f },
                {"Streamer Non-Prox", 5f },
                {"Streamer Prox", 5f },
                {"Chaotic", 5f },
                {"Beginner", 5f }
            };
            presetIdToMapping.Add(CustomOptionHolder.HeadHunterTrackerDuration.Id, new PresetMapping(headHunterTrackerDuration, 0));

            Dictionary<string, object> pyromaniacSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "20%" },
                {"Streamer Prox", "20%" },
                {"Chaotic", "30%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.PyromaniacSpawnRate.Id, new PresetMapping(pyromaniacSpawnRate, 0));

            Dictionary<string, object> pyromaniacDouseCooldown = new()
            {
                {"Stell's Lobby", 25f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 25f },
                {"Chaotic", 25f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.PyromaniacDouseCooldown.Id, new PresetMapping(pyromaniacDouseCooldown, 0));

            Dictionary<string, object> pyromaniacDousedDuration = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.PyromaniacDousedDuration.Id, new PresetMapping(pyromaniacDousedDuration, 0));

            Dictionary<string, object> pyromaniacDouseKillCooldown = new()
            {
                {"Stell's Lobby", 2f },
                {"Streamer Non-Prox", 2f },
                {"Streamer Prox", 3f },
                {"Chaotic", 2f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.PyromaniacDouseKillCooldown.Id, new PresetMapping(pyromaniacDouseKillCooldown, 0));

            /*     Dictionary<string, Object> pyromaniacDousedDisplayInMeetings = new Dictionary<string, Object>()
                 {
                     {"Stell's Lobby", "On" },
                     {"Streamer Non-Prox", "On" },
                     {"Streamer Prox", "On" },
                     {"Chaotic", "Off" },
                     {"Beginner", "On" }
                 };
                 presetIdToMapping.Add(CustomOptionHolder.pyromaniacDousedDisplayInMeetings.id, new PresetMapping(pyromaniacDousedDisplayInMeetings, 0));*/

            Dictionary<string, object> jailorSpawnRate = new()
            {
                {"Stell's Lobby", "40%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "50%" },
                {"Chaotic", "50%"},
                {"Beginner", "50%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.JailorSpawnRate.Id, new PresetMapping(jailorSpawnRate, 0));

            Dictionary<string, object> initialJailCharges = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.InitialJailCharges.Id, new PresetMapping(initialJailCharges, 0));

            Dictionary<string, object> jailorTasksPerRecharge = new()
            {
                {"Stell's Lobby", 4f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 2f },
                {"Chaotic", 3f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.JailorTasksPerRecharge.Id, new PresetMapping(jailorTasksPerRecharge, 0));

            Dictionary<string, object> jailorCanJailSelf = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox",  "On" },
                {"Streamer Prox",  "On" },
                {"Chaotic",  "On" },
                {"Beginner",  "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.JailorCanJailSelf.Id, new PresetMapping(jailorCanJailSelf, 0));

            /*
                        Dictionary<string, Object> saboAccess = new Dictionary<string, Object>()
                        {
                            {"Stell's Lobby", 3f },
                            {"Streamer Non-Prox", 5f },
                            {"Streamer Prox", 5f },
                            {"Chaotic", 1f },
                            {"Beginner", 2f }
                        };

                        presetIdToMapping.Add(CustomOptionHolder.saboAccess.id, new PresetMapping(saboAccess, 0));


                        Dictionary<string, Object> teamsLoseSabo = new Dictionary<string, Object>()
                        {
                            {"Stell's Lobby", 3f },
                            {"Streamer Non-Prox", 5f },
                            {"Streamer Prox", 5f },
                            {"Chaotic", 1f },
                            {"Beginner", 2f }
                        };

                        presetIdToMapping.Add(CustomOptionHolder.teamsLoseSabo.id, new PresetMapping(teamsLoseSabo, 0));
            */

            /*
            Dictionary<string, Object> saboSpawn = new Dictionary<string, Object>()
            {
                {"Stell's Lobby", 3f },
                {"Streamer Non-Prox", 5f },
                {"Streamer Prox", 5f },
                {"Chaotic", 1f },
                {"Beginner", 2f }
            };

            presetIdToMapping.Add(CustomOptionHolder.saboSpawn.id, new PresetMapping(saboSpawn, 0));
            */

            Dictionary<string, object> enabledNKRoles = new()
            {
                {"Stell's Lobby", "Hide" },
                {"Streamer Non-Prox", "Show" },
                {"Streamer Prox", "Hide" },
                {"Chaotic", "Show" },
                {"Beginner", "Hide" }
            };
            presetIdToMapping.Add(CustomOptionHolder.EnableRogueImpostors.Id, new PresetMapping(enabledNKRoles, 0));

            Dictionary<string, object> morphlingIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MorphlingIsNeutral.Id, new PresetMapping(morphlingIsNeutral, 0));

            Dictionary<string, object> vampireIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.VampireIsNeutral.Id, new PresetMapping(vampireIsNeutral, 0));

            Dictionary<string, object> undertakerIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.UndertakerIsNeutral.Id, new PresetMapping(undertakerIsNeutral, 0));

            Dictionary<string, object> wraithIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.WraithIsNeutral.Id, new PresetMapping(wraithIsNeutral, 0));

            Dictionary<string, object> shadeIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShadeIsNeutral.Id, new PresetMapping(shadeIsNeutral, 0));

            Dictionary<string, object> bountyHunterIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic","On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.BountyHunterIsNeutral.Id, new PresetMapping(bountyHunterIsNeutral, 0));

            Dictionary<string, object> bomberIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox","On" },
                {"Streamer Prox", "On"},
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.BomberIsNeutral.Id, new PresetMapping(bomberIsNeutral, 0));

            Dictionary<string, object> minerIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.MinerIsNeutral.Id, new PresetMapping(minerIsNeutral, 0));

            Dictionary<string, object> janitorIsNeutral = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.JanitorIsNeutral.Id, new PresetMapping(janitorIsNeutral, 0));

            Dictionary<string, object> parityCopSpawnRate = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "10%" },
                {"Chaotic", "0%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ParityCopSpawnRate.Id, new PresetMapping(parityCopSpawnRate, 0));

            Dictionary<string, object> parityCopNeutral = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox","On" },
                {"Chaotic","On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ParityCopNeutralsMatchKillers.Id, new PresetMapping(parityCopNeutral, 0));

            Dictionary<string, object> parityCopCompareCooldown = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ParityCopCompareCooldown.Id, new PresetMapping(parityCopCompareCooldown, 0));

            Dictionary<string, object> parityCopFakeCompare = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox",  "On" },
                {"Streamer Prox",  "Off" },
                {"Chaotic",  "Off" },
                {"Beginner",  "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ParityCopFakeCompare.Id, new PresetMapping(parityCopFakeCompare, 0));

            Dictionary<string, object> nightmareSpawnRate = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.NightmareSpawnRate.Id, new PresetMapping(nightmareSpawnRate, 0));

            Dictionary<string, object> nightmareParalyzeCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 20f },
                {"Streamer Prox", 20f },
                {"Chaotic", 20f },
                {"Beginner", 20f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NightmareParalyzeCooldown.Id, new PresetMapping(nightmareParalyzeCooldown, 0));

            Dictionary<string, object> nightmareParalyzeDuration = new()
            {
                {"Stell's Lobby", 7f },
                {"Streamer Non-Prox",  7f },
                {"Streamer Prox",  7f },
                {"Chaotic",  7f },
                {"Beginner",  7f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NightmareParalyzeDuration.Id, new PresetMapping(nightmareParalyzeDuration, 0));

            Dictionary<string, object> nightmareBlindCooldown = new()
            {
                {"Stell's Lobby", 20f },
                {"Streamer Non-Prox", 25f },
                {"Streamer Prox", 22.5f },
                {"Chaotic", 20f },
                {"Beginner", 25f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NightmareBlindCooldown.Id, new PresetMapping(nightmareBlindCooldown, 0));

            Dictionary<string, object> nightmareBlindDuration = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 10f },
                {"Streamer Prox", 10f },
                {"Chaotic", 10f },
                {"Beginner", 10f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NightmareBlindDuration.Id, new PresetMapping(nightmareBlindDuration, 0));

            Dictionary<string, object> nightmareBlindRadius = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.NightmareBlindRadius.Id, new PresetMapping(nightmareBlindRadius, 0));

            Dictionary<string, object> modifierCosmeticMin = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 0f },
                {"Streamer Prox", 0f },
                {"Chaotic", 0f },
                {"Beginner", 0f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierCosmeticMin.Id, new PresetMapping(modifierCosmeticMin, 0));

            Dictionary<string, object> modifierCosmeticMax = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 2f },
                {"Streamer Prox", 2f },
                {"Chaotic", 2f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierCosmeticMax.Id, new PresetMapping(modifierCosmeticMax, 0));

            Dictionary<string, object> modifierMini = new()
            {
                {"Stell's Lobby", "30%" },
                {"Streamer Non-Prox", "40%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "60%" },
                {"Beginner", "40%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierMini.Id, new PresetMapping(modifierMini, 0));

            Dictionary<string, object> modifierMiniSpeed = new()
            {
                {"Stell's Lobby", "1.0x" },
                {"Streamer Non-Prox", ".9x" },
                {"Streamer Prox", ".9x" },
                {"Chaotic", ".9x" },
                {"Beginner", ".9x" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierMiniSpeed.Id, new PresetMapping(modifierMiniSpeed, 0));

            Dictionary<string, object> modifierGiant = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "20%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "10%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierGiant.Id, new PresetMapping(modifierGiant, 0));

            Dictionary<string, object> modifierGiantSpeed = new()
            {
                {"Stell's Lobby", ".9x" },
                {"Streamer Non-Prox", ".9x" },
                {"Streamer Prox", ".9x" },
                {"Chaotic", ".9x" },
                {"Beginner", ".9x" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierGiantSpeed.Id, new PresetMapping(modifierGiantSpeed, 0));

            Dictionary<string, object> modifiersImpCountMin = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 0f },
                {"Streamer Prox", 0f },
                {"Chaotic", 0f },
                {"Beginner", 0f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifiersImpCountMin.Id, new PresetMapping(modifiersImpCountMin, 0));

            Dictionary<string, object> modifiersImpCountMax = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifiersImpCountMax.Id, new PresetMapping(modifiersImpCountMax, 0));

            Dictionary<string, object> modifierClutch = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierClutch.Id, new PresetMapping(modifierClutch, 0));

            Dictionary<string, object> modifierClutchImpact = new()
            {
                {"Stell's Lobby", "30%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "50%" },
                {"Chaotic", "50%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierClutchImpact.Id, new PresetMapping(modifierClutchImpact, 0));

            Dictionary<string, object> modifierClutchCount = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierClutchCount.Id, new PresetMapping(modifierClutchCount, 0));

            Dictionary<string, object> modifiersMiscCountMin = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 0f },
                {"Streamer Prox", 0f },
                {"Chaotic", 0f },
                {"Beginner", 0f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifiersMiscCountMin.Id, new PresetMapping(modifiersMiscCountMin, 0));

            Dictionary<string, object> modifiersMiscCountMax = new()
            {
                {"Stell's Lobby", 0f },
                {"Streamer Non-Prox", 3f },
                {"Streamer Prox", 3f },
                {"Chaotic", 3f },
                {"Beginner", 3f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifiersMiscCountMax.Id, new PresetMapping(modifiersMiscCountMax, 0));

            Dictionary<string, object> modifierSleepwalker = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierSleepwalker.Id, new PresetMapping(modifierSleepwalker, 0));

            Dictionary<string, object> modifierSleepwalkerQuantity = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierSleepwalkerQuantity.Id, new PresetMapping(modifierSleepwalkerQuantity, 0));

            Dictionary<string, object> modifierSpiteful = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "30%" },
                {"Chaotic", "30%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierSpiteful.Id, new PresetMapping(modifierSpiteful, 0));


            Dictionary<string, object> modifierSniper = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "20%" },
                {"Streamer Prox", "20%" },
                {"Chaotic", "30%" },
                {"Beginner", "20%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierSniper.Id, new PresetMapping(modifierSniper, 0));

            Dictionary<string, object> modifierSpitefulImpact = new()
            {
                {"Stell's Lobby", "50%" },
                {"Streamer Non-Prox", "50%" },
                {"Streamer Prox", "50%" },
                {"Chaotic", "50%" },
                {"Beginner", "50%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierSpitefulImpact.Id, new PresetMapping(modifierSpitefulImpact, 0));

            Dictionary<string, object> modifierGopher = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "30%" },
                {"Streamer Prox", "40%" },
                {"Chaotic", "50%" },
                {"Beginner", "30%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierGopher.Id, new PresetMapping(modifierGopher, 0));

            Dictionary<string, object> modifierGohpherQuantity = new()
            {
                {"Stell's Lobby", 1f },
                {"Streamer Non-Prox", 1f },
                {"Streamer Prox", 1f },
                {"Chaotic", 1f },
                {"Beginner", 1f }
            };
            presetIdToMapping.Add(CustomOptionHolder.ModifierGopherQuantity.Id, new PresetMapping(modifierGohpherQuantity, 0));



            Dictionary<string, object> shieldFirstKill = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.ShieldFirstKill.Id, new PresetMapping(shieldFirstKill, 0));

            Dictionary<string, object> restrictAdminOnMira = new()
            {
                {"Stell's Lobby", 10f },
                {"Streamer Non-Prox", 7f },
                {"Streamer Prox", 2f },
                {"Chaotic", 0f },
                {"Beginner", 0f }
            };

            presetIdToMapping.Add(CustomOptionHolder.RestrictAdminOnMira.Id, new PresetMapping(restrictAdminOnMira, 0));

            Dictionary<string, object> restrictCamsOnSkeld = new()
            {
                {"Stell's Lobby", 6f },
                {"Streamer Non-Prox", 5f },
                {"Streamer Prox", 0f },
                {"Chaotic", 0f },
                {"Beginner", 0f }
            };

            presetIdToMapping.Add(CustomOptionHolder.RestrictCamsOnSkeld.Id, new PresetMapping(restrictCamsOnSkeld, 0));

            Dictionary<string, object> ghostsHeader = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.GhostsHeader.Id, new PresetMapping(ghostsHeader, 0));

            Dictionary<string, object> ghostsSeeRoles = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.GhostsSeeRoles.Id, new PresetMapping(ghostsSeeRoles, 0));

            Dictionary<string, object> ghostsSeeTasks = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.GhostsSeeTasks.Id, new PresetMapping(ghostsSeeTasks, 0));

            Dictionary<string, object> ghostsSeeModifiers = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.GhostsSeeModifiers.Id, new PresetMapping(ghostsSeeModifiers, 0));

            Dictionary<string, object> ghostsSeeRomanticTarget = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.GhostsSeeRomanticTarget.Id, new PresetMapping(ghostsSeeRomanticTarget, 0));

            Dictionary<string, object> noCamsFirstRound = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.NoCamsFirstRound.Id, new PresetMapping(noCamsFirstRound, 0));

            Dictionary<string, object> joustingRoleNKWin = new()
            {
                {"Stell's Lobby", "Off" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "On" },
                {"Beginner", "Off" }
            };

            presetIdToMapping.Add(CustomOptionHolder.JoustingRoleNKWin.Id, new PresetMapping(joustingRoleNKWin, 0));

            Dictionary<string, object> dynamicMap = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "On" },
                {"Beginner", "On" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DynamicMap.Id, new PresetMapping(dynamicMap, 0));

            Dictionary<string, object> dynamicMapEnableSkeld = new()
            {
                {"Stell's Lobby", "20%" },
                {"Streamer Non-Prox", "10%" },
                {"Streamer Prox",  "10%" },
                {"Chaotic",  "10%" },
                {"Beginner", "10%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DynamicMapEnableSkeld.Id, new PresetMapping(dynamicMapEnableSkeld, 0));

            Dictionary<string, object> dynamicMapEnableMira = new()
            {
                {"Stell's Lobby", "30%"},
                {"Streamer Non-Prox", "10%" },
                {"Streamer Prox","10%" },
                {"Chaotic", "10%" },
                {"Beginner", "10%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DynamicMapEnableMira.Id, new PresetMapping(dynamicMapEnableMira, 0));

            Dictionary<string, object> dynamicMapEnableAirShip = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "0%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "10%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DynamicMapEnableAirShip.Id, new PresetMapping(dynamicMapEnableAirShip, 0));

            Dictionary<string, object> dynamicMapEnableSubmerged = new()
            {
                {"Stell's Lobby", "0%" },
                {"Streamer Non-Prox", "0%" },
                {"Streamer Prox", "0%" },
                {"Chaotic", "0%" },
                {"Beginner", "0%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DynamicMapEnableSubmerged.Id, new PresetMapping(dynamicMapEnableSubmerged, 0));

            Dictionary<string, object> dynamicMapEnablePolus = new()
            {
                {"Stell's Lobby", "70%" },
                {"Streamer Non-Prox", "70%" },
                {"Streamer Prox", "70%" },
                {"Chaotic", "70%" },
                {"Beginner", "70%" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DynamicMapEnablePolus.Id, new PresetMapping(dynamicMapEnablePolus, 0));

            Dictionary<string, object> turnOffAdministratorForMira = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DisableAdministratorOnMira.Id, new PresetMapping(turnOffAdministratorForMira, 0));

            Dictionary<string, object> turnOffWatcherForSkeld = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "On" },
                {"Streamer Prox", "On" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.DisableWatcherOnSkeld.Id, new PresetMapping(turnOffWatcherForSkeld, 0));

            Dictionary<string, object> crewRoleBlock = new()
            {
                {"Stell's Lobby", "On" },
                {"Streamer Non-Prox", "Off" },
                {"Streamer Prox", "Off" },
                {"Chaotic", "Off" },
                {"Beginner", "Off" }
            };
            presetIdToMapping.Add(CustomOptionHolder.CrewRoleBlock.Id, new PresetMapping(crewRoleBlock, 0));

            return presetIdToMapping;
        }
    }


    public class PresetMapping
    {

        public static readonly Dictionary<int, string> PresetNames = new()
        {
            {1, "Streamer Prox"},
            {2, "Streamer Non-Prox"},
            {3, "Beginner" }, //Vanilla Only
            {4, "Chaotic"},
            {5, "Stell's Lobby"},
        };


        public readonly Dictionary<string, object> Properties;
        public readonly object MainDefault;
        public PresetMapping(Dictionary<string, object> properties, object mainDefault)
        {
            Properties = properties;
            MainDefault = mainDefault;
        }

        public object GetDefaultValue(int preset)
        {
            if (PresetNames.TryGetValue(preset, out string presetName) && Properties.TryGetValue(presetName, out object value))
                return value;
            else
                return null;
        }

    }
}
