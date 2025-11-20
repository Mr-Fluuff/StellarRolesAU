using HarmonyLib;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class SleepwalkerPatches
    {
        // Save the position of the player prior to starting the climb / gap platform
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public static class ClimbLadderSleepwalker
        {
            [HarmonyPrefix]
            public static void ClimbLadder(PlayerPhysics __instance)
            {
                if (__instance.myPlayer == PlayerControl.LocalPlayer)
                {
                    Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
                }
            }
        }

        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
        public static class MovingPlatformBehaviourSleepwalker
        {
            [HarmonyPrefix]
            public static void UsePlatform(MovingPlatformBehaviour __instance, [HarmonyArgument(0)] PlayerControl player)
            {
                if (player == PlayerControl.LocalPlayer)
                {
                    Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
                }
            }
        }

        [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
        public static class FungleZiplineSleepwalker
        {
            [HarmonyPrefix]
            public static void UseZipline(ZiplineBehaviour __instance, PlayerControl player, bool fromTop)
            {
                if (player == PlayerControl.LocalPlayer)
                {
                    Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        public class StartMeetingPatch
        {
            [HarmonyPrefix]
            public static void StartMeeting()
            {
                var localplayer = PlayerControl.LocalPlayer;
               
                if (!localplayer.AmOwner) return;
                if (localplayer.inMovingPlat) return;
                if (localplayer.onLadder) return;
                // Save Sleepwalker position, if the player is able to move (i.e. not on a ladder or a gap thingy)
                if (localplayer.MyPhysics.enabled && localplayer.moveable || localplayer.inVent)
                    Sleepwalker.LastPosition = localplayer.transform.position;
            }
        }

        [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]  // Set position of AntiTp players AFTER they have selected a spawn.
        public class AirshipSpawnInPatch
        {
            static void Postfix()
            {
                Sleepwalker.RpcSleepwalkToPosition();
            }
        }
    }
}
