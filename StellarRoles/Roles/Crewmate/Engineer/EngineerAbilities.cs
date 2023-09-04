using HarmonyLib;
using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class EngineerAbilities
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || Engineer.Player == null) return;
            engineerUpdate();
            enginnerFixUpdate();
        }

        public static bool IsRoleBlocked()
        {
            return Engineer.RoleBlock && Helpers.IsCommsActive();
        }

        static void engineerUpdate()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            bool isEvil = localPlayer.Data.Role.IsImpostor || Helpers.IsNeutralKiller(localPlayer);
            bool evilHighlight = Engineer.HighlightForEvil && isEvil;
            bool engineerHighlight = localPlayer == Engineer.Player && Engineer.HighlightForEvil;
            if (MapUtilities.CachedShipStatus?.AllVents != null && Engineer.Player.inVent && (evilHighlight || engineerHighlight) && QualifiedEngineer())
            {
                foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
                {
                    vent?.myRend?.material?.SetFloat("_Outline", 1);
                    vent?.myRend?.material?.SetColor("_OutlineColor", Engineer.Color);
                }
            }
            if (Ascended.IsAscended(Engineer.Player))
            {
                if (Engineer.Player.inVent)
                {
                    Engineer.EngineerVentTimer += Time.deltaTime;
                }
                else
                {
                    Engineer.EngineerVentTimer = 0f;
                }
            }
        }

        static bool QualifiedEngineer()
        {
            return !Ascended.IsAscended(Engineer.Player) || Engineer.EngineerVentTimer > 5f;
        }

        private static void enginnerFixUpdate()
        {
            if (PlayerControl.LocalPlayer != Engineer.Player) return;

            (int playerCompleted, int totalTasks) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);

            if (playerCompleted == totalTasks && !Engineer.GaveFix && Engineer.GetsFix == RemoteFix.TaskCompletion)
            {
                Engineer.HasFix = true;
                Engineer.GaveFix = true;
            }
        }
    }
}
