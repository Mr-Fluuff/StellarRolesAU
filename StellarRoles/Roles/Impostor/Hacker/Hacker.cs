using UnityEngine;

namespace StellarRoles
{
    public enum InformationSource
    {
        Vitals,
        Admin,
        Cameras,
        None
    }

    public static class Hacker
    {
        public const float DownloadMultiplier = 2f;

        public static PlayerControl Player { get; set; }
        public static PlayerControl CurrentTarget { get; set; }
        public static readonly Color Color = Palette.ImpostorRed;

        public static float JamDuration => CustomOptionHolder.HackerJamDuration.GetFloat();
        // TODO: does this game Option need to be removed
        public static float JamCooldown => CustomOptionHolder.HackerJamCooldown.GetFloat();

        public static float MaxDownloadDuration => CustomOptionHolder.HackerMaximumDownloadDuration.GetFloat();

        public static float DownloadTime { get; set; } = 0f;
        public static float RemainingDownloadTime { get; set; } = 0f;
        public static bool IsDownloading { get; set; } = false;

        public static bool LockedOut { get; set; }
        public static int JamCharges { get; set; }
        public static int JamChargesPerKill => CustomOptionHolder.HackerJamChargesPerKill.GetInt();

        public static InformationSource CurrentTargetInformationSource { get; set; }
        public static InformationSource InformationSource { get; set; }

        public static bool AdminActive { get; set; } = false;
        public static bool VitalsActive { get; set; } = false;
        public static bool CamerasActive { get; set; } = false;

        public static Minigame HackedMinigame { get; set; } = null;

        private static Sprite _AdminSprite;
        private static Sprite _CamSprite;
        private static Sprite _NoCamSprite;
        private static Sprite _LogSprite;
        private static Sprite _VitalsSprite;
        private static Sprite _DownloadSprite;
        private static Sprite _JamSprite;

        public static Sprite GetAdminSprite()
        {
            if (_AdminSprite == null)
                SetAdminSprite();

            return _AdminSprite;
        }

        public static float CalculateHackerMultiplier()
        {
            float result = DownloadMultiplier;
            if (Ascended.IsAscended(Hacker.Player))
            {
                result = 3f;
            }
            return result;
        }
        public static void SetAdminSprite()
        {
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            switch (mapId)
            {
                case 0:
                case 3:
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.eviladminskeld.png", 115f);
                    break;
                case 1:
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.eviladminmira.png", 115f);
                    break;
                case 2:
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.eviladminpolus.png", 115f);
                    break;
                case 4:
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.eviladminairship.png", 115f);
                    break;
                case 5:
                    _AdminSprite = Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.eviladminsubmerged.png", 115f);
                    break;
            }
        }

        public static bool NoCamsFirstRound()
        {
            return MapOptions.IsFirstRound && MapOptions.NoCamsFirstRound && !Helpers.IsMap(Map.Mira) && CurrentTargetInformationSource == InformationSource.Cameras;
        }

        public static Sprite GetLogSprite()
        {
            return _LogSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.evildoorlog.png", 115f);
        }

        public static Sprite GetDownloadSprite()
        {
            return NoCamsFirstRound()
                ? (_NoCamSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.R1EvilCamera.png", 115f))
                : (_DownloadSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.hackerdownload.png", 115f));
        }

        public static Sprite GetCamSprite()
        {
            return _CamSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.evilcamera.png", 115f);

        }
        public static Sprite GetJamSprite()
        {
            return _JamSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.HackerJam.png", 115f);
        }

        public static Sprite GetVitalsSprite()
        {
            return _VitalsSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.Hacker.evilvitals.png", 115f);
        }

        public static void RoundReset()
        {
            CurrentTargetInformationSource = InformationSource.None;
            InformationSource = InformationSource.None;
            DownloadTime = 0f;
            IsDownloading = false;
            AdminActive = false;
            VitalsActive = false;
            CamerasActive = false;
            HackedMinigame = null;
            RemainingDownloadTime = MaxDownloadDuration;
            LockedOut = false;
        }

        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            JamCharges = 0;
            RoundReset();
            LockedOut = false;
        }

        public static void GetDescription()
        {
            string description =
                $"The {nameof(Hacker)} is able to download and disrupt system information.\n\n" +
                $"The {nameof(Hacker)}'s Download ability becomes available when standing near any information spot (Admin, Cams, Vitals, or Door Logs) " +
                $"and will be locked to that information spot for the rest of the round when used.\n\n" +
                $"Using Download near an information spot will charge up a mobile battery for that information spot. " +
                $"Every 1 second spent downloading will convert to 2 seconds of battery life. " +
                $"Battery time is cleared when the round ends.";

            if (JamChargesPerKill > 0)
                description +=
                    $"\n\nWhen the {nameof(Hacker)} kills a player it gets {Helpers.ColorString(Color.yellow, JamChargesPerKill.ToString())} charges of the ability Jam. " +
                    $"Jam disrupts all information spots for {Helpers.ColorString(Color.yellow, JamDuration.ToString())} seconds as if the comms sabotage has been called. " +
                    $"Emergency meetings may still be called while Jam is active.";

            RoleInfo.Hacker.SettingsDescription = Helpers.WrapText(description);
        }
    }
}
