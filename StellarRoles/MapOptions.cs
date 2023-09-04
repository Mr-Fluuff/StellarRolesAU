using System.Collections.Generic;
using UnityEngine;

//Thanks EvilScum
namespace StellarRoles
{
    public static class MapOptions
    {
        // Set values
        public static bool HidePetFromOthers { get; set; } = false;
        public static int MaxNumberOfMeetings { get; set; } = 10;
        public static bool HideOutOfSightNametags { get; set; } = false;
        public static bool GhostsSeeRoles { get; set; } = false;
        public static bool GhostsSeeModifier { get; set; } = false;
        public static bool GhostsSeeRomanticTarget { get; set; } = false;
        public static bool GhostsSeeTasks { get; set; } = false;
        public static bool GhostsSeeVotes { get; set; } = true;
        public static bool ShowRoleSummary { get; set; } = true;
        public static bool AllowParallelMedBayScans { get; set; } = false;
        public static bool DisableVentCleanEjections { get; set; } = false;
        public static bool DisableMedscanWalking { get; set; } = false;
        public static bool EnableSoundEffects { get; set; } = true;
        public static bool ShieldFirstKill { get; set; } = false;
        public static bool HideVentInFog { get; set; } = true;
        public static float Meetingtime { get; set; } = 20f;
        public static float TasksTilAdminAccessOnMira { get; set; } = 0;
        public static float TasksTilSkeldCams { get; set; } = 0f;
        public static bool NoCamsFirstRound { get; set; } = false;
        public static bool Allimpsdead { get; set; } = false;
        public static bool LastImp { get; set; } = false;
        public static bool ToggleRoles { get; set; } = false;
        public static bool ShowRoles { get; set; } = false;
        public static bool JoustingPreventImp { get; set; } = false;
        public static bool JoustingPreventNK { get; set; } = false;
        public static bool DeadCrewPreventTaskWin { get; set; } = false;

        public static bool ImposterRoleBlocks { get; set; } = false;
        public static bool ImposterKillAbilitiesRoleBlock { get; set; } = false;
        public static bool ImposterAbiltiesRoleBlock { get; set; } = false;
        public static bool NeutralKillerRoleBlock { get; set; } = false;
        public static bool NeutralRoleBlock { get; set; } = false;
        public static int PlayersAlive { get; set; } = 0;

        // Updating values
        public static int MeetingsCount { get; set; } = 0;
        public static readonly List<SurvCamera> CamerasToAdd = new();
        public static readonly List<Vent> VentsToSeal = new();
        public static readonly List<PlayerControl> PlayerPetsToHide = new();
        public static readonly Dictionary<byte, PoolablePlayer> PlayerIcons = new();
        public static readonly Dictionary<byte, List<int>> VentsInUse = new();
        public static string FirstKillName { get; set; }
        public static PlayerControl FirstKillPlayer { get; set; }

        public static readonly List<string> FirstKillPlayersNames = new();
        public static readonly List<PlayerControl> FirstKillPlayers = new();
        public static bool IsFirstRound { get; set; } = true;
        public static bool ShowRoundOneKillIndicators { get; set; } = false;
        public static bool DeadBodiesAdminTable { get; set; } = true;
        public static void ClearAndReloadMapOptions()
        {
            MeetingsCount = 0;
            CamerasToAdd.Clear();
            VentsToSeal.Clear();
            PlayerIcons.Clear();
            VentsInUse.Clear();
            PlayerPetsToHide.Clear();

            MaxNumberOfMeetings = Mathf.RoundToInt(CustomOptionHolder.MaxNumberOfMeetings.GetSelection());
            DisableMedscanWalking = CustomOptionHolder.DisableMedscanWalking.GetBool();
            HideOutOfSightNametags = CustomOptionHolder.HideOutOfSightNametags.GetBool();
            AllowParallelMedBayScans = CustomOptionHolder.AllowParallelMedBayScans.GetBool();
            DisableVentCleanEjections = CustomOptionHolder.DisableVentCleanEjections.GetBool();
            ShieldFirstKill = CustomOptionHolder.ShieldFirstKill.GetBool();
            HideVentInFog = CustomOptionHolder.VentInFog.GetBool();
            FirstKillPlayer = null;
            FirstKillPlayers.Clear();
            ShowRoundOneKillIndicators = CustomOptionHolder.RoundOneKilledIndicators.GetBool();

            ToggleRoles = CustomOptionHolder.ToggleRoles.GetBool();
            GhostsSeeRoles = CustomOptionHolder.GhostsSeeRoles.GetBool();
            GhostsSeeModifier = CustomOptionHolder.GhostsSeeModifiers.GetBool();
            GhostsSeeTasks = CustomOptionHolder.GhostsSeeTasks.GetBool();
            Meetingtime = GameOptionsManager.Instance.currentNormalGameOptions.VotingTime + GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime;
            TasksTilAdminAccessOnMira = CustomOptionHolder.RestrictAdminOnMira.GetFloat();
            TasksTilSkeldCams = CustomOptionHolder.RestrictCamsOnSkeld.GetFloat();
            NoCamsFirstRound = CustomOptionHolder.NoCamsFirstRound.GetBool();
            GhostsSeeRomanticTarget = CustomOptionHolder.GhostsSeeRomanticTarget.GetBool();
            ShowRoles = true;
            IsFirstRound = true;
            Allimpsdead = false;
            LastImp = false;
            ImposterRoleBlocks = CustomOptionHolder.ImposterRoleBlock.GetBool();
            ImposterKillAbilitiesRoleBlock = CustomOptionHolder.ImposterKillAbilitiesRoleBlock.GetBool() && ImposterRoleBlocks;
            ImposterAbiltiesRoleBlock = CustomOptionHolder.ImposterAbiltiesRoleBlock.GetBool() && ImposterRoleBlocks;
            NeutralKillerRoleBlock = CustomOptionHolder.NeutralKillerRoleBlock.GetBool();
            NeutralRoleBlock = CustomOptionHolder.NeutralRoleBlock.GetBool();
            DeadBodiesAdminTable = false;
            JoustingPreventImp = CustomOptionHolder.JoustingRoleImpWin.GetBool();
            JoustingPreventNK = CustomOptionHolder.JoustingRoleNKWin.GetBool();
            DeadCrewPreventTaskWin = CustomOptionHolder.DeadCrewPreventTaskWin.GetBool();
            PlayersAlive = PlayerControl.AllPlayerControls.Count;
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
                !Helpers.IsMap(Map.Mira) ||
                PlayerControl.LocalPlayer.Data.IsDead ||
                Helpers.IsNeutral(PlayerControl.LocalPlayer) ||
                PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                return true;

            (int playerCompleted, _) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);

            //ADMINISTRATOR?????

            return playerCompleted >= TasksTilAdminAccessOnMira;
        }

        public static bool CanUseCameras()
        {
            if (Helpers.IsHideAndSeek)
                return true;
            if (NoCamsFirstRound && IsFirstRound)
                return false;
            return true;
        }

        public static bool CanUseSkeldCameras()
        {
            if (!CanUseCameras()) return false;

            if (Helpers.IsHideAndSeek
                || !Helpers.IsMap(Map.Skeld)
                || PlayerControl.LocalPlayer.Data.IsDead
                || PlayerControl.LocalPlayer.IsNeutral()
                || PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                return true;

            (int playerCompleted, _) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);

            return playerCompleted >= TasksTilSkeldCams;
        }
    }
}
