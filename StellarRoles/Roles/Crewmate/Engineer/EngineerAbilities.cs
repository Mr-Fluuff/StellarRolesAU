using HarmonyLib;
using Hazel;
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
            if (ShipStatus.Instance?.AllVents != null && (evilHighlight || engineerHighlight))
            {
                if (Engineer.Player.inVent && QualifiedEngineer())
                {
                    foreach (Vent vent in ShipStatus.Instance.AllVents)
                    {
                        vent?.myRend?.material?.SetFloat("_Outline", 1);
                        vent?.myRend?.material?.SetColor("_OutlineColor", Engineer.Color);
                    }
                }
                else
                {
                    foreach (Vent vent in ShipStatus.Instance.AllVents)
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
            else if (sabo == SabatageTypes.LifeSupp)
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 0 | 64);
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 1 | 64);
            }
            else if (sabo == SabatageTypes.HeliSabotage)
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 0 | 16);
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 1 | 16);
            }
            else if (sabo == SabatageTypes.Laboratory)
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Laboratory, 16);
            }
            else if (sabo == SabatageTypes.Reactor)
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Reactor, 16);
            }
            else if (sabo == SabatageTypes.Comms)
            {
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
            }
            else if (sabo == SabatageTypes.OxyMask)
            {
                SubmergedCompatibility.RepairOxygen();
                RPCProcedure.Send(CustomRPC.EngineerFixSubmergedOxygen);
            }
        }
    }

    [HarmonyPatch(typeof(VentButton), (nameof(VentButton.SetTarget)))]
    public static class SetVentTarget
    {
        public static void Postfix(VentButton __instance, [HarmonyArgument(0)] Vent vent)
        {
            if (PlayerControl.LocalPlayer == Engineer.Player)
            {
                Engineer.currentTarget = vent;
            }
        }
    }

    [HarmonyPatch(typeof(DoorBreakerGame), (nameof(DoorBreakerGame.FlipSwitch)))]
    public static class DoorBreakerPatch
    {
        public static void Postfix(DoorBreakerGame __instance)
        {
            if (
                Engineer.Player == null ||
                !Engineer.Player.AmOwner ||
                !Engineer.AdvancedSabotageRepair ||
                EngineerAbilities.IsRoleBlocked() ||
                !Engineer.IsAlone(2.5f)
            )
                return;

            foreach (SpriteRenderer button in __instance.Buttons)
            {
                if (!button.flipX)
                    continue;

                button.color = Color.gray;
                button.flipX = false;
                button.GetComponent<PassiveButton>().enabled = false;
            }

            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, (byte)(__instance.MyDoor.Id | 64));
            __instance.MyDoor.SetDoorway(true);
            CoroutineHelper.Instance.StartCoroutine(__instance.CoStartClose(0.4f));
        }
    }

    [HarmonyPatch(typeof(MushroomDoorSabotageMinigame), nameof(MushroomDoorSabotageMinigame.UpdateMushroomWhackCount))]
    public static class FungalDoorEngineerPatch
    {
        public static void Postfix(MushroomDoorSabotageMinigame __instance)
        {
            if (
                Engineer.Player == null ||
                !Engineer.Player.AmOwner ||
                !Engineer.AdvancedSabotageRepair ||
                EngineerAbilities.IsRoleBlocked() ||
                !Engineer.IsAlone(2.5f)
            )
                return;

            __instance.mushroomWhackCount += 5;
            __instance.SetCounterText(__instance.mushroomWhackCount);
            if (__instance.mushroomWhackCount >= 6)
            {
                __instance.FixDoorAndCloseMinigame();
            }
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem), typeof(SystemTypes), typeof(PlayerControl), typeof(MessageReader))]
    class RepairSystemPatch
    {
        static byte amount;
        static PlayerControl Player;
        static SystemTypes System;
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] SystemTypes systemType, [HarmonyArgument(1)] PlayerControl player, [HarmonyArgument(2)] MessageReader msgReader)
        {
            var oldPos = msgReader.Position;
            Player = player;
            System = systemType;
            amount = msgReader.ReadByte();
            msgReader.Position = oldPos;
            return true;
        }
        //public static bool IsSubmergedOxygen;
        public static void Postfix(ShipStatus __instance)
        {
            if (Engineer.Player == null
                || Engineer.Player != Player
                || !Engineer.AdvancedSabotageRepair
                || EngineerAbilities.IsRoleBlocked()
                || !Engineer.IsAlone(2.5f))
                return;

            switch (System)
            {
                case SystemTypes.Reactor:
                    if (amount == 64 || amount == 65)
                    {
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Reactor, 16);
                    }
                    break;
                case SystemTypes.HeliSabotage:
                    if (amount == 16 || amount == 17)
                    {
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 0 | 16);
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 1 | 16);
                    }
                    break;
                case SystemTypes.Laboratory:
                    if (amount == 64 || amount == 65)
                    {
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Laboratory, 16);
                    }
                    break;
                case SystemTypes.LifeSupp:
                    if (amount == 64 || amount == 65)
                    {
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 0 | 64);
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 1 | 64);
                    }
                    break;
                case SystemTypes.Comms:
                    if (amount != 128)
                    {
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                    }
                    break;
                case SystemTypes.Electrical:
                    if (amount >= 0 && amount <= 4)
                    {
                        SwitchSystem switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        switchSystem.ActualSwitches = 0;
                        switchSystem.ExpectedSwitches = 0;
                    }
                    break;
            }
        }
    }
}
