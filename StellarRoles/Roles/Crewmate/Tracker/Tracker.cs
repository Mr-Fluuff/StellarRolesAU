using StellarRoles.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Tracker
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(100, 58, 220, byte.MaxValue);

        public static readonly List<Arrow> localArrows = new();
        public static readonly Dictionary<byte, Arrow> TrackedPlayerLocalArrows = new();

        public static bool RoleBlock => CustomOptionHolder.TrackerRoleBlock.GetBool();

        public static bool AnonymousArrows => CustomOptionHolder.TrackerAnonymousArrows.GetBool();

        public static float TrackDuration => CustomOptionHolder.TrackerTrackDuration.GetFloat();
        public static float TrackTargetCooldown => CustomOptionHolder.TrackerTrackCooldown.GetFloat();
        public static float DelayDuration => CustomOptionHolder.TrackerDelayDuration.GetFloat();

        public static PlayerControl CurrentTarget { get; set; } = null;
        public static readonly PlayerList TrackedPlayers = new();
        public static float NumberOfTracks { get; set; }

        public static bool UsedTracker { get; set; } = false;
        public static bool TrackActive { get; set; } = false;
        public static float TimeUntilUpdate { get; set; } = 0f;
        public static int TimeLeft { get; set; } = 0;

        private static Sprite _MarkButtonSprite;
        private static Sprite _TrackButtonSprite;
        private static Sprite _TrackedOverlaySprite;
        private static Sprite _TrackedToolTipSprite;

        public static void GetDescription()
        {
            string settingsDescription =
                $"The Tracker has two abilities: Mark and Track." +
                "\n\n" +
                $"The Mark ability is placed on a player to allow Track to be used on them." +
                "\n\n" +
                $"Pressing the track ability will trigger a {Helpers.ColorString(Color.yellow, DelayDuration.ToString())} second delay, " +
                $"then display the location of all marked targets for {Helpers.ColorString(Color.yellow, TrackDuration.ToString())} seconds. " +
                $"Non - crew players will be notified that they are about to be tracked as soon as this ability is used.";

            RoleInfo.Tracker.SettingsDescription = Helpers.WrapText(settingsDescription);
        }

        public static float CalculateTrackDuration()
        {
            float result = TrackDuration;
            if (Ascended.IsAscended(Tracker.Player))
            {
                result = result * 1.3f;
            }
            return result;
        }



        public static bool IsTrackedPlayer(byte playerId)
        {
            return TrackedPlayers.Contains(playerId);
        }

        public static Sprite GetTrackedOverlaySprite()
        {
            return _TrackedOverlaySprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.TrackerMeetingOverlay.png", 350f);
        }

        public static Sprite GetTrackedToolTipSprite()
        {
            return _TrackedToolTipSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.TrackedToolTip.png", 350f);
        }

        public static Sprite GetMarkButtonSprite()
        {
            return _MarkButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.TrackerMarkButton.png", 115f);
        }
        public static Sprite GetTrackButtonSprite()
        {
            return _TrackButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.TrackerTrackButton.png", 115f);
        }

        public static void ResetTracked()
        {
            CurrentTarget = null;
            UsedTracker = false;
            NumberOfTracks = CustomOptionHolder.TrackerTracksPerRound.GetFloat();
            foreach (Arrow arrow in TrackedPlayerLocalArrows.Values)
                if (arrow?.Object != null)
                    Object.Destroy(arrow.Object);
            TrackedPlayers.Clear();
            TrackedPlayerLocalArrows.Clear();
        }

        public static void ClearAndReload()
        {
            Player = null;
            ResetTracked();
            TimeUntilUpdate = 0f;
            TrackActive = false;
            foreach (Arrow arrow in localArrows)
                Object.Destroy(arrow.Object);
        }
    }

    public static class TrackerExtensions
    {
        public static bool IsTrackedPlayer(this PlayerControl player) => Tracker.IsTrackedPlayer(player.PlayerId);
    }
}
