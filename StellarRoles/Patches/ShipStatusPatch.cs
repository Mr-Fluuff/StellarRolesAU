using AmongUs.GameOptions;
using HarmonyLib;
using StellarRoles.Utilities;
using UnityEngine;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(DoorBreakerGame))]
    public static class DoorBreakerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DoorBreakerGame.FlipSwitch))]
        public static void FlipAllSwitches(DoorBreakerGame __instance)
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

            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, __instance.MyDoor.Id | 64);
            __instance.MyDoor.SetDoorway(true);
            CoroutineHelper.Instance.StartCoroutine(__instance.CoStartClose(0.4f));
        }
    }

    [HarmonyPatch(typeof(ShipStatus))]
    public static class ShipStatusPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        public static bool Prefix(ref float __result, ShipStatus __instance)
        {
            if (!__instance.Systems.ContainsKey(SystemTypes.Electrical) || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
                return true;

            SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            float lerpValue = switchSystem.Value / 255f;
            bool lightsout = Helpers.IsLightsActive();
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            foreach (Nightmare nightmare in Nightmare.PlayerToNightmare.Values)
            {
                float nightmareLerpValue = 1f;
                float timeLeft = nightmare.LightsOutTimer;
                float timeElapsed = Nightmare.BlindDuration - nightmare.LightsOutTimer;
                // If there is a Nightmare with their ability active
                if (timeLeft > 0f && !nightmare.Player.AmOwner && nightmare.BlindedPlayers.Contains(localPlayer.PlayerId))
                {
                    if (timeElapsed < 1f)
                        nightmareLerpValue = Mathf.Clamp01(timeElapsed * 3);
                    else if (timeLeft < 1.5f)
                        nightmareLerpValue = Mathf.Clamp01(timeLeft * 0.5f);

                    __result = Mathf.Lerp(__instance.MinLightRadius * .75f, __instance.MaxLightRadius * .75f, lightsout ? lerpValue : 1 - nightmareLerpValue) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                    return false;
                }
            }

            // If there is a Shade with their ability active
            if (Shade.Player != localPlayer && Shade.LightsOutTimer > 0f && Shade.BlindedPlayers.Contains(localPlayer))
            {
                float shadeLerpValue = 1f;
                float shadeTimer = Shade.BlindDuration - Shade.LightsOutTimer;
                if (shadeTimer < 0.8f)
                    shadeLerpValue = Mathf.Clamp01(shadeTimer * 3);
                else if (Shade.LightsOutTimer < 1.5)
                    shadeLerpValue = Mathf.Clamp01(Shade.LightsOutTimer * 0.5f);

                __result = Mathf.Lerp(__instance.MinLightRadius * .75f, __instance.MaxLightRadius * .75f, lightsout ? lerpValue : 1 - shadeLerpValue) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                return false;
            }

            // If player is a role which has Impostor vision
            if (localPlayer.Data.Role.IsImpostor || localPlayer.IsNeutralKiller())
            {
                __result = GetNeutralLightRadius(__instance, true);
                return false;
            }

            // If player is Lighter with ability active
            if (localPlayer.IsJester(out _))
            {
                __result = Mathf.Lerp(5 * Jester.LightsOffVision, 5 * Jester.LightsOnVision, lerpValue);
                return false;
            }
            // Default light radius
            else
                __result = GetNeutralLightRadius(__instance, false);

            return false;
        }

        public static float GetNeutralLightRadius(ShipStatus shipStatus, bool isImpostor)
        {
            if (SubmergedCompatibility.IsSubmerged)
                return SubmergedCompatibility.GetSubmergedNeutralLightRadius(isImpostor);

            if (isImpostor)
                return shipStatus.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;

            SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            float lerpValue = switchSystem.Value / 255f;

            return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
        public static void Postfix2(ref bool __result)
        {
            __result = false;
        }

        private static int OriginalNumCommonTasksOption = 0;
        private static int OriginalNumShortTasksOption = 0;
        private static int OriginalNumLongTasksOption = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool BeginPrefix(ShipStatus __instance)
        {
            int commonTaskCount = __instance.CommonTasks.Count;
            int normalTaskCount = __instance.NormalTasks.Count;
            int longTaskCount = __instance.LongTasks.Count;
            OriginalNumCommonTasksOption = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks;
            OriginalNumShortTasksOption = GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;
            OriginalNumLongTasksOption = GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks > commonTaskCount)
                GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = commonTaskCount;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks > normalTaskCount)
                GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = normalTaskCount;
            if (GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks > longTaskCount)
                GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = longTaskCount;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static void BeginPostfix()
        {
            // Restore original settings after the tasks have been selected
            GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks = OriginalNumCommonTasksOption;
            GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks = OriginalNumShortTasksOption;
            GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks = OriginalNumLongTasksOption;
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RepairSystem))]
    class RepairSystemPatch
    {
        //public static bool IsSubmergedOxygen;
        public static bool Prefix(
            [HarmonyArgument(0)] SystemTypes systemType,
            [HarmonyArgument(1)] PlayerControl player,
            [HarmonyArgument(2)] byte amount)
        {

            if (!AmongUsClient.Instance.AmHost)
                return true;

            if (Engineer.Player != null && player == Engineer.Player && Engineer.AdvancedSabotageRepair)
                switch (systemType)
                {
                    case SystemTypes.Reactor:
                        if (Engineer.IsAlone(2.5f))
                        {
                            if (amount is 64 or 65)
                            {
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 67);
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 66);
                            }

                            if (amount is 16 or 17)
                            {
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 19);
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 18);
                            }
                        }
                        break;
                    case SystemTypes.Laboratory:
                        if (Engineer.IsAlone(2.5f))
                        {
                            if (amount is 64 or 65)
                            {
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 67);
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 66);
                            }
                        }
                        break;
                    case SystemTypes.LifeSupp:
                        if (amount is 64 or 65)
                        {
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 67);
                            ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 66);
                        }
                        break;
                    case SystemTypes.Comms:
                        if (!EngineerAbilities.IsRoleBlocked())
                        {
                            if (amount is 16 or 17)
                            {
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 19);
                                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 18);
                            }

                        }
                        break;
                }

            /*
                IsSubmergedOxygen = false;
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                    if (task.TaskType == SubmergedCompatibility.RetrieveOxygenMask) IsSubmergedOxygen = true;

                if (SubmergedCompatibility.IsSubmerged && IsSubmergedOxygen)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)CustomRPC.EngineerFixSubmergedOxygen, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.engineerFixSubmergedOxygen();
                }*/

            return true;
        }

    }

    [HarmonyPatch(typeof(SwitchSystem), nameof(SwitchSystem.RepairDamage))]
    class SwitchSystemRepairPatch
    {
        public static void Postfix(SwitchSystem __instance, [HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] byte amount)
        {
            if (Engineer.Player != null && player == Engineer.Player && Engineer.AdvancedSabotageRepair && Engineer.IsAlone(2.5f))
                if (amount is >= 0 and <= 4)
                {
                    __instance.ActualSwitches = 0;
                    __instance.ExpectedSwitches = 0;
                }
        }
    }
}
