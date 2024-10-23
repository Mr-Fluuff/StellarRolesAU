using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;
using static StellarRoles.MapOptions;

namespace StellarRoles.Patches
{
    [HarmonyPatch(typeof(ShipStatus))]
    public static class ShipStatusPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        public static bool ShipStatusPrefix(ref float __result, ShipStatus __instance)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
                return true;

            float lerpValue = 1;
            if (__instance.Systems.TryGetValue(SystemTypes.Electrical, out ISystemType elec))
            {
                lerpValue = elec.TryCast<SwitchSystem>().Value / 255f;
            }
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

                    __result = Mathf.Lerp(__instance.MinLightRadius * .75f, __instance.MaxLightRadius * .75f, lightsout ? lerpValue : 1 - nightmareLerpValue) * StellarRoles.NormalOptions.CrewLightMod;
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

                __result = Mathf.Lerp(__instance.MinLightRadius * .75f, __instance.MaxLightRadius * .75f, lightsout ? lerpValue : 1 - shadeLerpValue) * StellarRoles.NormalOptions.CrewLightMod;
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
                return shipStatus.MaxLightRadius * StellarRoles.NormalOptions.ImpostorLightMod;

            float lerpValue = 1;
            if (shipStatus.Systems.TryGetValue(SystemTypes.Electrical, out ISystemType elec))
            {
                lerpValue = elec.TryCast<SwitchSystem>().Value / 255f;
            }

            return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue) * StellarRoles.NormalOptions.CrewLightMod;
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
        public static float OriginalKillCD = 0;
        public static int OriginalButtonCD = 0;



        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool BeginPrefix(ShipStatus __instance)
        {
            int commonTaskCount = __instance.CommonTasks.Count;
            int shortTaskCount = __instance.ShortTasks.Count;
            int longTaskCount = __instance.LongTasks.Count;
            OriginalNumCommonTasksOption = StellarRoles.NormalOptions.NumCommonTasks;
            OriginalNumShortTasksOption = StellarRoles.NormalOptions.NumShortTasks;
            OriginalNumLongTasksOption = StellarRoles.NormalOptions.NumLongTasks;
            OriginalKillCD = StellarRoles.NormalOptions.GetFloat(FloatOptionNames.KillCooldown);
            OriginalButtonCD = StellarRoles.NormalOptions.GetInt(Int32OptionNames.EmergencyCooldown);

            switch (Helpers.CurrentMap())
            {
                case Map.Skeld:
                case Map.Dleks:
                    if (ModifySkeld)
                    {
                        StellarRoles.NormalOptions.NumCommonTasks = CustomOptionHolder.SkeldCommonTasks.GetInt();
                        StellarRoles.NormalOptions.NumShortTasks = CustomOptionHolder.SkeldShortTasks.GetInt();
                        StellarRoles.NormalOptions.NumLongTasks = CustomOptionHolder.SkeldLongTasks.GetInt();
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.KillCooldown, CustomOptionHolder.SkeldKillCD.GetFloat());
                        StellarRoles.NormalOptions.SetInt(Int32OptionNames.EmergencyCooldown, CustomOptionHolder.SkeldButtonCD.GetInt());
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.CrewLightMod, CustomOptionHolder.SkeldCrewVision.GetFloat());
                    }
                    break;

                case Map.Mira:
                    if (ModifyMira)
                    {
                        StellarRoles.NormalOptions.NumCommonTasks = CustomOptionHolder.MiraCommonTasks.GetInt();
                        StellarRoles.NormalOptions.NumShortTasks = CustomOptionHolder.MiraShortTasks.GetInt();
                        StellarRoles.NormalOptions.NumLongTasks = CustomOptionHolder.MiraLongTasks.GetInt();
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.KillCooldown, CustomOptionHolder.MiraKillCD.GetFloat());
                        StellarRoles.NormalOptions.SetInt(Int32OptionNames.EmergencyCooldown, CustomOptionHolder.MiraButtonCD.GetInt());
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.CrewLightMod, CustomOptionHolder.MiraCrewVision.GetFloat());
                    }
                    break;

                case Map.Polus:
                    if (ModifyPolus)
                    {
                        StellarRoles.NormalOptions.NumCommonTasks = CustomOptionHolder.PolusCommonTasks.GetInt();
                        StellarRoles.NormalOptions.NumShortTasks = CustomOptionHolder.PolusShortTasks.GetInt();
                        StellarRoles.NormalOptions.NumLongTasks = CustomOptionHolder.PolusLongTasks.GetInt();
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.KillCooldown, CustomOptionHolder.PolusKillCD.GetFloat());
                        StellarRoles.NormalOptions.SetInt(Int32OptionNames.EmergencyCooldown, CustomOptionHolder.PolusButtonCD.GetInt());
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.CrewLightMod, CustomOptionHolder.PolusCrewVision.GetFloat());
                    }
                    break;

                case Map.Airship:
                    if (ModifyAirship)
                    {
                        StellarRoles.NormalOptions.NumCommonTasks = CustomOptionHolder.AirShipCommonTasks.GetInt();
                        StellarRoles.NormalOptions.NumShortTasks = CustomOptionHolder.AirShipShortTasks.GetInt();
                        StellarRoles.NormalOptions.NumLongTasks = CustomOptionHolder.AirShipLongTasks.GetInt();
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.KillCooldown, CustomOptionHolder.AirShipKillCD.GetFloat());
                        StellarRoles.NormalOptions.SetInt(Int32OptionNames.EmergencyCooldown, CustomOptionHolder.AirShipButtonCD.GetInt());
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.CrewLightMod, CustomOptionHolder.AirShipCrewVision.GetFloat());
                    }
                    break;

                case Map.Fungal:
                    if (ModifyFungle)
                    {
                        StellarRoles.NormalOptions.NumCommonTasks = CustomOptionHolder.FungalCommonTasks.GetInt();
                        StellarRoles.NormalOptions.NumShortTasks = CustomOptionHolder.FungalShortTasks.GetInt();
                        StellarRoles.NormalOptions.NumLongTasks = CustomOptionHolder.FungalLongTasks.GetInt();
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.KillCooldown, CustomOptionHolder.FungalKillCD.GetFloat());
                        StellarRoles.NormalOptions.SetInt(Int32OptionNames.EmergencyCooldown, CustomOptionHolder.FungalButtonCD.GetInt());
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.CrewLightMod, CustomOptionHolder.FungalCrewVision.GetFloat());
                    }
                    break;

                case Map.Submerged:
                    if (ModifySubmerged)
                    {
                        StellarRoles.NormalOptions.NumCommonTasks = CustomOptionHolder.SubmergedCommonTasks.GetInt();
                        StellarRoles.NormalOptions.NumShortTasks = CustomOptionHolder.SubmergedShortTasks.GetInt();
                        StellarRoles.NormalOptions.NumLongTasks = CustomOptionHolder.SubmergedLongTasks.GetInt();
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.KillCooldown, CustomOptionHolder.SubmergedKillCD.GetFloat());
                        StellarRoles.NormalOptions.SetInt(Int32OptionNames.EmergencyCooldown, CustomOptionHolder.SubmergedButtonCD.GetInt());
                        StellarRoles.NormalOptions.SetFloat(FloatOptionNames.CrewLightMod, CustomOptionHolder.SubmergedCrewVision.GetFloat());
                    }
                    break;
            }
            if (StellarRoles.NormalOptions.NumCommonTasks > commonTaskCount)
                StellarRoles.NormalOptions.NumCommonTasks = commonTaskCount;
            if (StellarRoles.NormalOptions.NumShortTasks > shortTaskCount)
                StellarRoles.NormalOptions.NumShortTasks = shortTaskCount;
            if (StellarRoles.NormalOptions.NumLongTasks > longTaskCount)
                StellarRoles.NormalOptions.NumLongTasks = longTaskCount;

            GameManager.Instance.LogicOptions.SyncOptions();

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        public static void ResetOptionsPostfix()
        {
            if (!AmongUsClient.Instance.AmHost) return;
            // Restore original settings after the tasks have been selected
            StellarRoles.NormalOptions.NumCommonTasks = OriginalNumCommonTasksOption;
            StellarRoles.NormalOptions.NumShortTasks = OriginalNumShortTasksOption;
            StellarRoles.NormalOptions.NumLongTasks = OriginalNumLongTasksOption;
            StellarRoles.NormalOptions.SetFloat(FloatOptionNames.KillCooldown, OriginalKillCD);
            StellarRoles.NormalOptions.SetInt(Int32OptionNames.EmergencyCooldown, OriginalButtonCD);
            GameManager.Instance.LogicOptions.SyncOptions();

        }
    }
}
