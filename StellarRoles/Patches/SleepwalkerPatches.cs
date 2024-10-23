using HarmonyLib;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class SleepwalkerPatches
    {
        // Save the position of the player prior to starting the climb / gap platform
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
        public static void ClimbLadder()
        {
            Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
        }

        [HarmonyPatch(typeof(PlatformConsole), nameof(PlatformConsole.Use))]
        public static class AirShipPlatformSleepwalker
        {
            [HarmonyPrefix]
            public static void Use()
            {
                Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
            }
        }

        [HarmonyPatch(typeof(ZiplineBehaviour), nameof(ZiplineBehaviour.Use), typeof(PlayerControl), typeof(bool))]
        public static class FungleZiplineSleepwalker
        {
            [HarmonyPrefix]
            public static void UseZipline()
            {
                Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch
        {
            public static void Prefix()
            {
                // Save Sleepwalker position, if the player is able to move (i.e. not on a ladder or a gap thingy)
                if (PlayerControl.LocalPlayer.MyPhysics.enabled && PlayerControl.LocalPlayer.moveable || PlayerControl.LocalPlayer.inVent)
                    Sleepwalker.LastPosition = PlayerControl.LocalPlayer.transform.position;
            }
        }

        [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]  // Set position of AntiTp players AFTER they have selected a spawn.
        class AirshipSpawnInPatch
        {
            static void Postfix()
            {
                Sleepwalker.SetPosition();
            }
        }
    }
}
