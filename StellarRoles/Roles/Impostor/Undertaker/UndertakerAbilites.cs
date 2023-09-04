using HarmonyLib;
using UnityEngine;

namespace StellarRoles
{
    public static class UndertakerAbilities
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class UndertakerDrag
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (!Helpers.GameStarted || Undertaker.Player == null) return;
                if (__instance != Undertaker.Player) return;

                DeadBody body = Undertaker.DeadBodyDragged;
                if (body == null) return;
                if (__instance.Data.IsDead)
                {
                    Undertaker.DeadBodyDragged = null;
                    return;
                }

                Vector2 currentPosition = __instance.GetTruePosition();
                Vector2 velocity = __instance.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
                Vector3 newPos = ((Vector2)__instance.transform.position) - (velocity / 3) + body.myCollider.offset;
                newPos.z = currentPosition.y / 1000 + 0.0005f;

                if (!PhysicsHelpers.AnythingBetween(
                    currentPosition,
                    newPos,
                    Constants.ShipAndObjectsMask,
                    false
                )) body.transform.localPosition = newPos;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class UndertakerSetBodyTarget
        {
            public static void Postfix()
            {
                if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Undertaker.Player || Undertaker.DeadBodyDragged != null) return;
                Undertaker.DeadBodyCurrentTarget = Helpers.SetBodyTarget();
            }
        }
    }
}
