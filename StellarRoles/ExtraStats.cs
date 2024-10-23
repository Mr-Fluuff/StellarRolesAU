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
            if (!localPlayer.Data.Role.IsImpostor)
            {
                if (exiledPlayer?.PlayerId == playerVoted?.PlayerId)
                {
                    if (exiledPlayer.Data.Role.IsImpostor)
                    {
                        localPlayer.RPCAddGameInfo(InfoType.AddCorrectEject);
                    }
                    else
                    {
                        localPlayer.RPCAddGameInfo(InfoType.AddIncorrectEject);
                    }
                }

                if (Helpers.isCriticalMeetingError(exiledPlayer))
                {
                    localPlayer.RPCAddGameInfo(InfoType.CriticalMeetingError);
                }
            }

            if (exiledPlayer?.IsCrew() == true)
            {
                localPlayer.RPCAddGameInfo(InfoType.AddCrewmatesEjected);
            }
            playerVoted = null;
        }

        public static void UpdateSurvivability()
        {
            if (PlayerControl.LocalPlayer.AmOwner && !localPlayer.Data.IsDead)
            {
                Helpers.LogConsole("Update Survivability: " + MapOptions.PlayersAlive);
                RPCProcedure.Send(CustomRPC.UpdateSurvivability, localPlayer);
                RPCProcedure.UpdateSurvivability(localPlayer);
            }
        }
    }

}
