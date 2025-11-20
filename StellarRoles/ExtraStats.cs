using HarmonyLib;
using StellarRoles.Utilities;
using System.Linq;

namespace StellarRoles
{
    public static class ExtraStats
    {
        public static PlayerControl playerVoted = null;
        public static PlayerControl localPlayer => PlayerControl.LocalPlayer;
        public static void ClearAndReload()
        {
            playerVoted = null;
        }

        public static void ExileStats(PlayerControl exiledPlayer)
        {
            var exiled = exiledPlayer != null && playerVoted != null && exiledPlayer.PlayerId == playerVoted.PlayerId;
            if (!localPlayer.Data.Role.IsImpostor)
            {
                if (exiled)
                {
                    if (!exiledPlayer.IsCrew())
                    {
                        localPlayer.RPCAddGameInfo(InfoType.AddCorrectEject);
                    }
                    else
                    {
                        localPlayer.RPCAddGameInfo(InfoType.AddIncorrectEject);
                    }
                }

                if (Helpers.isCriticalMeetingError(exiledPlayer) && !localPlayer.Data.IsDead)
                {
                    localPlayer.RPCAddGameInfo(InfoType.CriticalMeetingError);
                }
            }
            else if (exiled && exiledPlayer.IsCrew())
            {
                localPlayer.RPCAddGameInfo(InfoType.AddCrewmatesEjected);
            }

            playerVoted = null;
        }

/*        public static void UpdateSurvivability()
        {
            if (PlayerControl.LocalPlayer.AmOwner && !localPlayer.Data.IsDead)
            {
                Helpers.LogConsole("Update Survivability: " + MapOptions.PlayersAlive);


                RPCProcedure.Send(CustomRPC.UpdateSurvivability, localPlayer);
                RPCProcedure.UpdateSurvivability(localPlayer);
            }
        }

        [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
        public static class ReEnableGamplayPatch
        {
            public static void Postfix()
            {
                Helpers.DelayedAction(0.2f, () =>
                {
                    Helpers.CheckPlayersAlive();
                    if (MapOptions.ImpsAlive > 0)
                    {
                        UpdateSurvivability();
                    }
                });
            }
        }*/
    }

}
