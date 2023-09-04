using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;

namespace StellarRoles.Patches
{
    [HarmonyPatch]
    public static class HauntMenuMinigamePatch
    {

        // Show the role name instead of just Crewmate / Impostor
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.SetFilterText))]
        public static void Postfix(HauntMenuMinigame __instance)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.Normal) return;
            PlayerControl target = __instance.HauntTarget;
            System.Collections.Generic.List<RoleInfo> targetRoleInfo = RoleInfo.GetRoleInfoForPlayer(target);
            RoleInfo roleInfo = targetRoleInfo.Where(x => x.FactionId != Faction.Modifier).FirstOrDefault();
            RoleInfo modifierInfo = targetRoleInfo.Where(x => x.FactionId == Faction.Modifier).FirstOrDefault();
            string roleString = (MapOptions.GhostsSeeRoles && roleInfo != null) ? roleInfo.Name : "";
            string modifierString = (MapOptions.GhostsSeeModifier && modifierInfo != null) ? " - " + modifierInfo.Name : "";
            string finishedString = roleString + modifierString;
            if (__instance.HauntTarget.Data.IsDead)
            {
                __instance.FilterText.text = finishedString + " - Ghost";
                return;
            }
            __instance.FilterText.text = finishedString;
            return;
        }

        // The impostor filter now includes neutral roles
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.MatchesFilter))]
        public static void MatchesFilterPostfix(HauntMenuMinigame __instance, PlayerControl pc, ref bool __result)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.Normal) return;
            if (__instance.filterMode == HauntMenuMinigame.HauntFilters.Impostor)
            {
                System.Collections.Generic.List<RoleInfo> info = RoleInfo.GetRoleInfoForPlayer(pc, false);
                __result = (pc.Data.Role.IsImpostor || info.Any(x => x.FactionId == Faction.Neutral || (x.FactionId == Faction.NK))) && !pc.Data.IsDead;
            }
        }


        // Shows the "haunt evil roles button"
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.Start))]
        public static bool StartPrefix(HauntMenuMinigame __instance)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.Normal || !MapOptions.GhostsSeeRoles) return true;
            __instance.FilterButtons[0].gameObject.SetActive(true);
            int numActive = 0;
            int numButtons = __instance.FilterButtons.Count((PassiveButton s) => s.isActiveAndEnabled);
            float edgeDist = 0.6f * numButtons;
            for (int i = 0; i < __instance.FilterButtons.Length; i++)
            {
                PassiveButton passiveButton = __instance.FilterButtons[i];
                if (passiveButton.isActiveAndEnabled)
                {
                    passiveButton.transform.SetLocalX(FloatRange.SpreadToEdges(-edgeDist, edgeDist, numActive, numButtons));
                    numActive++;
                }
            }
            return false;
        }
    }
}