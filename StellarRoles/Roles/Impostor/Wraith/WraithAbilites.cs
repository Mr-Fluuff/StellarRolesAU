using HarmonyLib;
using StellarRoles.Objects;
using UnityEngine;

namespace StellarRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class WraithAbilites
    {
        public static void Postfix(HudManager __instance)
        {
            if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Wraith.Player) return;
            WraithUpdate();
        }

        public static void WraithUpdate()
        {
            Wraith.LanternTimer -= Time.deltaTime;
            Wraith.InvisibleTimer -= Time.deltaTime;

            if (Lantern.CurrentLantern != null && Wraith.LanternTimer <= 0f)
            {
                RPCProcedure.Send(CustomRPC.WraithLanternBreak);
                Lantern.BreakLantern();
                WraithButtons.WraithLanternPlaceButton.Timer = Wraith.LanternTimer;
            }

            if (Wraith.IsInvisible && Wraith.InvisibleTimer <= 0f)
            {
                RPCProcedure.Send(CustomRPC.SetInvisible, Wraith.Player, true);
                RPCProcedure.SetInvisible(Wraith.Player, true);
            }
        }
    }
}
