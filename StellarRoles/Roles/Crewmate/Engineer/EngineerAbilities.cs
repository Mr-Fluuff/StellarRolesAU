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
            if (MapUtilities.CachedShipStatus?.AllVents != null && (evilHighlight || engineerHighlight))
            {
                if (Engineer.Player.inVent && QualifiedEngineer())
                {
                    foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
                    {
                        vent?.myRend?.material?.SetFloat("_Outline", 1);
                        vent?.myRend?.material?.SetColor("_OutlineColor", Engineer.Color);
                    }
                }
                else
                {
                    foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
                    {
                        if (vent?.myRend?.material?.GetColor("_OutlineColor") == Engineer.Color)
                        {
                            vent?.myRend?.material?.SetFloat("_Outline", 0);
                            vent?.myRend?.material?.SetColor("_OutlineColor", Color.clear);
                        }
                    }
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

            if (playerCompleted == totalTasks && totalTasks > 0 && !Engineer.GaveFix && Engineer.GetsFix == RemoteFix.TaskCompletion)
            {
                Engineer.HasFix = true;
                Engineer.GaveFix = true;
            }
        }
        public static void RepairSabo(this SabatageTypes sabo)
        {
            if (sabo == SabatageTypes.Lights)
            {
                RPCProcedure.Send(CustomRPC.EngineerFixLights);
                RPCProcedure.EngineerFixLights();
            }
            else if (sabo == SabatageTypes.O2)
            {
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
            }
            else if (sabo == SabatageTypes.Charles)
            {
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);
            }
            else if (sabo == SabatageTypes.Seismic)
            {
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Laboratory, 16);
            }
            else if (sabo == SabatageTypes.Reactor)
            {
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 16);
            }
            else if (sabo == SabatageTypes.Comms)
            {
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
            }
            else if (sabo == SabatageTypes.OxyMask)
            {
                SubmergedCompatibility.RepairOxygen();
                RPCProcedure.Send(CustomRPC.EngineerFixSubmergedOxygen);
            }
        }
    }
}
