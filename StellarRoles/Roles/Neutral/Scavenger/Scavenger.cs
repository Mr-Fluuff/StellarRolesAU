using StellarRoles.Objects;
using StellarRoles.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace StellarRoles
{
    public static class Scavenger
    {
        public static PlayerControl Player { get; set; } = null;
        public static readonly Color Color = new Color32(139, 69, 19, byte.MaxValue);
        public static readonly List<Arrow> LocalArrows = new();
        public static float Cooldown => CustomOptionHolder.ScavengerCooldown.GetFloat();
        public static float Duration => CustomOptionHolder.ScavengerCorpsesTrackingDuration.GetFloat();
        public static int ScavengerNumberToWin => CustomOptionHolder.ScavengerNumberToWin.GetInt();
        public static int EatenBodies { get; set; } = 0;
        public static bool TriggerScavengerWin { get; set; } = false;
        public static bool CanUseVents => CustomOptionHolder.ScavengerCanUseVents.GetBool();
        public static float CorpsesTrackingCooldown => CustomOptionHolder.ScavengerCorpsesTrackingCooldown.GetFloat();
        public static float CorpsesTrackingTimer { get; set; }

        private static Sprite _VentButtonSprite;
        private static Sprite _ButtonSprite;
        private static Sprite _TrackCorpsesButtonSprite;

        public static void GetDescription()
        {
            RoleInfo.Scavenger.SettingsDescription = Helpers.WrapText(
                $"The {nameof(Scavenger)} must eat {Helpers.ColorString(Color.yellow, ScavengerNumberToWin.ToString())} bodies to win the game." +
                $"\nEating a body has a {Helpers.ColorString(Color.yellow, Cooldown.ToString())} second cooldown.\n\n" +
                $"The {nameof(Scavenger)} has an ability called Scavenge that it may use to search for any existing bodies using arrows. " +
                $"This ability has a {Helpers.ColorString(Color.yellow, CorpsesTrackingCooldown.ToString())} second cooldown and lasts for {Helpers.ColorString(Color.yellow, Duration.ToString())} seconds.\n\n" +
                $"In the event that victory becomes mathematically impossible, the {nameof(Scavenger)} will turn into a {nameof(Refugee)}.");
        }

        public static Sprite GetButtonSprite()
        {
            return _ButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.VultureButton.png", 115f);
        }

        public static Sprite GetTrackCorpsesButtonSprite()
        {
            return _TrackCorpsesButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.BodyFindButton.png", 115f);
        }

        public static Sprite GetVentButtonSprite()
        {
            return _VentButtonSprite ??= Helpers.LoadSpriteFromResources("StellarRoles.Resources.VentButtons.ScavengerVent.png", 115f);
        }

        public static int BodiesRemainingToWin()
        {
            return ScavengerNumberToWin - EatenBodies;
        }

        public static int BodiesRemaining()
        {
            return Object.FindObjectsOfType<DeadBody>().Length;
        }

        public static void ScavengerToRefugeeCheck()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (Player == null || Player != localPlayer || Player.Data.IsDead || TriggerScavengerWin || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended) return;
            int CrewAlive = 0;
            int ImpsAlive = 0;
            int NksAlive = 0;
            int AlivePlayers = 0;
            foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (data.IsDead || data == null || data.Disconnected || data.PlayerId == localPlayer.PlayerId) continue;
                var player = data.Object;
                AlivePlayers++;

                if (data.Role.IsImpostor)
                {
                    ImpsAlive++;
                }
                else if (player.IsNeutralKiller())
                {
                    NksAlive++;
                }
                else
                {
                    CrewAlive++;
                }
            }

            int KillersAlive = ImpsAlive + NksAlive;
            bool AloneKillers = (ImpsAlive > 0 && NksAlive == 0) || (NksAlive == 1 && ImpsAlive <= 0);
            bool StillPotentialBodies = !AloneKillers && BodiesRemainingToWin() <= (AlivePlayers + BodiesRemaining() - KillersAlive);
            bool StillPotentialBodies2 = AloneKillers && (KillersAlive <= (CrewAlive + BodiesRemaining() - KillersAlive)) && (BodiesRemainingToWin() <= (CrewAlive + BodiesRemaining() - KillersAlive));

            bool shouldConvert = !StillPotentialBodies && !StillPotentialBodies2;

            if (shouldConvert)
            {
                RPCProcedure.Send(CustomRPC.ScavengerTurnToRefugee);
                RPCProcedure.ScavengerTurnToRefugee();
            }
        }

        public static void ClearAndReload()
        {
            Player = null;
            EatenBodies = 0;
            TriggerScavengerWin = false;
            foreach (Arrow arrow in LocalArrows)
                Object.Destroy(arrow.Object);
            LocalArrows.Clear();
            CorpsesTrackingTimer = 0f;
        }
    }
}
