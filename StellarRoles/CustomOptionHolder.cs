using System.Collections.Generic;
using UnityEngine;
using Types = StellarRoles.CustomOption.CustomOptionType;

namespace StellarRoles
{
    public class CustomOptionHolder
    {
        private static readonly string[] RoleRates = new string[] { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };
        private static readonly string[] ModifierRates = new string[] { "1", "2", "3" };
        private static readonly string[] Presets = new string[] { "Default", "Streamer Prox", "Streamer Non-Prox", "Beginner", "Chaotic", "Stell's Lobby", "Custom 1", "Custom 2", "Custom 3" };
        private static readonly string[] SpeedRates = new string[] { ".7x", ".8x", ".9x", "1.0x", "1.1x", "1.2x", "1.3x", "1.4x", "1.5x", "1.6x", "1.7x", "1.8x", "1.9x", "2.0x", "2.1x", "2.2x", "2.3x" };
        private static readonly string[] OddRate15 = new string[] { "Off", "0", "2.5", "5", "7.5", "10", "12.5", "15" };
        #region Presets
        public static CustomOption ActivateRoles { get; set; }
        public static CustomOption CrewmateRolesCountMin { get; set; }
        public static CustomOption CrewmateRolesCountMax { get; set; }
        public static CustomOption NeutralRolesCountMin { get; set; }
        public static CustomOption NeutralRolesCountMax { get; set; }
        public static CustomOption ImpostorRolesCountMin { get; set; }
        public static CustomOption ImpostorRolesCountMax { get; set; }
        public static CustomOption NeutralKillerRolesCountMin { get; set; }
        public static CustomOption NeutralKillerRolesCountMax { get; set; }
        public static CustomOption ModifiersMiscCountMin { get; set; }
        public static CustomOption ModifiersMiscCountMax { get; set; }
        public static CustomOption ModifiersImpCountMin { get; set; }
        public static CustomOption ModifiersImpCountMax { get; set; }
        public static CustomOption ModifierCosmeticMin { get; set; }
        public static CustomOption ModifierCosmeticMax { get; set; }
        #endregion Presets

        #region Roles
        public static CustomOption AssassinCount { get; set; }
        public static CustomOption AssassinNumberOfShots { get; set; }
        public static CustomOption AssassinMultipleShotsPerMeeting { get; set; }
        public static CustomOption AssassinCanKillSpy { get; set; }
        public static CustomOption AssassinCanGuessCrewmate { get; set; }
        public static CustomOption NeutralKillerAssassinNumberOfShots { get; set; }
        public static CustomOption NeutralKillerAssassinMultipleShotsPerMeeting { get; set; }
        public static CustomOption NeutralKillerAssassinCanGuessCrewmate { get; set; }

        public static CustomOption MorphlingSpawnRate { get; set; }
        public static CustomOption MorphlingCooldown { get; set; }
        public static CustomOption MorphlingDuration { get; set; }

        public static CustomOption BomberSpawnRate { get; set; }
        public static CustomOption BomberBombCooldown { get; set; }
        public static CustomOption BomberDelay { get; set; }
        public static CustomOption BomberTimer { get; set; }
        public static CustomOption BomberImpsSeeBombed { get; set; }
        public static CustomOption BomberCanReport { get; set; }
        public static CustomOption BomberHotPotatoMode { get; set; }

        public static CustomOption ChangelingSpawnRate { get; set; }

        public static CustomOption WraithSpawnRate { get; set; }
        public static CustomOption WraithPhaseDuration { get; set; }
        public static CustomOption WraithPhaseCooldown { get; set; }
        public static CustomOption WraithLantern { get; set; }
        public static CustomOption WraithLanternCooldown { get; set; }
        public static CustomOption WraithLanternDuration { get; set; }
        public static CustomOption WraithInvisibleDuration { get; set; }

        public static CustomOption ShadeSpawnRate { get; set; }
        public static CustomOption ShadeCooldown { get; set; }
        public static CustomOption ShadeDuration { get; set; }
        public static CustomOption ShadeEvidence { get; set; }
        public static CustomOption ShadeKillsToGainBlind { get; set; }
        public static CustomOption ShadeBlindRange { get; set; }
        public static CustomOption ShadeBlindCooldown { get; set; }
        public static CustomOption ShadeBlindDuration { get; set; }

        public static CustomOption CamouflagerSpawnRate { get; set; }
        public static CustomOption CamouflagerCooldown { get; set; }
        public static CustomOption CamouflagerDuration { get; set; }
        public static CustomOption CamouflagerChargesPerKill { get; set; }

        public static CustomOption VampireSpawnRate { get; set; }
        public static CustomOption VampireKillDelay { get; set; }
        public static CustomOption VampireCooldown { get; set; }
        public static CustomOption VampireKillButton { get; set; }

        public static CustomOption VigilanteSpawnRate { get; set; }
        public static CustomOption VigilanteNumberOfShots { get; set; }
        public static CustomOption VigilanteHasMultipleShotsPerMeeting { get; set; }

        public static CustomOption JesterSpawnRate { get; set; }
        public static CustomOption JesterCanCallEmergency { get; set; }
        public static CustomOption JesterCanEnterVents { get; set; }
        public static CustomOption JesterLightsOnVision { get; set; }
        public static CustomOption JesterLightsOffVision { get; set; }

        public static CustomOption ExecutionerSpawnRate { get; set; }
        public static CustomOption ExecutionerPromotesTo { get; set; }
        public static CustomOption ExecutionerConvertsImmediately { get; set; }

        public static CustomOption ArsonistSpawnRate { get; set; }
        public static CustomOption ArsonistDouseIgniteRoundCooldown { get; set; }
        public static CustomOption ArsonistCooldown { get; set; }
        public static CustomOption ArsonistDuration { get; set; }
        public static CustomOption DisableArsonistOnMira { get; set; }

        public static CustomOption BountyHunterSpawnRate { get; set; }
        public static CustomOption BountyHunterBountyDuration { get; set; }
        public static CustomOption BountyHunterReducedCooldown { get; set; }
        public static CustomOption BountyHunterPunishmentTime { get; set; }
        public static CustomOption BountyHunterShowArrow { get; set; }
        public static CustomOption BountyHunterArrowUpdateIntervall { get; set; }

        public static CustomOption UndertakerSpawnRate { get; set; }
        public static CustomOption UndertakerDragingDelaiAfterKill { get; set; }
        public static CustomOption UndertakerDragCooldown { get; set; }
        public static CustomOption UndertakerCanDragAndVent { get; set; }

        public static CustomOption CultistSpawnRate { get; set; }
        public static CustomOption CultistSpecialRolesEnabled { get; set; }

        public static CustomOption MinerSpawnRate { get; set; }
        public static CustomOption MinerCooldown { get; set; }
        public static CustomOption MinerCharges { get; set; }
        public static CustomOption MinerVentsActiveWhen { get; set; }
        public static CustomOption MinerVentsDelay { get; set; }

        public static CustomOption MayorSpawnRate { get; set; }
        public static CustomOption MayorCanSeeVoteColors { get; set; }
        public static CustomOption MayorTasksNeededToSeeVoteColors { get; set; }
        public static CustomOption MayorCanRetire { get; set; }

        public static CustomOption EngineerSpawnRate { get; set; }
        public static CustomOption EngineerHasFix { get; set; }
        public static CustomOption EngineerCanVent { get; set; }
        public static CustomOption EngineerHighlightForEvil { get; set; }
        public static CustomOption EngineerAdvancedSabotageRepairs { get; set; }

        public static CustomOption SheriffSpawnRate { get; set; }
        public static CustomOption SheriffMisfireKills { get; set; }
        public static CustomOption SheriffCanKillNeutrals { get; set; }
        public static CustomOption SheriffCanKillArsonist { get; set; }
        public static CustomOption SheriffCanKillJester { get; set; }
        public static CustomOption SheriffCanKillExecutioner { get; set; }
        public static CustomOption SheriffCanKillScavenger { get; set; }

        public static CustomOption InvestigatorSpawnRate { get; set; }
        public static CustomOption InvestigatorAnonymousFootprints { get; set; }
        public static CustomOption InvestigatorFootprintInterval { get; set; }
        public static CustomOption InvestigatorFootprintDuration { get; set; }

        public static CustomOption GuardianSpawnRate { get; set; }
        public static CustomOption GuardianShieldVisibilityDelay { get; set; }
        public static CustomOption GuardianShieldFadesOnDeath { get; set; }
        public static CustomOption GuardianVisionRangeOfShield { get; set; }
        public static CustomOption GuardianShieldIsVisibleTo { get; set; }

        public static CustomOption AdministratorSpawnRate { get; set; }
        public static CustomOption AdministratorInitialBatteryTime { get; set; }
        public static CustomOption AdministratorBatteryTimePerTask { get; set; }
        public static CustomOption AdministratorSelfChargingBatteryCooldown { get; set; }
        public static CustomOption AdministratorDisableRoundOneAccess { get; set; }

        public static CustomOption PsychicSpawnRate { get; set; }
        public static CustomOption PsychicPlayerRange { get; set; }
        public static CustomOption PsychicDetectInvisible { get; set; }
        public static CustomOption PsychicDetectInVent { get; set; }

        public static CustomOption MedicSpawnRate { get; set; }
        public static CustomOption MedicNonCrewFlash { get; set; }
        public static CustomOption MedicInitialBatteryTime { get; set; }
        public static CustomOption MedicBatteryTimePerTask { get; set; }
        public static CustomOption MedicSelfChargingBatteryCooldown { get; set; }
        public static CustomOption MedicDisableRoundOneAccess { get; set; }

        public static CustomOption TrackerSpawnRate { get; set; }
        public static CustomOption TrackerDelayDuration { get; set; }
        public static CustomOption TrackerTrackDuration { get; set; }
        public static CustomOption TrackerTracksPerRound { get; set; }
        public static CustomOption TrackerAnonymousArrows { get; set; }

        public static CustomOption TrackerTrackCooldown { get; set; }

        public static CustomOption SpySpawnRate { get; set; }
        public static CustomOption SpyCanDieToSheriff { get; set; }
        public static CustomOption SpyImpostorsCanKillAnyone { get; set; }

        public static CustomOption JanitorSpawnRate { get; set; }
        public static CustomOption JanitorCooldown { get; set; }
        public static CustomOption JanitorInitialCharges { get; set; }
        public static CustomOption JanitorChargesPerKill { get; set; }

        public static CustomOption WarlockSpawnRate { get; set; }
        public static CustomOption WarlockCooldown { get; set; }
        public static CustomOption WarlockRootTime { get; set; }

        public static CustomOption ScavengerSpawnRate { get; set; }
        public static CustomOption ScavengerCooldown { get; set; }
        public static CustomOption ScavengerNumberToWin { get; set; }
        public static CustomOption ScavengerCanUseVents { get; set; }
        public static CustomOption ScavengerShowArrows { get; set; }
        public static CustomOption ScavengerCorpsesTrackingCooldown { get; set; }
        public static CustomOption ScavengerCorpsesTrackingDuration { get; set; }

        public static CustomOption RefugeeSpawnRate { get; set; }
        public static CustomOption VestCooldown { get; set; }
        public static CustomOption VestDuration { get; set; }
        public static CustomOption CanBeGuessed { get; set; }

        public static CustomOption WatcherSpawnRate { get; set; }
        public static CustomOption WatcherNonCrewFlash { get; set; }
        public static CustomOption WatcherInitialBatteryTime { get; set; }
        public static CustomOption WatcherBatteryTimePerTask { get; set; }
        public static CustomOption WatcherAnonymousArrows { get; set; }
        public static CustomOption WatcherSelfChargingBatteryCooldown { get; set; }
        public static CustomOption WatcherRoundOneAccess { get; set; }

        public static CustomOption DetectiveSpawnRate { get; set; }
        public static CustomOption DetectiveInspectDuration { get; set; }
        public static CustomOption DetectiveInspectsPerRound { get; set; }
        public static CustomOption DetectiveEnableCrimeScenes { get; set; }

        public static CustomOption TrapperSpawnRate { get; set; }
        public static CustomOption TrapperNumberOfTraps { get; set; }
        public static CustomOption TrapperNumberOfCovers { get; set; }
        public static CustomOption TrapperTrapCoverCooldown { get; set; }
        public static CustomOption TrapperRootDuration { get; set; }

        public static CustomOption RomanticSpawnRate { get; set; }
        public static CustomOption RomanticProtectCooldown { get; set; }
        public static CustomOption RomanticProtectDuration { get; set; }
        public static CustomOption RomanticKnowsTargetRoleWhen { get; set; }
        public static CustomOption RomanticOnAllImpsDead { get; set; }
        public static CustomOption RomanticLoverSeesLove { get; set; }
        public static CustomOption TurnOffRomanticToRefugee { get; set; }

        public static CustomOption HeadHunterSpawnRate { get; set; }
        public static CustomOption HeadHunterTrackingCooldown { get; set; }
        public static CustomOption HeadHunterTrackerDuration { get; set; }

        public static CustomOption PyromaniacSpawnRate { get; set; }
        public static CustomOption PyromaniacDouseCooldown { get; set; }
        public static CustomOption PyromaniacDousedDuration { get; set; }
        public static CustomOption PyromaniacDouseKillCooldown { get; set; }

        public static CustomOption JailorSpawnRate { get; set; }
        public static CustomOption InitialJailCharges { get; set; }
        public static CustomOption JailorTasksPerRecharge { get; set; }
        public static CustomOption JailorCanJailSelf { get; set; }

        public static CustomOption NeutralKillersGetNonCritSabo { get; set; }
        public static CustomOption ImpsLoseDoors { get; set; }
        public static CustomOption ImpsLooseCritSabo { get; set; }
        public static CustomOption EnableRogueImpostors { get; set; }
        public static CustomOption NeutralKillerGainsAssassin { get; set; }

        public static CustomOption MorphlingIsNeutral { get; set; }
        public static CustomOption VampireIsNeutral { get; set; }
        public static CustomOption UndertakerIsNeutral { get; set; }
        public static CustomOption WarlockIsNeutral { get; set; }
        public static CustomOption CamouflagerIsNeutral { get; set; }
        public static CustomOption WraithIsNeutral { get; set; }
        public static CustomOption ShadeIsNeutral { get; set; }
        public static CustomOption BountyHunterIsNeutral { get; set; }
        public static CustomOption MinerIsNeutral { get; set; }
        public static CustomOption JanitorIsNeutral { get; set; }
        public static CustomOption BomberIsNeutral { get; set; }

        public static CustomOption ParityCopSpawnRate { get; set; }
        public static CustomOption ParityCopNeutralsMatchKillers { get; set; }
        public static CustomOption ParityCopCompareCooldown { get; set; }
        public static CustomOption ParityCopFakeCompare { get; set; }

        public static CustomOption NightmareSpawnRate { get; set; }
        public static CustomOption NightmareParalyzeCooldown { get; set; }
        public static CustomOption NightmareParalyzeDuration { get; set; }
        public static CustomOption NightmareBlindCooldown { get; set; }
        public static CustomOption NightmareBlindDuration { get; set; }
        public static CustomOption NightmareBlindRadius { get; set; }

        #endregion Roles

        #region Modifiers
        public static CustomOption ModifierSleepwalker { get; set; }
        public static CustomOption ModifierSleepwalkerQuantity { get; set; }
        public static CustomOption ModifierSleepwalkerSeesModifier { get; set; }

        public static CustomOption ModifierMini { get; set; }
        public static CustomOption ModifierMiniSpeed { get; set; }

        public static CustomOption ModifierGiant { get; set; }
        public static CustomOption ModifierGiantSpeed { get; set; }

        public static CustomOption ModifierSpiteful { get; set; }
        public static CustomOption ModifierSpitefulImpact { get; set; }
        public static CustomOption ModifierSpitefulCount { get; set; }
        public static CustomOption ModifierSpitefulSeeModifier { get; set; }

        public static CustomOption ModifierClutch { get; set; }
        public static CustomOption ModifierClutchImpact { get; set; }
        public static CustomOption ModifierClutchSeeModifier { get; set; }
        public static CustomOption ModifierClutchCount { get; set; }

        public static CustomOption ModifierGopher { get; set; }
        public static CustomOption ModifierGopherQuantity { get; set; }

        public static CustomOption ModifierSniper { get; set; }
        public static CustomOption ModifierSniperDistance { get; set; }
        public static CustomOption ModifierSniperQuantity { get; set; }

        public static CustomOption ModifierAscended { get; set; }
        public static CustomOption ModifierAscendedQuantity { get; set; }

        #endregion Modifiers
        public static CustomOption MaxNumberOfMeetings { get; set; }
        public static CustomOption AllowParallelMedBayScans { get; set; }
        public static CustomOption DisableMedscanWalking { get; set; }
        public static CustomOption DisableVentCleanEjections { get; set; }
        public static CustomOption ShieldFirstKill { get; set; }
        public static CustomOption RoundOneKilledIndicators { get; set; }
        public static CustomOption DeadCrewPreventTaskWin { get; set; }
        public static CustomOption JoustingRoleNKWin { get; set; }
        public static CustomOption JoustingRoleImpWin { get; set; }
        public static CustomOption HideOutOfSightNametags { get; set; }
        public static CustomOption VentInFog { get; set; }
        public static CustomOption ToggleRoles { get; set; }
        public static CustomOption GameStartKillCD { get; set; }

        public static CustomOption NoCamsFirstRound { get; set; }
        public static CustomOption RestrictAdminOnMira { get; set; }
        public static CustomOption RestrictCamsOnSkeld { get; set; }
        public static CustomOption DynamicMap { get; set; }
        public static CustomOption DynamicMapEnableSkeld { get; set; }
        public static CustomOption DynamicMapEnableMira { get; set; }
        public static CustomOption DynamicMapEnablePolus { get; set; }
        public static CustomOption DynamicMapEnableAirShip { get; set; }
        public static CustomOption DynamicMapEnableSubmerged { get; set; }
        public static CustomOption DisableWatcherOnSkeld { get; set; }
        public static CustomOption DisableAdministratorOnMira { get; set; }

        public static CustomOption GhostsHeader { get; set; }
        public static CustomOption GhostsSeeRoles { get; set; }
        public static CustomOption GhostsSeeTasks { get; set; }
        public static CustomOption GhostsSeeModifiers { get; set; }
        public static CustomOption GhostsSeeRomanticTarget { get; set; }

        public static CustomOption DeveloperSettings { get; set; }
        public static CustomOption LobbySize { get; set; }

        public static CustomOption RoleBlockComms { get; set; }

        public static CustomOption DisableRolesSkeld { get; set; }
        public static CustomOption DisableArsonistOnSkeld { get; set; }
        public static CustomOption DisableScavengerOnSkeld { get; set; }
        public static CustomOption DisableJanitorOnSkeld { get; set; }
        public static CustomOption DisableMedicOnSkeld { get; set; }

        public static CustomOption DisableRolesMira { get; set; }
        public static CustomOption DisableScavengerOnMira { get; set; }
        public static CustomOption DisableJanitorOnMira { get; set; }
        public static CustomOption DisableMedicOnMira { get; set; }
        public static CustomOption DisableMinerOnMira { get; set; }


        public static CustomOption ImposterRoleBlock { get; set; }
        public static CustomOption ImposterKillAbilitiesRoleBlock { get; set; }
        public static CustomOption ImposterAbiltiesRoleBlock { get; set; }

        public static CustomOption NeutralKillerRoleBlock { get; set; }
        public static CustomOption NeutralRoleBlock { get; set; }

        public static CustomOption CrewRoleBlock { get; set; }
        public static CustomOption ParityCopRoleBlock { get; set; }
        public static CustomOption EngineerRoleBlock { get; set; }
        public static CustomOption InvestigatorRoleBlock { get; set; }
        public static CustomOption GuardianRoleBlock { get; set; }
        public static CustomOption TrackerRoleBlock { get; set; }
        public static CustomOption MedicRoleBlock { get; set; }
        public static CustomOption SpyRoleBlock { get; set; }
        public static CustomOption WatcherRoleBlock { get; set; }
        public static CustomOption DetectiveRoleBlock { get; set; }
        public static CustomOption TrapperRoleBlock { get; set; }
        public static CustomOption PsychicRoleBlock { get; set; }


        public static CustomOption EnableBetterPolus { get; set; }
        public static CustomOption MoveVitals { get; set; }
        public static CustomOption VentSystem { get; set; }
        public static CustomOption ColdTempDeathValley { get; set; }
        public static CustomOption WifiChartCourseSwap { get; set; }

        public static CustomOption HackerSpawnRate { get; set; }
        public static CustomOption HackerMaximumDownloadDuration { get; set; }
        public static CustomOption HackerJamChargesPerKill { get; set; }
        public static CustomOption HackerJamCooldown { get; set; }
        public static CustomOption HackerJamDuration { get; set; }

        public static readonly Dictionary<RoleId, RoleId[]> BlockedRolePairings = new()
        {
            { RoleId.Vampire, new[] { RoleId.Warlock, RoleId.Shade, RoleId.Bomber } },
            { RoleId.Warlock, new[] { RoleId.Vampire, RoleId.Shade, RoleId.Bomber } },
            { RoleId.Shade, new[] { RoleId.Warlock, RoleId.Vampire, RoleId.Bomber } },
            { RoleId.Bomber, new[] { RoleId.Warlock, RoleId.Shade, RoleId.Vampire } },
            { RoleId.Scavenger, new[] { RoleId.Janitor, RoleId.JanitorNK } },
            { RoleId.Janitor, new[] { RoleId.Scavenger } },
            { RoleId.JanitorNK, new[] { RoleId.Scavenger } },
            { RoleId.Cultist, new[] { RoleId.Spy, RoleId.Romantic, RoleId.Executioner, RoleId.HeadHunter } },
            { RoleId.Spy, new[] { RoleId.Cultist } },
            { RoleId.Romantic, new[] { RoleId.Cultist } },
            { RoleId.Executioner, new[] { RoleId.Cultist } },
            { RoleId.HeadHunter, new[] { RoleId.Cultist } },
        };

        public static string cs(Color c, string s)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void Load()
        {
            // Using new id's for the options to not break compatibilty with older versions

            CustomOption.VanillaSettings = StellarRolesPlugin.Instance.Config.Bind("Preset0", "VanillaOptions", "");

            CustomOption.Create(0, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Preset"), Presets, null, true);

            ActivateRoles = CustomOption.Create(1, Types.General, cs(new Color(204f / 255f, 204f / 255f, 0, 1f), "Enable Mod Roles &\nBlock Vanilla Roles"), true, null, true);

            #region Impostors
            ImpostorRolesCountMin = CustomOption.Create(100, Types.Impostor, "Minimum " + cs(Palette.ImpostorRed, "Impostor") + " Roles", 2f, 0f, 3f, 1f, null, true);
            ImpostorRolesCountMax = CustomOption.Create(101, Types.Impostor, "Maximum " + cs(Palette.ImpostorRed, "Impostor") + " Roles", 2f, 0f, 3f, 1f);

            //Assassin
            AssassinCount = CustomOption.Create(200, Types.Impostor, cs(Palette.ImpostorRed, "Assassin"), 0f, 0f, 3f, 1f, null, true);
            AssassinNumberOfShots = CustomOption.Create(201, Types.Impostor, "Number Of Shots", 5f, 0f, 15f, 1f, AssassinCount);
            AssassinMultipleShotsPerMeeting = CustomOption.Create(202, Types.Impostor, "Limit One Shot Per Meeting", false, AssassinCount);
            AssassinCanGuessCrewmate = CustomOption.Create(205, Types.Impostor, "Can Guess Crewmate", false, AssassinCount);

            //Bomber
            BomberSpawnRate = CustomOption.Create(250, Types.Impostor, cs(Palette.ImpostorRed, "Bomber"), RoleRates, null, true);
            BomberBombCooldown = CustomOption.Create(251, Types.Impostor, "Bomb Cooldown", 10f, 5f, 60f, 2.5f, BomberSpawnRate);
            BomberDelay = CustomOption.Create(252, Types.Impostor, "Bomb Delay", 5f, 1f, 10f, 0.5f, BomberSpawnRate);
            BomberTimer = CustomOption.Create(253, Types.Impostor, "Bomb Timer", 25f, 5f, 30f, 5f, BomberSpawnRate);
            BomberImpsSeeBombed = CustomOption.Create(254, Types.Impostor, "Partners See Bombed Target", true, BomberSpawnRate);
            BomberCanReport = CustomOption.Create(255, Types.Impostor, "Bombed Target Can Report", false, BomberSpawnRate);
            BomberHotPotatoMode = CustomOption.Create(256, Types.Impostor, "Hot Potato Mode", false, BomberSpawnRate);

            //BountyHunter
            BountyHunterSpawnRate = CustomOption.Create(750, Types.Impostor, cs(Palette.ImpostorRed, "Bounty Hunter"), RoleRates, null, true);
            BountyHunterBountyDuration = CustomOption.Create(751, Types.Impostor, "Bounty Duration", 30f, 10f, 180f, 5f, BountyHunterSpawnRate);
            BountyHunterReducedCooldown = CustomOption.Create(752, Types.Impostor, "Bounty Kill Cooldown Bonus", 15f, 0f, 30f, 2.5f, BountyHunterSpawnRate);
            BountyHunterPunishmentTime = CustomOption.Create(753, Types.Impostor, "Non-Bounty Kill Cooldown Penalty", 10f, 0f, 60f, 2.5f, BountyHunterSpawnRate);
            BountyHunterShowArrow = CustomOption.Create(754, Types.Impostor, "Enable Bounty Arrow", true, BountyHunterSpawnRate);
            BountyHunterArrowUpdateIntervall = CustomOption.Create(755, Types.Impostor, "Arrow Update Interval", 2.5f, 2.5f, 60f, 2.5f, BountyHunterShowArrow);

            //Camouflager
            CamouflagerSpawnRate = CustomOption.Create(550, Types.Impostor, cs(Palette.ImpostorRed, "Camouflager"), RoleRates, null, true);
            CamouflagerCooldown = CustomOption.Create(551, Types.Impostor, "Camouflage Cooldown", 22.5f, 10f, 60f, 2.5f, CamouflagerSpawnRate);
            CamouflagerDuration = CustomOption.Create(552, Types.Impostor, "Camouflage Duration", 12.5f, 5f, 20f, 2.5f, CamouflagerSpawnRate);
            CamouflagerChargesPerKill = CustomOption.Create(570, Types.Impostor, "Charges Per Kill", 2f, 1f, 5f, 1f, CamouflagerSpawnRate);

            //Changling
            ChangelingSpawnRate = CustomOption.Create(350, Types.Impostor, cs(Palette.ImpostorRed, "Changeling"), RoleRates, null, true);

            //Cultist
            CultistSpawnRate = CustomOption.Create(600, Types.Impostor, cs(Palette.ImpostorRed, "Cultist"), RoleRates, null, true);
            CultistSpecialRolesEnabled = CustomOption.Create(8011, Types.Impostor, cs(Cultist.Color, "Converted Role\nResembles Previous Role"), false, CultistSpawnRate);

            // Hacker
            HackerSpawnRate = CustomOption.Create(610, Types.Impostor, cs(Hacker.Color, "Hacker"), RoleRates, null, true);
            HackerMaximumDownloadDuration = CustomOption.Create(611, Types.Impostor, "Max Download Duration", 10f, 3f, 15f, 1f, HackerSpawnRate);
            HackerJamChargesPerKill = CustomOption.Create(612, Types.Impostor, "Jam Charges Per Kill", 2f, 0f, 5f, 1f, HackerSpawnRate);
            HackerJamCooldown = CustomOption.Create(613, Types.Impostor, "Jam Cooldown", 20f, 10f, 35f, 2.5f, HackerJamChargesPerKill);
            HackerJamDuration = CustomOption.Create(614, Types.Impostor, "Jam Duration", 12.5f, 5f, 20f, 2.5f, HackerJamChargesPerKill);

            //Janitor
            JanitorSpawnRate = CustomOption.Create(650, Types.Impostor, cs(Palette.ImpostorRed, "Janitor"), RoleRates, null, true);
            JanitorCooldown = CustomOption.Create(651, Types.Impostor, "Clean Cooldown", 30f, 10f, 60f, 2.5f, JanitorSpawnRate);
            JanitorInitialCharges = CustomOption.Create(652, Types.Impostor, "Initial Clean Charges", 10f, 1f, 15f, 1f, JanitorSpawnRate);
            JanitorChargesPerKill = CustomOption.Create(653, Types.Impostor, "Charges Per Kill", 2f, 0f, 3f, 1f, JanitorSpawnRate);

            //Miner
            MinerSpawnRate = CustomOption.Create(400, Types.Impostor, cs(Palette.ImpostorRed, "Miner"), RoleRates, null, true);
            MinerCooldown = CustomOption.Create(401, Types.Impostor, "Mine Cooldown", 15f, 10f, 60f, 2.5f, MinerSpawnRate);
            MinerCharges = CustomOption.Create(402, Types.Impostor, "Maximum Miner Vents", 12f, 3f, 60f, 1f, MinerSpawnRate);
            MinerVentsActiveWhen = CustomOption.Create(403, Types.Impostor, "Miner Vents Are Active", new string[] { "After Meeting", "On Place" }, MinerSpawnRate);
            MinerVentsDelay = CustomOption.Create(404, Types.Impostor, "Vent Activation Delay", 15f, 0f, 30f, 2.5f, MinerVentsActiveWhen);

            //Morphling
            MorphlingSpawnRate = CustomOption.Create(300, Types.Impostor, cs(Palette.ImpostorRed, "Morphling"), RoleRates, null, true);
            MorphlingCooldown = CustomOption.Create(301, Types.Impostor, "Morph Cooldown", 20f, 5f, 60f, 2.5f, MorphlingSpawnRate);
            MorphlingDuration = CustomOption.Create(302, Types.Impostor, "Morph Duration", 10f, 1f, 20f, 0.5f, MorphlingSpawnRate);

            //Shade
            ShadeSpawnRate = CustomOption.Create(500, Types.Impostor, cs(Palette.ImpostorRed, "Shade"), RoleRates, null, true);
            ShadeCooldown = CustomOption.Create(501, Types.Impostor, "Vanish Cooldown", 30f, 2.5f, 60f, 2.5f, ShadeSpawnRate);
            ShadeDuration = CustomOption.Create(502, Types.Impostor, "Vanish Duration", 10f, 1f, 60f, 1f, ShadeSpawnRate);
            ShadeEvidence = CustomOption.Create(503, Types.Impostor, "Evidence Duration", 10f, 1f, 30f, 1f, ShadeSpawnRate);
            ShadeKillsToGainBlind = CustomOption.Create(505, Types.Impostor, "Kills To Gain Blind", 3f, 1f, 10f, 1f, ShadeSpawnRate);
            ShadeBlindRange = CustomOption.Create(520, Types.Impostor, "Blind Range", new string[] { ".75x", "1x", "1.25x", "1.5x", "1.75x", "2x", "2.25x", "2.5x", "2.75x", "3x", "Global" }, ShadeSpawnRate);
            ShadeBlindCooldown = CustomOption.Create(506, Types.Impostor, "Blind Cooldown", 25f, 5f, 60f, 2.5f, ShadeSpawnRate);
            ShadeBlindDuration = CustomOption.Create(507, Types.Impostor, "Blind Duration", 10f, 5f, 15f, 1f, ShadeSpawnRate);

            //Undertaker
            UndertakerSpawnRate = CustomOption.Create(508, Types.Impostor, cs(Palette.ImpostorRed, "Undertaker"), RoleRates, null, true);
            UndertakerDragingDelaiAfterKill = CustomOption.Create(509, Types.Impostor, "Drag Cooldown After Kill", 0f, 0f, 15f, 1f, UndertakerSpawnRate);
            UndertakerDragCooldown = CustomOption.Create(510, Types.Impostor, "Drag Cooldown", 10f, 5f, 30f, 2.5f, UndertakerSpawnRate);
            UndertakerCanDragAndVent = CustomOption.Create(511, Types.Impostor, "Can Vent While Dragging", true, UndertakerSpawnRate);

            //Vampire
            VampireSpawnRate = CustomOption.Create(553, Types.Impostor, cs(Palette.ImpostorRed, "Vampire"), RoleRates, null, true);
            VampireKillDelay = CustomOption.Create(554, Types.Impostor, "Bite Kill Delay", 5f, 1f, 10f, 1f, VampireSpawnRate);
            VampireCooldown = CustomOption.Create(555, Types.Impostor, "Bite Cooldown", 25f, 10f, 60f, 2.5f, VampireSpawnRate);
            VampireKillButton = CustomOption.Create(556, Types.Impostor, "Vampire Also Has Kill Button", false, VampireSpawnRate);

            //Warlock
            WarlockSpawnRate = CustomOption.Create(700, Types.Impostor, cs(Palette.ImpostorRed, "Warlock"), RoleRates, null, true);
            WarlockCooldown = CustomOption.Create(701, Types.Impostor, "Curse Cooldown", 30f, 10f, 60f, 2.5f, WarlockSpawnRate);
            WarlockRootTime = CustomOption.Create(702, Types.Impostor, "Curse-Kill Root Time", 4f, 0f, 15f, 1f, WarlockSpawnRate);

            //Wraith
            WraithSpawnRate = CustomOption.Create(450, Types.Impostor, cs(Palette.ImpostorRed, "Wraith"), RoleRates, null, true);
            WraithPhaseCooldown = CustomOption.Create(451, Types.Impostor, "Dash Cooldown", 30f, 10f, 60f, 2.5f, WraithSpawnRate);
            WraithPhaseDuration = CustomOption.Create(452, Types.Impostor, "Dash Duration", 10f, 3f, 15f, 1f, WraithSpawnRate);
            WraithLantern = CustomOption.Create(454, Types.Impostor, "Lantern", true, WraithSpawnRate);
            WraithLanternCooldown = CustomOption.Create(455, Types.Impostor, "Lantern Cooldown", 30f, 5f, 60f, 2.5f, WraithLantern);
            WraithLanternDuration = CustomOption.Create(456, Types.Impostor, "Lantern Duration", 10f, 1f, 20f, 0.5f, WraithLantern);
            WraithInvisibleDuration = CustomOption.Create(458, Types.Impostor, "Invisible Duration", 2f, 0f, 5f, 0.5f, WraithLantern);
            #endregion Impostors

            #region Neutral Killing
            NeutralKillerRolesCountMin = CustomOption.Create(1500, Types.NeutralK, "Minimum " + cs(NeutralKiller.Color, "Neutral Killing") + " Roles", 0f, 0f, 15f, 1f, null, true);
            NeutralKillerRolesCountMax = CustomOption.Create(1501, Types.NeutralK, "Maximum " + cs(NeutralKiller.Color, "Neutral Killing") + " Roles", 0f, 0f, 15f, 1f);

            NeutralKillersGetNonCritSabo = CustomOption.Create(1550, Types.NeutralK, "Neutral Killers Get Non-Critical Sabotage", new string[] { "Imp Team Wipe", "Never", "Game Start" });
            ImpsLooseCritSabo = CustomOption.Create(1551, Types.NeutralK, "Impostors Lose Critical Sabotage", new string[] { "Team Wipe", "Never" });
            ImpsLoseDoors = CustomOption.Create(1552, Types.NeutralK, "Impostors Lose Door Sabotage", new string[] { "Team Wipe", "Never" });
            NeutralKillerGainsAssassin = CustomOption.Create(1553, Types.NeutralK, "Neutral Killers Gain Assassin Ability", new string[] { "Never", "Imp Team Wipe", "Game Start" });
            NeutralKillerAssassinNumberOfShots = CustomOption.Create(1554, Types.NeutralK, "Number Of Shots", 5f, 0f, 15f, 1f, NeutralKillerGainsAssassin);
            NeutralKillerAssassinMultipleShotsPerMeeting = CustomOption.Create(1555, Types.NeutralK, "Limit One Shot Per Meeting", false, NeutralKillerGainsAssassin);
            NeutralKillerAssassinCanGuessCrewmate = CustomOption.Create(1556, Types.NeutralK, "Can Guess Crewmate", false, NeutralKillerGainsAssassin);

            //Rogue Impostors
            EnableRogueImpostors = CustomOption.Create(1602, Types.NeutralK, cs(NeutralKiller.Color, "Enabled Rogue Impostors"), new string[] { "Hide", "Show" }, null, true);
            BomberIsNeutral = CustomOption.Create(1613, Types.NeutralK, "Bomber", false, EnableRogueImpostors);
            BountyHunterIsNeutral = CustomOption.Create(1610, Types.NeutralK, "Bounty Hunter", false, EnableRogueImpostors);
            CamouflagerIsNeutral = CustomOption.Create(1607, Types.NeutralK, "Camouflager", false, EnableRogueImpostors);
            JanitorIsNeutral = CustomOption.Create(1612, Types.NeutralK, "Janitor", false, EnableRogueImpostors);
            MinerIsNeutral = CustomOption.Create(1611, Types.NeutralK, "Miner", false, EnableRogueImpostors);
            MorphlingIsNeutral = CustomOption.Create(1603, Types.NeutralK, "Morphling", false, EnableRogueImpostors);
            ShadeIsNeutral = CustomOption.Create(1609, Types.NeutralK, "Shade", false, EnableRogueImpostors);
            UndertakerIsNeutral = CustomOption.Create(1605, Types.NeutralK, "Undertaker", false, EnableRogueImpostors);
            VampireIsNeutral = CustomOption.Create(1604, Types.NeutralK, "Vampire", false, EnableRogueImpostors);
            WarlockIsNeutral = CustomOption.Create(1606, Types.NeutralK, "Warlock", false, EnableRogueImpostors);
            WraithIsNeutral = CustomOption.Create(1608, Types.NeutralK, "Wraith", false, EnableRogueImpostors);

            //Headhunter
            HeadHunterSpawnRate = CustomOption.Create(1650, Types.NeutralK, cs(HeadHunter.Color, "HeadHunter"), RoleRates, null, true);
            HeadHunterTrackingCooldown = CustomOption.Create(1651, Types.NeutralK, "Pursue Cooldown", 20f, 15f, 30f, 2.5f, HeadHunterSpawnRate);
            HeadHunterTrackerDuration = CustomOption.Create(1652, Types.NeutralK, "Pursue Duration", 5f, 5f, 30f, 2.5f, HeadHunterSpawnRate);

            //Nightmare
            NightmareSpawnRate = CustomOption.Create(1700, Types.NeutralK, cs(Nightmare.Color, "Nightmare"), RoleRates, null, true);
            NightmareParalyzeCooldown = CustomOption.Create(1701, Types.NeutralK, "Paralyze Cooldown", 20f, 15f, 20f, 2.5f, NightmareSpawnRate);
            NightmareParalyzeDuration = CustomOption.Create(1702, Types.NeutralK, "Paralyze Duration", 7f, 3f, 15f, 1f, NightmareSpawnRate);
            NightmareBlindCooldown = CustomOption.Create(1703, Types.NeutralK, "Blind Cooldown", 20f, 10f, 35f, 2.5f, NightmareSpawnRate);
            NightmareBlindDuration = CustomOption.Create(1704, Types.NeutralK, "Blind Duration", 10f, 5f, 15f, 2.5f, NightmareSpawnRate);
            NightmareBlindRadius = CustomOption.Create(1705, Types.NeutralK, "Blind Radius", 1.0f, .25f, 5, .25f, NightmareSpawnRate);

            //Pyromaniac
            PyromaniacSpawnRate = CustomOption.Create(1800, Types.NeutralK, cs(Pyromaniac.Color, "Pyromaniac"), RoleRates, null, true);
            PyromaniacDouseCooldown = CustomOption.Create(1801, Types.NeutralK, "Douse Cooldown", 22.5f, 10f, 30f, 2.5f, PyromaniacSpawnRate);
            PyromaniacDousedDuration = CustomOption.Create(1802, Types.NeutralK, "Douse Duration", 1f, 1f, 5f, 1f, PyromaniacSpawnRate);
            PyromaniacDouseKillCooldown = CustomOption.Create(1803, Types.NeutralK, "Doused Kill Cooldown", 3f, 0f, 15f, 1f, PyromaniacSpawnRate);
            #endregion Neutral Killing

            #region Neutrals
            NeutralRolesCountMin = CustomOption.Create(2500, Types.Neutral, "Minimum " + cs(Color.grey, "Neutral") + " Roles", 0f, 0f, 15f, 1f, null, true);
            NeutralRolesCountMax = CustomOption.Create(2501, Types.Neutral, "Maximum " + cs(Color.grey, "Neutral") + " Roles", 0f, 0f, 15f, 1f);

            //Arsonist
            ArsonistSpawnRate = CustomOption.Create(2650, Types.Neutral, cs(Arsonist.Color, "Arsonist"), RoleRates, null, true);
            ArsonistCooldown = CustomOption.Create(2651, Types.Neutral, "Douse Cooldown", 22.5f, 2.5f, 60f, 2.5f, ArsonistSpawnRate);
            ArsonistDuration = CustomOption.Create(2652, Types.Neutral, "Douse Duration", 1f, 1f, 10f, 1f, ArsonistSpawnRate);
            ArsonistDouseIgniteRoundCooldown = CustomOption.Create(2654, Types.Neutral, "Douse/Ignite Round Start Cooldown", 15f, 10f, 30f, 2.5f, ArsonistSpawnRate);

            //Executioner
            ExecutionerSpawnRate = CustomOption.Create(2600, Types.Neutral, cs(Executioner.Color, "Executioner"), RoleRates, null, true);
            ExecutionerPromotesTo = CustomOption.Create(2601, Types.Neutral, "Role on Target Death", new string[] { "Jester", "Refugee" }, ExecutionerSpawnRate);
            ExecutionerConvertsImmediately = CustomOption.Create(2602, Types.Neutral, "Executioner Converts on Target Death", new string[] { "Immediately", "Next Meeting" }, ExecutionerSpawnRate);

            //Jester
            JesterSpawnRate = CustomOption.Create(2550, Types.Neutral, cs(Jester.Color, "Jester"), RoleRates, null, true);
            JesterCanCallEmergency = CustomOption.Create(2551, Types.Neutral, "Jester Can Call Emergency Meeting", true, JesterSpawnRate);
            JesterCanEnterVents = CustomOption.Create(2552, Types.Neutral, "Jester Can Enter Vents", true, JesterSpawnRate);
            JesterLightsOnVision = CustomOption.Create(2553, Types.Neutral, "Jester Lights On Vision", .75f, 0.25f, 5f, 0.25f, JesterSpawnRate);
            JesterLightsOffVision = CustomOption.Create(2554, Types.Neutral, "Jester Lights Off Vision", .5f, 0.25f, 5f, 0.25f, JesterSpawnRate);

            //Refugee
            RefugeeSpawnRate = CustomOption.Create(2800, Types.Neutral, cs(Refugee.Color, "Refugee"), new string[] { "Hide", "Show" }, null, true);
            VestCooldown = CustomOption.Create(2801, Types.Neutral, "Refuge Cooldown", 25f, 10f, 60f, 2.5f, RefugeeSpawnRate);
            VestDuration = CustomOption.Create(2802, Types.Neutral, "Refuge Duration", 7f, 3f, 20f, 1f, RefugeeSpawnRate);
            CanBeGuessed = CustomOption.Create(2803, Types.Neutral, "Refugee can be Guessed", false, RefugeeSpawnRate);

            //Romantic
            RomanticSpawnRate = CustomOption.Create(2750, Types.Neutral, cs(Romantic.Color, "Romantic"), RoleRates, null, true);
            RomanticProtectCooldown = CustomOption.Create(2751, Types.Neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, RomanticSpawnRate);
            RomanticProtectDuration = CustomOption.Create(2752, Types.Neutral, "Protect Duration", 7f, 3f, 15f, 1f, RomanticSpawnRate);
            RomanticKnowsTargetRoleWhen = CustomOption.Create(2753, Types.Neutral, "Romantic Knows Target Identity", new string[] { "Instantly", "On Target Death" }, RomanticSpawnRate);
            RomanticLoverSeesLove = CustomOption.Create(2755, Types.Neutral, "Romantic Target Knows They Are Selected", new string[] { "Never", "Next Meeting", "Instantly" }, RomanticSpawnRate);
            RomanticOnAllImpsDead = CustomOption.Create(8002, Types.Neutral, "Imp Romantic Becomes\non Last Impostor Death", new string[] { "Dead", "Refugee" }, RomanticSpawnRate);


            //Scavenger
            ScavengerSpawnRate = CustomOption.Create(2700, Types.Neutral, cs(Scavenger.Color, "Scavenger"), RoleRates, null, true);
            ScavengerCooldown = CustomOption.Create(2701, Types.Neutral, "Eat Cooldown", 20f, 10f, 60f, 2.5f, ScavengerSpawnRate);
            ScavengerNumberToWin = CustomOption.Create(2702, Types.Neutral, "Number of Eats to Win", 3f, 1f, 10f, 1f, ScavengerSpawnRate);
            ScavengerCanUseVents = CustomOption.Create(2703, Types.Neutral, "Scavenger Can Use Vents", true, ScavengerSpawnRate);
            ScavengerCorpsesTrackingCooldown = CustomOption.Create(2705, Types.Neutral, "Scavenge Cooldown", 20f, 5f, 120f, 2.5f, ScavengerSpawnRate);
            ScavengerCorpsesTrackingDuration = CustomOption.Create(2706, Types.Neutral, "Scavenge Duration", 2.5f, 2.5f, 30f, 2.5f, ScavengerSpawnRate);
            #endregion Neutrals

            #region Crewmates
            CrewmateRolesCountMin = CustomOption.Create(3500, Types.Crewmate, "Minimum " + cs(Color.cyan, "Crewmate") + " Roles", 0f, 0f, 15f, 1f, null, true);
            CrewmateRolesCountMax = CustomOption.Create(3501, Types.Crewmate, "Maximum " + cs(Color.cyan, "Crewmate") + " Roles", 0f, 0f, 15f, 1f);

            //Administrator
            AdministratorSpawnRate = CustomOption.Create(3900, Types.Crewmate, cs(Administrator.Color, "Administrator"), RoleRates, null, true); ;
            AdministratorInitialBatteryTime = CustomOption.Create(3901, Types.Crewmate, "Initial Battery Charge", 0f, 0f, 25f, 1f, AdministratorSpawnRate);
            AdministratorBatteryTimePerTask = CustomOption.Create(3902, Types.Crewmate, "Charge Amount (Seconds)", 1f, 0f, 5f, 0.5f, AdministratorSpawnRate);
            AdministratorSelfChargingBatteryCooldown = CustomOption.Create(3903, Types.Crewmate, "Self-Charge Cooldown", 20f, 10f, 30f, 2.5f, AdministratorSpawnRate);
            AdministratorDisableRoundOneAccess = CustomOption.Create(3905, Types.Crewmate, "Disable Charging and Access Round One", false, AdministratorSpawnRate);

            //Detective
            DetectiveSpawnRate = CustomOption.Create(4300, Types.Crewmate, cs(Detective.Color, "Detective"), RoleRates, null, true);
            DetectiveInspectDuration = CustomOption.Create(4301, Types.Crewmate, "Inspect Duration", 2f, .5f, 5f, .5f, DetectiveSpawnRate);
            DetectiveInspectsPerRound = CustomOption.Create(4302, Types.Crewmate, "Inspects Per Round", 3f, 1f, 8f, 1f, DetectiveSpawnRate);
            DetectiveEnableCrimeScenes = CustomOption.Create(4303, Types.Crewmate, "Enable Crime Scenes", true, DetectiveSpawnRate);

            //Engineer
            EngineerSpawnRate = CustomOption.Create(3700, Types.Crewmate, cs(Engineer.Color, "Engineer"), RoleRates, null, true);
            EngineerHasFix = CustomOption.Create(3701, Types.Crewmate, "Enable Remote Fix", new string[] { "Off", "On", "On Task Completion" }, EngineerSpawnRate);
            EngineerCanVent = CustomOption.Create(3702, Types.Crewmate, "Can Vent", true, EngineerSpawnRate);
            EngineerHighlightForEvil = CustomOption.Create(3703, Types.Crewmate, "Evil Killers See Vent Highlights", true, EngineerCanVent);
            EngineerAdvancedSabotageRepairs = CustomOption.Create(3704, Types.Crewmate, "Advanced Sabotage Repair", true, EngineerSpawnRate);

            //Guardian
            GuardianSpawnRate = CustomOption.Create(3875, Types.Crewmate, cs(Guardian.Color, "Guardian"), RoleRates, null, true);
            GuardianShieldIsVisibleTo = CustomOption.Create(3882, Types.Crewmate, "Shield Is Visible To", new string[] { "Protected", "Unprotected", "Killers", "Everyone", "Guardian Only" }, GuardianSpawnRate);
            GuardianShieldVisibilityDelay = CustomOption.Create(3877, Types.Crewmate, "Shield Visibility Delay", 10f, 0f, 30f, 2.5f, GuardianSpawnRate);
            GuardianVisionRangeOfShield = CustomOption.Create(3880, Types.Crewmate, "Shield Vision Range", .5f, .25f, 1.25f, .25f, GuardianSpawnRate);
            GuardianShieldFadesOnDeath = CustomOption.Create(3878, Types.Crewmate, "Shield Fades On Death", false, GuardianSpawnRate);

            //Investigator
            InvestigatorSpawnRate = CustomOption.Create(3800, Types.Crewmate, cs(Investigator.Color, "Investigator"), RoleRates, null, true);
            InvestigatorAnonymousFootprints = CustomOption.Create(3801, Types.Crewmate, "Anonymous Footprints", false, InvestigatorSpawnRate);
            InvestigatorFootprintInterval = CustomOption.Create(3802, Types.Crewmate, "Footprint Interval", 0.25f, 0.25f, 10f, 0.25f, InvestigatorSpawnRate);
            InvestigatorFootprintDuration = CustomOption.Create(3803, Types.Crewmate, "Footprint Duration", 3.75f, 0.25f, 10f, 0.25f, InvestigatorSpawnRate);

            //Jailor
            JailorSpawnRate = CustomOption.Create(4200, Types.Crewmate, cs(Jailor.Color, "Jailor"), RoleRates, null, true);
            InitialJailCharges = CustomOption.Create(4201, Types.Crewmate, "Initial Jail Charges", 0f, 0f, 10f, 1f, JailorSpawnRate);
            JailorTasksPerRecharge = CustomOption.Create(4203, Types.Crewmate, "Tasks Per Recharge", 2f, 1f, 10f, 1f, JailorSpawnRate);
            JailorCanJailSelf = CustomOption.Create(4205, Types.Crewmate, "Can Jail Self", true, JailorSpawnRate);

            //Mayor
            MayorSpawnRate = CustomOption.Create(3650, Types.Crewmate, cs(Mayor.Color, "Mayor"), RoleRates, null, true);
            MayorCanSeeVoteColors = CustomOption.Create(3651, Types.Crewmate, "Can See Vote Colors", true, MayorSpawnRate);
            MayorTasksNeededToSeeVoteColors = CustomOption.Create(3652, Types.Crewmate, "Completed Tasks Needed To See Vote Colors", 10f, 0f, 20f, 1f, MayorCanSeeVoteColors);
            MayorCanRetire = CustomOption.Create(3653, Types.Crewmate, "Enable Retire", true, MayorSpawnRate);

            //Medic
            MedicSpawnRate = CustomOption.Create(3950, Types.Crewmate, cs(Medic.Color, "Medic"), RoleRates, null, true);
            MedicNonCrewFlash = CustomOption.Create(3956, Types.Crewmate, "Non-Crew Monitor Notification Delay", OddRate15, MedicSpawnRate);
            MedicInitialBatteryTime = CustomOption.Create(3951, Types.Crewmate, "Initial Battery Charge", 2f, 1f, 20f, 1f, MedicSpawnRate);
            MedicBatteryTimePerTask = CustomOption.Create(3952, Types.Crewmate, "Charge Amount (Seconds)", 1f, 0f, 5f, 0.5f, MedicSpawnRate);
            MedicSelfChargingBatteryCooldown = CustomOption.Create(3953, Types.Crewmate, "Self-Charge Cooldown", 20f, 10f, 30f, 2.5f, MedicSpawnRate);
            MedicDisableRoundOneAccess = CustomOption.Create(3955, Types.Crewmate, "Disable Charging and Access Round One", false, MedicSpawnRate);

            //ParityCop
            ParityCopSpawnRate = CustomOption.Create(3600, Types.Crewmate, cs(ParityCop.Color, "Parity Cop"), RoleRates, null, true);
            ParityCopNeutralsMatchKillers = CustomOption.Create(3601, Types.Crewmate, "Neutral Roles Match Killers", true, ParityCopSpawnRate);
            ParityCopCompareCooldown = CustomOption.Create(3605, Types.Crewmate, "Compare Cooldown", 10f, 5f, 30f, 2.5f, ParityCopSpawnRate);
            ParityCopFakeCompare = CustomOption.Create(3606, Types.Crewmate, "Enable Fake Out", false, ParityCopSpawnRate);

            PsychicSpawnRate = CustomOption.Create(3610, Types.Crewmate, cs(Psychic.Color, "Psychic"), RoleRates, null, true);
            PsychicPlayerRange = CustomOption.Create(3611, Types.Crewmate, "Player Counter Range", .75f, .5f, 1.5f, .25f, PsychicSpawnRate);
            PsychicDetectInvisible = CustomOption.Create(3612, Types.Crewmate, "Include Invisible Players", false, PsychicSpawnRate);
            PsychicDetectInVent = CustomOption.Create(3613, Types.Crewmate, "Include Venting Players", false, PsychicSpawnRate);

            //Sheriff
            SheriffSpawnRate = CustomOption.Create(3750, Types.Crewmate, cs(Sheriff.Color, "Sheriff"), RoleRates, null, true);
            SheriffMisfireKills = CustomOption.Create(3751, Types.Crewmate, "Misfire Kills", new string[] { "Self", "One Target", "Both" }, SheriffSpawnRate);
            SheriffCanKillNeutrals = CustomOption.Create(3752, Types.Crewmate, "Can Kill Neutrals", true, SheriffSpawnRate);
            SheriffCanKillArsonist = CustomOption.Create(3753, Types.Crewmate, "Can Kill " + cs(Arsonist.Color, "Arsonist"), true, SheriffCanKillNeutrals);
            SheriffCanKillJester = CustomOption.Create(3754, Types.Crewmate, "Can Kill " + cs(Jester.Color, "Jester"), true, SheriffCanKillNeutrals);
            SheriffCanKillExecutioner = CustomOption.Create(3755, Types.Crewmate, "Can Kill " + cs(Executioner.Color, "Executioner"), true, SheriffCanKillNeutrals);
            SheriffCanKillScavenger = CustomOption.Create(3756, Types.Crewmate, "Can Kill " + cs(Scavenger.Color, "Scavenger"), true, SheriffCanKillNeutrals);

            //Spy
            SpySpawnRate = CustomOption.Create(4050, Types.Crewmate, cs(Spy.Color, "Spy"), RoleRates, null, true);
            SpyCanDieToSheriff = CustomOption.Create(4051, Types.Crewmate, "Spy Dies to Sheriff", false, SpySpawnRate);
            SpyImpostorsCanKillAnyone = CustomOption.Create(4052, Types.Crewmate, "Impostor Friendly Fire", true, SpySpawnRate);
            AssassinCanKillSpy = CustomOption.Create(203, Types.Crewmate, "Spy Can Be Guessed", false, SpySpawnRate);

            //Tracker
            TrackerSpawnRate = CustomOption.Create(4000, Types.Crewmate, cs(Tracker.Color, "Tracker"), RoleRates, null, true);
            TrackerTracksPerRound = CustomOption.Create(4002, Types.Crewmate, "Max Marks Per Round", 1f, 1f, 3f, 1f, TrackerSpawnRate);
            TrackerAnonymousArrows = CustomOption.Create(4006, Types.Crewmate, "Anonymous Tracking Arrows", true, TrackerSpawnRate);
            TrackerTrackCooldown = CustomOption.Create(4003, Types.Crewmate, "Track Cooldown", 15f, 10f, 30f, 2.5f, TrackerSpawnRate);
            TrackerTrackDuration = CustomOption.Create(4001, Types.Crewmate, "Track Duration", 10f, 5f, 20f, 2.5f, TrackerSpawnRate);
            TrackerDelayDuration = CustomOption.Create(4005, Types.Crewmate, "Track Delay Duration", 5f, 3f, 10f, 1f, TrackerSpawnRate);

            //Trapper
            TrapperSpawnRate = CustomOption.Create(4150, Types.Crewmate, cs(Trapper.Color, "Trapper"), RoleRates, null, true);
            TrapperNumberOfTraps = CustomOption.Create(4151, Types.Crewmate, "Number of Traps", 3f, 1f, 15f, 1f, TrapperSpawnRate);
            TrapperNumberOfCovers = CustomOption.Create(4152, Types.Crewmate, "Number of Covers", 3f, 1f, 15f, 1f, TrapperSpawnRate);
            TrapperTrapCoverCooldown = CustomOption.Create(4153, Types.Crewmate, "Trap/Cover Cooldown", 20f, 5f, 60f, 2.5f, TrapperSpawnRate);
            TrapperRootDuration = CustomOption.Create(4155, Types.Crewmate, "Trap Root Duration", 4f, 1f, 10f, 1f, TrapperSpawnRate);

            //Vigilante
            VigilanteSpawnRate = CustomOption.Create(3550, Types.Crewmate, cs(Vigilante.Color, "Vigilante"), RoleRates, null, true);
            VigilanteNumberOfShots = CustomOption.Create(3551, Types.Crewmate, "Number Of Shots", 5f, 1f, 15f, 1f, VigilanteSpawnRate);
            VigilanteHasMultipleShotsPerMeeting = CustomOption.Create(3552, Types.Crewmate, "Limit One Shot Per Meeting", false, VigilanteNumberOfShots);

            //Watcher
            WatcherSpawnRate = CustomOption.Create(4100, Types.Crewmate, cs(Watcher.Color, "Watcher"), RoleRates, null, true);
            WatcherNonCrewFlash = CustomOption.Create(4110, Types.Crewmate, "Non-Crew Sensor Notification Delay", OddRate15, WatcherSpawnRate);
            WatcherAnonymousArrows = CustomOption.Create(4106, Types.Crewmate, "Anonymous Sensor Arrows", false, WatcherSpawnRate);
            WatcherInitialBatteryTime = CustomOption.Create(4104, Types.Crewmate, "Initial Battery Charge", 6f, 1f, 20f, 1f, WatcherSpawnRate);
            WatcherBatteryTimePerTask = CustomOption.Create(4105, Types.Crewmate, "Charge Amount (Seconds)", 2.5f, 0f, 5f, 0.5f, WatcherSpawnRate);
            WatcherSelfChargingBatteryCooldown = CustomOption.Create(4107, Types.Crewmate, "Self-Charge Cooldown", 12.5f, 10f, 30f, 2.5f, WatcherSpawnRate);
            WatcherRoundOneAccess = CustomOption.Create(4109, Types.Crewmate, "Disable Charging and Access Round One", true, WatcherSpawnRate);
            #endregion Crewmates

            #region Modifiers
            ModifierCosmeticMin = CustomOption.Create(5002, Types.Modifier, "Minimum Cosmetic " + cs(Spiteful.Color, "Modifiers"), 0f, 0f, 15f, 1f, null, true);
            ModifierCosmeticMax = CustomOption.Create(5003, Types.Modifier, "Maximum Cosmetic " + cs(Spiteful.Color, "Modifiers"), 0f, 0f, 15f, 1f);

            //Giant
            ModifierGiant = CustomOption.Create(5200, Types.Modifier, cs(Spiteful.Color, "Giant"), RoleRates, null, true);
            ModifierGiantSpeed = CustomOption.Create(5201, Types.Modifier, cs(Spiteful.Color, "Giant") + " Speed Multiplier", SpeedRates, ModifierGiant);

            //Mini
            ModifierMini = CustomOption.Create(5150, Types.Modifier, cs(Spiteful.Color, "Mini"), RoleRates, null, true);
            ModifierMiniSpeed = CustomOption.Create(5151, Types.Modifier, cs(Spiteful.Color, "Mini") + " Speed Multiplier", SpeedRates, ModifierMini);

            ModifiersImpCountMin = CustomOption.Create(5004, Types.Modifier, "Minimum Imposter " + cs(Spiteful.Color, "Modifiers"), 0f, 0f, 15f, 1f, null, true);
            ModifiersImpCountMax = CustomOption.Create(5005, Types.Modifier, "Maximum Imposter " + cs(Spiteful.Color, "Modifiers"), 0f, 0f, 15f, 1f);

            //Clutch
            ModifierClutch = CustomOption.Create(5260, Types.Modifier, cs(Spiteful.Color, "Clutch"), RoleRates, null, true);
            ModifierClutchImpact = CustomOption.Create(5261, Types.Modifier, cs(Spiteful.Color, "Clutch") + " Impact", new string[] { "10%", "20%", "30%", "40%", "50%" }, ModifierClutch);
            ModifierClutchCount = CustomOption.Create(5263, Types.Modifier, cs(Spiteful.Color, "Clutch") + " Quantity", ModifierRates, ModifierClutch);
            ModifierClutchSeeModifier = CustomOption.Create(5262, Types.Modifier, "Player Sees Modifier", true, ModifierClutch);

            ModifiersMiscCountMin = CustomOption.Create(5000, Types.Modifier, "Minimum Misc " + cs(Spiteful.Color, "Modifiers"), 0f, 0f, 15f, 1f, null, true);
            ModifiersMiscCountMax = CustomOption.Create(5001, Types.Modifier, "Maximum Misc " + cs(Spiteful.Color, "Modifiers"), 0f, 0f, 15f, 1f);

            //Gopher
            ModifierGopher = CustomOption.Create(5300, Types.Modifier, cs(Spiteful.Color, "Gopher"), RoleRates, null, true);
            ModifierGopherQuantity = CustomOption.Create(5301, Types.Modifier, cs(Spiteful.Color, "Gopher") + " Quantity", ModifierRates, ModifierGopher);

            //Sleepwalker
            ModifierSleepwalker = CustomOption.Create(5100, Types.Modifier, cs(Spiteful.Color, "Sleepwalker"), RoleRates, null, true);
            ModifierSleepwalkerQuantity = CustomOption.Create(5101, Types.Modifier, cs(Spiteful.Color, "Sleepwalker") + " Quantity", ModifierRates, ModifierSleepwalker);
            ModifierSleepwalkerSeesModifier = CustomOption.Create(5202, Types.Modifier, "Player Sees Modifier", true, ModifierSleepwalker);

            //Sniper
            ModifierSniper = CustomOption.Create(5400, Types.Modifier, cs(Spiteful.Color, "Sniper"), RoleRates, null, true);
            ModifierSniperQuantity = CustomOption.Create(5401, Types.Modifier, cs(Spiteful.Color, "Sniper") + " Quantity", ModifierRates, ModifierSniper);

            //Spiteful
            ModifierSpiteful = CustomOption.Create(5250, Types.Modifier, cs(Spiteful.Color, "Spiteful"), RoleRates, null, true);
            ModifierSpitefulImpact = CustomOption.Create(5251, Types.Modifier, cs(Spiteful.Color, "Spiteful") + " Impact", new string[] { "25%", "50%", "75%", "100%" }, ModifierSpiteful);
            ModifierSpitefulCount = CustomOption.Create(5252, Types.Modifier, cs(Spiteful.Color, "Spiteful") + " Quantity", 1f, 1f, 5f, 1f, ModifierSpiteful);
            ModifierSpitefulSeeModifier = CustomOption.Create(5253, Types.Modifier, "Player Sees Modifier", false, ModifierSpiteful);
            #endregion Modifiers

            #region Other options
            LobbySize = CustomOption.Create(5901, Types.General, "Lobby Size", 12, 4, 15, 1);
            GameStartKillCD = CustomOption.Create(5902, Types.General, "Game Start " + cs(Palette.ImpostorRed, "Kill Cooldown"), 10, 10, 35, 1);

            MaxNumberOfMeetings = CustomOption.Create(6000, Types.General, "Number Of Meetings", 10, 0, 15, 1, null, true);
            ShieldFirstKill = CustomOption.Create(6005, Types.General, "Shield Last Game First Kill", false);
            RoundOneKilledIndicators = CustomOption.Create(6008, Types.General, "Mark Players Who Die\nRound 1 Next Game", true);
            HideOutOfSightNametags = CustomOption.Create(6006, Types.General, "Hide Obstructed Player Names", true);

            AllowParallelMedBayScans = CustomOption.Create(6002, Types.General, "Parallel MedBay Scans", true, null, true);
            DisableMedscanWalking = CustomOption.Create(6003, Types.General, "Disable MedBay Animations", true);

            DisableVentCleanEjections = CustomOption.Create(6004, Types.General, "Disable Vent Cleaning Ejections", true, null, true);
            VentInFog = CustomOption.Create(6007, Types.General, "Hide Vent Animations\nOut of Vision", true);

            RestrictAdminOnMira = CustomOption.Create(6009, Types.General, "Tasks to Unlock MIRA Admin", 0f, 0f, 20f, 1, null, true);
            RestrictCamsOnSkeld = CustomOption.Create(6010, Types.General, "Tasks to Unlock Skeld Cameras", 0f, 0f, 20f, 1);
            NoCamsFirstRound = CustomOption.Create(6011, Types.General, "No Cameras First Round", true);

            EnableBetterPolus = CustomOption.Create(6075, Types.General, "Better Polus", false, null, true);
            MoveVitals = CustomOption.Create(6076, Types.General, "Vitals Moved To Lab", true, EnableBetterPolus);
            VentSystem = CustomOption.Create(6077, Types.General, "Reactor Vent Layout Change", true, EnableBetterPolus);
            ColdTempDeathValley = CustomOption.Create(6078, Types.General, "Cold Temp Moved To Death Valley", true, EnableBetterPolus);
            WifiChartCourseSwap = CustomOption.Create(6079, Types.General, "Reboot Wifi And Chart Course Swapped", true, EnableBetterPolus);

            JoustingRoleImpWin = CustomOption.Create(6013, Types.General, "Jousting Roles \nPrevent Impostor Victory", false, null, true);
            JoustingRoleNKWin = CustomOption.Create(6014, Types.General, "Jousting Roles \nPrevent Neutral Killer Victory", false);
            DeadCrewPreventTaskWin = CustomOption.Create(6015, Types.General, "Crewmate Wipe\nPrevents Task Win", false);

            DisableRolesSkeld = CustomOption.Create(9000, Types.General, "Disabled Roles on Skeld", new string[] { "Hide", "Show" }, null, true);
            DisableArsonistOnSkeld = CustomOption.Create(9001, Types.General, cs(Arsonist.Color, "Arsonist"), false, DisableRolesSkeld);
            DisableScavengerOnSkeld = CustomOption.Create(9002, Types.General, cs(Scavenger.Color, "Scavenger"), true, DisableRolesSkeld);
            DisableJanitorOnSkeld = CustomOption.Create(9003, Types.General, cs(Palette.ImpostorRed, "Janitor"), true, DisableRolesSkeld);
            DisableMedicOnSkeld = CustomOption.Create(9004, Types.General, cs(Medic.Color, "Medic"), false, DisableRolesSkeld);
            DisableWatcherOnSkeld = CustomOption.Create(6107, Types.General, cs(Watcher.Color, "Watcher"), true, DisableRolesSkeld);
            DisableRolesMira = CustomOption.Create(9005, Types.General, "Disabled Roles on Mira", new string[] { "Hide", "Show" });
            DisableArsonistOnMira = CustomOption.Create(2653, Types.General, cs(Arsonist.Color, "Arsonist"), false, DisableRolesMira);
            DisableScavengerOnMira = CustomOption.Create(9006, Types.General, cs(Scavenger.Color, "Scavenger"), true, DisableRolesMira);
            DisableJanitorOnMira = CustomOption.Create(9007, Types.General, cs(Palette.ImpostorRed, "Janitor"), true, DisableRolesMira);
            DisableMedicOnMira = CustomOption.Create(9008, Types.General, cs(Medic.Color, "Medic"), false, DisableRolesMira);
            DisableMinerOnMira = CustomOption.Create(9009, Types.General, cs(Palette.ImpostorRed, "Miner"), false, DisableRolesMira);
            DisableAdministratorOnMira = CustomOption.Create(6106, Types.General, cs(Administrator.Color, "Administrator"), true, DisableRolesMira);

            RoleBlockComms = CustomOption.Create(7000, Types.General, "Roles Blocked by Comms", new string[] { "Show", "Hide" }, null, true);
            ImposterRoleBlock = CustomOption.Create(7001, Types.General, cs(Palette.ImpostorRed, "Impostors"), false, RoleBlockComms);
            ImposterKillAbilitiesRoleBlock = CustomOption.Create(7002, Types.General, "Kill Abilties Role Block", false, ImposterRoleBlock);
            ImposterAbiltiesRoleBlock = CustomOption.Create(7003, Types.General, "Abilities Role Block", false, ImposterRoleBlock);
            NeutralKillerRoleBlock = CustomOption.Create(7004, Types.General, cs(NeutralKiller.Color, "Neutral Killers"), false, RoleBlockComms);
            NeutralRoleBlock = CustomOption.Create(7005, Types.General, cs(Color.gray, "Neutrals"), false, RoleBlockComms);

            CrewRoleBlock = CustomOption.Create(7100, Types.General, cs(Palette.CrewmateBlue, "Crewmates"), true, RoleBlockComms);
            DetectiveRoleBlock = CustomOption.Create(7109, Types.General, cs(Detective.Color, "Detective"), false, CrewRoleBlock);
            EngineerRoleBlock = CustomOption.Create(7102, Types.General, cs(Engineer.Color, "Engineer"), false, CrewRoleBlock);
            GuardianRoleBlock = CustomOption.Create(7104, Types.General, cs(Guardian.Color, "Guardian"), true, CrewRoleBlock);
            InvestigatorRoleBlock = CustomOption.Create(7103, Types.General, cs(Investigator.Color, "Investigator"), false, CrewRoleBlock);
            MedicRoleBlock = CustomOption.Create(7106, Types.General, cs(Medic.Color, "Medic"), false, CrewRoleBlock);
            ParityCopRoleBlock = CustomOption.Create(7101, Types.General, cs(ParityCop.Color, "Parity Cop"), false, CrewRoleBlock);
            PsychicRoleBlock = CustomOption.Create(7111, Types.General, cs(Psychic.Color, "Psychic"), false, CrewRoleBlock);
            SpyRoleBlock = CustomOption.Create(7107, Types.General, cs(Spy.Color, "Spy"), false, CrewRoleBlock);
            TrackerRoleBlock = CustomOption.Create(7105, Types.General, cs(Tracker.Color, "Tracker"), true, CrewRoleBlock);
            TrapperRoleBlock = CustomOption.Create(7110, Types.General, cs(Trapper.Color, "Trapper"), false, CrewRoleBlock);
            WatcherRoleBlock = CustomOption.Create(7108, Types.General, cs(Watcher.Color, "Watcher"), true, CrewRoleBlock);

            GhostsHeader = CustomOption.Create(6050, Types.General, "Ghost Options", new string[] { "Hide", "Show" }, null, true);
            GhostsSeeRoles = CustomOption.Create(6051, Types.General, "Ghosts See Roles", false, GhostsHeader);
            GhostsSeeTasks = CustomOption.Create(6052, Types.General, "Ghosts See Tasks Done", false, GhostsHeader);
            GhostsSeeModifiers = CustomOption.Create(6053, Types.General, "Ghosts See Modifiers", false, GhostsHeader);
            GhostsSeeRomanticTarget = CustomOption.Create(6054, Types.General, "Ghosts see Romantic Target", false, GhostsHeader);
            ToggleRoles = CustomOption.Create(6049, Types.General, "Toggle Roles With Shift", false, GhostsHeader);

            DynamicMap = CustomOption.Create(6100, Types.General, "Play On A Random Map", false, null, true);
            DynamicMapEnableSkeld = CustomOption.Create(6101, Types.General, "Skeld Rotation Chance", RoleRates, DynamicMap, false);
            DynamicMapEnableMira = CustomOption.Create(6102, Types.General, "Mira Rotation Chance", RoleRates, DynamicMap, false);
            DynamicMapEnablePolus = CustomOption.Create(6103, Types.General, "Polus Rotation Chance", RoleRates, DynamicMap, false);
            DynamicMapEnableAirShip = CustomOption.Create(6104, Types.General, "Airship Rotation Chance", RoleRates, DynamicMap, false);
            DynamicMapEnableSubmerged = CustomOption.Create(6105, Types.General, "Submerged Rotation Chance", RoleRates, DynamicMap, false);

            DeveloperSettings = CustomOption.Create(8000, Types.General, cs(Administrator.Color, "Developer Settings"), new string[] { "Hide", "Show" }, null, true);
            TurnOffRomanticToRefugee = CustomOption.Create(8007, Types.General, "Turn Off Romantic to Refugee", false, DeveloperSettings);
            ModifierAscended = CustomOption.Create(8005, Types.General, cs(Spiteful.Color, "Ascended"), RoleRates, DeveloperSettings);
            ModifierAscendedQuantity = CustomOption.Create(8006, Types.General, cs(Spiteful.Color, "Ascended") + " Quantity", 1f, 1f, 5f, 1f, ModifierAscended);

            #endregion Other options
        }
    }
}
