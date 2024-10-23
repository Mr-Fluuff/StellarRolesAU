using StellarRoles.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Thanks EvilScum
namespace StellarRoles
{
    public static class MapOptions
    {
        // Set values
        public static bool HidePetFromOthers { get; set; } = false;
        public static int MaxNumberOfMeetings => Mathf.RoundToInt(CustomOptionHolder.MaxNumberOfMeetings.GetSelection());
        public static bool HideOutOfSightNametags => CustomOptionHolder.HideOutOfSightNametags.GetBool();
        public static bool GhostsSeeRoles => CustomOptionHolder.GhostsSeeRoles.GetBool();
        public static bool GhostsSeeModifier => CustomOptionHolder.GhostsSeeModifiers.GetBool();
        public static bool GhostsSeeRomanticTarget => CustomOptionHolder.GhostsSeeRomanticTarget.GetBool();
        public static bool GhostsSeeTasks => CustomOptionHolder.GhostsSeeTasks.GetBool();
        public static bool GhostsSeeVotes { get; set; } = true;
        public static bool ShowRoleSummary { get; set; } = true;
        public static bool AllowParallelMedBayScans => CustomOptionHolder.AllowParallelMedBayScans.GetBool();
        public static bool DisableVentCleanEjections => CustomOptionHolder.DisableVentCleanEjections.GetBool();
        public static bool DisableMedscanWalking => CustomOptionHolder.DisableMedscanWalking.GetBool();
        public static bool EnableSoundEffects { get; set; } = true;
        public static bool ShieldFirstKill => CustomOptionHolder.ShieldFirstKill.GetBool();
        public static bool HideVentInFog => CustomOptionHolder.VentInFog.GetBool();
        public static float Meetingtime { get; set; } = 20f;
        public static bool AddFungleAdmin => CustomOptionHolder.FungalAdminTable.GetBool();
        public static bool EasierFungalDoors => CustomOptionHolder.FungalEasierDoorSabo.GetBool();
        public static bool EasierFungalFish => CustomOptionHolder.FungalEasierFish.GetBool();
        public static bool CanUseFungalSecurity => CustomOptionHolder.FungalSecurityTasks.GetBool();

        public static bool ModifySkeld => CustomOptionHolder.OverrideSkeld.GetBool();
        public static bool ModifyMira => CustomOptionHolder.OverrideMira.GetBool();
        public static bool ModifyPolus => CustomOptionHolder.OverridePolus.GetBool();
        public static bool ModifyAirship => CustomOptionHolder.OverrideAirship.GetBool();
        public static bool ModifyFungle => CustomOptionHolder.OverrideFungle.GetBool();
        public static bool ModifySubmerged => CustomOptionHolder.OverrideSubmerged.GetBool();

        public static int TasksTilSkeldCams => CustomOptionHolder.SkeldCamsTasks.GetInt();
        public static int TasksTilPolusCams => CustomOptionHolder.PolusCamsTasks.GetInt();
        public static int TasksTilAdminAccessOnMira => CustomOptionHolder.MiraAdminTasks.GetInt();
        public static int TasksTilLogsAccessOnMira => CustomOptionHolder.MiraLogsTasks.GetInt();
        public static int TasksTilAdminAccessOnSkeld => CustomOptionHolder.SkeldAdminTasks.GetInt();
        public static int TasksTilAdminAccessOnPolus => CustomOptionHolder.PolusAdminTasks.GetInt();
        public static int TaskTilFungalSecurity => CustomOptionHolder.FungalSecurityTasks.GetInt();

        public static bool NoCamsFirstRound => CustomOptionHolder.NoCamsFirstRound.GetBool();
        public static bool Allimpsdead { get; set; } = false;
        public static bool LastImp { get; set; } = false;
        public static bool ToggleRoles => CustomOptionHolder.ToggleRoles.GetBool();
        public static bool ShowRoles { get; set; } = false;
        public static bool JoustingPreventImp => CustomOptionHolder.JoustingRoleImpWin.GetBool();
        public static bool JoustingPreventNK => CustomOptionHolder.JoustingRoleNKWin.GetBool();
        public static bool DeadCrewPreventTaskWin => CustomOptionHolder.DeadCrewPreventTaskWin.GetBool();
        public static bool ImposterKillAbilitiesRoleBlock => CustomOptionHolder.ImposterKillAbilitiesRoleBlock.GetBool();
        public static bool ImposterAbiltiesRoleBlock => CustomOptionHolder.ImposterAbiltiesRoleBlock.GetBool();
        public static bool NeutralKillerRoleBlock => CustomOptionHolder.NeutralKillerRoleBlock.GetBool();
        public static bool NeutralRoleBlock => CustomOptionHolder.NeutralRoleBlock.GetBool();
        public static int PlayersAlive { get; set; } = 0;
        public static int CrewAlive { get; set; } = 0;


        public static bool TournamentLogs => CustomOptionHolder.TournamentLogs.GetBool();

        // Updating values
        public static int MeetingsCount { get; set; } = 0;
        public static readonly List<SurvCamera> CamerasToAdd = new();
        public static readonly List<Vent> VentsToSeal = new();
        public static readonly PlayerList PlayerPetsToHide = new();
        public static readonly Dictionary<byte, PoolablePlayer> PlayerIcons = new();
        public static readonly Dictionary<byte, List<int>> VentsInUse = new();
        public static string FirstKillName { get; set; }
        public static PlayerControl FirstKillPlayer { get; set; }

        public static readonly List<string> FirstKillPlayersNames = new();
        public static readonly List<PlayerControl> FirstKillPlayers = new();
        public static bool IsFirstRound { get; set; } = true;

        public static bool firstRoundWithDead { get; set; } = true;
        public static bool ShowRoundOneKillIndicators => CustomOptionHolder.RoundOneKilledIndicators.GetBool();
        public static bool DeadBodiesAdminTable { get; set; } = true;
        public static void ClearAndReloadMapOptions()
        {
            MeetingsCount = 0;
            CamerasToAdd.Clear();
            VentsToSeal.Clear();
            PlayerIcons.Clear();
            VentsInUse.Clear();
            PlayerPetsToHide.Clear();

            FirstKillPlayer = null;
            FirstKillPlayers.Clear();

            Meetingtime = GameOptionsManager.Instance.currentNormalGameOptions.VotingTime + GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime;

            ShowRoles = true;
            IsFirstRound = true;
            firstRoundWithDead = true;
            Allimpsdead = false;
            LastImp = false;
            DeadBodiesAdminTable = false;
            PlayersAlive = PlayerControl.AllPlayerControls.Count;
            CrewAlive = PlayerControl.AllPlayerControls.Count;
        }

        public static void ReloadPluginOptions()
        {
            GhostsSeeVotes = StellarRolesPlugin.GhostsSeeVotes.Value;
            ShowRoleSummary = StellarRolesPlugin.ShowRoleSummary.Value;
            EnableSoundEffects = StellarRolesPlugin.EnableSoundEffects.Value;
            HidePetFromOthers = StellarRolesPlugin.HidePetFromOthers.Value;
        }

        public static bool CanUseAdmin()
        {
            if (Helpers.IsHideAndSeek ||
                PlayerControl.LocalPlayer.Data.IsDead ||
                Helpers.IsNeutral(PlayerControl.LocalPlayer) ||
                PlayerControl.LocalPlayer.Data.Role.IsImpostor ||
                Administrator.Player == PlayerControl.LocalPlayer && Administrator.IsActive)
                return true;

            (int playerCompleted, int playerTotal) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);

            switch (Helpers.CurrentMap())
            {
                case Map.Skeld:
                case Map.Dleks:
                    return playerCompleted >= Math.Min(TasksTilAdminAccessOnSkeld, playerTotal);
                case Map.Mira:
                    return playerCompleted >= Math.Min(TasksTilAdminAccessOnMira, playerTotal);
                case Map.Polus:
                    return playerCompleted >= Math.Min(TasksTilAdminAccessOnPolus, playerTotal);
                default:
                    return true;
            }
        }

        public static bool CanUseCameras()
        {
            if (Helpers.IsHideAndSeek)
                return true;
            if (NoCamsFirstRound && IsFirstRound)
                return false;
            if (PlayerControl.LocalPlayer.Data.IsDead
                || Helpers.IsNeutral(PlayerControl.LocalPlayer)
                || PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                return true;

            (int playerCompleted, int playerTotal) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
            switch (Helpers.CurrentMap())
            {
                case Map.Skeld:
                case Map.Dleks:
                    return playerCompleted >= Math.Min(TasksTilSkeldCams, playerTotal);
                case Map.Polus:
                    return playerCompleted >= Math.Min(TasksTilPolusCams, playerTotal);
                case Map.Fungal:
                    return playerCompleted >= Math.Min(TaskTilFungalSecurity, playerTotal) && CanUseFungalSecurity;
                default:
                    return true;
            }
        }
    }
}
