using Epic.OnlineServices.RTCAudio;
using System.Collections.Generic;
using UnityEngine;
using Types = StellarRoles.CustomOption.CustomOptionType;

namespace StellarRoles
{
    public class CustomOptionHolder
    {
        private static readonly string[] RoleRates = ["0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%"];
        private static readonly string[] ModifierRates = ["1", "2", "3"];
        private static readonly string[] Presets = ["Default", "Proximity", "Standard", "Beginner", "Chaotic", "Vanilla Tweaks", "Vanilla Plus", "Custom 1", "Custom 2", "Custom 3"];
        private static readonly string[] SpeedRates = [".7x", ".8x", ".9x", "1.0x", "1.1x", "1.2x", "1.3x", "1.4x", "1.5x", "1.6x", "1.7x", "1.8x", "1.9x", "2.0x", "2.1x", "2.2x", "2.3x"];
        private static readonly object[] OddRate15 = ["Off", 0f, 2.5f, 5f, 7.5f, 10f, 12.5f, 15f];
        private static readonly object[] Off020 = ["Off", 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f, 20f];
        private static readonly object[] MinerVents = ["After Meeting", 0f, 2.5f, 5f, 7.5f, 10f, 12.5f, 15f, 17.5f, 20f, 22.5f, 25f, 27.5f, 30f];
        private static readonly object[] EngiVents = ["Unlimited", 5f, 7.5f, 10f, 12.5f, 15f, 17.5f, 20f, 22.5f, 25f];
        private static readonly object[] MayorVotes = ["Disabled", 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16f, 17f, 18f, 19f, 20f];
        private static readonly object[] Timer = ["Disabled", 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f, 5.5f, 6f,6.5f, 7f, 7.5f, 8f, 8.5f, 9f, 9.5f, 10f, 10.5f, 11f, 11.5f, 12f, 12.5f, 13f, 13.5f, 14f, 14.5f, 15f, 15.5f, 16f, 16.5f, 17f, 17.5f, 18f, 18.5f, 19f, 19.5f, 20f];
        private static readonly object[] ParasiteControl = ["Unlimited", 5f, 7.5f, 10f, 12.5f, 15f, 17.5f, 20f, 22.5f, 25f, 27.5f, 30f, 32.5f, 35f, 37.5f, 40f, 42.5f, 45f];



        #region Presets
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

        public static CustomOption ParasiteSpawnRate { get; set; }
        public static CustomOption ParasiteInfestCooldown { get; set; }
        public static CustomOption ParasiteControlDuration { get; set; }
        public static CustomOption ParasiteSaveInfested { get; set; }
        public static CustomOption ParasiteNormalKillButton { get; set; }



        public static CustomOption CamouflagerSpawnRate { get; set; }
        public static CustomOption CamouflagerCooldown { get; set; }
        public static CustomOption CamouflagerDuration { get; set; }
        public static CustomOption CamouflagerChargesPerKill { get; set; }

        public static CustomOption CharlatanSpawnRate { get; set; }
        public static CustomOption CharlatanDeceiveBaseDuration { get; set; }
        public static CustomOption CharlatanDeceiveDurationPerKill { get; set; }
        public static CustomOption CharlatanConcealChargesPerKill { get; set; }
        public static CustomOption CharlatanConcealBaseCharges { get; set; }
        public static CustomOption CharlatanConcealChannelDuration { get; set; }
        public static CustomOption CharlatanConcealReportRange { get; set; }



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

        public static CustomOption UndertakerSpawnRate { get; set; }
        public static CustomOption UndertakerDragingDelaiAfterKill { get; set; }
        public static CustomOption UndertakerDragCooldown { get; set; }
        public static CustomOption UndertakerCanDragAndVent { get; set; }

        public static CustomOption CultistSpawnRate { get; set; }
        public static CustomOption CultistSpecialRolesEnabled { get; set; }
        public static CustomOption CultistChatInMeeting { get; set; }


        public static CustomOption MinerSpawnRate { get; set; }
        public static CustomOption MinerCooldown { get; set; }
        public static CustomOption MinerCharges { get; set; }
        public static CustomOption MinerVentsDelay { get; set; }

        public static CustomOption MayorSpawnRate { get; set; }
        public static CustomOption MayorTasksNeededToSeeVoteColors { get; set; }
        public static CustomOption MayorCanRetire { get; set; }

        public static CustomOption EngineerSpawnRate { get; set; }
        public static CustomOption EngineerHasFix { get; set; }
        public static CustomOption EngineerCanVent { get; set; }
        public static CustomOption EngineerVentCooldown { get; set; }
        public static CustomOption EngineerVentTimer { get; set; }

        public static CustomOption EngineerHighlightForEvil { get; set; }
        public static CustomOption EngineerAdvancedSabotageRepairs { get; set; }

        public static CustomOption SheriffSpawnRate { get; set; }
        public static CustomOption SheriffMisfireKills { get; set; }
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
        public static CustomOption GuardianSelfShield { get; set; }


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
        public static CustomOption JailedTargetsGuessed { get; set; }


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
        public static CustomOption CharlatanIsNeutral { get; set; }
        public static CustomOption WraithIsNeutral { get; set; }
        public static CustomOption ShadeIsNeutral { get; set; }
        public static CustomOption ParasiteIsNeutral { get; set; }

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
        public static CustomOption JoustingHeader { get; set; }
        public static CustomOption VanillaTweaksHeader { get; set; }

        public static CustomOption JoustingRoleNKWin { get; set; }
        public static CustomOption JoustingRoleImpWin { get; set; }
        public static CustomOption HideOutOfSightNametags { get; set; }
        public static CustomOption VentInFog { get; set; }
        public static CustomOption ToggleRoles { get; set; }
        public static CustomOption GameStartKillCD { get; set; }

        public static CustomOption isDraftMode;
        public static CustomOption draftModeAmountOfChoices;
        public static CustomOption draftModeTimeToChoose;
        public static CustomOption draftModeShowRoles;
        public static CustomOption draftModeHideImpRoles;
        public static CustomOption draftModeHideNeutralRoles;

        public static CustomOption EnableImpChat { get; set; }

        public static CustomOption GeneralHeader { get; set; }
        public static CustomOption DisabledRolesSkeldHeader { get; set; }
        public static CustomOption DisabledRolesMiraHeader { get; set; }
        public static CustomOption DisabledRolesFungleHeader { get; set; }


        public static CustomOption EnableFlashlightMode { get; set; }
        public static CustomOption CrewFlashlightRange { get; set; }
        public static CustomOption ImpFlashlightRange { get; set; }

        public static CustomOption NoCamsFirstRound { get; set; }
        public static CustomOption MiraAdminTasks { get; set; }
        public static CustomOption MiraLogsTasks { get; set; }

        public static CustomOption PolusAdminTasks { get; set; }
        public static CustomOption SkeldAdminTasks { get; set; }

        public static CustomOption FungalSecurityTasks { get; set; }
        public static CustomOption FungalAdminTable { get; set; }
        public static CustomOption FungalEasierDoorSabo { get; set; }
        public static CustomOption FungalEasierFish { get; set; }

        public static CustomOption SkeldCamsTasks { get; set; }
        public static CustomOption PolusCamsTasks { get; set; }
        public static CustomOption OverrideMapSettings { get; set; }
        public static CustomOption OverrideSkeld { get; set; }
        public static CustomOption OverrideMira { get; set; }
        public static CustomOption OverridePolus { get; set; }
        public static CustomOption OverrideAirship { get; set; }
        public static CustomOption OverrideFungle { get; set; }
        public static CustomOption OverrideSubmerged { get; set; }

        public static CustomOption DynamicMap { get; set; }
        public static CustomOption DynamicMapEnableSkeld { get; set; }
        public static CustomOption SkeldCommonTasks { get; set; }
        public static CustomOption SkeldLongTasks { get; set; }
        public static CustomOption SkeldShortTasks { get; set; }
        public static CustomOption SkeldKillCD { get; set; }
        public static CustomOption SkeldButtonCD { get; set; }
        public static CustomOption SkeldCrewVision { get; set; }

        public static CustomOption DynamicMapEnableMira { get; set; }
        public static CustomOption MiraCommonTasks { get; set; }
        public static CustomOption MiraLongTasks { get; set; }
        public static CustomOption MiraShortTasks { get; set; }
        public static CustomOption MiraKillCD { get; set; }
        public static CustomOption MiraButtonCD { get; set; }
        public static CustomOption MiraCrewVision { get; set; }

        public static CustomOption DynamicMapEnablePolus { get; set; }
        public static CustomOption PolusCommonTasks { get; set; }
        public static CustomOption PolusLongTasks { get; set; }
        public static CustomOption PolusShortTasks { get; set; }
        public static CustomOption PolusKillCD { get; set; }
        public static CustomOption PolusButtonCD { get; set; }
        public static CustomOption PolusCrewVision { get; set; }



        public static CustomOption DynamicMapEnableAirShip { get; set; }
        public static CustomOption AirShipCommonTasks { get; set; }
        public static CustomOption AirShipLongTasks { get; set; }
        public static CustomOption AirShipShortTasks { get; set; }
        public static CustomOption AirShipKillCD { get; set; }
        public static CustomOption AirShipButtonCD { get; set; }
        public static CustomOption AirShipCrewVision { get; set; }



        public static CustomOption DynamicMapEnableFungal { get; set; }
        public static CustomOption FungalCommonTasks { get; set; }
        public static CustomOption FungalLongTasks { get; set; }
        public static CustomOption FungalShortTasks { get; set; }
        public static CustomOption FungalKillCD { get; set; }
        public static CustomOption FungalButtonCD { get; set; }
        public static CustomOption FungalCrewVision { get; set; }



        public static CustomOption DynamicMapEnableSubmerged { get; set; }
        public static CustomOption SubmergedCommonTasks { get; set; }
        public static CustomOption SubmergedLongTasks { get; set; }
        public static CustomOption SubmergedShortTasks { get; set; }
        public static CustomOption SubmergedKillCD { get; set; }
        public static CustomOption SubmergedButtonCD { get; set; }
        public static CustomOption SubmergedCrewVision { get; set; }


        public static CustomOption DisableWatcherOnSkeld { get; set; }
        public static CustomOption DisableAdministratorOnMira { get; set; }

        public static CustomOption GhostsHeader { get; set; }
        public static CustomOption GhostsSeeRoles { get; set; }
        public static CustomOption GhostsSeeTasks { get; set; }
        public static CustomOption GhostsSeeModifiers { get; set; }
        public static CustomOption GhostsSeeRomanticTarget { get; set; }
        public static CustomOption GameTimer { get; set; }
        public static CustomOption LobbySize { get; set; }
        public static CustomOption DisableArsonistOnSkeld { get; set; }
        public static CustomOption DisableScavengerOnSkeld { get; set; }
        public static CustomOption DisableJanitorOnSkeld { get; set; }
        public static CustomOption DisableMedicOnSkeld { get; set; }

        public static CustomOption DisableScavengerOnMira { get; set; }
        public static CustomOption DisableJanitorOnMira { get; set; }
        public static CustomOption DisableMedicOnMira { get; set; }
        public static CustomOption DisableMinerOnMira { get; set; }

        public static CustomOption DisableScavengerOnFungle { get; set; }
        public static CustomOption DisableJanitorOnFungle { get; set; }
        public static CustomOption DisableMinerOnFungle { get; set; }
        public static CustomOption DisableAdministratorOnFungle { get; set; }
        public static CustomOption ImposterKillAbilitiesRoleBlock { get; set; }
        public static CustomOption ImposterAbiltiesRoleBlock { get; set; }

        public static CustomOption NeutralKillerRoleBlock { get; set; }
        public static CustomOption NeutralRoleBlock { get; set; }

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


        public static CustomOption MoveVitals { get; set; }
        public static CustomOption VentSystem { get; set; }
        public static CustomOption ColdTempDeathValley { get; set; }
        public static CustomOption WifiChartCourseSwap { get; set; }

        public static CustomOption HackerSpawnRate { get; set; }
        public static CustomOption HackerMaximumDownloadDuration { get; set; }
        public static CustomOption HackerJamChargesPerKill { get; set; }
        public static CustomOption HackerJamCooldown { get; set; }
        public static CustomOption HackerJamDuration { get; set; }
        public static CustomOption TournamentLogs { get; set; }

        public static readonly Dictionary<RoleId, RoleId[]> BlockedRolePairings = new()
        {
            { RoleId.Vampire, new[] { RoleId.Warlock, RoleId.Shade, RoleId.Bomber } },
            { RoleId.Warlock, new[] { RoleId.Vampire, RoleId.Shade, RoleId.Bomber } },
            { RoleId.Shade, new[] { RoleId.Warlock, RoleId.Vampire, RoleId.Bomber } },
            { RoleId.Bomber, new[] { RoleId.Warlock, RoleId.Shade, RoleId.Vampire } },
            { RoleId.Scavenger, new[] { RoleId.Janitor, RoleId.JanitorNK, RoleId.CharlatanNK, RoleId.Charlatan } },
            { RoleId.Janitor, new[] { RoleId.Scavenger, RoleId.Charlatan, RoleId.CharlatanNK } },
            { RoleId.JanitorNK, new[] { RoleId.Scavenger, RoleId.Charlatan, RoleId.CharlatanNK } },
            { RoleId.Charlatan, new[] { RoleId.Scavenger, RoleId.Janitor, RoleId.JanitorNK } },
            { RoleId.CharlatanNK, new[] { RoleId.Scavenger, RoleId.Janitor, RoleId.JanitorNK } },

            { RoleId.Cultist, new[] { RoleId.Spy, RoleId.Romantic, RoleId.Executioner, RoleId.HeadHunter } },
            { RoleId.Spy, new[] { RoleId.Cultist } },
            { RoleId.Romantic, new[] { RoleId.Cultist } },
            { RoleId.Executioner, new[] { RoleId.Cultist } },
            { RoleId.HeadHunter, new[] { RoleId.Cultist } },
            { RoleId.Undertaker, new[] { RoleId.Parasite} },
            { RoleId.Parasite, new[] { RoleId.Undertaker} },

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

        private static Color FungleColor = Color.green;
        private static Color SkeldColor = Color.gray;
        private static Color MiraColor = Spiteful.Color;
        private static Color PolusColor = Administrator.Color;
        private static Color SubmergedColor = Color.blue;
        private static Color AirshipColor = Color.red;
        private static string FungleName => cs(Color.green, "Fungle");
        private static string SkeldName => cs(Color.gray, "Skeld");
        private static string MiraName => cs(Spiteful.Color, "Mira");
        private static string PolusName => cs(Administrator.Color, "Polus");
        private static string SubmergedName => cs(Color.blue, "Submerged");
        private static string AirshipName => cs(Color.red, "Airship");


        public static void Load()
        {
            // Using new id's for the options to not break compatibilty with older versions

            CustomOption.VanillaSettings = StellarRolesPlugin.Instance.Config.Bind("Preset0", "VanillaOptions", "");

            CustomOption.Create(0, Types.General, "Current Preset", Presets, null, true, heading: "Presets");

            #region Impostors
            ImpostorRolesCountMin = CustomOption.Create(100, Types.Impostor, "Minimum Impostor Roles", 2f, 0f, 3f, 1f, null, true, null, "Impostor Min/Max Roles");
            ImpostorRolesCountMax = CustomOption.Create(101, Types.Impostor, "Maximum Impostor Roles", 2f, 0f, 3f, 1f);

            //Assassin
            CustomOption.CreateHeader(199, Types.Impostor, "Assassin", headerColor: Palette.ImpostorRed);
            AssassinCount = CustomOption.Create(200, Types.Impostor, cs(Palette.ImpostorRed, "Assassin") + " Count", 0f, 0f, 3f, 1f, null);
            AssassinNumberOfShots = CustomOption.Create(201, Types.Impostor, "Number Of Shared Shots", 5f, 0f, 15f, 1f, AssassinCount);
            AssassinMultipleShotsPerMeeting = CustomOption.Create(202, Types.Impostor, "Limit One Shot Per Meeting", false, AssassinCount);
            AssassinCanGuessCrewmate = CustomOption.Create(205, Types.Impostor, "Can Guess Crewmate", false, AssassinCount);

            //Bomber
            BomberSpawnRate = CustomOption.Create(250, Types.Impostor, "Bomber", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            BomberBombCooldown = CustomOption.Create(251, Types.Impostor, "Bomb Cooldown", 10f, 5f, 60f, 2.5f, BomberSpawnRate);
            BomberDelay = CustomOption.Create(252, Types.Impostor, "Bomb Delay", 5f, 1f, 10f, 0.5f, BomberSpawnRate);
            BomberTimer = CustomOption.Create(253, Types.Impostor, "Bomb Timer", 25f, 5f, 30f, 5f, BomberSpawnRate);
            BomberImpsSeeBombed = CustomOption.Create(254, Types.Impostor, "Partners See Bombed Target", true, BomberSpawnRate);
            BomberCanReport = CustomOption.Create(255, Types.Impostor, "Bombed Target Can Report", false, BomberSpawnRate);
            BomberHotPotatoMode = CustomOption.Create(256, Types.Impostor, "Hot Potato Mode", false, BomberSpawnRate);

            //Camouflager
            CamouflagerSpawnRate = CustomOption.Create(550, Types.Impostor, "Camouflager", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            CamouflagerCooldown = CustomOption.Create(551, Types.Impostor, "Camouflage Cooldown", 22.5f, 10f, 60f, 2.5f, CamouflagerSpawnRate);
            CamouflagerDuration = CustomOption.Create(552, Types.Impostor, "Camouflage Duration", 12.5f, 5f, 20f, 2.5f, CamouflagerSpawnRate);
            CamouflagerChargesPerKill = CustomOption.Create(570, Types.Impostor, "Camouflage Charges per Kill", 2f, 1f, 5f, 1f, CamouflagerSpawnRate);

            //Changling
            ChangelingSpawnRate = CustomOption.Create(350, Types.Impostor, "Changeling", RoleRates, null, true, headerColor: Palette.ImpostorRed);

            //Charlatan
            CharlatanSpawnRate = CustomOption.Create(355, Types.Impostor, "Charlatan", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            CharlatanDeceiveBaseDuration = CustomOption.Create(356, Types.Impostor, "Deceive Base Duration", 5f, 0f, 10f, 2.5f, CharlatanSpawnRate);
            CharlatanDeceiveDurationPerKill = CustomOption.Create(357, Types.Impostor, "Deceive Duration Increase per Kill", 2f, 0f, 5f, 1f, CharlatanSpawnRate);
            CharlatanConcealBaseCharges = CustomOption.Create(358, Types.Impostor, "Conceal Uses", 1f, 0f, 5f, 1f, CharlatanSpawnRate);
            CharlatanConcealChargesPerKill = CustomOption.Create(359, Types.Impostor, "Conceal Charges per Kill", 1f, 1f, 5f, 1f, CharlatanSpawnRate);
            CharlatanConcealReportRange = CustomOption.Create(353, Types.Impostor, "Conceal Report Range", ["Very Short", "Short"], CharlatanSpawnRate);
            CharlatanConcealChannelDuration = CustomOption.Create(354, Types.Impostor, "Conceal Channel Duration", 1f, 1f, 5f, 0.25f, CharlatanSpawnRate);

            //Cultist
            CultistSpawnRate = CustomOption.Create(600, Types.Impostor, "Cultist", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            CultistSpecialRolesEnabled = CustomOption.Create(8011, Types.Impostor, "Converted Role Resembles Previous Role", false, CultistSpawnRate);
            CultistChatInMeeting = CustomOption.Create(8012, Types.Impostor, "Cultist/Follower Meeting Chat", false, CultistSpawnRate);

            // Hacker
            HackerSpawnRate = CustomOption.Create(610, Types.Impostor, "Hacker", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            HackerMaximumDownloadDuration = CustomOption.Create(611, Types.Impostor, "Max Download Duration", 10f, 3f, 15f, 1f, HackerSpawnRate);
            HackerJamChargesPerKill = CustomOption.Create(612, Types.Impostor, "Jam Charges Per Kill", 2f, 0f, 5f, 1f, HackerSpawnRate);
            HackerJamCooldown = CustomOption.Create(613, Types.Impostor, "Jam Cooldown", 20f, 10f, 35f, 2.5f, HackerJamChargesPerKill);
            HackerJamDuration = CustomOption.Create(614, Types.Impostor, "Jam Duration", 12.5f, 5f, 20f, 2.5f, HackerJamChargesPerKill);

            //Janitor
            JanitorSpawnRate = CustomOption.Create(650, Types.Impostor, "Janitor", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            JanitorCooldown = CustomOption.Create(651, Types.Impostor, "Clean Cooldown", 30f, 10f, 60f, 2.5f, JanitorSpawnRate);
            JanitorInitialCharges = CustomOption.Create(652, Types.Impostor, "Initial Clean Charges", 10f, 1f, 15f, 1f, JanitorSpawnRate);
            JanitorChargesPerKill = CustomOption.Create(653, Types.Impostor, "Clean Charges per Kill", 2f, 0f, 3f, 1f, JanitorSpawnRate);

            //Miner
            MinerSpawnRate = CustomOption.Create(400, Types.Impostor, "Miner", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            MinerCooldown = CustomOption.Create(401, Types.Impostor, "Mine Cooldown", 15f, 10f, 60f, 2.5f, MinerSpawnRate);
            MinerCharges = CustomOption.Create(402, Types.Impostor, "Maximum Miner Vents", 12f, 3f, 60f, 1f, MinerSpawnRate);
            MinerVentsDelay = CustomOption.Create(404, Types.Impostor, "Vent Activation Delay", MinerVents, MinerSpawnRate);

            //Morphling
            MorphlingSpawnRate = CustomOption.Create(300, Types.Impostor, "Morphling", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            MorphlingCooldown = CustomOption.Create(301, Types.Impostor, "Morph Cooldown", 20f, 5f, 60f, 2.5f, MorphlingSpawnRate);
            MorphlingDuration = CustomOption.Create(302, Types.Impostor, "Morph Duration", 10f, 1f, 20f, 0.5f, MorphlingSpawnRate);

            //Parasite
            ParasiteSpawnRate = CustomOption.Create(307, Types.Impostor, "Parasite", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            ParasiteInfestCooldown = CustomOption.Create(308, Types.Impostor, "Infest Cooldown", 25f, 10f, 45f, 2.5f, ParasiteSpawnRate);
            ParasiteControlDuration = CustomOption.Create(306, Types.Impostor, "Control Duration", ParasiteControl, ParasiteSpawnRate);
            ParasiteSaveInfested = CustomOption.Create(309, Types.Impostor, "Parasite Death\nSaves Infested", false, ParasiteSpawnRate);
            ParasiteNormalKillButton = CustomOption.Create(305, Types.Impostor, "Parasite Normal Kill Button", false, ParasiteSpawnRate);


            //Shade
            ShadeSpawnRate = CustomOption.Create(500, Types.Impostor, "Shade", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            ShadeCooldown = CustomOption.Create(501, Types.Impostor, "Vanish Cooldown", 30f, 2.5f, 60f, 2.5f, ShadeSpawnRate);
            ShadeDuration = CustomOption.Create(502, Types.Impostor, "Vanish Duration", 10f, 1f, 60f, 1f, ShadeSpawnRate);
            ShadeEvidence = CustomOption.Create(503, Types.Impostor, "Evidence Duration", 10f, 1f, 30f, 1f, ShadeSpawnRate);
            ShadeKillsToGainBlind = CustomOption.Create(505, Types.Impostor, "Kills To Gain Blind", 3f, 1f, 10f, 1f, ShadeSpawnRate);
            ShadeBlindRange = CustomOption.Create(520, Types.Impostor, "Blind Range", [".75x", "1x", "1.25x", "1.5x", "1.75x", "2x", "2.25x", "2.5x", "2.75x", "3x", "Global"], ShadeSpawnRate);
            ShadeBlindCooldown = CustomOption.Create(506, Types.Impostor, "Blind Cooldown", 25f, 5f, 60f, 2.5f, ShadeSpawnRate);
            ShadeBlindDuration = CustomOption.Create(507, Types.Impostor, "Blind Duration", 10f, 5f, 15f, 1f, ShadeSpawnRate);

            //Undertaker
            UndertakerSpawnRate = CustomOption.Create(508, Types.Impostor, "Undertaker", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            UndertakerDragingDelaiAfterKill = CustomOption.Create(509, Types.Impostor, "Drag Cooldown After Kill", 0f, 0f, 15f, 1f, UndertakerSpawnRate);
            UndertakerDragCooldown = CustomOption.Create(510, Types.Impostor, "Drag Cooldown", 10f, 5f, 30f, 2.5f, UndertakerSpawnRate);
            UndertakerCanDragAndVent = CustomOption.Create(511, Types.Impostor, "Can Vent While Dragging", true, UndertakerSpawnRate);

            //Vampire
            VampireSpawnRate = CustomOption.Create(553, Types.Impostor, "Vampire", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            VampireKillDelay = CustomOption.Create(554, Types.Impostor, "Bite Kill Delay", 5f, 1f, 10f, 1f, VampireSpawnRate);
            VampireCooldown = CustomOption.Create(555, Types.Impostor, "Bite Cooldown", 25f, 10f, 60f, 2.5f, VampireSpawnRate);
            VampireKillButton = CustomOption.Create(556, Types.Impostor, "Vampire Also Has Kill Button", false, VampireSpawnRate);

            //Warlock
            WarlockSpawnRate = CustomOption.Create(700, Types.Impostor, "Warlock", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            WarlockCooldown = CustomOption.Create(701, Types.Impostor, "Curse Cooldown", 30f, 10f, 60f, 2.5f, WarlockSpawnRate);
            WarlockRootTime = CustomOption.Create(702, Types.Impostor, "Curse-Kill Root Time", 4f, 0f, 15f, 1f, WarlockSpawnRate);

            //Wraith
            WraithSpawnRate = CustomOption.Create(450, Types.Impostor, "Wraith", RoleRates, null, true, headerColor: Palette.ImpostorRed);
            WraithPhaseCooldown = CustomOption.Create(451, Types.Impostor, "Dash Cooldown", 30f, 10f, 60f, 2.5f, WraithSpawnRate);
            WraithPhaseDuration = CustomOption.Create(452, Types.Impostor, "Dash Duration", 10f, 3f, 15f, 1f, WraithSpawnRate);
            WraithLantern = CustomOption.Create(454, Types.Impostor, "Lantern", true, WraithSpawnRate);
            WraithLanternCooldown = CustomOption.Create(455, Types.Impostor, "Lantern Cooldown", 30f, 5f, 60f, 2.5f, WraithLantern);
            WraithLanternDuration = CustomOption.Create(456, Types.Impostor, "Lantern Duration", 10f, 1f, 20f, 0.5f, WraithLantern);
            WraithInvisibleDuration = CustomOption.Create(458, Types.Impostor, "Invisible Duration After Return", 2f, 0f, 5f, 0.5f, WraithLantern);
            #endregion Impostors

            #region Neutral Killing
            NeutralKillerRolesCountMin = CustomOption.Create(1500, Types.NeutralK, "Minimum Neutral Killing Roles", 0f, 0f, 15f, 1f, null, true, null, "Neutral Killing Min/Max Roles");
            NeutralKillerRolesCountMax = CustomOption.Create(1501, Types.NeutralK, "Maximum Neutral Killing Roles", 0f, 0f, 15f, 1f, null);

            CustomOption.CreateHeader(1549, Types.NeutralK, "Neutral Killer Extra Options");
            NeutralKillersGetNonCritSabo = CustomOption.Create(1550, Types.NeutralK, "Neutral Killers Get Non-Critical Sabotage", ["Imp Team Wipe", "Never", "Game Start"]);
            ImpsLooseCritSabo = CustomOption.Create(1551, Types.NeutralK, "Impostors Lose Critical Sabotage", ["Team Wipe", "Never"]);
            ImpsLoseDoors = CustomOption.Create(1552, Types.NeutralK, "Impostors Lose Door Sabotage", ["Team Wipe", "Never"]);

            CustomOption.CreateHeader(1548, Types.NeutralK, "Neutral Killer Assassin");
            NeutralKillerGainsAssassin = CustomOption.Create(1553, Types.NeutralK, "Neutral Killers Gain Assassin Ability", ["Never", "Imp Team Wipe", "Game Start"]);
            NeutralKillerAssassinNumberOfShots = CustomOption.Create(1554, Types.NeutralK, "Number Of Shots", 5f, 0f, 15f, 1f, NeutralKillerGainsAssassin);
            NeutralKillerAssassinMultipleShotsPerMeeting = CustomOption.Create(1555, Types.NeutralK, "Limit One Shot Per Meeting", false, NeutralKillerGainsAssassin);
            NeutralKillerAssassinCanGuessCrewmate = CustomOption.Create(1556, Types.NeutralK, "Can Guess Crewmate", false, NeutralKillerGainsAssassin);

            //Rogue Impostors
            EnableRogueImpostors = CustomOption.CreateHeader(1602, Types.NeutralK, "Rogue Impostors", headerColor: Color.grey);
            BomberIsNeutral = CustomOption.Create(1613, Types.NeutralK, "Bomber", false, EnableRogueImpostors);
            CamouflagerIsNeutral = CustomOption.Create(1607, Types.NeutralK, "Camouflager", false, EnableRogueImpostors);
            CharlatanIsNeutral = CustomOption.Create(1614, Types.NeutralK, "Charlatan", false, EnableRogueImpostors);
            JanitorIsNeutral = CustomOption.Create(1612, Types.NeutralK, "Janitor", false, EnableRogueImpostors);
            MinerIsNeutral = CustomOption.Create(1611, Types.NeutralK, "Miner", false, EnableRogueImpostors);
            MorphlingIsNeutral = CustomOption.Create(1603, Types.NeutralK, "Morphling", false, EnableRogueImpostors);
            ParasiteIsNeutral = CustomOption.Create(1610, Types.NeutralK, "Parasite", false, EnableRogueImpostors);
            ShadeIsNeutral = CustomOption.Create(1609, Types.NeutralK, "Shade", false, EnableRogueImpostors);
            UndertakerIsNeutral = CustomOption.Create(1605, Types.NeutralK, "Undertaker", false, EnableRogueImpostors);
            VampireIsNeutral = CustomOption.Create(1604, Types.NeutralK, "Vampire", false, EnableRogueImpostors);
            WarlockIsNeutral = CustomOption.Create(1606, Types.NeutralK, "Warlock", false, EnableRogueImpostors);
            WraithIsNeutral = CustomOption.Create(1608, Types.NeutralK, "Wraith", false, EnableRogueImpostors);

            //Headhunter
            HeadHunterSpawnRate = CustomOption.Create(1650, Types.NeutralK, "HeadHunter", RoleRates, null, true, headerColor: HeadHunter.Color);
            HeadHunterTrackingCooldown = CustomOption.Create(1651, Types.NeutralK, "Pursue Cooldown", 20f, 15f, 30f, 2.5f, HeadHunterSpawnRate);
            HeadHunterTrackerDuration = CustomOption.Create(1652, Types.NeutralK, "Pursue Duration", 5f, 5f, 30f, 2.5f, HeadHunterSpawnRate);

            //Nightmare
            NightmareSpawnRate = CustomOption.Create(1700, Types.NeutralK, "Nightmare", RoleRates, null, true, headerColor: Nightmare.Color);
            NightmareParalyzeCooldown = CustomOption.Create(1701, Types.NeutralK, "Paralyze Cooldown", 20f, 15f, 20f, 2.5f, NightmareSpawnRate);
            NightmareParalyzeDuration = CustomOption.Create(1702, Types.NeutralK, "Paralyze Duration", 7f, 3f, 15f, 1f, NightmareSpawnRate);
            NightmareBlindCooldown = CustomOption.Create(1703, Types.NeutralK, "Blind Cooldown", 20f, 10f, 35f, 2.5f, NightmareSpawnRate);
            NightmareBlindDuration = CustomOption.Create(1704, Types.NeutralK, "Blind Duration", 10f, 5f, 15f, 2.5f, NightmareSpawnRate);
            NightmareBlindRadius = CustomOption.Create(1705, Types.NeutralK, "Blind Radius", 1.0f, .25f, 5, .25f, NightmareSpawnRate);

            //Pyromaniac
            PyromaniacSpawnRate = CustomOption.Create(1800, Types.NeutralK, "Pyromaniac", RoleRates, null, true, headerColor: Pyromaniac.Color);
            PyromaniacDouseCooldown = CustomOption.Create(1801, Types.NeutralK, "Douse Cooldown", 22.5f, 10f, 30f, 2.5f, PyromaniacSpawnRate);
            PyromaniacDousedDuration = CustomOption.Create(1802, Types.NeutralK, "Douse Duration", 1f, 1f, 5f, 1f, PyromaniacSpawnRate);
            PyromaniacDouseKillCooldown = CustomOption.Create(1803, Types.NeutralK, "Doused Kill Cooldown", 3f, 0f, 15f, 1f, PyromaniacSpawnRate);
            #endregion Neutral Killing

            #region Neutrals
            NeutralRolesCountMin = CustomOption.Create(2500, Types.Neutral, "Minimum Neutral Roles", 0f, 0f, 15f, 1f, null, true, null, "Neutral Min/Max Roles");
            NeutralRolesCountMax = CustomOption.Create(2501, Types.Neutral, "Maximum Neutral Roles", 0f, 0f, 15f, 1f);

            //Arsonist
            ArsonistSpawnRate = CustomOption.Create(2650, Types.Neutral, "Arsonist", RoleRates, null, true, headerColor: Arsonist.Color);
            ArsonistCooldown = CustomOption.Create(2651, Types.Neutral, "Douse Cooldown", 22.5f, 2.5f, 60f, 2.5f, ArsonistSpawnRate);
            ArsonistDuration = CustomOption.Create(2652, Types.Neutral, "Douse Duration", 1f, 1f, 10f, 1f, ArsonistSpawnRate);
            ArsonistDouseIgniteRoundCooldown = CustomOption.Create(2654, Types.Neutral, "Douse/Ignite Round Start Cooldown", 15f, 10f, 30f, 2.5f, ArsonistSpawnRate);

            //Executioner
            ExecutionerSpawnRate = CustomOption.Create(2600, Types.Neutral, "Executioner", RoleRates, null, true, headerColor: Executioner.Color);
            ExecutionerPromotesTo = CustomOption.Create(2601, Types.Neutral, "Role on Target Death", ["Jester", "Refugee"], ExecutionerSpawnRate);
            ExecutionerConvertsImmediately = CustomOption.Create(2602, Types.Neutral, "Executioner Converts on Target Death", ["Immediately", "Next Meeting"], ExecutionerSpawnRate);

            //Jester
            JesterSpawnRate = CustomOption.Create(2550, Types.Neutral, "Jester", RoleRates, null, true, headerColor: Jester.Color);
            JesterCanCallEmergency = CustomOption.Create(2551, Types.Neutral, "Jester Can Call Emergency Meeting", true, JesterSpawnRate);
            JesterCanEnterVents = CustomOption.Create(2552, Types.Neutral, "Jester Can Enter Vents", true, JesterSpawnRate);
            JesterLightsOnVision = CustomOption.Create(2553, Types.Neutral, "Jester Lights On Vision", .75f, 0.25f, 5f, 0.25f, JesterSpawnRate);
            JesterLightsOffVision = CustomOption.Create(2554, Types.Neutral, "Jester Lights Off Vision", .5f, 0.25f, 5f, 0.25f, JesterSpawnRate);

            //Refugee
            RefugeeSpawnRate = CustomOption.CreateHeader(2800, Types.Neutral, "Refugee", headerColor: Refugee.Color);
            VestCooldown = CustomOption.Create(2801, Types.Neutral, "Refuge Cooldown", 25f, 10f, 60f, 2.5f, RefugeeSpawnRate);
            VestDuration = CustomOption.Create(2802, Types.Neutral, "Refuge Duration", 7f, 3f, 20f, 1f, RefugeeSpawnRate);

            //Romantic
            RomanticSpawnRate = CustomOption.Create(2750, Types.Neutral, "Romantic", RoleRates, null, true, headerColor: Romantic.Color);
            RomanticProtectCooldown = CustomOption.Create(2751, Types.Neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, RomanticSpawnRate);
            RomanticProtectDuration = CustomOption.Create(2752, Types.Neutral, "Protect Duration", 7f, 3f, 15f, 1f, RomanticSpawnRate);
            RomanticKnowsTargetRoleWhen = CustomOption.Create(2753, Types.Neutral, "Romantic Knows Target Identity", ["Instantly", "On Target Death"], RomanticSpawnRate);
            RomanticLoverSeesLove = CustomOption.Create(2755, Types.Neutral, "Romantic Target Knows They Are Selected", ["Never", "Next Meeting", "Instantly"], RomanticSpawnRate);
            RomanticOnAllImpsDead = CustomOption.Create(8002, Types.Neutral, "Imp Romantic Becomes on Last Impostor Death", ["Dead", "Refugee"], RomanticSpawnRate);


            //Scavenger
            ScavengerSpawnRate = CustomOption.Create(2700, Types.Neutral, "Scavenger", RoleRates, null, true, headerColor: Scavenger.Color);
            ScavengerCooldown = CustomOption.Create(2701, Types.Neutral, "Eat Cooldown", 20f, 10f, 60f, 2.5f, ScavengerSpawnRate);
            ScavengerNumberToWin = CustomOption.Create(2702, Types.Neutral, "Number of Eats to Win", 3f, 1f, 10f, 1f, ScavengerSpawnRate);
            ScavengerCanUseVents = CustomOption.Create(2703, Types.Neutral, "Scavenger Can Use Vents", true, ScavengerSpawnRate);
            ScavengerCorpsesTrackingCooldown = CustomOption.Create(2705, Types.Neutral, "Scavenge Cooldown", 20f, 5f, 120f, 2.5f, ScavengerSpawnRate);
            ScavengerCorpsesTrackingDuration = CustomOption.Create(2706, Types.Neutral, "Scavenge Duration", 2.5f, 2.5f, 30f, 2.5f, ScavengerSpawnRate);
            #endregion Neutrals

            #region Crewmates
            CrewmateRolesCountMin = CustomOption.Create(3500, Types.Crewmate, "Minimum Crewmate Roles", 0f, 0f, 15f, 1f, null, true, heading:"Crewmate Min/Max Roles");
            CrewmateRolesCountMax = CustomOption.Create(3501, Types.Crewmate, "Maximum Crewmate Roles", 0f, 0f, 15f, 1f);

            //Administrator
            AdministratorSpawnRate = CustomOption.Create(3900, Types.Crewmate, "Administrator", RoleRates, null, true, headerColor: Administrator.Color);
            AdministratorInitialBatteryTime = CustomOption.Create(3901, Types.Crewmate, "Initial Battery Charge", 0f, 0f, 25f, 1f, AdministratorSpawnRate);
            AdministratorBatteryTimePerTask = CustomOption.Create(3902, Types.Crewmate, "Charge Amount (Seconds)", 1f, 0f, 5f, 0.5f, AdministratorSpawnRate);
            AdministratorSelfChargingBatteryCooldown = CustomOption.Create(3903, Types.Crewmate, "(Tasks Complete) Self-Charge Interval", 20f, 10f, 30f, 2.5f, AdministratorSpawnRate);
            AdministratorDisableRoundOneAccess = CustomOption.Create(3905, Types.Crewmate, "Disable Charging and Access Round One", false, AdministratorSpawnRate);

            //Detective
            DetectiveSpawnRate = CustomOption.Create(4300, Types.Crewmate, "Detective", RoleRates, null, true, headerColor: Detective.Color);
            DetectiveInspectDuration = CustomOption.Create(4301, Types.Crewmate, "Inspect Duration", 2f, .5f, 5f, .5f, DetectiveSpawnRate);
            DetectiveInspectsPerRound = CustomOption.Create(4302, Types.Crewmate, "Inspects Per Round", 3f, 1f, 8f, 1f, DetectiveSpawnRate);
            DetectiveEnableCrimeScenes = CustomOption.Create(4303, Types.Crewmate, "Enable Crime Scenes", true, DetectiveSpawnRate);

            //Engineer
            EngineerSpawnRate = CustomOption.Create(3700, Types.Crewmate, "Engineer", RoleRates, null, true, headerColor: Engineer.Color);
            EngineerHasFix = CustomOption.Create(3701, Types.Crewmate, "Enable Remote Fix", ["Off", "On", "On Task Completion"], EngineerSpawnRate);
            EngineerCanVent = CustomOption.Create(3702, Types.Crewmate, "Can Vent", true, EngineerSpawnRate);
            EngineerVentCooldown = CustomOption.Create(3706, Types.Crewmate, "Vent Cooldown", 10f, 0f, 25f, 2.5f, EngineerSpawnRate);
            EngineerVentTimer = CustomOption.Create(3708, Types.Crewmate, "Max Time in Vents", EngiVents, EngineerSpawnRate);
            EngineerHighlightForEvil = CustomOption.Create(3703, Types.Crewmate, "Evil Killers See Vent Highlights", true, EngineerCanVent);
            EngineerAdvancedSabotageRepairs = CustomOption.Create(3704, Types.Crewmate, "Advanced Sabotage Repair", true, EngineerSpawnRate);

            //Guardian
            GuardianSpawnRate = CustomOption.Create(3875, Types.Crewmate, "Guardian", RoleRates, null, true, headerColor: Guardian.Color);
            GuardianShieldIsVisibleTo = CustomOption.Create(3882, Types.Crewmate, "Shield Is Visible To", ["Protected", "Unprotected", "Killers", "Everyone", "Guardian Only"], GuardianSpawnRate);
            GuardianShieldVisibilityDelay = CustomOption.Create(3877, Types.Crewmate, "Shield Visibility Delay", 10f, 0f, 30f, 2.5f, GuardianSpawnRate);
            GuardianVisionRangeOfShield = CustomOption.Create(3880, Types.Crewmate, "Shield Vision Range", .5f, .25f, 1.25f, .25f, GuardianSpawnRate);
            GuardianShieldFadesOnDeath = CustomOption.Create(3878, Types.Crewmate, "Shield Fades On Death", false, GuardianSpawnRate);
            GuardianSelfShield = CustomOption.Create(3879, Types.Crewmate, "One Time Self Shield", false, GuardianSpawnRate);

            //Investigator
            InvestigatorSpawnRate = CustomOption.Create(3800, Types.Crewmate, "Investigator", RoleRates, null, true, headerColor: Investigator.Color);
            InvestigatorAnonymousFootprints = CustomOption.Create(3801, Types.Crewmate, "Anonymous Footprints", false, InvestigatorSpawnRate);
            InvestigatorFootprintInterval = CustomOption.Create(3802, Types.Crewmate, "Footprint Interval", 0.25f, 0.25f, 10f, 0.25f, InvestigatorSpawnRate);
            InvestigatorFootprintDuration = CustomOption.Create(3803, Types.Crewmate, "Footprint Duration", 3.75f, 0.25f, 10f, 0.25f, InvestigatorSpawnRate);

            //Jailor
            JailorSpawnRate = CustomOption.Create(4200, Types.Crewmate, "Jailor", RoleRates, null, true, headerColor: Jailor.Color);
            InitialJailCharges = CustomOption.Create(4201, Types.Crewmate, "Initial Jail Charges", 0f, 0f, 10f, 1f, JailorSpawnRate);
            JailorTasksPerRecharge = CustomOption.Create(4203, Types.Crewmate, "Tasks Per Recharge", 2f, 1f, 10f, 1f, JailorSpawnRate);
            JailorCanJailSelf = CustomOption.Create(4205, Types.Crewmate, "Can Jail Self", true, JailorSpawnRate);
            JailedTargetsGuessed = CustomOption.Create(4206, Types.Crewmate, "Jailed Targets can be Guessed as Jailor", true, JailorCanJailSelf);

            //Mayor
            MayorSpawnRate = CustomOption.Create(3650, Types.Crewmate, "Mayor", RoleRates, null, true, headerColor: Mayor.Color);
            MayorTasksNeededToSeeVoteColors = CustomOption.Create(3652, Types.Crewmate, "Completed Tasks Needed To See Vote Colors", MayorVotes, MayorSpawnRate);
            MayorCanRetire = CustomOption.Create(3653, Types.Crewmate, "Enable Retire", true, MayorSpawnRate);

            //Medic
            MedicSpawnRate = CustomOption.Create(3950, Types.Crewmate, "Medic", RoleRates, null, true, headerColor: Medic.Color);
            MedicNonCrewFlash = CustomOption.Create(3956, Types.Crewmate, "Non-Crew Monitor Notification Delay", OddRate15, MedicSpawnRate);
            MedicInitialBatteryTime = CustomOption.Create(3951, Types.Crewmate, "Initial Battery Charge", 2f, 1f, 20f, 1f, MedicSpawnRate);
            MedicBatteryTimePerTask = CustomOption.Create(3952, Types.Crewmate, "Charge Amount (Seconds)", 1f, 0f, 5f, 0.5f, MedicSpawnRate);
            MedicSelfChargingBatteryCooldown = CustomOption.Create(3953, Types.Crewmate, "(Tasks Complete) Self-Charge Interval", 20f, 10f, 30f, 2.5f, MedicSpawnRate);
            MedicDisableRoundOneAccess = CustomOption.Create(3955, Types.Crewmate, "Disable Charging and Access Round One", false, MedicSpawnRate);

            //ParityCop
            ParityCopSpawnRate = CustomOption.Create(3600, Types.Crewmate, "Parity Cop", RoleRates, null, true, headerColor: ParityCop.Color);
            ParityCopNeutralsMatchKillers = CustomOption.Create(3601, Types.Crewmate, "Neutral Roles Match Killers", true, ParityCopSpawnRate);
            ParityCopCompareCooldown = CustomOption.Create(3605, Types.Crewmate, "Compare Cooldown", 10f, 5f, 30f, 2.5f, ParityCopSpawnRate);
            ParityCopFakeCompare = CustomOption.Create(3606, Types.Crewmate, "Enable Fake Out", false, ParityCopSpawnRate);

            //Psychic
            PsychicSpawnRate = CustomOption.Create(3610, Types.Crewmate, "Psychic", RoleRates, null, true, headerColor: Psychic.Color);
            PsychicPlayerRange = CustomOption.Create(3611, Types.Crewmate, "Player Counter Range", .75f, .5f, 1.5f, .25f, PsychicSpawnRate);
            PsychicDetectInvisible = CustomOption.Create(3612, Types.Crewmate, "Include Invisible Players", false, PsychicSpawnRate);
            PsychicDetectInVent = CustomOption.Create(3613, Types.Crewmate, "Include Venting Players", false, PsychicSpawnRate);

            //Sheriff
            SheriffSpawnRate = CustomOption.Create(3750, Types.Crewmate, "Sheriff", RoleRates, null, true, headerColor: Sheriff.Color);
            SheriffMisfireKills = CustomOption.Create(3751, Types.Crewmate, "Misfire Kills", ["Self", "One Target", "Both"], SheriffSpawnRate);
            SheriffCanKillArsonist = CustomOption.Create(3753, Types.Crewmate, "Can Kill " + cs(Arsonist.Color, "Arsonist"), true);
            SheriffCanKillJester = CustomOption.Create(3754, Types.Crewmate, "Can Kill " + cs(Jester.Color, "Jester"), true);
            SheriffCanKillExecutioner = CustomOption.Create(3755, Types.Crewmate, "Can Kill " + cs(Executioner.Color, "Executioner"), true);
            SheriffCanKillScavenger = CustomOption.Create(3756, Types.Crewmate, "Can Kill " + cs(Scavenger.Color, "Scavenger"), true);

            //Spy
            SpySpawnRate = CustomOption.Create(4050, Types.Crewmate, "Spy", RoleRates, null, true, headerColor: Spy.Color);
            SpyCanDieToSheriff = CustomOption.Create(4051, Types.Crewmate, "Spy Dies to Sheriff", false, SpySpawnRate);
            SpyImpostorsCanKillAnyone = CustomOption.Create(4052, Types.Crewmate, "Impostor Friendly Fire", true, SpySpawnRate);
            AssassinCanKillSpy = CustomOption.Create(203, Types.Crewmate, "Spy Can Be Guessed", false, SpySpawnRate);

            //Tracker
            TrackerSpawnRate = CustomOption.Create(4000, Types.Crewmate, "Tracker", RoleRates, null, true, headerColor: Tracker.Color);
            TrackerTracksPerRound = CustomOption.Create(4002, Types.Crewmate, "Max Marks Per Round", 1f, 1f, 3f, 1f, TrackerSpawnRate);
            TrackerAnonymousArrows = CustomOption.Create(4006, Types.Crewmate, "Anonymous Tracking Arrows", true, TrackerSpawnRate);
            TrackerTrackCooldown = CustomOption.Create(4003, Types.Crewmate, "Track Cooldown", 15f, 10f, 30f, 2.5f, TrackerSpawnRate);
            TrackerTrackDuration = CustomOption.Create(4001, Types.Crewmate, "Track Duration", 10f, 5f, 20f, 2.5f, TrackerSpawnRate);
            TrackerDelayDuration = CustomOption.Create(4005, Types.Crewmate, "Track Delay Duration", 5f, 3f, 10f, 1f, TrackerSpawnRate);

            //Trapper
            TrapperSpawnRate = CustomOption.Create(4150, Types.Crewmate, "Trapper", RoleRates, null, true, headerColor: Trapper.Color);
            TrapperNumberOfTraps = CustomOption.Create(4151, Types.Crewmate, "Number of Traps", 3f, 1f, 15f, 1f, TrapperSpawnRate);
            TrapperNumberOfCovers = CustomOption.Create(4152, Types.Crewmate, "Number of Covers", 3f, 1f, 15f, 1f, TrapperSpawnRate);
            TrapperTrapCoverCooldown = CustomOption.Create(4153, Types.Crewmate, "Trap/Cover Cooldown", 20f, 5f, 60f, 2.5f, TrapperSpawnRate);
            TrapperRootDuration = CustomOption.Create(4155, Types.Crewmate, "Trap Root Duration", 4f, 1f, 10f, 1f, TrapperSpawnRate);

            //Vigilante
            VigilanteSpawnRate = CustomOption.Create(3550, Types.Crewmate, "Vigilante", RoleRates, null, true, headerColor: new Color(Vigilante.Color.r, Vigilante.Color.g, Vigilante.Color.b, 0.8f));
            VigilanteNumberOfShots = CustomOption.Create(3551, Types.Crewmate, "Number Of Shots", 5f, 1f, 15f, 1f, VigilanteSpawnRate);
            VigilanteHasMultipleShotsPerMeeting = CustomOption.Create(3552, Types.Crewmate, "Limit One Shot Per Meeting", false, VigilanteNumberOfShots);

            //Watcher
            WatcherSpawnRate = CustomOption.Create(4100, Types.Crewmate, "Watcher", RoleRates, null, true, headerColor: Watcher.Color);
            WatcherNonCrewFlash = CustomOption.Create(4110, Types.Crewmate, "Non-Crew Sensor Notification Delay", OddRate15, WatcherSpawnRate);
            WatcherAnonymousArrows = CustomOption.Create(4106, Types.Crewmate, "Anonymous Sensor Arrows", false, WatcherSpawnRate);
            WatcherInitialBatteryTime = CustomOption.Create(4104, Types.Crewmate, "Initial Battery Charge", 6f, 1f, 20f, 1f, WatcherSpawnRate);
            WatcherBatteryTimePerTask = CustomOption.Create(4105, Types.Crewmate, "Charge Amount (Seconds)", 2.5f, 0f, 5f, 0.5f, WatcherSpawnRate);
            WatcherSelfChargingBatteryCooldown = CustomOption.Create(4107, Types.Crewmate, "(Tasks Complete) Self-Charge Interval", 12.5f, 10f, 30f, 2.5f, WatcherSpawnRate);
            WatcherRoundOneAccess = CustomOption.Create(4109, Types.Crewmate, "Disable Charging and Access Round One", true, WatcherSpawnRate);
            #endregion Crewmates

            #region Modifiers
            ModifierCosmeticMin = CustomOption.Create(5002, Types.Modifier, "Minimum Cosmetic Modifiers", 0f, 0f, 15f, 1f, null, true, null, "Cosmetic Modifier Min/Max");
            ModifierCosmeticMax = CustomOption.Create(5003, Types.Modifier, "Maximum Cosmetic Modifiers", 0f, 0f, 15f, 1f);

            //Giant
            ModifierGiant = CustomOption.Create(5200, Types.Modifier, "Giant", RoleRates, null, true, headerColor: Spiteful.Color);
            ModifierGiantSpeed = CustomOption.Create(5201, Types.Modifier, cs(Spiteful.Color, "Giant") + " Speed Multiplier", SpeedRates, ModifierGiant);

            //Mini
            ModifierMini = CustomOption.Create(5150, Types.Modifier, "Mini", RoleRates, null, true, headerColor: Spiteful.Color);
            ModifierMiniSpeed = CustomOption.Create(5151, Types.Modifier, cs(Spiteful.Color, "Mini") + " Speed Multiplier", SpeedRates, ModifierMini);

            ModifiersImpCountMin = CustomOption.Create(5004, Types.Modifier, "Minimum Imposter Modifiers", 0f, 0f, 15f, 1f, null, true, null, "Impostor Modifer Min/Max");
            ModifiersImpCountMax = CustomOption.Create(5005, Types.Modifier, "Maximum Imposter Modifiers", 0f, 0f, 15f, 1f);

            //Clutch
            ModifierClutch = CustomOption.Create(5260, Types.Modifier, "Clutch", RoleRates, null, true, headerColor: Spiteful.Color);
            ModifierClutchImpact = CustomOption.Create(5261, Types.Modifier, cs(Spiteful.Color, "Clutch") + " Impact", ["10%", "20%", "30%", "40%", "50%"], ModifierClutch);
            ModifierClutchCount = CustomOption.Create(5263, Types.Modifier, cs(Spiteful.Color, "Clutch") + " Quantity", ModifierRates, ModifierClutch);
            ModifierClutchSeeModifier = CustomOption.Create(5262, Types.Modifier, "Player Sees Modifier", true, ModifierClutch);

            ModifiersMiscCountMin = CustomOption.Create(5000, Types.Modifier, "Minimum Misc Modifiers", 0f, 0f, 15f, 1f, null, true, null, "Misc Modifiers Min/Max");
            ModifiersMiscCountMax = CustomOption.Create(5001, Types.Modifier, "Maximum Misc Modifiers", 0f, 0f, 15f, 1f);

            //Gopher
            ModifierGopher = CustomOption.Create(5300, Types.Modifier, "Gopher", RoleRates, null, true, headerColor: Spiteful.Color);
            ModifierGopherQuantity = CustomOption.Create(5301, Types.Modifier, cs(Spiteful.Color, "Gopher") + " Quantity", ModifierRates, ModifierGopher);

            //Sleepwalker
            ModifierSleepwalker = CustomOption.Create(5100, Types.Modifier, "Sleepwalker", RoleRates, null, true, headerColor: Spiteful.Color);
            ModifierSleepwalkerQuantity = CustomOption.Create(5101, Types.Modifier, cs(Spiteful.Color, "Sleepwalker") + " Quantity", ModifierRates, ModifierSleepwalker);
            ModifierSleepwalkerSeesModifier = CustomOption.Create(5202, Types.Modifier, "Player Sees Modifier", true, ModifierSleepwalker);

            //Sniper
            ModifierSniper = CustomOption.Create(5400, Types.Modifier, "Sniper", RoleRates, null, true, headerColor: Spiteful.Color);
            ModifierSniperQuantity = CustomOption.Create(5401, Types.Modifier, cs(Spiteful.Color, "Sniper") + " Quantity", ModifierRates, ModifierSniper);

            //Spiteful
            ModifierSpiteful = CustomOption.Create(5250, Types.Modifier, "Spiteful", RoleRates, null, true, headerColor: Spiteful.Color);
            ModifierSpitefulImpact = CustomOption.Create(5251, Types.Modifier, cs(Spiteful.Color, "Spiteful") + " Impact", ["25%", "50%", "75%", "100%"], ModifierSpiteful);
            ModifierSpitefulCount = CustomOption.Create(5252, Types.Modifier, cs(Spiteful.Color, "Spiteful") + " Quantity", 1f, 1f, 5f, 1f, ModifierSpiteful);
            ModifierSpitefulSeeModifier = CustomOption.Create(5253, Types.Modifier, "Player Sees Modifier", false, ModifierSpiteful);
            #endregion Modifiers

            #region Map Settings
            DynamicMap = CustomOption.Create(6100, Types.Map, "Play On A Random Map", false, null, true, null, "Dynamic Map Settings");
            DynamicMapEnableSkeld = CustomOption.Create(6101, Types.Map, $"{SkeldName} Rotation Chance", RoleRates, DynamicMap);
            DynamicMapEnableMira = CustomOption.Create(6102, Types.Map, $"{MiraName} Rotation Chance", RoleRates, DynamicMap);
            DynamicMapEnablePolus = CustomOption.Create(6103, Types.Map, $"{PolusName} Rotation Chance", RoleRates, DynamicMap);
            DynamicMapEnableAirShip = CustomOption.Create(6104, Types.Map, $"{AirshipName} Rotation Chance", RoleRates, DynamicMap);
            DynamicMapEnableFungal = CustomOption.Create(6110, Types.Map, $"{FungleName} Rotation Chance", RoleRates, DynamicMap);
            DynamicMapEnableSubmerged = CustomOption.Create(6105, Types.Map, $"{SubmergedName} Rotation Chance", RoleRates, DynamicMap);

            OverrideMapSettings = CustomOption.CreateHeader(6250, Types.Map, "Overide Map Settings");

            OverrideSkeld = CustomOption.Create(6251, Types.Map, "Overide Base Setting", false, OverrideMapSettings, true, null, "Skeld", SkeldColor);
            SkeldCommonTasks = CustomOption.Create(6120, Types.Map, "Common Tasks", 2f, 0f, 4f, 1, OverrideSkeld);
            SkeldLongTasks = CustomOption.Create(6121, Types.Map, "Long Tasks", 3f, 0f, 15f, 1, OverrideSkeld);
            SkeldShortTasks = CustomOption.Create(6122, Types.Map, "Short Tasks", 5f, 0f, 23f, 1, OverrideSkeld);
            SkeldButtonCD = CustomOption.Create(6123, Types.Map, "Emergency Button Cooldown", 20f, 0f, 60f, 5, OverrideSkeld);
            SkeldCrewVision = CustomOption.Create(6124, Types.Map, "Crew Vision", 0.5f, 0.25f, 5f, 0.25f, OverrideSkeld);
            SkeldKillCD = CustomOption.Create(6125, Types.Map, "Kill Cooldown", 22.5f, 10f, 60f, 2.5f, OverrideSkeld);

            OverrideMira = CustomOption.Create(6252, Types.Map, "Overide Base Setting", false, OverrideMapSettings, true, null, "Mira", MiraColor);
            MiraCommonTasks = CustomOption.Create(6130, Types.Map, "Common Tasks", 2f, 0f, 4f, 1, OverrideMira);
            MiraLongTasks = CustomOption.Create(6131, Types.Map, "Long Tasks", 3f, 0f, 15f, 1, OverrideMira);
            MiraShortTasks = CustomOption.Create(6132, Types.Map, "Short Tasks", 5f, 0f, 23f, 1, OverrideMira);
            MiraButtonCD = CustomOption.Create(6133, Types.Map, "Emergency Button Cooldown", 20f, 0f, 60f, 5, OverrideMira);
            MiraCrewVision = CustomOption.Create(6134, Types.Map, "Crew Vision", 0.5f, 0.25f, 5f, 0.25f, OverrideMira);
            MiraKillCD = CustomOption.Create(6135, Types.Map, "Kill Cooldown", 22.5f, 10f, 60f, 2.5f, OverrideMira);

            OverridePolus = CustomOption.Create(6253, Types.Map, "Overide Base Setting", false, OverrideMapSettings, true, null, "Polus", PolusColor);
            PolusCommonTasks = CustomOption.Create(6140, Types.Map, "Common Tasks", 2f, 0f, 4f, 1, OverridePolus);
            PolusLongTasks = CustomOption.Create(6141, Types.Map, "Long Tasks", 3f, 0f, 15f, 1, OverridePolus);
            PolusShortTasks = CustomOption.Create(6142, Types.Map, "Short Tasks", 5f, 0f, 23f, 1, OverridePolus);
            PolusButtonCD = CustomOption.Create(6143, Types.Map, "Emergency Button Cooldown", 20f, 0f, 60f, 5, OverridePolus);
            PolusCrewVision = CustomOption.Create(6144, Types.Map, "Crew Vision", 0.5f, 0.25f, 5f, 0.25f, OverridePolus);
            PolusKillCD = CustomOption.Create(6145, Types.Map, "Kill Cooldown", 22.5f, 10f, 60f, 2.5f, OverridePolus);

            OverrideAirship = CustomOption.Create(6254, Types.Map, "Overide Base Setting", false, OverrideMapSettings, true, null, "Airship", AirshipColor);
            AirShipCommonTasks = CustomOption.Create(6150, Types.Map, "Common Tasks", 2f, 0f, 4f, 1, OverrideAirship);
            AirShipLongTasks = CustomOption.Create(6151, Types.Map, "Long Tasks", 3f, 0f, 15f, 1, OverrideAirship);
            AirShipShortTasks = CustomOption.Create(6152, Types.Map, "Short Tasks", 5f, 0f, 23f, 1, OverrideAirship);
            AirShipButtonCD = CustomOption.Create(6153, Types.Map, "Emergency Button Cooldown", 25f, 0f, 60f, 5, OverrideAirship);
            AirShipCrewVision = CustomOption.Create(6154, Types.Map, "Crew Vision", 0.5f, 0.25f, 5f, 0.25f, OverrideAirship);
            AirShipKillCD = CustomOption.Create(6155, Types.Map, "Kill Cooldown", 27.5f, 10f, 60f, 2.5f, OverrideAirship);

            OverrideFungle = CustomOption.Create(6255, Types.Map, "Overide Base Setting", false, OverrideMapSettings, true, null, "Fungle", FungleColor);
            FungalCommonTasks = CustomOption.Create(6160, Types.Map, "Common Tasks", 2f, 0f, 4f, 1, OverrideFungle);
            FungalLongTasks = CustomOption.Create(6161, Types.Map, "Long Tasks", 4f, 0f, 15f, 1, OverrideFungle);
            FungalShortTasks = CustomOption.Create(6162, Types.Map, "Short Tasks", 5f, 0f, 23f, 1, OverrideFungle);
            FungalButtonCD = CustomOption.Create(6163, Types.Map, "Emergency Button Cooldown", 25f, 0f, 60f, 5, OverrideFungle);
            FungalCrewVision = CustomOption.Create(6164, Types.Map, "Crew Vision", 0.5f, 0.25f, 5f, 0.25f, OverrideFungle);
            FungalKillCD = CustomOption.Create(6165, Types.Map, "Kill Cooldown", 27.5f, 10f, 60f, 2.5f, OverrideFungle);

            OverrideSubmerged = CustomOption.Create(6256, Types.Map, "Overide Base Setting", false, OverrideMapSettings, true, null, "Submerged", SubmergedColor);
            SubmergedCommonTasks = CustomOption.Create(6170, Types.Map, "Common Tasks", 2f, 0f, 4f, 1, OverrideSubmerged);
            SubmergedLongTasks = CustomOption.Create(6171, Types.Map, "Long Tasks", 3f, 0f, 15f, 1, OverrideSubmerged);
            SubmergedShortTasks = CustomOption.Create(6172, Types.Map, "Short Tasks", 5f, 0f, 23f, 1, OverrideSubmerged);
            SubmergedButtonCD = CustomOption.Create(6173, Types.Map, "Emergency Button Cooldown", 20f, 0f, 60f, 5, OverrideSubmerged);
            SubmergedCrewVision = CustomOption.Create(6174, Types.Map, "Crew Vision", 0.5f, 0.25f, 5f, 0.25f, OverrideSubmerged);
            SubmergedKillCD = CustomOption.Create(6175, Types.Map, "Kill Cooldown", 22.5f, 10f, 60f, 2.5f, OverrideSubmerged);

            CustomOption.CreateHeader(6012, Types.Map, "Task-Locked Information");
            MiraAdminTasks = CustomOption.Create(6009, Types.Map, $"Tasks to Unlock {MiraName} Admin", 0f, 0f, 20f, 1, null);
            MiraLogsTasks = CustomOption.Create(6020, Types.Map, $"Tasks to Unlock {MiraName} Logs", 0f, 0f, 20f, 1);

            SkeldAdminTasks = CustomOption.Create(6018, Types.Map, $"Tasks to Unlock {SkeldName} Admin", 0f, 0f, 20f, 1, null);
            SkeldCamsTasks = CustomOption.Create(6010, Types.Map, $"Tasks to Unlock {SkeldName} Cameras", 0f, 0f, 20f, 1);

            PolusAdminTasks = CustomOption.Create(6019, Types.Map, $"Tasks to Unlock {PolusName} Admin", 0f, 0f, 20f, 1, null);
            PolusCamsTasks = CustomOption.Create(6017, Types.Map, $"Tasks to Unlock {PolusName} Cameras", 0f, 0f, 20f, 1);

            CustomOption.CreateHeader(7010, Types.Map, "Fungle Tweaks");
            FungalSecurityTasks = CustomOption.Create(7012, Types.Map, "Tasks to Unlock Binoculars", Off020);
            FungalAdminTable = CustomOption.Create(7013, Types.Map, "Add Admin Table", false);
            FungalEasierDoorSabo = CustomOption.Create(7014, Types.Map, "Easier Doors", false);
            FungalEasierFish = CustomOption.Create(7015, Types.Map, "Easier Fish/Grill", false);

            CustomOption.CreateHeader(6075, Types.Map, $"Better {PolusName}");
            MoveVitals = CustomOption.Create(6076, Types.Map, "Vitals Moved To Lab", true);
            VentSystem = CustomOption.Create(6077, Types.Map, "Reactor Vent Layout Change", true);
            ColdTempDeathValley = CustomOption.Create(6078, Types.Map, "Cold Temp Moved To Death Valley", true);
            WifiChartCourseSwap = CustomOption.Create(6079, Types.Map, "Reboot Wifi And Chart Course Swapped", true);

            DisabledRolesSkeldHeader = CustomOption.CreateHeader(9959, Types.Map, "Roles Disabled on Skeld");
            DisableArsonistOnSkeld = CustomOption.Create(9001, Types.Map, cs(Arsonist.Color, "Arsonist"), false, DisabledRolesSkeldHeader);
            DisableScavengerOnSkeld = CustomOption.Create(9002, Types.Map, cs(Scavenger.Color, "Scavenger"), true, DisabledRolesSkeldHeader);
            DisableJanitorOnSkeld = CustomOption.Create(9003, Types.Map, cs(Palette.ImpostorRed, "Janitor"), true, DisabledRolesSkeldHeader);
            DisableMedicOnSkeld = CustomOption.Create(9004, Types.Map, cs(Medic.Color, "Medic"), false, DisabledRolesSkeldHeader);
            DisableWatcherOnSkeld = CustomOption.Create(6107, Types.Map, cs(Watcher.Color, "Watcher"), true, DisabledRolesSkeldHeader);

            DisabledRolesMiraHeader = CustomOption.CreateHeader(9960, Types.Map, "Roles Disabled on Mira");
            DisableArsonistOnMira = CustomOption.Create(2653, Types.Map, cs(Arsonist.Color, "Arsonist"), false, DisabledRolesMiraHeader);
            DisableScavengerOnMira = CustomOption.Create(9006, Types.Map, cs(Scavenger.Color, "Scavenger"), true, DisabledRolesMiraHeader);
            DisableJanitorOnMira = CustomOption.Create(9007, Types.Map, cs(Palette.ImpostorRed, "Janitor"), true, DisabledRolesMiraHeader);
            DisableMedicOnMira = CustomOption.Create(9008, Types.Map, cs(Medic.Color, "Medic"), false, DisabledRolesMiraHeader);
            DisableMinerOnMira = CustomOption.Create(9009, Types.Map, cs(Palette.ImpostorRed, "Miner"), false, DisabledRolesMiraHeader);
            DisableAdministratorOnMira = CustomOption.Create(6106, Types.Map, cs(Administrator.Color, "Administrator"), true, DisabledRolesMiraHeader);

            DisabledRolesFungleHeader = CustomOption.CreateHeader(9961, Types.Map, "Roles Disabled on Fungle");
            DisableAdministratorOnFungle = CustomOption.Create(9011, Types.Map, cs(Administrator.Color, "Administrator"), false, DisabledRolesFungleHeader);
            DisableJanitorOnFungle = CustomOption.Create(9012, Types.Map, cs(Janitor.Color, "Janitor"), false, DisabledRolesFungleHeader);
            DisableMinerOnFungle = CustomOption.Create(9013, Types.Map, cs(Miner.Color, "Miner"), false, DisabledRolesFungleHeader);
            DisableScavengerOnFungle = CustomOption.Create(9014, Types.Map, cs(Scavenger.Color, "Scavenger"), false, DisabledRolesFungleHeader);

            #endregion Map Settings

            #region General options
            GeneralHeader = CustomOption.CreateHeader(5900, Types.General, "General Options");

            LobbySize = CustomOption.Create(5901, Types.General, "Lobby Size", 12, 4, 15, 1, GeneralHeader);
            GameStartKillCD = CustomOption.Create(5902, Types.General, "Game Start " + cs(Palette.ImpostorRed, "Kill Cooldown"), 10, 10, 35, 1, GeneralHeader);
            GameTimer = CustomOption.Create(5903, Types.General, "Game Timer (Minutes)", Timer, GeneralHeader);

            MaxNumberOfMeetings = CustomOption.Create(6000, Types.General, "Number Of Meetings", 10, 0, 15, 1, GeneralHeader);

            isDraftMode = CustomOption.Create(6080, Types.General, cs(Color.red, "Enable Role Draft"), false, null, true, null, "Role Draft");
            draftModeAmountOfChoices = CustomOption.Create(6081, Types.General, "Max Amount Of Roles\nTo Choose From", 5f, 2f, 15f, 1f, isDraftMode, false);
            draftModeTimeToChoose = CustomOption.Create(6082, Types.General,"Time For Selection", 5f, 5f, 25f, 1f, isDraftMode, false);
            draftModeShowRoles = CustomOption.Create(6083, Types.General, "Show Picked Roles", false, isDraftMode, false);
            draftModeHideImpRoles = CustomOption.Create(6084, Types.General, "Hide Impostor Roles", false, draftModeShowRoles, false);
            draftModeHideNeutralRoles = CustomOption.Create(6085, Types.General, "Hide Neutral Roles", false, draftModeShowRoles, false);

            //Imp Chat
            EnableImpChat = CustomOption.Create(8009, Types.General, "Enabled", false, null, true, null, "Impostor Chat");

            ShieldFirstKill = CustomOption.Create(6005, Types.General, "Shield Last Game First Kill", false, null, true, null, "First Round Options");
            RoundOneKilledIndicators = CustomOption.Create(6008, Types.General, "Mark Players Who Die\nRound 1 Next Game", true);
            NoCamsFirstRound = CustomOption.Create(6011, Types.General, "No Cameras First Round", true);

            EnableFlashlightMode = CustomOption.Create(6890, Types.General, "Enabled", false, null, true, null, "Flashlight Mode");
            CrewFlashlightRange = CustomOption.Create(6891, Types.General, "Crew Range", 1f, 0.5f, 2f, 0.25f, EnableFlashlightMode);
            ImpFlashlightRange = CustomOption.Create(6892, Types.General, "Impostor/Neutral Range", 1f, 0.5f, 2f, 0.25f, EnableFlashlightMode);

            VanillaTweaksHeader = CustomOption.CreateHeader(9984, Types.General, "Vanilla Tweaks");
            HideOutOfSightNametags = CustomOption.Create(6006, Types.General, "Hide Obstructed Player Names", true, VanillaTweaksHeader);
            AllowParallelMedBayScans = CustomOption.Create(6002, Types.General, "Parallel MedBay Scans", true, VanillaTweaksHeader);
            DisableMedscanWalking = CustomOption.Create(6003, Types.General, "Disable MedBay Animations", true, VanillaTweaksHeader);
            DisableVentCleanEjections = CustomOption.Create(6004, Types.General, "Disable Vent Cleaning Ejections", true, VanillaTweaksHeader);
            VentInFog = CustomOption.Create(6007, Types.General, "Hide Vent Animations Out of Vision", true, VanillaTweaksHeader);

            JoustingHeader = CustomOption.CreateHeader(6016, Types.General, "Jousting Options");
            JoustingRoleImpWin = CustomOption.Create(6013, Types.General, "Jousting Roles Prevent Impostor Victory", false, JoustingHeader);
            JoustingRoleNKWin = CustomOption.Create(6014, Types.General, "Jousting Roles Prevent Neutral Killer Victory", false, JoustingHeader);
            DeadCrewPreventTaskWin = CustomOption.Create(6015, Types.General, "Crewmate Wipe Prevents Task Win", false, JoustingHeader);

            CustomOption.CreateHeader(7001, Types.General, "Impostor Role Block Comms");
            ImposterKillAbilitiesRoleBlock = CustomOption.Create(7002, Types.General, "Kill Abilties Role Block", false);
            ImposterAbiltiesRoleBlock = CustomOption.Create(7003, Types.General, "Abilities Role Block", false);
            NeutralKillerRoleBlock = CustomOption.Create(7004, Types.General, "Enabled", false, null, true, heading:"Neutral Killing Role Block Comms");
            NeutralRoleBlock = CustomOption.Create(7005, Types.General, "Enabled", false, null, true, heading: "Neutral Role Block Comms");

            CustomOption.CreateHeader(7100, Types.General, "Crewmate Role Block Comms");
            DetectiveRoleBlock = CustomOption.Create(7109, Types.General, cs(Detective.Color, "Detective"), false);
            EngineerRoleBlock = CustomOption.Create(7102, Types.General, cs(Engineer.Color, "Engineer"), false);
            GuardianRoleBlock = CustomOption.Create(7104, Types.General, cs(Guardian.Color, "Guardian"), true);
            InvestigatorRoleBlock = CustomOption.Create(7103, Types.General, cs(Investigator.Color, "Investigator"), false);
            MedicRoleBlock = CustomOption.Create(7106, Types.General, cs(Medic.Color, "Medic"), false);
            ParityCopRoleBlock = CustomOption.Create(7101, Types.General, cs(ParityCop.Color, "Parity Cop"), false);
            PsychicRoleBlock = CustomOption.Create(7111, Types.General, cs(Psychic.Color, "Psychic"), false);
            SpyRoleBlock = CustomOption.Create(7107, Types.General, cs(Spy.Color, "Spy"), false);
            TrackerRoleBlock = CustomOption.Create(7105, Types.General, cs(Tracker.Color, "Tracker"), true);
            TrapperRoleBlock = CustomOption.Create(7110, Types.General, cs(Trapper.Color, "Trapper"), false);
            WatcherRoleBlock = CustomOption.Create(7108, Types.General, cs(Watcher.Color, "Watcher"), true);

            GhostsHeader = CustomOption.CreateHeader(6050, Types.General, "Ghost Options");
            GhostsSeeRoles = CustomOption.Create(6051, Types.General, "Ghosts See Roles", false, GhostsHeader);
            GhostsSeeTasks = CustomOption.Create(6052, Types.General, "Ghosts See Tasks Done", false, GhostsHeader);
            GhostsSeeModifiers = CustomOption.Create(6053, Types.General, "Ghosts See Modifiers", false, GhostsHeader);
            GhostsSeeRomanticTarget = CustomOption.Create(6054, Types.General, "Ghosts see Romantic Target", false, GhostsHeader);
            ToggleRoles = CustomOption.Create(6049, Types.General, "Toggle Role Visibility With Shift", false, GhostsHeader);

            CustomOption.CreateHeader(8000, Types.General, "Developer Settings", headerColor: Administrator.Color);
            TurnOffRomanticToRefugee = CustomOption.Create(8007, Types.General, "Turn Off Romantic to Refugee", false);
            ModifierAscended = CustomOption.Create(8005, Types.General, cs(Spiteful.Color, "Ascended") + " Spawn Chance", RoleRates);
            ModifierAscendedQuantity = CustomOption.Create(8006, Types.General, cs(Spiteful.Color, "Ascended") + " Quantity", 1f, 1f, 5f, 1f, ModifierAscended);
            TournamentLogs = CustomOption.Create(8008, Types.General, "Tournament Logs", false);

            #endregion General options
        }
    }
}
