using HarmonyLib;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class AirShipSleepwalker
    {
        // Save the position of the player prior to starting the climb / gap platform
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public static void ClimbLadder()
        {
            Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
        public static void UsePlatform()
        {
            Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
        }
    }
}
