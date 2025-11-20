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

                if (__instance.inVent)
                {
                    currentPosition.x += 0.1f;
                    currentPosition.y += 0.1f;
                    Vector3 ventpostion = new Vector3(currentPosition.x, currentPosition.y, currentPosition.y / 1000f + 0.0005f);
                    body.transform.position = ventpostion;
                    return;
                }

                Vector2 velocity = __instance.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;

                Vector3 newPos = ((Vector2)__instance.transform.position) - (velocity / 2.5f)/* + body.myCollider.offset*/;
                if (velocity != Vector2.zero)
                {
                    newPos.x += 0.1f;
                    newPos.y -= 0.1f;
                }

                var distance = Vector2.Distance(currentPosition, body.transform.position);

                if (distance > 0.2f)
                {

                    if (distance > 1f)
                    {
                        if (!PhysicsHelpers.AnythingBetween(currentPosition, newPos, Constants.ShipAndAllObjectsMask, false))
                        {
                            body.transform.position = newPos;
                            return;
                        }
                    }
                    Vector3 nextPos = Vector2.MoveTowards(body.TruePosition, newPos, 0.1f);
                    nextPos.z = nextPos.y / 1000f + 0.0005f;

                    if (!PhysicsHelpers.AnythingBetween(currentPosition, nextPos, Constants.ShipAndAllObjectsMask, false))
                    {
                        body.transform.position = nextPos;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class UndertakerSetBodyTarget
        {
            public static void Postfix()
            {
                if (!Helpers.GameStarted || PlayerControl.LocalPlayer != Undertaker.Player || Undertaker.DeadBodyDragged != null) return;
                Undertaker.DeadBodyCurrentTarget = Helpers.SetBodyTarget(Helpers.GetKillDistance() * 0.75f);
            }
        }
    }
}
