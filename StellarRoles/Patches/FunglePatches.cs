using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static StellarRoles.MapOptions;

namespace StellarRoles.Patches
{
    public class FunglePatches
    {
        [HarmonyPatch(typeof(MushroomDoorSabotageMinigame), nameof(MushroomDoorSabotageMinigame.SetCounterText))]
        class MushroomDoorSaboCounterTextPatch
        {
            static bool Prefix(MushroomDoorSabotageMinigame __instance, int whackedCount)
            {
                if (EasierFungalDoors)
                {
                    __instance.counterText.SetText(string.Format("{0}/{1}", whackedCount, 4), true);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CatchFishMinigame), nameof(CatchFishMinigame.Begin))]
        class CatchFishNumPatch
        {
            static void Postfix(CatchFishMinigame __instance)
            {
                if (EasierFungalFish)
                {
                    __instance.numCaughtFish = 2;
                    __instance.fishCounters[0].enabled = false;
                    __instance.fishCounters[1].enabled = false;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixMixedUpOutfit))]
        class FixMixedUpOutfitPatch1
        {
            static void Postfix(PlayerControl __instance)
            {
                if (__instance.IsInvisible())
                {
                    RPCProcedure.SetInvisible(__instance, false, true);
                }
                if (__instance == Parasite.Controlled && Parasite.Player != null)
                {
                    RPCProcedure.SetLook(Parasite.Controlled, Parasite.Player);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MixUpOutfit))]
        public static class MixUpOutfitPatch1
        {
            public static Dictionary<byte, NetworkedPlayerInfo.PlayerOutfit> outfits = new();

            static void Prefix(PlayerControl __instance) 
            {
                if (Camouflager.CamouflageTimer > 0) Camouflager.CamouflageTimer = 0f;
                if (PlayerControl.LocalPlayer == Camouflager.Player && __instance == PlayerControl.LocalPlayer)
                {
                    var button = CamouflagerButtons.CamouflagerButton;
                    if (button.IsEffectActive)
                    {
                        button.OnEffectEnd();
                        Camouflager.ChargesRemaining++;
                    }
                }
            }

            static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo.PlayerOutfit outfit)
            {
                if (!outfits.TryAdd(__instance.PlayerId, outfit))
                {
                    outfits[__instance.PlayerId] = outfit;
                }
                if (__instance.IsInvisible())
                {
                    RPCProcedure.SetInvisible(__instance, false, true);
                }
            }
        }

        [HarmonyPatch(typeof(CookFishMinigame), nameof(CookFishMinigame.CheckIfAllFishPlaced))]
        class CookFishPatch2
        {
            static bool Prefix(CookFishMinigame __instance)
            {
                if (!EasierFungalFish) return true;

                if (__instance.cookableFishes.Any((CookableFish fish) => fish.CurrentState != CookableFish.State.FishPlaced))
                {
                    return false;
                }
                foreach (var fish in __instance.cookableFishes)
                {
                    fish.CurrentState = CookableFish.State.OtherSideCooking;
                    fish.SetState(CookableFish.State.OtherSideCooking);
                }
                __instance.MyNormTask.TimerStarted = NormalPlayerTask.TimerState.Started;
                __instance.MyNormTask.TaskTimer = __instance.cookingTimeSeconds + 10;
                __instance.CurrentState = CookFishMinigame.State.CookingSide2;
                __instance.timer.Show();
                __instance.timer.SetValue(__instance.MyNormTask.TaskTimer + 10);
                __instance.checkForTimerDone = true;
                __instance.BlockClick();
                return false;
            }
        }

        [HarmonyPatch(typeof(MushroomDoorSabotageMinigame), nameof(MushroomDoorSabotageMinigame.UpdateMushroomWhackCount))]
        class MushroomDoorSaboWhackCountPatch
        {
            static void Postfix(MushroomDoorSabotageMinigame __instance)
            {
                if (EasierFungalDoors)
                {
                    if (__instance.mushroomWhackCount >= 4)
                    {
                        __instance.FixDoorAndCloseMinigame();
                    }
                }
            }
        }
    }
}
